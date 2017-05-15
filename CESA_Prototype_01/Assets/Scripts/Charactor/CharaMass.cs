using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class CharaMass : FieldObjectBase {

    SpriteRenderer _SpRend = null;
    TriangleWave<Color> _triangleWaveColor = null;

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

        //  拡縮運動
        _SpRend = GetComponent<SpriteRenderer>();
        _triangleWaveColor = TriangleWaveFactory.Color(new Color(1,1,1,0.5f), new Color(1,1,1,1), _fPeriod_Sec / 2.0f);
        this.UpdateAsObservable()
            .Where(_ => this.enabled)
            .Subscribe(_ =>
                {
                    _triangleWaveColor.Progress();
                    _SpRend.color = _triangleWaveColor.CurrentValue;
                });
    }
}
