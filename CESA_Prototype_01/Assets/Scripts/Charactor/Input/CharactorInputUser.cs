using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharactorInputUser : CharactorInput
{
    MoveButton _moveButton = null;

    void Start()
    {
        Transform InputCanvas = GameObject.Find("InputCanvas").transform;

        Transform Move = InputCanvas.Find("Move");
        _moveButton = Move.GetComponent<MoveButton>();

        CreateActionEvent(InputCanvas.Find("Put").gameObject.GetComponent<EventTrigger>(), Charactor.eAction.PUT);
        CreateActionEvent(InputCanvas.Find("Break").gameObject.GetComponent<EventTrigger>(), Charactor.eAction.BREAK);
    }

    void CreateActionEvent(EventTrigger eventTrigger, Charactor.eAction act)
    {
        eventTrigger.triggers.Add(CreateActionEntry(act));
    }

    EventTrigger.Entry CreateActionEntry(Charactor.eAction act)
    {
        EventTrigger.Entry press = new EventTrigger.Entry();
        press.eventID = EventTriggerType.PointerDown;
        press.callback.AddListener((data) => { StartCoroutine(ActionClick(act)); });

        return press;
    }

    public IEnumerator ActionClick(Charactor.eAction act)
    {
        switch(act)
        {
            case Charactor.eAction.PUT:
                _IsPut = true;
                break;
            case Charactor.eAction.BREAK:
                _IsBreak = true;
                break;
        }

        yield return null;

        switch (act)
        {
            case Charactor.eAction.PUT:
                _IsPut = false;
                break;
            case Charactor.eAction.BREAK:
                _IsBreak = false;
                break;
        }
    }

    override protected void InputCheck()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (!_moveButton.IsActiveAndMove)
            {
                _IsForawrd = _IsBack = _IsRight = _IsLeft = false;
                return;
            }

            int angle = (int)(_moveButton.GetMoveAngle);
            _IsForawrd = (angle <  135 && angle >=   45);
            _IsBack    = (angle <  -45 && angle >= -135);
            _IsRight   = (angle <   45 && angle >=  -45);
            _IsLeft    = (angle < -135 || angle >=  135);
        }
        else
        {
            if (transform.name.Contains("1P"))
            {
                _IsForawrd = Input.GetKey(KeyCode.W);
                _IsBack = Input.GetKey(KeyCode.S);
                _IsRight = Input.GetKey(KeyCode.D);
                _IsLeft = Input.GetKey(KeyCode.A);
                _IsPut = Input.GetKeyDown(KeyCode.R);
                _IsBreak = Input.GetKeyDown(KeyCode.T);
            }
            else if ((transform.name.Contains("2P")))
            {
                _IsForawrd = Input.GetKey(KeyCode.UpArrow);
                _IsBack = Input.GetKey(KeyCode.DownArrow);
                _IsRight = Input.GetKey(KeyCode.RightArrow);
                _IsLeft = Input.GetKey(KeyCode.LeftArrow);
                _IsPut = Input.GetKeyDown(KeyCode.O);
                _IsBreak = Input.GetKeyDown(KeyCode.P);
            }
            else if ((transform.name.Contains("3P")))
            {
                _IsForawrd = Input.GetKey(KeyCode.F);
                _IsBack = Input.GetKey(KeyCode.V);
                _IsRight = Input.GetKey(KeyCode.B);
                _IsLeft = Input.GetKey(KeyCode.C);
                _IsPut = Input.GetKeyDown(KeyCode.H);
                _IsBreak = Input.GetKeyDown(KeyCode.J);

            }
            else if ((transform.name.Contains("4P")))
            {
                _IsForawrd = Input.GetKey(KeyCode.Alpha7);
                _IsBack = Input.GetKey(KeyCode.U);
                _IsRight = Input.GetKey(KeyCode.I);
                _IsLeft = Input.GetKey(KeyCode.Y);
                _IsPut = Input.GetKeyDown(KeyCode.Alpha9);
                _IsBreak = Input.GetKeyDown(KeyCode.Alpha0);
            }
        }
    }
}
