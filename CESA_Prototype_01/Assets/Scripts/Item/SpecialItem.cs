using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class SpecialItem : ItemBase
{
    [SerializeField] float _fDuration_Sec = 0.0f;
    Charactor _charactor = null;
    CharactorGauge _charactorGauge = null;

    MeshRenderer _MeRend = null;

    void Start()
    {
        _MeRend = GetComponentInChildren<MeshRenderer>();
        this.UpdateAsObservable()
            .Where(_ => this.enabled)
            .Subscribe(_ => {
                _MeRend.material.color = Random.ColorHSV();
            });

        base.Start();
    }

    override public void Run()
    {
        _charactor = this.GetComponentInParent<Charactor>();
        _charactorGauge = this.GetComponentInParent<CharactorGauge>();
        _charactorGauge.GaugeMax();
        //  特殊開始

        this.UpdateAsObservable()
            .Subscribe(_ => {
                _fDuration_Sec -= Time.deltaTime;

                if(_fDuration_Sec > 0.0f)
                    return;
                
                // 特殊終了
                Destroy(this.gameObject);
            });
    }
}
