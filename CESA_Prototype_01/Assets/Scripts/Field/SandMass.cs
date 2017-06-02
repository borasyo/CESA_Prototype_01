using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class SandMass : FieldObjectBase
{
    SpriteRenderer _SpRend = null;
    TriangleWave<Vector3> _triangleScaler = null;
    TriangleWave<float> _triangleAlpha = null;
	
    void Start()
    {
        _SpRend = GetComponent<SpriteRenderer>();
        _SpRend.enabled = false;

        float halfPeriod = 0.25f;

        _triangleScaler = TriangleWaveFactory.Vector3(transform.localScale / 1.5f, transform.localScale, halfPeriod);
        this.UpdateAsObservable()
            .Where(_ => this.enabled)
            .Subscribe(_ => {
                _triangleScaler.Progress();
                transform.localScale = _triangleScaler.CurrentValue;
            });

        StartCoroutine(StartUpdate());
    }

    IEnumerator StartUpdate()
    {
        //  Fieldの生成が終わるまで待つ
        yield return new WaitWhile(() => FieldData.Instance.IsStart == false);

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                ColorUpdate();
            });
    }

    void ColorUpdate()
    {
        _SpRend.enabled = false;
        _SpRend.color = new Color(0, 0, 0, 0);
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
                break;
        }
    }

    void AddColor(Color col) 
    {
        _SpRend.enabled = true;
        _SpRend.color += col;
    }
}
