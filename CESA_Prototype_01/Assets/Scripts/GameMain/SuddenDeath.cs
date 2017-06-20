using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class SuddenDeath : MonoBehaviour
{
    [SerializeField]
    float fTime = 2.0f;

    void Awake()
    {
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        Time.timeScale = 0.0f;
        RectTransform rectTrans = GetComponent<RectTransform>();
        rectTrans.position = new Vector2(960 * 2 + 200.0f, 540);  // 2320.0f / 2.0f, 0.0f);

        Image image = GetComponent<Image>();
        Color min = new Color(1, 1, 1, 0);
        Color max = new Color(1, 1, 1, 1);
        TriangleWave<Color> triangleColor = TriangleWaveFactory.Color(min, max, 0.05f);
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                triangleColor.Progress(true);
                image.color = triangleColor.CurrentValue;
                rectTrans.position -= new Vector3((960 * 2 + 400) * (Time.unscaledDeltaTime / fTime), 0.0f, 0.0f);
            });

        float time = fTime;
        yield return new WaitWhile(() =>
        {
            time -= Time.unscaledDeltaTime;

            if (time > 0.0f)
                return true;

            return false;
        });

        Time.timeScale = 1.5f;
        foreach (Character chara in FieldData.Instance.GetCharactors)
        {
            chara.GetComponent<CharacterGauge>().SuperMode();
        }
        Destroy(gameObject);
    }
}
    
