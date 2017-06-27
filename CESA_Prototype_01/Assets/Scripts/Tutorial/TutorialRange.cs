using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UniRx;
using UniRx.Triggers;

public class TutorialRange : MonoBehaviour
{
    public bool IsNext { get; private set; }
    [SerializeField] [Multiline] string[] _Description = null;
    int _nCnt = 0;

    void Awake()
    {
        IsNext = false;
        GetComponent<Image>().enabled = false;
    }

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

    public IEnumerator OnWindow()
    {
        Image image = GetComponent<Image>();
        image.enabled = false;
        IsNext = false;

        if (_nCnt >= _Description.Length)
        {
            image.enabled = true;
            IsNext = true;
            yield break;
        }

        GameObject descriptionFlame = transform.parent.Find("DescriptionFlame").gameObject;
        descriptionFlame.GetComponentInChildren<Text>().text = _Description[_nCnt];
        _nCnt++;

        SoundManager.Instance.PlaySE(SoundManager.eSeValue.ONWINDOW);
        float time = 0.0f;
        yield return new WaitWhile(() =>
        {
            time += Time.deltaTime / 0.5f;
            if (time > 1.0f)
                time = 1.0f;

            descriptionFlame.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, time);

            return (time < 1.0f);
        });

        image.enabled = true;
        IsNext = true;
    }

    public IEnumerator OffWindow()
    {
        Image image = GetComponent<Image>();
        image.enabled = false;
        IsNext = false;

        if (_nCnt >= _Description.Length)
        {
            image.enabled = false;
            IsNext = true;
            yield break;
        }

        GameObject descriptionFlame = transform.parent.Find("DescriptionFlame").gameObject;

        float time = 0.0f;
        yield return new WaitWhile(() =>
        {
            time += Time.deltaTime / 0.5f;
            descriptionFlame.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, time);

            return (time < 1.0f);
        });

        image.enabled = false;
        IsNext = true;
    }
}
