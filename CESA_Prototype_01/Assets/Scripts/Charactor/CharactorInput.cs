using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorInput : MonoBehaviour
{
    private bool _IsForawrd = false;
    private bool _IsBack = false;
    private bool _IsRight = false;
    private bool _IsLeft = false;

    private bool _IsPut = false;
    private bool _IsBreak = false;
	
    void Update()
    {
        if (transform.name.Contains("1P")) 
        {
            _IsForawrd = Input.GetKey(KeyCode.W);
            _IsBack    = Input.GetKey(KeyCode.S);
            _IsRight   = Input.GetKey(KeyCode.D);
            _IsLeft    = Input.GetKey(KeyCode.A); 
            _IsPut   = Input.GetKeyDown(KeyCode.R);
            _IsBreak = Input.GetKeyDown(KeyCode.T); 
        }
        else if ((transform.name.Contains("2P")))
        {
            _IsForawrd = Input.GetKey(KeyCode.UpArrow);
            _IsBack    = Input.GetKey(KeyCode.DownArrow);
            _IsRight   = Input.GetKey(KeyCode.RightArrow);
            _IsLeft    = Input.GetKey(KeyCode.LeftArrow);   
            _IsPut   = Input.GetKeyDown(KeyCode.O);
            _IsBreak = Input.GetKeyDown(KeyCode.P); 
        }
        else if ((transform.name.Contains("3P")))
        {
            _IsForawrd = Input.GetKey(KeyCode.F);
            _IsBack    = Input.GetKey(KeyCode.V);
            _IsRight   = Input.GetKey(KeyCode.B);
            _IsLeft    = Input.GetKey(KeyCode.C);   
            _IsPut   = Input.GetKeyDown(KeyCode.H);
            _IsBreak = Input.GetKeyDown(KeyCode.J); 

        }
        else if ((transform.name.Contains("4P")))
        {
            _IsForawrd = Input.GetKey(KeyCode.Alpha7);
            _IsBack    = Input.GetKey(KeyCode.U);
            _IsRight   = Input.GetKey(KeyCode.I);
            _IsLeft    = Input.GetKey(KeyCode.Y);   
            _IsPut   = Input.GetKeyDown(KeyCode.Alpha9);
            _IsBreak = Input.GetKeyDown(KeyCode.Alpha0); 
        }
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
