using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorInputUser : CharactorInput
{
    override protected void InputCheck()
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
}
