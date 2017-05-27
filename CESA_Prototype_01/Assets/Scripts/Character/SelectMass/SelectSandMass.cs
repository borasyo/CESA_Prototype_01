using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class SelectSandMass : MonoBehaviour
{
    TriangleWave<Color> _triangleAlpha = null;
    	
	public void Init (Color initCol)
    {
        SpriteRenderer spRend = GetComponent<SpriteRenderer>();
        spRend.color = initCol;
        Color minCol = spRend.color;
        minCol.a = 0.5f;
        Color maxCol = spRend.color;
        _triangleAlpha = TriangleWaveFactory.Color(minCol, maxCol, 0.5f);
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                _triangleAlpha.Progress();
                spRend.color = _triangleAlpha.CurrentValue;
            });
	}
}
