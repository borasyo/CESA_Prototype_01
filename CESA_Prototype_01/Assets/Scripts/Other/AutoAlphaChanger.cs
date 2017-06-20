using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class AutoAlphaChanger : MonoBehaviour
{
    [SerializeField] float min;
    [SerializeField] float max;
    [SerializeField] float time;
    public bool _IsOn { get; set; }

    void Start()
    {
        _IsOn = true;

        Image image = GetComponent<Image>();
        TriangleWave<float> triangleAlpha = TriangleWaveFactory.Float(min, max, time);
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (!_IsOn)
                    return;

                triangleAlpha.Progress();
                Color setCol = image.color;
                setCol.a = triangleAlpha.CurrentValue;
                image.color = setCol;
            });

        this.ObserveEveryValueChanged(_ => _IsOn)
            //.Where(_ => _IsOn)
            .Subscribe(_ =>
            {
                Color setCol = image.color;
                setCol.a = min;
                image.color = setCol;
                triangleAlpha.Reset();
            });
    }
}
