using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TouchStart : MonoBehaviour
{
    void  Start()
    {
        StartCoroutine(Init());

        Image myImage = GetComponent<Image>();
        Color min = Color.white - new Color(0, 0, 0, 1);
        Color max = Color.white;
        TriangleWave<Color> fade = TriangleWaveFactory.Color(min, max, 0.5f);
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                fade.Progress();
                myImage.color = fade.CurrentValue;
            });
    }

    IEnumerator Init()
    {
        yield return null;

        if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            this.UpdateAsObservable()
                .Where(_ => Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                .Subscribe(_ =>
                {
                    LoadModeSelect();
                });
        }
        else
        {
            this.UpdateAsObservable()
                .Where(_ => Input.GetMouseButtonDown(0))
                .Subscribe(_ =>
                {
                    LoadModeSelect();
                });
        }
    }

    void LoadModeSelect()
    {
        SceneChanger.Instance.ChangeScene("ModeSelect", true);
        //SceneManager.LoadScene("ModeSelect");
    }
}

