using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MoveAI : MonoBehaviour
{
    protected  enum eState
    {
        WAIT = 0,
        WALK,
        LAST,   //  最後に指定した方向を向かせる
        STOP,   //  障害物を検知

        MAX,
    }
    [SerializeField]
    protected eState _state = eState.WAIT;
    //  状態を外部から取得
    public bool StateWait { get { return _state == eState.WAIT; } }
    public bool StateWalk { get { return _state == eState.WALK || _state == eState.LAST; } }
    public bool StateStop { get { return _state == eState.STOP; } } //  行きたい箇所への経路が絶たれている
    protected bool RouteOver { get { return _nNowRoute >= _astar.GetRoute.Count; } }

    protected AStar _astar = null;
    protected EnemyAI _enemyAI = null;
    FieldObjectBase _fieldObjBase = null;
    protected Character _character = null;

    protected int _nNowRoute = 0;
    protected int _nNowNumber = 0;
    int _nNowArrive = 0;

    Character.eDirection _LastDirection = Character.eDirection.MAX; //  移動終了時の向きを指定
    Vector3 _oldNumberPos = Vector3.zero;

    protected int _nMoveRange = 0;  //  大きいほど遠くでも移動する

    public int GetTarget {
        get
        {
            if (_state == eState.WAIT || _astar.GetRoute.Count <= 0)
                return -1;

            return _astar.GetRoute[_astar.GetRoute.Count - 1];
        }
    }
    
    public void Init (int level, Character.eCharaType type)
    {
        _astar = gameObject.AddComponent<AStar>();
        _enemyAI = GetComponent<EnemyAI>();
        _fieldObjBase = GetComponent<FieldObjectBase>();
        _character = GetComponent<Character>();

        switch (type)
        {
            case Character.eCharaType.BALANCE:
                _nMoveRange = 4;
                break;
            case Character.eCharaType.POWER:
                _nMoveRange = 2;
                break;
            case Character.eCharaType.SPEED:
                _nMoveRange = 6;
                break;
            case Character.eCharaType.TECHNICAL:
                _nMoveRange = 4;
                break;
        }
        _nMoveRange *= (level + 1);
    }

    void Update ()
    {
        MoveUpdate();

        //  再検索 (ストップしていて、対象マスにオブジェクトがないなら)
        if(StateStop)
        {
            if (_astar.GetRoute.Count <= 0)
            {
                _state = eState.WAIT;
                return;
            }

            FieldObjectBase target = FieldData.Instance.GetObjData(_astar.GetRoute[_astar.GetRoute.Count - 1]);
            if ((_enemyAI.GetState == EnemyAI.eState.BREAK &&  target) ||
                (_enemyAI.GetState != EnemyAI.eState.BREAK && !target))
            {
                //Debug.Log("再検索");
                if (SearchRoute(GetTarget, _nNowArrive))
                    return;
            }

            _state = eState.WAIT;
        }
    }

    #region AI

    public bool OnMove()
    {
        return SearchRoute(RandomNullMass(), 0);
    }

    protected virtual int RandomNullMass()
    {
        List<int> nullMassList = new List<int>();
        FieldObjectBase[] objList = FieldData.Instance.GetObjDataArray;
        for (int i = 0; i < objList.Length; i++)
        {
            if (_enemyAI._DistanceDatas[i]._nDistance > _nMoveRange)
                continue;

            if (objList[i])
                continue;

            SandItem.eType type = SandData.Instance.GetSandDataList[i]._type;
            if (type != SandItem.eType.MAX && !FieldDataChecker.Instance.TypeCheck(name, type))
                continue;

            nullMassList.Add(i);
        }

        if (nullMassList.Count <= 0)
            return -1;

        return nullMassList[Random.Range(0, nullMassList.Count)];
    }

    #endregion

    #region Search 

    public bool SearchRoute(int nTarget, int nArrive)
    {
        if (nTarget < 0)
            return false;

        _nNowRoute = 1;
        _nNowArrive = nArrive;
        NumberUpdate();
        _LastDirection = Character.eDirection.MAX;

        if (!_astar.Search(nTarget))
        {
            _state = eState.STOP;
            _nNowRoute = _astar.GetRoute.Count;
            //Debug.Log("その場所へのルートはありません");
            return false;
        }

        _state = eState.WALK;
        //Debug.Log("対象マス : " + nTarget);
        
        if (nArrive <= 0)
            return true;

        //  移動後にアクションを行う場合の処理
        if (!SetDirection(nArrive))
            return false;

        // 目の前の場合は向いて終了
        if (_nNowRoute == _astar.GetRoute.Count)
            _state = eState.LAST;

        return true;
    }

    //  移動後がアクションの場合、１マス前を終了とする
    bool SetDirection(int nArrive)
    {
        if (nArrive >= _astar.GetRoute.Count)
            return false;

        int max = _astar.GetRoute.Count - nArrive;
        int value = _astar.GetRoute[max - 1] - _astar.GetRoute[max];
        if (value == 1)  
            _LastDirection = Character.eDirection.LEFT;
        else if (value == -1)
            _LastDirection = Character.eDirection.RIGHT;
        else if (value ==  GameScaler._nWidth)
            _LastDirection = Character.eDirection.BACK;
        else if (value == -GameScaler._nWidth)
            _LastDirection = Character.eDirection.FORWARD;

        // 最後は削除
        for (int i = 0; i < nArrive; i++)
            _astar.GetRoute.RemoveAt(_astar.GetRoute.Count - 1);

        return true;
    }

    #endregion

    #region Move

    void MoveUpdate()
    {
        if (!StateWalk)
            return;

        if (DistanceCheck())
        {
            _nNowRoute++;
            NumberUpdate();

            // LASTの向きが指定されているかをチェック
            if (_nNowRoute == _astar.GetRoute.Count)
                _state = _LastDirection != Character.eDirection.MAX ? eState.LAST : eState.WAIT;
        }
    }

    public Character.eDirection GetMoveData()
    {
        if(_state == eState.LAST)
        {
            _state = eState.WAIT;
            return _LastDirection;
        }

        if (!StateWalk || StateStop)
            return Character.eDirection.MAX;

        if (CheckObstacle())
            return Character.eDirection.MAX;
      
        if(RouteOver)
            return Character.eDirection.MAX;

        int dis = _astar.GetRoute[_nNowRoute] - _nNowNumber;
        
        if (dis == GameScaler._nWidth)
            return Character.eDirection.FORWARD;
        else if (dis == -GameScaler._nWidth)
            return Character.eDirection.BACK;
        else if (dis == 1)
            return Character.eDirection.RIGHT;
        else if (dis == -1)
            return Character.eDirection.LEFT;

        _state = eState.WAIT;
        _nNowRoute = _astar.GetRoute.Count; //  強制終了
        Debug.LogError("経路に問題がある恐れがあります。" + " 行先 : " + _astar.GetRoute[_nNowRoute] + ", 現地 : " + _nNowNumber);
        return Character.eDirection.MAX;
    }

    void NumberUpdate()
    {
        if (!_fieldObjBase)
            return;
        
        _oldNumberPos = _fieldObjBase.GetPosForNumber();
        _nNowNumber = _fieldObjBase.GetDataNumber(_oldNumberPos);
    }

    bool DistanceCheck()
    {
        if (Mathf.Abs(_oldNumberPos.x - transform.position.x) >= GameScaler._fScale)
            return true;

        if (Mathf.Abs(_oldNumberPos.z - transform.position.z) >= GameScaler._fScale)
            return true;

        return false;
    }

    protected virtual bool CheckObstacle()
    {
        if (RouteOver)
            return false;

        int idx = _astar.GetRoute[_nNowRoute];

        if (!FieldDataChecker.Instance.CheckObstacleObj(idx, _character) &&
            !FieldDataChecker.Instance.SandCheck(idx, this.name))
            return false;

        OnObstacle();
        return true;
    }

    void OnObstacle()
    {
        _state = eState.STOP;
        _nNowRoute = _astar.GetRoute.Count; //  強制終了
        //Debug.Log("障害物を検知！");
    }

    #endregion
}
