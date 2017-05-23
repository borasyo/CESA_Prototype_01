using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BreakAI : MonoBehaviour
{
    EnemyAI _enemyAI;
    MoveAI _moveAI;
    FieldObjectBase _fieldObjBase = null;

    public void Init(int level)
    { 
        _enemyAI = GetComponent<EnemyAI>();
        _moveAI = GetComponent<MoveAI>();
        _fieldObjBase = GetComponent<FieldObjectBase>();
    }

    public bool OnBreak()
    {
        switch (Random.Range(0, 3))
        {
            case 0:
                return RandomBreak();
            case 1:
                return CharaBreak("", false);
            case 2:
                return CharaBreak("", true);
        }
        return false;
    }

    #region AI

    bool RandomBreak()
    {
        int rand = RandomBreakMass();
        if (rand < 0)
            return false;

        _moveAI.SearchRoute(rand, 1);
        return true;
    }

    //  特定のキャラのアイテムを破壊
    bool CharaBreak(string player, bool isNear)
    {
        player = DebugStringPlayer();

        List<FieldObjectBase> dataList =  FieldData.Instance.GetObjDataArray.Where(_ => _ && _.tag == "SandItem" && _.name.Contains(player)).ToList();
        if (dataList.Count <= 0)
            return false;

        FieldObjectBase data = null;
        if (isNear)
        {
            //  最も近いはさめる箇所を探索
            int min = 1000;
            int nowNumber = _fieldObjBase.GetDataNumber();
            int x = nowNumber % GameScaler._nWidth;
            int z = nowNumber / GameScaler._nWidth;
            dataList = dataList.Where(_ =>
            {
                int number = _.GetDataNumber();
                int dis = Mathf.Abs(x - (number % GameScaler._nWidth)) + Mathf.Abs(z - (number / GameScaler._nWidth));
                if (dis >= min)
                    return false;

                min = dis;
                return true;
            }).ToList();
            data = dataList[dataList.Count - 1];
        }
        else
        {
            data = dataList[Random.Range(0, dataList.Count)];
        }

        _moveAI.SearchRoute(data.GetDataNumber(), 1);

        return true;
    }

    #endregion

    int RandomBreakMass()
    {
        FieldObjectBase[] sandItemList = FieldData.Instance.GetObjDataArray.Where(element => element && element.tag == "SandItem").ToArray();

        if (sandItemList.Length <= 0)
            return -1;

        return sandItemList[Random.Range(0, sandItemList.Length)].GetDataNumber();
    }

    string DebugStringPlayer()
    {
        string player = "";
        List<FieldObjectBase> charaList = FieldData.Instance.GetCharactors.Where(x => x && x != _fieldObjBase).ToList();
        if (charaList.Count <= 0)
            return "";

        FieldObjectBase chara = charaList[Random.Range(0, charaList.Count)];
        if (chara.name.Contains("1P"))
            player = "1";
        else if (chara.name.Contains("2P"))
            player = "2";
        else if (chara.name.Contains("3P"))
            player = "3";
        else if (chara.name.Contains("4P"))
            player = "4";

        return player;
    }
}
