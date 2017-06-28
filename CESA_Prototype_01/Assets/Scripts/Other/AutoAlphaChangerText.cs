using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class AutoAlphaChangerText : MonoBehaviour
{
    [SerializeField] float min;
    [SerializeField] float max;
    [SerializeField] float time;
    public bool _IsOn { get; set; }

    void Start()
    {
        _IsOn = true;

        Text text = GetComponent<Text>();
        TriangleWave<float> triangleAlpha = TriangleWaveFactory.Float(min, max, time);
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (!_IsOn)
                    return;

                triangleAlpha.Progress();
                Color setCol = text.color;
                setCol.a = triangleAlpha.CurrentValue;
                text.color = setCol;
            });

        this.ObserveEveryValueChanged(_ => _IsOn)
            //.Where(_ => _IsOn)
            .Subscribe(_ =>
            {
                Color setCol = text.color;
                setCol.a = min;
                text.color = setCol;
                triangleAlpha.Reset();
            });
    }
}
