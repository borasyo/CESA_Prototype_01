using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using UniRx;
using UniRx.Triggers;

public class CharacterInputUser : CharacterInput
{
    protected MoveButton _moveButton = null;

    protected void Start()
    {
        if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
            return;

        Transform InputCanvas = GameObject.Find("InputCanvas").transform;

        Transform Move = InputCanvas.Find("Move");
        _moveButton = Move.GetComponent<MoveButton>();

        this.ObserveEveryValueChanged(_ => Input.touchCount)
            .Where(_ => Input.touchCount > 0)
            .Subscribe(_ =>
            {
                if (Time.timeScale <= 0.0f)
                    return;

                for(int i = 0; i < Input.touchCount; i++)
                { 
                    if (Input.GetTouch(i).phase == TouchPhase.Began && 
                        Input.GetTouch(i).position.x >= Screen.width / 2.0f && 
                        Input.GetTouch(i).position.y <= Screen.height / 1.5f)
                    {
                        StartCoroutine(ActionClick());
                        return;
                    }
                }
            });
    }

    public virtual IEnumerator ActionClick()
    {
        _IsPut = true;
        _IsBreak = true;

        yield return null;

        _IsPut = false;
        _IsBreak = false;
    }

    override protected void InputCheck()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (!_moveButton.IsActiveAndMove)
            {
                _IsForawrd = _IsBack = _IsRight = _IsLeft = false;
                return;
            }

            int angle = (int)(_moveButton.GetMoveAngle);
            _IsForawrd = (angle < 135 && angle >= 45);
            _IsBack = (angle < -45 && angle >= -135);
            _IsRight = (angle < 45 && angle >= -45);
            _IsLeft = (angle < -135 || angle >= 135);
        }
        else
        {
#if DEBUG
            _IsForawrd = Input.GetKey(KeyCode.RightShift) ? Input.GetKeyDown(KeyCode.W) : Input.GetKey(KeyCode.W);
            _IsBack    = Input.GetKey(KeyCode.RightShift) ? Input.GetKeyDown(KeyCode.S) : Input.GetKey(KeyCode.S);
            _IsRight   = Input.GetKey(KeyCode.RightShift) ? Input.GetKeyDown(KeyCode.D) : Input.GetKey(KeyCode.D);
            _IsLeft    = Input.GetKey(KeyCode.RightShift) ? Input.GetKeyDown(KeyCode.A) : Input.GetKey(KeyCode.A);
#else
            _IsForawrd = (Input.GetAxisRaw("Vertical") >= 1.0f);// Input.GetKey(KeyCode.W);
            _IsBack = (Input.GetAxisRaw("Vertical") <= -1.0f);  //Input.GetKey(KeyCode.S);
            _IsRight = (Input.GetAxisRaw("Horizontal") >= 1.0f); //Input.GetKey(KeyCode.D);
            _IsLeft = (Input.GetAxisRaw("Horizontal") <= -1.0f); //Input.GetKey(KeyCode.A);
#endif
            _IsPut = _IsBreak = Input.GetButtonDown("Action"); // Input.GetKeyDown(KeyCode.T);
        }
    }
}
