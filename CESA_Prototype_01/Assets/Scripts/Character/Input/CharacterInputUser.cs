using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterInputUser : CharacterInput
{
    protected MoveButton _moveButton = null;

    protected void Start()
    {
        Transform InputCanvas = GameObject.Find("InputCanvas").transform;

        Transform Move = InputCanvas.Find("Move");
        _moveButton = Move.GetComponent<MoveButton>();

        CreateActionEvent(InputCanvas.Find("Action").gameObject.GetComponent<EventTrigger>());
    }

    void CreateActionEvent(EventTrigger eventTrigger)
    {
        eventTrigger.triggers.Add(CreateActionEntry());
    }

    EventTrigger.Entry CreateActionEntry()
    {
        EventTrigger.Entry press = new EventTrigger.Entry();
        press.eventID = EventTriggerType.PointerDown;
        press.callback.AddListener((data) => { StartCoroutine(ActionClick()); });

        return press;
    }

    public IEnumerator ActionClick()
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
            _IsForawrd = Input.GetKey(KeyCode.W);
            _IsBack = Input.GetKey(KeyCode.S);
            _IsRight = Input.GetKey(KeyCode.D);
            _IsLeft = Input.GetKey(KeyCode.A);
            _IsPut = Input.GetKeyDown(KeyCode.R);
            _IsBreak = Input.GetKeyDown(KeyCode.T);
        }
    }
}
