using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalanceTypeSelectMassSprite : SelectMassSprite
{
    [SerializeField]
    int Num = 0;

    override protected void SpriteCheck()
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
                        break;
                }
            }
        }
        else if (Num == 1)
        {
            return;
        }

        _SpRend.sprite = _defaultSprite;

        //  置ける、壊せる、何もできないを判定
        FieldObjectBase obj = FieldData.Instance.GetObjData(number);
        if (obj)
        {
            if (obj.GetSandType() != SandItem.eType.MAX && _charactorGauge.BreakGaugeCheck())
            {
                return;
            }
        }
        else if (_charactorGauge.PutGaugeCheck())
        {
            return;
        }

        _SpRend.sprite = _notSprite;
        //transform.localScale = new Vector3(5,5,5);
    }
}
