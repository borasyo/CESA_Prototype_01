using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class SelectMass : FieldObjectBase 
{
    Transform _normalizeTrans = null;
    protected Character _character = null;
    protected CharacterGauge _charactorGauge = null;

    protected SpriteRenderer _SpRend = null;

    [SerializeField] float _fInterval_Sec = 0.5f;

    protected Color _notColor   = new Color(1,1,1,1);
    protected Color _putColor   = new Color(1,1,1,1);
    protected Color _breakColor = new Color(1,1,1,1);

    void Start()
    {
        _normalizeTrans = transform.parent.Find("NormalizePos");
        _character = GetComponentInParent<Character>();
        _charactorGauge = GetComponentInParent<CharacterGauge>();
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        float interval = _fInterval_Sec / 2.0f;

        // テクスチャ点滅処理
        _SpRend = GetComponent<SpriteRenderer>();
        TriangleWave<float> triangleWaveFloat = TriangleWaveFactory.Float(1.0f, 0.0f, interval);
        this.UpdateAsObservable()
            .Where(_ => this.enabled)
            .Subscribe(_ => {
                triangleWaveFloat.Progress();
                Color setCol = _SpRend.color;
                setCol.a = triangleWaveFloat.CurrentValue;
                _SpRend.color = setCol;
            });

        //  テクスチャ拡縮処理
        /*TriangleWave<Vector3> triangleWaveVector3 = TriangleWaveFactory.Vector3(Vector3.zero, Vector3.one * 0.8f, interval);
        this.UpdateAsObservable()
            .Where(_ => this.enabled && _SpRend.sprite.name == "SelectMass")
            .Subscribe(_ => {
                triangleWaveVector3.Progress();
                transform.localScale = triangleWaveVector3.CurrentValue;
            });*/

        _notColor = SelectMassColor.Instance.GetNotColor(transform.parent.name);
        _putColor = SelectMassColor.Instance.GetPutColor(transform.parent.name);
        _breakColor = SelectMassColor.Instance.GetBreakColor(transform.parent.name);
    }

    void Update()
    {
        SetAlpha();
        ColorCheck();
    }

    void SetAlpha()
    {
        float alpha = _SpRend.color.a;
        _notColor.a = alpha;
        _putColor.a = alpha;
        _breakColor.a = alpha;
    }

    virtual protected void ColorCheck()
    {
        int number = _character.GetDataNumberForDir();
        transform.position = GetPosForNumber(number);

        //  置ける、壊せる、何もできないを判定
        FieldObjectBase obj = FieldData.Instance.GetObjData(number);
        Color setCol = _notColor;
        if (obj)
        {
            if (obj.GetSandType() != SandItem.eType.MAX && _charactorGauge.BreakGaugeCheck())
            {
                setCol = _breakColor;
            }
        }
        else
        {
            if (_charactorGauge.PutGaugeCheck())
            {
                setCol = _putColor;
            }
        }

        _SpRend.color = setCol;
    }
}
