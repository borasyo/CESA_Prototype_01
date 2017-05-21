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
        int framecount = 0;
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                time += Time.deltaTime;
                framecount++;

                if (time < 1.0f)
                    return;

                text.text = "FPS : " + framecount.ToString();
                time = 0.0f;
                framecount = 0;
            });
    }

}
