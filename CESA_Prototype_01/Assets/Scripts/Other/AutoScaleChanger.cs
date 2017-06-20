using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class AutoScaleChanger : MonoBehaviour
{
    [SerializeField] Vector3 min;
    [SerializeField] Vector3 max;
    [SerializeField] float time;
    public bool _IsOn { get; set; }

    void Start()
    {
        _IsOn = true;

        TriangleWave<Vector3> triangleScaler = TriangleWaveFactory.Vector3(min, max, time);
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (!_IsOn)
                    return;

                triangleScaler.Progress();
                transform.localScale = triangleScaler.CurrentValue;
            });

        this.ObserveEveryValueChanged(_ => _IsOn)
            //.Where(_ => _IsOn)
            .Subscribe(_ =>
            {
                transform.localScale = min;
                triangleScaler.Reset();
            });
    }
}
