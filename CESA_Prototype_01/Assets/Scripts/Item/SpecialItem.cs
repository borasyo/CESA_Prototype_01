﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using System.Linq;

public class SpecialItem : ItemBase
{
    [SerializeField] float _fDuration_Sec = 0.0f;
    Character _character = null;
    CharacterGauge _charactorGauge = null;

    void Start()
    {
        MeshRenderer meRend = GetComponent<MeshRenderer>();
        List<ParticleSystem> particleList = transform.GetComponentsInChildren<ParticleSystem>().ToList();
        foreach (ParticleSystem particle in particleList)
        {
            //particle.startSize *= 0.75f;
        }
        this.UpdateAsObservable()
            .Where(_ => this.enabled && !transform.parent.parent)
            .Subscribe(_ => {
                Color setCol = Random.ColorHSV();
                setCol.a = meRend.material.color.a;

                foreach (ParticleSystem particle in particleList)
                {
                    particle.startColor = setCol;
                }
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
        List<ParticleSystem> particleList = transform.GetComponentsInChildren<ParticleSystem>().ToList();
        foreach (ParticleSystem particle in particleList)
        {
            particle.startColor = GetColor(_character.GetPlayerNumber());
            particle.startSize *= 1.75f;
        }
        transform.localPosition = Vector3.zero + new Vector3(0.0f, 0.52f, 0.0f);

        _charactorGauge = this.GetComponentInParent<CharacterGauge>();
        _charactorGauge.GaugeMax();

        float max = _fDuration_Sec;
        this.UpdateAsObservable()
            .Subscribe(_ => {
                _fDuration_Sec -= Time.deltaTime;

                foreach (ParticleSystem particle in particleList)
                    particle.startColor -= new Color(0, 0, 0, 1 * (Time.deltaTime / max));

                if (_fDuration_Sec > 0.0f
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
