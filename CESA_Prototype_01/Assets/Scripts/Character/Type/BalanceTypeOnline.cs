using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalanceTypeOnline : CharacterOnline
{
    const int _nNum = 2;

    public override void Init(int level)
    {
        _charaType = eCharaType.BALANCE;
        base.Init(level);
    }

    override protected void ItemPut()
    {
        if (!_charactorGauge.PutGaugeCheck() || !_charactorInput.GetActionInput(eAction.PUT))
            return;

        if (FieldData.Instance.GetObjData(GetDataNumberForDir()))
            return;

        int[] dirNumbers = GetNumberList();
        for (int i = 0; i < dirNumbers.Length; i++)
        {
            if (dirNumbers[i] < 0 || GameScaler.GetRange < dirNumbers[i])
                continue;

            FieldObjectBase obj = FieldData.Instance.GetObjData(dirNumbers[i]);
            if (obj) // && obj.gameObject != gameObject)
            {
                //if(i == 0)  //  目の前に置けない場合は置けない
                //    return;
                continue;
            }

            _animator.SetBool("Put", true);
            Vector3 pos = GetPosForNumber(dirNumbers[i]);
            photonView.RPC("OnlineItemPut", PhotonTargets.All, pos, dirNumbers[i], i == dirNumbers.Length - 1);
        }
    }

    int[] GetNumberList()
    {
        int[] numbers;

        if (!_IsSpecialMode)
        {
            numbers = new int[1];
            numbers[0] = GetDataNumberForDir();
            return numbers;
        }

        numbers = new int[_nNum];

        int dirNumber = GetDataNumberForDir();
        switch (_nowDirection)
        {
            case eDirection.FORWARD:
            case eDirection.BACK:
                numbers[0] = dirNumber - 1;
                numbers[1] = dirNumber + 1;
                break;
            case eDirection.RIGHT:
            case eDirection.LEFT:
                numbers[0] = dirNumber - GameScaler._nWidth;
                numbers[1] = dirNumber + GameScaler._nWidth;
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
