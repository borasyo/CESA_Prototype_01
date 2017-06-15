using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;

public class MoveButton : MonoBehaviour
{
    bool _IsActive = false;
    public bool IsActiveAndMove { get { return _IsActive && Vector2.Distance(CenterPosition, Input.GetTouch(0).position) >= 40;  } }
    Vector3 CenterPosition = Vector3.zero;
    public float GetMoveAngle { get { return Mathf.Atan2(Input.GetTouch(0).position.y - CenterPosition.y, Input.GetTouch(0).position.x - CenterPosition.x) * Mathf.Rad2Deg; } }

    int _fingerID = 0;
    Image[] _moveVecList = null;
    [SerializeField] Color  _onVecColor = Color.white;
    [SerializeField] Color _offVecColor = Color.white;

    void Start()
    {
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        yield return null;

        _moveVecList = new Image[4];
        for (int i = 0; i < 4; i++)
            _moveVecList[i] = transform.GetChild(i).GetComponent<Image>();
        SetActive(false);

        RectTransform rectTrans = GetComponent<RectTransform>();
        Vector2 oldTouchPos = Vector2.zero;
        float oldDistance = 0.0f;

        this.ObserveEveryValueChanged(_ => Input.touchCount)
            .Where(_ => Input.touchCount <= 1)
            .Subscribe(_ =>
            {
                bool nowActive = (Input.touchCount == 1);

                if (nowActive && _IsActive && Input.GetTouch(0).fingerId != _fingerID)
                    nowActive = false;

                if (nowActive == _IsActive)
                    return;

                if(nowActive && Input.GetTouch(0).position.x > Screen.width / 2.0f)
                    return;

                _IsActive = nowActive;
                SetActive(nowActive);

                if (_IsActive)
                {
                    _fingerID = Input.GetTouch(0).fingerId;
                    CenterPosition = Input.GetTouch(0).position;
                    rectTrans.anchoredPosition = Input.GetTouch(0).position - new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
                }
            });

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                ColorChange();
            });

        this.UpdateAsObservable()
            .Where(_ => !IsActiveAndMove && Input.touchCount > 0)
            .Subscribe(_ =>
            {
                oldTouchPos = Input.GetTouch(0).position;
                oldDistance = Vector2.Distance(CenterPosition, Input.GetTouch(0).position);
            });

        this.UpdateAsObservable()
            .Where(_ => IsActiveAndMove)
            .Subscribe(_ =>
            { 
                Vector2 nowTouchPos = Input.GetTouch(0).position;
                float nowDistance = Vector2.Distance(CenterPosition, Input.GetTouch(0).position);

                if(nowDistance < oldDistance)
                {
                    oldTouchPos = nowTouchPos;
                    oldDistance = nowDistance;
                    return;
                }

                rectTrans.anchoredPosition += (nowTouchPos - oldTouchPos);// * 0.5f;
                CenterPosition += (Vector3)(nowTouchPos - oldTouchPos);
                oldTouchPos = nowTouchPos;
                oldDistance = nowDistance;
            });
    }

    void ColorChange()
    {
        if (IsActiveAndMove)
        {
            int angle = (int)GetMoveAngle;
            _moveVecList[0].color = angle < 135 && angle >= 45 ? _onVecColor : _offVecColor;
            _moveVecList[1].color = angle < -45 && angle >= -135 ? _onVecColor : _offVecColor;
            _moveVecList[2].color = angle < 45 && angle >= -45 ? _onVecColor : _offVecColor;
            _moveVecList[3].color = angle < -135 || angle >= 135 ? _onVecColor : _offVecColor;
        }
        else
        {
            foreach (Image image in _moveVecList)
                image.color = _offVecColor;
        }
    }

    void SetActive(bool isActive)
    {
        if (isActive)
        {
            foreach (Image image in _moveVecList)
                image.gameObject.SetActive(isActive);
        }
        else
        {
            foreach (Image image in _moveVecList)
            {
                image.color = _offVecColor;
                image.gameObject.SetActive(isActive);
            }
        }
    }
}
