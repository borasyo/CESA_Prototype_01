using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechnicalType : Charactor 
{
    override protected void ItemPut()
    {
        if (!_charactorGauge.PutGaugeCheck() || 
            !_charactorInput.GetActionInput(eAction.PUT))
            return;

        int dirNumber = GetDataNumberForDir();
        if (dirNumber < 0 || GameScaler._nWidth * GameScaler._nHeight < dirNumber)
            return;

        FieldObjectBase obj = FieldData.Instance.GetObjData(dirNumber);
        if (obj)
            return;

        GameObject item = (GameObject)Instantiate(_sandItem, GetPosForNumber(dirNumber), Quaternion.identity);
        item.AddComponent<DelayPut>().Init(dirNumber);
        _charactorGauge.PutAction();
    }

    override public void RunSpecialMode(bool IsRun)
    {
        _IsSpecialMode = IsRun;
        if (IsRun)
        {
            
        }
        else
        {
            
        }
    }
}
