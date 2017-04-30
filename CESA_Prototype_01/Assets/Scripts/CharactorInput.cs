using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorInput : MonoBehaviour
{
    public enum eDirection
    {
        FORWARD = 0,
        BACK,
        RIGHT,
        LEFT,

        MAX,
    };

    [SerializeField] bool _IsPlayer = true;

    private bool _IsForawrd = false;
    private bool _IsBack = false;
    private bool _IsRight = false;
    private bool _IsLeft = false;
	
    void Update()
    {
        if(_IsPlayer) 
        {
            _IsForawrd = Input.GetKey(KeyCode.W);
            _IsBack    = Input.GetKey(KeyCode.S);
            _IsRight   = Input.GetKey(KeyCode.D);
            _IsLeft    = Input.GetKey(KeyCode.A); 
        }
        else 
        {
            _IsForawrd = Input.GetKey(KeyCode.UpArrow);
            _IsBack    = Input.GetKey(KeyCode.DownArrow);
            _IsRight   = Input.GetKey(KeyCode.RightArrow);
            _IsLeft    = Input.GetKey(KeyCode.LeftArrow);   
        }
    }

    public bool GetInput(eDirection dir)
    {
        switch(dir)
        {
            case eDirection.FORWARD:
                return _IsForawrd;
            case eDirection.BACK:
                return _IsBack;
            case eDirection.RIGHT:
                return _IsRight;
            case eDirection.LEFT:
                return _IsLeft;
            default:
                break;
        }

        return false;
    }
}
