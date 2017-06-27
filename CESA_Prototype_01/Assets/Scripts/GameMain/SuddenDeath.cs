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
        transform.localScale = Vector3.one * (Screen.width / 1920.0f);
        float size = Screen.width / 9.6f;
        Time.timeScale = 0.0f;
        RectTransform rectTrans = GetComponent<RectTransform>();
        rectTrans.position = new Vector2(Screen.width + size, Screen.height / 2.0f);  // 2320.0f / 2.0f, 0.0f);
        SoundManager.Instance.PlaySE(SoundManager.eSeValue.TIMEUP);

        Image image = GetComponent<Image>();
        Color min = new Color(1, 1, 1, 0);
        Color max = new Color(1, 1, 1, 1);
        TriangleWave<Color> triangleColor = TriangleWaveFactory.Color(min, max, 0.05f);
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                triangleColor.Progress(true);
                image.color = triangleColor.CurrentValue;
                rectTrans.position -= new Vector3((Screen.width + size * 2.0f) * (Time.unscaledDeltaTime / fTime), 0.0f, 0.0f);
            });

        float time = fTime;
        yield return new WaitWhile(() =>
        {
            time -= Time.unscaledDeltaTime;

            if (time > 0.0f)
                return true;

            return false;
        });

        //Time.timeScale = 1.5f;
        Time.timeScale = 1.0f;
        //foreach (Character chara in FieldData.Instance.GetCharactors)
        //{
        //    chara.GetComponent<CharacterGauge>().SuperMode();
        //}
        SandMassData.Instance.Run();
        SoundManager.Instance.StopSE(SoundManager.eSeValue.TIMEUP);
        Destroy(gameObject);
    }
}
    
