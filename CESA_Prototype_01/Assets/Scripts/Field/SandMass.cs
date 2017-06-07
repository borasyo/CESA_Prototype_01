﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class SandMass : FieldObjectBase
{
    List<LineRenderer> _ThunderList = new List<LineRenderer>();
	
    void Start()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            _ThunderList.Add(child.GetComponent<LineRenderer>());
            child.gameObject.SetActive(false);
        }

        StartCoroutine(StartUpdate());
    }

    IEnumerator StartUpdate()
    {
        //  Fieldの生成が終わるまで待つ
        yield return new WaitWhile(() => FieldData.Instance.IsStart == false);
        
        SandData.tData data = new SandData.tData();
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                data = SandData.Instance.GetSandDataList[GetDataNumber()];
                //Debug.Log("data" + data._type);
            });

        this.ObserveEveryValueChanged(_ => data._type)
            .Subscribe(_ =>
            {
                if(data._type != SandItem.eType.MAX)
                {
                    foreach (LineRenderer thunder in _ThunderList)
                    {
                        thunder.gameObject.SetActive(true);
                    }
                    ThunderUpdate(data);
                    //Debug.Log("true");
                }
                else
                {
                    foreach (LineRenderer thunder in _ThunderList)
                    {
                        thunder.gameObject.SetActive(false);
                    }
                    //Debug.Log("false");
                }
            });
    }

    void ThunderUpdate(SandData.tData data)
    {
        //  色を更新
        Color setColor = Color.clear;
        switch (data._type)
        {
            case SandItem.eType.ONE_P:
                setColor = Color.red;
                break;
            case SandItem.eType.TWO_P:
                setColor = Color.blue;
                break;
            case SandItem.eType.THREE_P:
                setColor = Color.green;
                break;
            case SandItem.eType.FOUR_P:
                setColor = Color.yellow;
                break;
            default:
                break;
        }

        foreach (LineRenderer thunder in _ThunderList)
        {
            thunder.startColor = setColor;
            thunder.endColor = setColor;
        }

        //  向きを更新
        if (data._isVertical)
            transform.eulerAngles = new Vector3(0, 90, 0);
        else
            transform.eulerAngles = new Vector3(0, 0, 0);
    }
}
