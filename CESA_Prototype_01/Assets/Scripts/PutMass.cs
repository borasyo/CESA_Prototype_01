using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class PutMass : FieldObjectBase 
{
    Transform _normalizeTrans = null;
    Charactor _charactor = null;

    SpriteRenderer _SpRend = null;
    TriangleWave<float> _triangleWave = null;
    [SerializeField] float _fInterval_Sec = 0.5f;

    [SerializeField] Color _putColor   = new Color(1,1,1,1);
    [SerializeField] Color _crashColor = new Color(1,1,1,1);

    void Start()
    {
        _normalizeTrans = transform.parent.FindChild("NormalizePos");
        _charactor = GetComponentInParent<Charactor>();

        // テクスチャ点滅処理
        _SpRend = GetComponent<SpriteRenderer>();
        _triangleWave = TriangleWaveFactory.Float(0.0f, 1.0f, _fInterval_Sec/2.0f);
        this.UpdateAsObservable()
            .Where(_ => this.enabled)
            .Subscribe(_ => {
                _triangleWave.Progress();
                Color setCol = _SpRend.color;
                setCol.a = _triangleWave.CurrentValue;
                _SpRend.color = setCol;
            });
    }

    void Update()
    {
        int number = _charactor.GetDataNumberForDir();
        transform.position = GetPosForNumber(number);

        //  置ける、壊せる、何もできないを判定
        float alpha = _SpRend.color.a;
        FieldObjectBase obj = FieldData.Instance.GetObjData(number);
        if (obj)
        {
            if (obj.tag == "SandItem")
            {
                _crashColor.a = alpha;
                _SpRend.color = _crashColor;
            }
            else
            {
                _SpRend.color = new Color(1, 1, 1, alpha);
            }
        }
        else
        {
            _putColor.a = alpha;
            _SpRend.color = _putColor;
        }
    }
}
