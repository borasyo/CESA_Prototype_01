using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class CharaMass : FieldObjectBase {
    
    [SerializeField] float _fPeriod_Sec = 0.5f;

	// Use this for initialization
	void Start ()
    {
        transform.localScale *= 0.2f;

        //  座標更新処理
        this.UpdateAsObservable()
            .Where(_ => this.enabled)
            .Subscribe(_ =>
                {
                    int number = GetDataNumber(transform.parent.position); //FieldData.Instance.GetObjIdx(transform.parent.gameObject);
                    transform.position = GetPosForNumber(number);
                });

        SpriteRenderer spRend = GetComponent<SpriteRenderer>();
        spRend.color = SelectMassColor.Instance.GetBreakColor(transform.parent.name);
        CharacterGauge charaGauge = GetComponentInParent<CharacterGauge>();

        //  回転処理
        this.UpdateAsObservable()
            .Where(_ => this.enabled)
            .Subscribe(_ =>
            {
                int gauge = (int)(charaGauge.GaugePercent * 4.0f);
                transform.eulerAngles += new Vector3(0, 0, 90.0f * gauge * Time.deltaTime);
            });

        List<Sprite> circleList = new List<Sprite>();
        circleList.Add(Resources.Load<Sprite>("Texture/GameMain/CharaCircle_Zero"));
        circleList.Add(Resources.Load<Sprite>("Texture/GameMain/CharaCircle_One"));
        circleList.Add(Resources.Load<Sprite>("Texture/GameMain/CharaCircle_Two"));
        circleList.Add(Resources.Load<Sprite>("Texture/GameMain/CharaCircle_Three"));
        circleList.Add(Resources.Load<Sprite>("Texture/GameMain/CharaCircle_Four"));
        this.ObserveEveryValueChanged(_ => (int)(charaGauge.GaugePercent * 4.0f))
            .Subscribe(_ =>
            {
                spRend.sprite = circleList[(int)(charaGauge.GaugePercent * 4.0f)];
            });
    }

    Color ColorChange()
    {
        Color result = Color.white;
        string name = transform.parent.name;
        if (name.Contains("1P"))
        {
            result = Color.red;
        }
        else if (name.Contains("2P"))
        {
            result = Color.blue;
        }
        else if (name.Contains("3P"))
        {
            result = Color.green;
        }
        else if (name.Contains("4P"))
        {
            result = Color.yellow;
        }

        result.a = 0.75f;
        return result;
    }
}
