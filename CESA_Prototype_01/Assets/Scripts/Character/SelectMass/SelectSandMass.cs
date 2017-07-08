using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class SelectSandMass : MonoBehaviour
{
    TriangleWave<Color> _triangleAlpha = null;
    static Sprite _sandMassSprite = null;

    void Awake()
    {
        if (_sandMassSprite)
            return;

        _sandMassSprite = Resources.Load<Sprite>("Texture/GameMain/SelectSandMass");
    }
    	
	public void Init (Color initCol)
    {
        SpriteRenderer spRend = GetComponent<SpriteRenderer>();
        spRend.color = initCol;
        spRend.sprite = _sandMassSprite;

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

        /*TriangleWave<Vector3> triangleScaler = TriangleWaveFactory.Vector3(Vector3.zero, transform.localScale, 0.5f);
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                triangleScaler.Progress();
                transform.localScale = triangleScaler.CurrentValue;
            });*/

        transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                transform.eulerAngles += new Vector3(0, 120 * Time.deltaTime, 0);
            });
    }
}
