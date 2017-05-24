using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class MoveButton : MonoBehaviour
{
    bool _IsActive = false;
    public bool IsActive { get { return _IsActive && Vector2.Distance(CenterPosition, Input.GetTouch(0).position) >= 100;  } }
    Vector3 CenterPosition = Vector3.zero;
    public float GetMoveAngle { get { return Mathf.Atan2(Input.GetTouch(0).position.y - CenterPosition.y, Input.GetTouch(0).position.x - CenterPosition.x) * Mathf.Rad2Deg; } }

    void Start()
    {
        RectTransform rectTrans = GetComponent<RectTransform>();
        this.ObserveEveryValueChanged(_ => Input.touchCount)
            .Where(_ => Input.touchCount <= 1)
            .Subscribe(_ =>
            {
                bool nowActive = (Input.touchCount == 1);
                if (nowActive == _IsActive)
                    return;

                if(nowActive && Input.GetTouch(0).position.x > Screen.width / 2.0f)
                    return;

                _IsActive = nowActive;
                SetActive(_IsActive);

                if(_IsActive)
                {
                    Vector3 pos = CenterPosition = Input.GetTouch(0).position;
                    CenterPosition.z = 0;
                    pos.x -= Screen.width / 2.0f;
                    pos.y -= Screen.height / 2.0f;
                    pos.z = 0;
                    rectTrans.anchoredPosition = pos;
                }
            });

        SetActive(false);
    }

    void SetActive(bool isActive)
    {
        for(int idx = 0; idx < transform.childCount; idx++)
        {
            transform.GetChild(idx).gameObject.SetActive(isActive);
        }
    }
}
