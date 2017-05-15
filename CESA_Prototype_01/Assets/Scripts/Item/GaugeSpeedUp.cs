using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class GaugeSpeedUp : ItemBase 
{
    [SerializeField] float _fUpAmountPer = 0.5f;
    [SerializeField] float _fDuration_Sec = 5.0f;

    CharactorGauge _charactorGauge = null;

    override public void Run()
    {
        _charactorGauge = this.GetComponentInParent<CharactorGauge>();
        _charactorGauge.ChangeChargeSpeed(_fUpAmountPer);

        this.UpdateAsObservable()
            .Subscribe(_ => {
                _fDuration_Sec -= Time.deltaTime;

                if(_fDuration_Sec > 0.0f)
                    return;

                _charactorGauge.ChangeChargeSpeed(1.0f / _fUpAmountPer);
                Destroy(this.gameObject);
            });
    }
}
