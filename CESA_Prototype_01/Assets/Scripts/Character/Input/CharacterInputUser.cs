using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterInputUser : CharacterInput
{
    protected MoveButton _moveButton = null;

    void Start()
    {
        Transform InputCanvas = GameObject.Find("InputCanvas").transform;

        Transform Move = InputCanvas.Find("Move");
        _moveButton = Move.GetComponent<MoveButton>();

        CreateActionEvent(InputCanvas.Find("Put").gameObject.GetComponent<EventTrigger>(), Character.eAction.PUT);
        CreateActionEvent(InputCanvas.Find("Break").gameObject.GetComponent<EventTrigger>(), Character.eAction.BREAK);
    }

    void CreateActionEvent(EventTrigger eventTrigger, Character.eAction act)
    {
        eventTrigger.triggers.Add(CreateActionEntry(act));
    }

    EventTrigger.Entry CreateActionEntry(Character.eAction act)
    {
        EventTrigger.Entry press = new EventTrigger.Entry();
        press.eventID = EventTriggerType.PointerDown;
        press.callback.AddListener((data) => { StartCoroutine(ActionClick(act)); });

        return press;
    }

    public IEnumerator ActionClick(Character.eAction act)
    {
        switch(act)
        {
            case Character.eAction.PUT:
                _IsPut = true;
                break;
            case Character.eAction.BREAK:
                _IsBreak = true;
                break;
        }

        yield return null;

        switch (act)
        {
            case Character.eAction.PUT:
                _IsPut = false;
                break;
            case Character.eAction.BREAK:
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
            _IsForawrd = Input.GetKey(KeyCode.W);
            _IsBack = Input.GetKey(KeyCode.S);
            _IsRight = Input.GetKey(KeyCode.D);
            _IsLeft = Input.GetKey(KeyCode.A);
            _IsPut = Input.GetKeyDown(KeyCode.R);
            _IsBreak = Input.GetKeyDown(KeyCode.T);
        }

        //  同期する
        //photonView.RPC("SetMove", PhotonTargets.All, _IsForawrd, _IsBack, _IsRight, _IsLeft);
        //photonView.RPC("SetAction", PhotonTargets.All, _IsPut, _IsBreak);
    }
}
