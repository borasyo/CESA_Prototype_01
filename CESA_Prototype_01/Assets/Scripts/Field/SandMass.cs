using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class SandMass : FieldObjectBase
{
    //SpriteRenderer _SpRend = null;
    //TriangleWave<Vector3> _triangleScaler = null;
    //TriangleWave<float> _triangleAlpha = null;
    List<LineRenderer> _ThunderList = new List<LineRenderer>();
	
    void Start()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            _ThunderList.Add(child.GetComponent<LineRenderer>());
            child.gameObject.SetActive(false);
        }

        /*_SpRend = GetComponent<SpriteRenderer>();
        _SpRend.enabled = false;

        float halfPeriod = 0.25f;

        _triangleScaler = TriangleWaveFactory.Vector3(transform.localScale / 1.5f, transform.localScale, halfPeriod);
        this.UpdateAsObservable()
            .Where(_ => this.enabled)
            .Subscribe(_ => {
                _triangleScaler.Progress();
                transform.localScale = _triangleScaler.CurrentValue;
            });*/

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
            });

        this.ObserveEveryValueChanged(_ => data._type)
            .Subscribe(_ =>
            {
                if(data._type != SandItem.eType.MAX)
                {
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        transform.GetChild(i).gameObject.SetActive(true);
                        ThunderUpdate(data);
                    }
                }
                else
                {
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
            });

        /*this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                ColorUpdate();
            });*/
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

    /*void ColorUpdate()
    {
        //_SpRend.enabled = false;
        //_SpRend.color = new Color(0, 0, 0, 0);
        SandItem.eType type = SandData.Instance.GetSandDataList[GetDataNumber()];
        switch (type)
        {
            case SandItem.eType.ONE_P:
                AddColor(new Color(255, 0, 0, 255));
                break;
            case SandItem.eType.TWO_P:
                AddColor(new Color(0, 0, 255, 255));
                break;
            case SandItem.eType.THREE_P:
                AddColor(new Color(0, 255, 0, 255));
                break;
            case SandItem.eType.FOUR_P:
                AddColor(new Color(255, 255, 0, 255));
                break;
            default:
                AddColor(Color.clear);
                break;
        }
    }

    void AddColor(Color col) 
    {
        _SpRend.enabled = true;
        _SpRend.color += col;
    }*/
}
