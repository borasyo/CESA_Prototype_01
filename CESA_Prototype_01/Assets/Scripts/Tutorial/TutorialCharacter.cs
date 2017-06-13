using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCharacter : BalanceType
{
    public enum eNowAction
    {
        FORWARD = 0,
        BACK,
        RIGHT,
        LEFT,
        PUT,
        BREAK,
        MAX,
    };
    eNowAction _nowAction = eNowAction.MAX;
    public eNowAction SetNowAction { set { _nowAction = value; } }

    override protected bool MoveCheck(eDirection dir)
    {
        switch(dir)
        {
            case eDirection.FORWARD:
                if (_nowAction != eNowAction.FORWARD)
                    return false;
                break;
            case eDirection.BACK:
                if (_nowAction != eNowAction.BACK)
                    return false;
                break;
            case eDirection.RIGHT:
                if (_nowAction != eNowAction.RIGHT)
                    return false;
                break;
            case eDirection.LEFT:
                if (_nowAction != eNowAction.LEFT)
                    return false;
                break;
        }

        return base.MoveCheck(dir);
    }

    protected override void DirUpdate()
    {
        switch (_nowDirection)
        {
            case eDirection.FORWARD:
                if (_nowAction != eNowAction.FORWARD)
                    return;
                break;
            case eDirection.BACK:
                if (_nowAction != eNowAction.BACK)
                    return;
                break;
            case eDirection.RIGHT:
                if (_nowAction != eNowAction.RIGHT)
                    return;
                break;
            case eDirection.LEFT:
                if (_nowAction != eNowAction.LEFT)
                    return;
                break;
        }

        base.DirUpdate();
    }

    override protected void ItemPut()
    {
        if (_nowAction != eNowAction.PUT)
            return;

        base.ItemPut();
    }

    override protected void ItemBreak()
    {
        if (_nowAction != eNowAction.BREAK)
            return;

        base.ItemBreak();
    }
}
