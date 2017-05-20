using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAI
{
    AStar _astar = null;
    FieldObjectBase _fieldObjBase = null;

    int _nNowRoute = 0;
    int _nNowNumber = 0;

    Vector3 _oldNumberPos = Vector3.zero;

    public bool NowMove { get { return _nNowRoute != _astar.GetRoute.Count; } }

    public void Start(AStar astar, FieldObjectBase fieldObjBase)
    {
        _astar = astar;
        _fieldObjBase = fieldObjBase;
    }

    public void SearchRoute(int nTarget)
    {
        _nNowRoute = 1;
        NumberUpdate();
        _astar.Search(nTarget);
    }

    public Charactor.eDirection GetMoveData()
    {
        List<int> route = _astar.GetRoute;
        if (_nNowRoute >= route.Count)
        {
            _nNowRoute = route.Count;
            return Charactor.eDirection.MAX;
        }

        if (DistanceCheck())
        {
            _nNowRoute++;
            NumberUpdate();
            return Charactor.eDirection.MAX;
        }

        int dis = _nNowNumber - route[_nNowRoute];
        //Debug.Log("今 : " + _nNowNumber + " 行先 :　" + route[_nNowRoute] + " 距離 : " + dis);
        
        if (dis >= GameScaler._nWidth)
            return Charactor.eDirection.BACK;
        else if (dis <= -GameScaler._nWidth)
            return Charactor.eDirection.FORWARD;
        else if (dis >= 1)
            return Charactor.eDirection.LEFT;
        else if (dis <= -1)
            return Charactor.eDirection.RIGHT;

        return Charactor.eDirection.MAX;
    }

    void NumberUpdate()
    {
        _nNowNumber = _fieldObjBase.GetDataNumber();
        _oldNumberPos = _fieldObjBase.GetPosForNumber();
    }

    bool DistanceCheck()
    {
        if (Mathf.Abs(_oldNumberPos.x - _fieldObjBase.transform.position.x) >= GameScaler._fScale)
            return true;

        if (Mathf.Abs(_oldNumberPos.z - _fieldObjBase.transform.position.z) >= GameScaler._fScale)
            return true;

        return false;
    }
}
