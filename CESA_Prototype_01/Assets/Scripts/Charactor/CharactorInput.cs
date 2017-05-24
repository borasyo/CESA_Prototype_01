using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorInput : MonoBehaviour
{
    protected bool _IsForawrd = false;
    protected bool _IsBack = false;
    protected bool _IsRight = false;
    protected bool _IsLeft = false;

    protected bool _IsPut = false;
    protected bool _IsBreak = false;
	
    void Update()
    {
        InputCheck();
    }

    virtual protected void InputCheck()
    {      
        //  継承先で記述
    }

    public bool GetMoveInput(Charactor.eDirection dir)
    {
        switch(dir)
        {
            case Charactor.eDirection.FORWARD:
                return _IsForawrd;
            case Charactor.eDirection.BACK:
                return _IsBack;
            case Charactor.eDirection.RIGHT:
                return _IsRight;
            case Charactor.eDirection.LEFT:
                return _IsLeft;
            default:
                break;
        }

        return false;
    }

    public bool GetActionInput(Charactor.eAction act)
    {
        switch(act)
        {
            case Charactor.eAction.PUT:
                return _IsPut;
            case Charactor.eAction.BREAK:
                return _IsBreak;
            default:
                break;
        }

        return false;
    }
}
