using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAI : MonoBehaviour
{ 
    enum eState
    {
        WAIT = 0,
        WALK,
        STOP,   //  障害物を検知

        MAX,
    }
    eState _state = eState.WAIT;
    //  状態を外部から取得
    public bool StateWait { get { return _state == eState.WAIT; } }
    public bool StateWalk { get { return _state == eState.WALK; } }
    public bool StateStop { get { return _state == eState.STOP; } } //  行きたい箇所への経路が絶たれている状態

    AStar _astar = null;
    FieldObjectBase _fieldObjBase = null;

    int _nNowRoute = 0;
    int _nNowNumber = 0;
    
    Vector3 _oldNumberPos = Vector3.zero;

    public bool NowMove { get { return _nNowRoute != _astar.GetRoute.Count; } }
    public int GetTarget {
        get
        {
            if (_state == eState.WAIT || _astar.GetRoute.Count <= 0)
                return -1;

            return _astar.GetRoute[_astar.GetRoute.Count - 1];
        }
    }
    
    public void Init (FieldObjectBase fieldObjBase)
    {
        _astar = gameObject.AddComponent<AStar>();
        _fieldObjBase = fieldObjBase;
    }

    void Update ()
    {
        if (DistanceCheck())
        {
            _nNowRoute++;
            NumberUpdate();
        }

        if(StateStop && GetTarget >= 0)
        {
            SearchRoute(GetTarget);
        }
    }

    #region Search 

    public void SearchRoute(int nTarget)
    {
        _nNowRoute = 1;
        NumberUpdate();

        if (!_astar.Search(nTarget))
        {
            _state = eState.STOP;
            _nNowRoute = _astar.GetRoute.Count;
            Debug.Log("その場所へのルートはありません");
            return;
        }

        _state = eState.WALK;
    }

    #endregion

    public Charactor.eDirection GetMoveData()
    {
        if (!NowMove || StateStop)
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

        Debug.LogError("経路に問題がある恐れがあります");
        return Charactor.eDirection.MAX;
    }


    void NumberUpdate()
    {
        _nNowNumber = _fieldObjBase.GetDataNumber();
        _oldNumberPos = _fieldObjBase.GetPosForNumber();
    }

    bool DistanceCheck()
    {
        if (!NowMove)
            return false;

        if (Mathf.Abs(_oldNumberPos.x - transform.position.x) >= GameScaler._fScale)
            return true;

        if (Mathf.Abs(_oldNumberPos.z - transform.position.z) >= GameScaler._fScale)
            return true;

        return false;
    }
    
    bool CheckObstacle()
    {   
        int idx = _astar.GetRoute[_nNowRoute];

        if (!FieldDataChecker.Instance.CheckObstacleObj(idx, this.gameObject) &&
            !FieldDataChecker.Instance.SandCheck(idx, this.name))
            return false;

        _state = eState.STOP;
        _nNowRoute = _astar.GetRoute.Count; //  強制終了
        Debug.Log("障害物を検知！");
        return true;
    }
}
