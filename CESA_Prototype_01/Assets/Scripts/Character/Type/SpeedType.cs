using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedType : Character
{
    [SerializeField] bool _IsAuthBlock = false;
    float _fInitSpeed = 0.0f;

    public override void Init(int level)
    {
        _charaType = eCharaType.SPEED;
        base.Init(level);

        _fInitSpeed = _moveAmount_Sec;
    }

    override protected bool MoveCheck(eDirection dir)
    {
        if (!_charactorInput.GetMoveInput(dir))
            return false;

        _nowDirection = dir;
        int number = GetDataNumber();

        int check = 0;
        switch(dir) 
        {
            case eDirection.FORWARD:
                check = number + GameScaler._nWidth;
                break;
            case eDirection.BACK:
                check = number - GameScaler._nWidth;
                break;
            case eDirection.RIGHT:
                check = number + 1;
                break;
            case eDirection.LEFT:
                check = number - 1;
                break;
        }

        if (check < 0 || GameScaler._nWidth * GameScaler._nHeight <= check)
            return false;

        FieldObjectBase checkData = FieldData.Instance.GetObjData(check);

        if (_IsSpecialMode)
        {
            if (checkData && checkData.tag == "Block" && (!_IsAuthBlock || checkData.name.Contains("Fence")))
            {
                return DistanceCheck(checkData);
            }
        }
        else
        {
            if (checkData && checkData.tag != "Character")
            {
                return DistanceCheck(checkData);
            }
        }

        _IsNowMove = true;
        return true;
    }

    bool DistanceCheck(FieldObjectBase checkData)
    {
        float distance = 0.0f;
        switch (_nowDirection)
        {
            case eDirection.FORWARD:
                distance = NormalizePosition(checkData.transform.position).z - NormalizePosition().z;
                break;
            case eDirection.BACK:
                distance = NormalizePosition().z - NormalizePosition(checkData.transform.position).z;
                break;
            case eDirection.RIGHT:
                distance = NormalizePosition(checkData.transform.position).x - NormalizePosition().x;
                break;
            case eDirection.LEFT:
                distance = NormalizePosition().x - NormalizePosition(checkData.transform.position).x;
                break;
        }

        if (distance <= 1.0f)
            return false;

        return true;
    }
    protected override void NumberUpdate()
    {
        int nowNumber = GetDataNumber();

        if(_IsSpecialMode && !FieldData.Instance.GetObjData(nowNumber))
            FieldData.Instance.SetObjData(this, nowNumber);

        if (_nOldNumber == nowNumber)
            return;

        FieldData.Instance.SetObjData(null, _nOldNumber);

        if (FieldData.Instance.GetObjData(nowNumber))
            return;

        FieldData.Instance.SetObjData(this, nowNumber);
    }

    override public bool RunSpecialMode(bool IsRun)
    {
        if (_IsSpecialMode == IsRun)
            return false;

        _IsSpecialMode = IsRun;
        if (IsRun)
        {
            SetSpeed(_fInitSpeed * 2.0f);
        }
        else
        {
            SetSpeed(_fInitSpeed);
        }
        return true;
    }
}
