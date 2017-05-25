using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PowerTypeBreakAI : BreakAI {

    Charactor _charactor = null;

    public override void Init(int level)
    {
        _charactor = GetComponent<Charactor>();
        base.Init(level);
    }

    protected override bool CharaBreak(string player, bool isNear)
    {
        List<FieldObjectBase> dataList = new List<FieldObjectBase>();
        if (_charactor.GetSpecialModeFlg)
            dataList = FieldData.Instance.GetObjDataArray.Where(x => x && ((x.tag == "SandItem" && x.name.Contains(player) && x.GetSandType() != SandItem.eType.MAX) || (x.tag == "Block" && !x.name.Contains("Fence")))).ToList();
        else
            dataList = FieldData.Instance.GetObjDataArray.Where(x => x && x.tag == "SandItem" && x.name.Contains(player) && x.GetSandType() != SandItem.eType.MAX).ToList();

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

    protected override int RandomBreakMass()
    {
        FieldObjectBase[] sandItemList = null;

        if(_charactor.GetSpecialModeFlg)
            sandItemList = FieldData.Instance.GetObjDataArray.Where(x => x && ((x.tag == "SandItem" &&  x.GetSandType() != SandItem.eType.MAX) || (x.tag == "Block" && !x.name.Contains("Fence")))).ToArray();
        else
            sandItemList = FieldData.Instance.GetObjDataArray.Where(x => x && x.tag == "SandItem" && x.GetSandType() != SandItem.eType.MAX).ToArray();

        if (sandItemList.Length <= 0)
            return -1;

        return sandItemList[Random.Range(0, sandItemList.Length)].GetDataNumber();
    }
}
