using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class SandMass : FieldObjectBase
{
    List<LineRenderer> _ThunderList = new List<LineRenderer>();
    SandData.eSandDir _sandDir = SandData.eSandDir.NONE; 
    public SandData.eSandDir SetSandDir { set { _sandDir = value; } }

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

        int idx = (int)_sandDir;
        this.ObserveEveryValueChanged(_ => data._type[idx])
            .Subscribe(_ =>
            {
                if (data._sandDir[idx] == _sandDir && data._type[idx] != SandItem.eType.MAX)
                {
                    foreach (LineRenderer thunder in _ThunderList)
                    {
                        thunder.gameObject.SetActive(true);
                        SoundManager.Instance.PlaySE(SoundManager.eSeValue.THUNDER);
                    }
                    ThunderUpdate(data._type[idx]);
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

    void ThunderUpdate(SandItem.eType type)
    {
        //  色を更新
        Color setColor = Color.clear;
        switch (type)
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
            case SandItem.eType.ALL:
                setColor = Color.gray * 1.5f;
                break;
            default:
                break;
        }

        foreach (LineRenderer thunder in _ThunderList)
        {
            thunder.startColor = setColor;
            thunder.endColor = setColor;
        }
    }
}
