using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechnicalTypeMoveAI : MoveAI
{
    protected override int RandomNullMass()
    {
        List<int> nullMassList = new List<int>();
        FieldObjectBase[] objList = FieldData.Instance.GetObjDataArray;
        if (!_character.GetSpecialModeFlg)
        {
            for (int i = 0; i < objList.Length; i++)
            {
                if (_enemyAI._DistanceDatas[i]._nDistance > _nMoveRange)
                    continue;

                if (objList[i])
                    continue;

                SandItem.eType type = SandData.Instance.GetSandDataList[i];
                if (type != SandItem.eType.MAX && !FieldDataChecker.Instance.TypeCheck(name, type))
                    continue;

                nullMassList.Add(i);
            }
        }
        else
        {
            for (int i = 0; i < objList.Length; i++)
            {
                if (_enemyAI._DistanceDatas[i]._nDistance > _nMoveRange)
                    continue;

                //if (!objList[i] || objList[i].tag != "SandItem")
                //    continue;   //  蹴れるオブジェクトのみ

                if (objList[i] && objList[i].tag != "SandItem")
                    continue; //  蹴れるオブジェクトとNullマス

                SandItem.eType type = SandData.Instance.GetSandDataList[i];
                if (type != SandItem.eType.MAX && !FieldDataChecker.Instance.TypeCheck(name, type))
                    continue;

                nullMassList.Add(i);
            }
        }

        if (nullMassList.Count <= 0)
            return -1;

        return nullMassList[Random.Range(0, nullMassList.Count)];
    }

    protected override bool CheckObstacle()
    {
        if (RouteOver)
            return false;

        int idx = _astar.GetRoute[_nNowRoute];

        //  行先に障害物がないかチェック
        if (!FieldDataChecker.Instance.CheckObstacleObj(idx, gameObject) &&
            !FieldDataChecker.Instance.SandCheck(idx, name))
            return false;


        //  特殊時はさらに先も検索し、無ければ蹴れるので進む
        if (_character.GetSpecialModeFlg && !FieldDataChecker.Instance.CheckObstacleObj(idx + (idx - _nNowNumber), gameObject))
            return false;

        _state = eState.STOP;
        _nNowRoute = _astar.GetRoute.Count; //  強制終了
        Debug.Log("障害物を検知！");
        return true;
    }
}
