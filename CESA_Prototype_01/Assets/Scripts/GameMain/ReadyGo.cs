using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UniRx;
using UniRx.Triggers;

public class ReadyGo : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        StartCoroutine(Ready());        
    }

    IEnumerator Ready()
    {
        Time.timeScale = 0.0f;

        Image ready = transform.FindChild("Ready").GetComponent<Image>();
        Image go = transform.FindChild("Go").GetComponent<Image>();

        ready.transform.localScale = Vector3.zero;
        go.transform.localScale = Vector3.zero;

        yield return null;  //  開始後1Fずらす

        this.UpdateAsObservable()
            .Where(_ => ready.transform.localScale.x < 1.0f)
            .Subscribe(_ =>
            {
                ready.transform.localScale += Vector3.one * (Time.unscaledDeltaTime / 0.5f);
            });

        yield return new WaitWhile(() => ready.transform.localScale.x <= 1.0f);
        ready.transform.localScale = Vector3.one;

        float time = 0.0f;
        yield return new WaitWhile(() =>
        {
            time += Time.unscaledDeltaTime;

            if (time < 1.0f)
                return true;

            return false;
        });

        ready.gameObject.SetActive(false);

        this.UpdateAsObservable()
            .Where(_ => go.transform.localScale.x < 1.0f)
            .Subscribe(_ =>
            {
                go.transform.localScale += Vector3.one * (Time.unscaledDeltaTime / 0.5f);
            }); 

        yield return new WaitWhile(() => go.transform.localScale.x <= 1.0f);
        go.transform.localScale = Vector3.one;

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                go.color -= new Color(0, 0, 0, 1) * (Time.unscaledDeltaTime / 0.25f);
            });

        yield return new WaitWhile(() => go.color.a > 0.0f);

        Time.timeScale = 1.0f;
        gameObject.SetActive(false);
    }
}
