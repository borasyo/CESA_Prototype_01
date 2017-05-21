using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class FPSDrawer : MonoBehaviour
{
    void Start()
    {
        Text text = GetComponent<Text>();
        float time = 1.0f;
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                time += Time.deltaTime;

                if (time < 1.0f)
                    return;

                text.text = "FPS : " + Mathf.FloorToInt(1.0f / Time.deltaTime).ToString();
                time = 0.0f;
            });
    }

}
