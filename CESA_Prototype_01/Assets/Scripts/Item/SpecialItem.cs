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


    void Start()
    {
        MeshRenderer meRend = GetComponent<MeshRenderer>();
        ParticleSystem particle = GetComponentInChildren<ParticleSystem>();
        particle.startSize *= 0.5f;
        this.UpdateAsObservable()
            .Where(_ => this.enabled && !transform.parent.parent)
            .Subscribe(_ => {
                Color setCol = Random.ColorHSV();
                setCol.a = meRend.material.color.a;
                particle.startColor = setCol;
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

        //transform.Find("ItemEffect").gameObject.SetActive(true);
        ParticleSystem particle = transform.GetComponentInChildren<ParticleSystem>();
        particle.startColor = GetColor(_character.GetPlayerNumber());
        particle.startSize *= 2.0f;
        transform.localPosition = Vector3.zero + new Vector3(0.0f, 0.1f, 0.0f);

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

    Color GetColor(string player)
    {
        Color setCol = Color.clear; 
        switch(player)
        {
            case "1":
                setCol = Color.red + new Color(0.0f, 0.1f, 0.1f, 0.0f);
                break;
            case "2":
                setCol = Color.blue + new Color(0.1f, 0.1f, 0.0f, 0.0f);
                break;
            case "3":
                setCol = Color.green + new Color(0.1f, 0.0f, 0.1f, 0.0f);
                break;
            case "4":
                setCol = Color.yellow + new Color(0.0f, 0.0f, 0.1f, 0.0f);
                break;
        }
        return setCol;
    }
}
