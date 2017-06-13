using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UniRx;
using UniRx.Triggers;

public class TutorialRange : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        Color min = new Color(1, 1, 1, 0);
        Color max = new Color(1, 1, 1, 1);
        TriangleWave<Color> triangleCol = TriangleWaveFactory.Color(max, min, 0.5f);
        Image image = GetComponent<Image>();
        this.UpdateAsObservable()
            .Where(_ => Time.timeScale > 0)
            .Subscribe(_ =>
            {
                triangleCol.Progress();
                image.color = triangleCol.CurrentValue;
            });
    }
}
