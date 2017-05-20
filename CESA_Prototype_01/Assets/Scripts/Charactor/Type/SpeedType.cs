﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedType : Charactor 
{
    void Start()
    {
        _charaType = eCharaType.SPEED;
        base.Start();
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
            if (checkData && checkData.tag == "Block" && checkData.name.Contains("Fence"))
            {
                return DistanceCheck(checkData);
            }
        }
        else
        {
            if (checkData && checkData != this)
            {
                return DistanceCheck(checkData);
            }
        }

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

    override public void RunSpecialMode(bool IsRun)
    {
        _IsSpecialMode = IsRun;
        if (IsRun)
        {
            ChangeSpeed(2.0f);
        }
        else
        {
            ChangeSpeed(0.5f);
        }
    }
}
