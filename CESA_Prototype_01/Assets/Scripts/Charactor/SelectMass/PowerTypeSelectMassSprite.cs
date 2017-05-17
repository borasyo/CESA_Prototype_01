using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerTypeSelectMassSprite : SelectMassSprite 
{
    override protected void SpriteCheck()
    {
        int number = _charactor.GetDataNumberForDir();
        _SpRend.sprite = _defaultSprite;

        //  置ける、壊せる、何もできないを判定
        FieldObjectBase obj = FieldData.Instance.GetObjData(number);
        if (obj)
        {
            if (_charactor.GetSpecialModeFlg)
            {
                if ((obj.tag == "SandItem" || (obj.tag == "Block" && !obj.name.Contains("Fence"))) && 
                    _charactorGauge.BreakGaugeCheck())
                {
                    return;
                }
            }
            else
            {
                if (obj.tag == "SandItem" && _charactorGauge.BreakGaugeCheck())
                {
                    return;
                }
            }
        }
        else if(_charactorGauge.PutGaugeCheck())
        {
            return;
        }

        _SpRend.sprite = _notSprite;
        transform.localScale = new Vector3(1,1,1);
    }
}
