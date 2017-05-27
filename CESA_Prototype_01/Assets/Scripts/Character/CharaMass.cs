using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class CharaMass : FieldObjectBase {

    SpriteRenderer _SpRend = null;
    TriangleWave<Color> _triangleWaveColor = null;
    TriangleWave<Vector3> _triangleWaveScaler = null;

    [SerializeField] float _fPeriod_Sec = 0.5f;

	// Use this for initialization
	void Start ()
    {
        //  座標更新処理
        this.UpdateAsObservable()
            .Where(_ => this.enabled)
            .Subscribe(_ =>
                {
                    int number = GetDataNumber(transform.parent.position);
                    transform.position = GetPosForNumber(number);
                });

        //  点滅運動
        _SpRend = GetComponent<SpriteRenderer>();
        Color defaultColor = _SpRend.color =  ColorChange();
        defaultColor.a = 0.25f;  //  min
        _triangleWaveColor = TriangleWaveFactory.Color(defaultColor, _SpRend.color, _fPeriod_Sec / 2.0f);
        this.UpdateAsObservable()
            .Where(_ => this.enabled)
            .Subscribe(_ =>
                {
                    _triangleWaveColor.Progress();
                    _SpRend.color = _triangleWaveColor.CurrentValue;
                });

        // 拡大縮小処理
       /*_triangleWaveScaler = TriangleWaveFactory.Vector3(Vector3.zero, transform.localScale, _fPeriod_Sec / 4.0f);
        this.UpdateAsObservable()
            .Where(_ => this.enabled)
            .Subscribe(_ =>
                {
                    _triangleWaveScaler.Progress();
                    transform.localScale = _triangleWaveScaler.CurrentValue;
                });*/
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
