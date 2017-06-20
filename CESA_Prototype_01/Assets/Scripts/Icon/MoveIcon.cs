using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using UniRx;
using UniRx.Triggers;

public class MoveIcon : MonoBehaviour
{
    [SerializeField] float iconSpeed = 5.0f;	//　アイコンが移動するスピード
    [SerializeField] bool _IsWorld = true;
    Vector3 nowPos = Vector3.zero;
    Vector3 tempPos = Vector3.zero;

    bool _IsOn = false;

    void Awake()
    {
        //if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
        //    return;

        Destroy(gameObject);
    }

    void Start()
    {
        RectTransform rect = GetComponent<RectTransform>();
        //　オフセット値をアイコンのサイズの半分で設定
        Vector2 offset = new Vector2(rect.sizeDelta.x / 2, rect.sizeDelta.y / 2);
        nowPos = transform.position;

        if (_IsWorld)
        {
            nowPos = new Vector3(Screen.width / 2.0f, Screen.height / 2.0f);
        }

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                //　移動キーの入力値でアイコンの移動値を決定
                float x = Input.GetAxis("Horizontal") * iconSpeed;
                float y = Input.GetAxis("Vertical") * iconSpeed;

                //　アイコンの位置に移動値を足して更新
                nowPos += new Vector3(x, y);
                tempPos = nowPos;

                if (_IsWorld)
                {
                    nowPos.z = 10.0f;
                    Vector3 pos = Camera.main.ScreenToWorldPoint(nowPos);
                    pos.z = -200.0f;
                    rect.anchoredPosition = pos;
                }
                else
                {
                    transform.position = nowPos;
                }
            });

        Vector3 min = transform.localScale;
        Vector3 max = transform.localScale * 0.75f;
        TriangleWave<Vector3> triangleScaler = TriangleWaveFactory.Vector3(min, max, 0.5f);
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                triangleScaler.Progress();
                transform.localScale = triangleScaler.CurrentValue;
            });

        this.ObserveEveryValueChanged(_ => Input.GetButtonUp("Action"))
            .Where(_ => Input.GetButtonUp("Action"))
            .Subscribe(_ =>
            {
                _IsOn = false;
            });
    }

    void OnTriggerStay(Collider collider)
    {
        if (!Input.GetButtonDown("Action"))
            return;

        if (_IsOn)
            return;
        _IsOn = true;

        Button button = collider.GetComponent<Button>();

        if (!button)
        {
            //Debug.LogError("Buttonでないオブジェクトと接触しています!");

            InputField inputField = collider.GetComponent<InputField>();
            if (!inputField)
                return;

            PointerEventData inputData = new PointerEventData(FindObjectOfType<EventSystem>());
            inputField.OnPointerClick(inputData);
            return;
        }
        //Debug.Log("クリック : " + button.name);

        PointerEventData data = new PointerEventData(FindObjectOfType<EventSystem>());
        button.OnPointerClick(data);
    }

    void OnBecameInvisible()
    {
        Debug.Log("asd");
        nowPos = tempPos;
    }
}
