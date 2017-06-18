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

                if (Input.touchCount >= 1)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Began && 
                        Input.GetTouch(0).position.x >= Screen.width / 2.0f && 
                        Input.GetTouch(0).position.y <= Screen.height / 1.5f)
                    {
                        StartCoroutine(ActionClick());
                        return;
                    }
                }
                if (Input.touchCount >= 2)
                {
                    if (Input.GetTouch(1).phase == TouchPhase.Began &&
                        Input.GetTouch(1).position.x >= Screen.width / 2.0f && 
                        Input.GetTouch(1).position.y >= Screen.height / 1.5f)
                    {
                        StartCoroutine(ActionClick());
                        return;
                    }
                }

            });
        //CreateActionEvent(InputCanvas.Find("Action").gameObject.GetComponent<EventTrigger>());
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
            _IsForawrd = (Input.GetAxisRaw("Vertical") >= 1.0f);// Input.GetKey(KeyCode.W);
            _IsBack = (Input.GetAxisRaw("Vertical") <= -1.0f);  //Input.GetKey(KeyCode.S);
            _IsRight = (Input.GetAxisRaw("Horizontal") >= 1.0f); //Input.GetKey(KeyCode.D);
            _IsLeft = (Input.GetAxisRaw("Horizontal") <= -1.0f); //Input.GetKey(KeyCode.A);
            _IsPut = _IsBreak = Input.GetButtonDown("Action"); // Input.GetKeyDown(KeyCode.T);
        }
    }
}
