using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechnicalTypeMoveAI : MoveAI
{
    public override int RandomNullMass(int moveRange = 0)
    {
        if (moveRange == 0)
            moveRange = _nMoveRange;

        List<int> nullMassList = new List<int>();
        FieldObjectBase[] objList = FieldData.Instance.GetObjDataArray;
        if (!_character.GetSpecialModeFlg)
        {
            for (int i = 0; i < objList.Length; i++)
            {
                if (_enemyAI._DistanceDatas[i]._nDistance > moveRange)
                    continue;

                if (objList[i])
                    continue;

                SandItem.eType type = SandData.Instance.GetSandDataList[i]._type;
                if (type != SandItem.eType.MAX && !FieldDataChecker.Instance.TypeCheck(name, type))
                    continue;

                nullMassList.Add(i);
            }
        }
        else
        {
            for (int i = 0; i < objList.Length; i++)
            {
                if (_enemyAI._DistanceDatas[i]._nDistance > moveRange)
                    continue;

                //if (!objList[i] || objList[i].tag != "SandItem")
                //    continue;   //  蹴れるオブジェクトのみ

                if (objList[i] && objList[i].tag != "SandItem")
                    continue; //  蹴れるオブジェクトとNullマス

                SandItem.eType type = SandData.Instance.GetSandDataList[i]._type;
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
        FieldObjectBase obj = FieldDataChecker.Instance.CheckObstacleObj(idx, gameObject);

        //  行先に障害物がないかチェック
        if (!obj && !FieldDataChecker.Instance.SandCheck(idx, name))
            return false;

        //  特殊時はさらに先も検索し、蹴れるなら蹴る
        if (_character.GetSpecialModeFlg && KickCheck(obj, idx))
            return false;

        _state = eState.STOP;
        _nNowRoute = _astar.GetRoute.Count; //  強制終了
        //Debug.Log("障害物を検知！");
        return true;
    }

    //  目の前の障害物はキックできるかをチェック
    bool KickCheck(FieldObjectBase obj, int idx)
    {
        //  ブロックorキャラなら蹴れない
        if (obj.tag == "Block" || obj.tag == "Character")
            return false;

        //  蹴れるオブジェクトなので、その先を検索
        FieldObjectBase next = FieldData.Instance.GetObjData(idx + (idx - _nNowNumber));

        //  何かあるなら蹴っても動かないので停止
        if (next)
            return false;

        //  蹴って動かせるので蹴る
        return true;
    }
}
