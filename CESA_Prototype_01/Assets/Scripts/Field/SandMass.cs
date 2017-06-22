using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class SandMass : FieldObjectBase
{
    List<LineRenderer> _ThunderList = new List<LineRenderer>();

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
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
        yield return new WaitWhile(() => Time.timeScale <= 0.0f);

        SandData.tData data = SandData.Instance.GetSandDataList[GetDataNumber()];
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                data = SandData.Instance.GetSandDataList[GetDataNumber()];
                //Debug.Log("data" + data._type);
            });

        this.ObserveEveryValueChanged(_ => data._type)
            .Subscribe(_ =>
            {
                if (data._type != SandItem.eType.MAX)
                {
                    foreach (LineRenderer thunder in _ThunderList)
                    {
                        thunder.gameObject.SetActive(true);
                        SoundManager.Instance.PlaySE(SoundManager.eSeValue.THUNDER);
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

        this.ObserveEveryValueChanged(_ => data._isVertical)
            .Subscribe(_ =>
            {
                DirUpdate(data._isVertical);
            });
    }

    void ThunderUpdate(SandData.tData data)
    {
        //  色を更新
        Color setColor = Color.clear;
        switch (data._type)
        {
            case SandItem.eType.ONE_P:
                setColor = new Color(1.0f, 0.2f, 0.2f, 1.0f);
                break;
            case SandItem.eType.TWO_P:
                setColor = new Color(0.25f, 0.25f, 1.0f, 1.0f);
                break;
            case SandItem.eType.THREE_P:
                setColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);
                break;
            case SandItem.eType.FOUR_P:
                setColor = new Color(1.0f, 1.0f, 0.0f, 1.0f);
                break;
            default:
                break;
        }

        foreach (LineRenderer thunder in _ThunderList)
        {
            thunder.startColor = setColor;
            thunder.endColor = setColor;
        }

        //DirUpdate(data._isVertical);
    }

    void DirUpdate(bool isVertical)
    {
        //  向きを更新
        if (isVertical)
            transform.eulerAngles = new Vector3(0, 90, 0);
        else
            transform.eulerAngles = new Vector3(0, 0, 0);
    }
}
