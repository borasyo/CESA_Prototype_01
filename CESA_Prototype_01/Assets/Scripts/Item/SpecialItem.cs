using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;

public class SpecialItem : ItemBase
{
    [SerializeField] float _fDuration_Sec = 0.0f;
    Charactor _charactor = null;
    CharactorGauge _charactorGauge = null;

    MeshRenderer _MeRend = null;

    void Start()
    {
        TextMesh textMesh = GetComponentInChildren<TextMesh>();
        _MeRend = GetComponentInChildren<MeshRenderer>();
        this.UpdateAsObservable()
            .Where(_ => this.enabled)
            .Subscribe(_ => {
                textMesh.color = _MeRend.material.color = Random.ColorHSV();
            });

        base.Start();
    }

    override public void Run()
    {
        _charactor = this.GetComponentInParent<Charactor>();
        _charactor.RunSpecialMode(true);

        _charactorGauge = this.GetComponentInParent<CharactorGauge>();
        _charactorGauge.GaugeMax();

        this.UpdateAsObservable()
            .Subscribe(_ => {
                _fDuration_Sec -= Time.deltaTime;

                if(_fDuration_Sec > 0.0f
                    #if DEBUG
                    && !Input.GetKeyDown(KeyCode.Return)
                    #endif
                )
                    return;

                _charactor.RunSpecialMode(false);
                Destroy(this.gameObject);
            });
    }
}
