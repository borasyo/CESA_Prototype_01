﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class PowerTypeSelectMass : SelectMass 
{
    override protected void ColorCheck()
    {
        int number = _character.GetDataNumberForDir();
        transform.position = GetPosForNumber(number);

        //  置ける、壊せる、何もできないを判定
        FieldObjectBase obj = FieldData.Instance.GetObjData(number);
        Color setCol = _notColor;
        if (obj)
        {
            if (_character.GetSpecialModeFlg)
            {
                if ((obj.GetSandType() != SandItem.eType.MAX || (obj.tag == "Block" && !obj.name.Contains("Fence"))) &&
                    _charactorGauge.BreakGaugeCheck())
                {
                    setCol = _breakColor;
                }
            }
            else
            {
                if (obj.GetSandType() != SandItem.eType.MAX && _charactorGauge.BreakGaugeCheck())
                {
                    setCol = _breakColor;
                }
            }
        }
        else
        {
            if (_charactorGauge.PutGaugeCheck())
            {
                setCol = _putColor;
            }
        }

        _SpRend.color = setCol;
    }
}