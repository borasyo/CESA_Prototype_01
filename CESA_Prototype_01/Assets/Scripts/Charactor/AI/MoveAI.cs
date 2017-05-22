using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MoveAI : MonoBehaviour
{ 
    enum eState
    {
        WAIT = 0,
        WALK,
        LAST,   //  最後に指定した方向を向かせる
        STOP,   //  障害物を検知

        MAX,
    }
    [SerializeField] eState _state = eState.WAIT;
    //  状態を外部から取得
    public bool StateWait { get { return _state == eState.WAIT; } }
    public bool StateWalk { get { return _state == eState.WALK || _state == eState.LAST; } }
    public bool StateStop { get { return _state == eState.STOP; } } //  行きたい箇所への経路が絶たれている状態

    AStar _astar = null;
    FieldObjectBase _fieldObjBase = null;

    int _nNowRoute = 0;
    int _nNowNumber = 0;

    Charactor.eDirection _LastDirection = Charactor.eDirection.MAX; //  移動終了時の向きを指定
    Vector3 _oldNumberPos = Vector3.zero;
    
    public int GetTarget {
        get
        {
            if (_state == eState.WAIT || _astar.GetRoute.Count <= 0)
                return -1;

            return _astar.GetRoute[_astar.GetRoute.Count - 1];
        }
    }
    
    void Awake ()
    {
        _astar = gameObject.AddComponent<AStar>();
        _fieldObjBase = GetComponent<FieldObjectBase>();
    }

    void Update ()
    {
        MoveUpdate();

        //  再検索
        /*if(StateStop && GetTarget >= 0)
        {
            SearchRoute(GetTarget);
        }*/
    }

    #region AI

    public bool OnMove()
    {
        return SearchRoute(RandomNullMass());
    }

    int RandomNullMass()
    {
        List<int> nullMassList = new List<int>();
        FieldObjectBase[] objList = FieldData.Instance.GetObjDataArray;
        for (int i = 0; i < objList.Length; i++)
        {
            if (objList[i])
                continue;

            List<SandData.tSandData> dataList = SandData.Instance.GetSandDataList.FindAll(_ => _._number == i);
            if (dataList.Count > 0 && !FieldDataChecker.Instance.TypeCheck(dataList[0]._Type))
                continue;

            nullMassList.Add(i);
        }

        if (nullMassList.Count <= 0)
            return -1;

        return nullMassList[Random.Range(0, nullMassList.Count)];
    }

    #endregion

    #region Search 

    public bool SearchRoute(int nTarget, int nArrive = 0)
    {
        if (nTarget < 0)
            return false;

        _nNowRoute = 1;
        NumberUpdate();
        _LastDirection = Charactor.eDirection.MAX;

        if (!_astar.Search(nTarget))
        {
            _state = eState.STOP;
            _nNowRoute = _astar.GetRoute.Count;
            Debug.Log("その場所へのルートはありません");
            return false;
        }

        _state = eState.WALK;
        Debug.Log("対象マス : " + nTarget);
        
        if (nArrive <= 0)
            return true;

        //  移動後にアクションを行う場合の処理
        SetDirection(nArrive);

        // 目の前の場合は向いて終了
        if (_nNowRoute == _astar.GetRoute.Count)
            _state = eState.LAST;

        return true;
    }

    //  移動後がアクションの場合、１マス前を終了とする
    void SetDirection(int nArrive)
    {
        if (nArrive > _astar.GetRoute.Count)
            return;

        int max = _astar.GetRoute.Count - nArrive;
        int value = _astar.GetRoute[max - 1] - _astar.GetRoute[max];
        if (value == 1)  
            _LastDirection = Charactor.eDirection.LEFT;
        else if (value == -1)
            _LastDirection = Charactor.eDirection.RIGHT;
        else if (value ==  GameScaler._nWidth)
            _LastDirection = Charactor.eDirection.BACK;
        else if (value == -GameScaler._nWidth)
            _LastDirection = Charactor.eDirection.FORWARD;

        // 最後は削除
        for (int i = 0; i < nArrive; i++)
        {
            _astar.GetRoute.RemoveAt(_astar.GetRoute.Count - 1);
        }
    }

    #endregion

    #region Move

    void MoveUpdate()
    {
        if (!StateWalk)
            return;

        if (DistanceCheck())
        {
            transform.position = _fieldObjBase.GetPosForNumber(_astar.GetRoute[_nNowRoute]);    //  pos補正
            _nNowRoute++;
            NumberUpdate();

            // LASTの向きが指定されているかをチェック
            if (_nNowRoute == _astar.GetRoute.Count)
                _state = _LastDirection != Charactor.eDirection.MAX ? eState.LAST : eState.WAIT;
        }
    }

    public Charactor.eDirection GetMoveData()
    {
        if(_state == eState.LAST)
        {
            _state = eState.WAIT;
            return _LastDirection;
        }

        if (!StateWalk || StateStop)
            return Charactor.eDirection.MAX;

        if (CheckObstacle())
            return Charactor.eDirection.MAX;
      
        int dis = _astar.GetRoute[_nNowRoute] - _nNowNumber;
        
        if (dis == GameScaler._nWidth)
            return Charactor.eDirection.FORWARD;
        else if (dis == -GameScaler._nWidth)
            return Charactor.eDirection.BACK;
        else if (dis == 1)
            return Charactor.eDirection.RIGHT;
        else if (dis == -1)
            return Charactor.eDirection.LEFT;

        Debug.LogError("経路に問題がある恐れがあります。" + " 行先 : " + _astar.GetRoute[_nNowRoute] + ", 現地 : " + _nNowNumber);
        return Charactor.eDirection.MAX;
    }

    void NumberUpdate()
    {
        if (!_fieldObjBase)
            return;

        _nNowNumber = _fieldObjBase.GetDataNumber();
        _oldNumberPos = _fieldObjBase.GetPosForNumber();
    }

    bool DistanceCheck()
    {
        if (Mathf.Abs(_oldNumberPos.x - transform.position.x) >= GameScaler._fScale)
            return true;

        if (Mathf.Abs(_oldNumberPos.z - transform.position.z) >= GameScaler._fScale)
            return true;

        return false;
    }
    
    bool CheckObstacle()
    {
        if (_nNowRoute >= _astar.GetRoute.Count)
            return false;

        int idx = _astar.GetRoute[_nNowRoute];

        if (!FieldDataChecker.Instance.CheckObstacleObj(idx, this.gameObject) &&
            !FieldDataChecker.Instance.SandCheck(idx, this.name))
            return false;

        _state = eState.STOP;
        _nNowRoute = _astar.GetRoute.Count; //  強制終了
        Debug.Log("障害物を検知！");
        return true;
    }

    #endregion
}
