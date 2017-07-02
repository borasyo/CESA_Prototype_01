using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalanceTypeSelectMass : SelectMass
{
    [SerializeField]
    int Num = 0;

    override protected void ColorCheck()
    {
        int number = _character.GetDataNumberForDir();

        if (_character.GetSpecialModeFlg && !FieldData.Instance.GetObjData(number))
        {
            //  横向き
            if (Mathf.Abs(number - _character.GetDataNumber()) == 1)
            {
                switch (Num)
                {
                    case 0:
                        number += GameScaler._nWidth;
                        break;
                    case 1:
                        number -= GameScaler._nWidth;
                        _SpRend.enabled = true;
                        break;
                }
            }
            //  縦向き
            else
            {
                switch (Num)
                {
                    case 0:
                        number += 1;
                        break;
                    case 1:
                        number -= 1;
                        _SpRend.enabled = true;
                        break;
                }
            }
        }
        else if(Num == 1)
        {
            _SpRend.enabled = false;
            return;
        }

        transform.position = GetPosForNumber(number);

        //  置ける、壊せる、何もできないを判定
        FieldObjectBase obj = FieldData.Instance.GetObjData(number);
        Color setCol = _notColor;
        if (obj)
        {
            if (obj.GetSandType() != SandItem.eType.MAX && _charactorGauge.BreakGaugeCheck() && number == _character.GetDataNumberForDir())
            {
                setCol = _breakColor;
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
