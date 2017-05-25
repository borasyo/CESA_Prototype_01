using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BreakAI : MonoBehaviour
{
    EnemyAI _enemyAI;
    protected MoveAI _moveAI;
    protected FieldObjectBase _fieldObjBase = null;

    int[] _nActionRatio = null;

    public virtual void Init(int level)
    { 
        _enemyAI = GetComponent<EnemyAI>();
        _moveAI = GetComponent<MoveAI>();
        _fieldObjBase = GetComponent<FieldObjectBase>();

        _nActionRatio = new int[3];
        switch (level)
        {
            case 0:
                _nActionRatio[0] = 10;
                _nActionRatio[1] = 20;
                _nActionRatio[2] = 70;
                break;
            case 1:
                _nActionRatio[0] = 30;
                _nActionRatio[1] = 30;
                _nActionRatio[2] = 40;
                break;
            case 2:
                _nActionRatio[0] = 70;
                _nActionRatio[1] = 20;
                _nActionRatio[2] = 10;
                break;
        }
    }

    public bool OnBreak()
    {
        int rand = Random.Range(0, _nActionRatio.Sum());
        for (int action = 0; action < _nActionRatio.Length; action++)
        {
            rand -= _nActionRatio[action];
            if (rand > 0)
                continue;

            switch (action)
            {
                case 0:
                    return RandomBreak();
                case 1:
                    return CharaBreak(GetPlayerString(), false);
                case 2:
                    return CharaBreak(GetPlayerString(), true);
            }

            break;
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
    protected virtual bool CharaBreak(string player, bool isNear)
    {
        List<FieldObjectBase> dataList =  FieldData.Instance.GetObjDataArray.Where(x => x && x.tag == "SandItem" && x.name.Contains(player) && x.GetSandType() != SandItem.eType.MAX).ToList();
        if (dataList.Count <= 0)
            return false;

        FieldObjectBase data = null;
        if (isNear)
        {
            //  最も近い壊せる箇所を探索
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

    protected virtual　int RandomBreakMass()
    {
        FieldObjectBase[] sandItemList = FieldData.Instance.GetObjDataArray.Where(x=> x && x.tag == "SandItem" && x.GetSandType() != SandItem.eType.MAX).ToArray();

        if (sandItemList.Length <= 0)
            return -1;

        return sandItemList[Random.Range(0, sandItemList.Length)].GetDataNumber();
    }

    string GetPlayerString()
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
