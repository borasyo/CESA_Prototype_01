using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class AlreadyRoomName : MonoBehaviour
{
    bool _isRun = false;
    TriangleWave<Color> _triangleAlpha = null;

    void Start()
    {
        Image image = GetComponent<Image>();
        Color min = new Color(1, 1, 1, 1);
        Color max = new Color(1, 1, 1, 0);
        _triangleAlpha = TriangleWaveFactory.Color(min, max, 0.125f);
        this.UpdateAsObservable()
            .Where(_ => _isRun)
            .Subscribe(_ =>
            {
                _triangleAlpha.Progress();
                image.color = _triangleAlpha.CurrentValue;

                if (_triangleAlpha.GetLapCnt < 2)
                    return;

                _isRun = false;
                image.color = Color.white;
            });
    }

    public void Run()
    {
        _isRun = true;
        _triangleAlpha.Reset();
    }
}
