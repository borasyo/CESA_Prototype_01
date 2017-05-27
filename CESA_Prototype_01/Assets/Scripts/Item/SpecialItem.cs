using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;

public class SpecialItem : ItemBase
{
    [SerializeField] float _fDuration_Sec = 0.0f;
    Character _character = null;
    CharacterGauge _charactorGauge = null;

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

        _itemType = ItemBase.eItemType.SPECIAL;
        base.Start();
    }

    override public void Run()
    {
        _character = this.GetComponentInParent<Character>();
        if(!_character.RunSpecialMode(true))
        {
            Destroy();
            return;
        }

        _charactorGauge = this.GetComponentInParent<CharacterGauge>();
        _charactorGauge.GaugeMax();

        this.UpdateAsObservable()
            .Subscribe(_ => {
                _fDuration_Sec -= Time.deltaTime;

                if(_fDuration_Sec > 0.0f
                    #if DEBUG
                    && !Input.GetKeyDown(KeyCode.RightShift)
                    #endif
                )
                    return;

                _character.RunSpecialMode(false);
                Destroy(this.gameObject);
            });
    }
}
