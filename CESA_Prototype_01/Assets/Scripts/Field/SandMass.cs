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

        float halfPeriod = 0.25f;

        _triangleScaler = TriangleWaveFactory.Vector3(transform.localScale / 1.5f, transform.localScale, halfPeriod);
        this.UpdateAsObservable()
            .Where(_ => this.enabled)
            .Subscribe(_ => {
                _triangleScaler.Progress();
                transform.localScale = _triangleScaler.CurrentValue;
//                transform.eulerAngles += new Vector3(0, 360 * Time.deltaTime ,0);
            });

        /*_triangleAlpha = TriangleWaveFactory.Float(0.5f, 1.0f, halfPeriod);
        this.UpdateAsObservable()
            .Where(_ => this.enabled)
            .Subscribe(_ => {
                _triangleAlpha.Progress();
                Color setCol = _SpRend.color;
                setCol.a = _triangleAlpha.CurrentValue;
                _SpRend.color = setCol;
            });*/
    }

	void Update () 
    {
        _SpRend.enabled = false;
        _SpRend.color = new Color(0,0,0,0);
        List<SandData.tSandData> sandDataList = SandData.Instance.GetSandDataList;
        for (int i = 0; i < sandDataList.Count; i++)
        {
            if (sandDataList[i]._number != GetDataNumber())
                continue;

            SandData.tSandData data = sandDataList[i];
            switch (data._Type)
            {
                case SandItem.eType.ONE_P:
                    AddColor(new Color(255,0,0,255));
                    break;
                case SandItem.eType.TWO_P:
                    AddColor(new Color(0,0,255,255));
                    break;
                case SandItem.eType.THREE_P:
                    AddColor(new Color(0,255,0,255));
                    break;
                case SandItem.eType.FOUR_P:
                    AddColor(new Color(255,255,0,255));
                    break;
            }
        }
	}

    void AddColor(Color col) 
    {
        _SpRend.enabled = true;
        _SpRend.color += col;
    }
}
