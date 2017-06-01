using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalanceTypeOnline : CharacterOnline
{
    [SerializeField]
    int _nNum = 3;

    public override void Init(int level)
    {
        _charaType = eCharaType.BALANCE;
        base.Init(level);
    }

    override protected void ItemPut()
    {
        if (!_charactorGauge.PutGaugeCheck() || !_charactorInput.GetActionInput(eAction.PUT))
            return;
        
        int[] dirNumbers = GetNumberList();
        for (int i = 0; i < dirNumbers.Length; i++)
        {
            if (dirNumbers[i] < 0 || GameScaler.GetRange < dirNumbers[i])
                continue;

            FieldObjectBase obj = FieldData.Instance.GetObjData(dirNumbers[i]);
            if (obj)
            {
                if (i == 0)  //  目の前に置けない場合は置けない
                    return;
                continue;
            }

            Vector3 pos = GetPosForNumber(dirNumbers[i]);
            photonView.RPC("OnlineItemPut", PhotonTargets.All, pos, dirNumbers[i], i == dirNumbers.Length - 1);
        }
    }

    int[] GetNumberList()
    {
        int[] numbers;

        if (_IsSpecialMode)
        {
            numbers = new int[_nNum];
        }
        else
        {
            numbers = new int[1];
        }

        numbers[0] = GetDataNumberForDir();

        if (!_IsSpecialMode)
            return numbers;

        switch (_nowDirection)
        {
            case eDirection.FORWARD:
            case eDirection.BACK:
                numbers[1] = numbers[0] - 1;
                numbers[2] = numbers[0] + 1;
                break;
            case eDirection.RIGHT:
            case eDirection.LEFT:
                numbers[1] = numbers[0] - GameScaler._nWidth;
                numbers[2] = numbers[0] + GameScaler._nWidth;
                break;
        }

        return numbers;
    }

    override public bool RunSpecialMode(bool IsRun)
    {
        if (_IsSpecialMode == IsRun)
            return false;

        _IsSpecialMode = IsRun;
        //  今のところなし
        if (IsRun)
        {

        }
        else
        {

        }
        return true;
    }

}
