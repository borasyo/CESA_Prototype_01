using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInput : Photon.MonoBehaviour
{
    protected bool _IsForawrd = false;
    protected bool _IsBack = false;
    protected bool _IsRight = false;
    protected bool _IsLeft = false;

    protected bool _IsPut = false;
    protected bool _IsBreak = false;
	
    void Update()
    {
        if (!photonView.isMine)
            return;

        InputCheck();
    }

    virtual protected void InputCheck()
    {      
        //  継承先で記述
    }

    public bool GetMoveInput(Character.eDirection dir)
    {
        switch(dir)
        {
            case Character.eDirection.FORWARD:
                return _IsForawrd;
            case Character.eDirection.BACK:
                return _IsBack;
            case Character.eDirection.RIGHT:
                return _IsRight;
            case Character.eDirection.LEFT:
                return _IsLeft;
            default:
                break;
        }

        return false;
    }

    public bool GetActionInput(Character.eAction act)
    {
        switch(act)
        {
            case Character.eAction.PUT:
                return _IsPut;
            case Character.eAction.BREAK:
                return _IsBreak;
            default:
                break;
        }

        return false;
    }
}
