using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UniRx;
using UniRx.Triggers;

public class TutorialArrow : MonoBehaviour
{
    public enum eArrowDir
    {
        UP = 0,
        DOWN,
        RIGHT,
        LEFT,

        MAX
    };
    [SerializeField]
    eArrowDir _arrowDir = eArrowDir.MAX;
    Vector3 _initPos = Vector3.zero;
    RectTransform _rectTrans = null;

    public bool IsNext { get; private set; }
    [SerializeField] [Multiline] string[] _Description = new string[1];
    int _nCnt = 0;

    void Awake()
    {
        IsNext = false;
        GetComponent<Image>().enabled = false;
        _rectTrans = GetComponent<RectTransform>();
        _initPos = _rectTrans.anchoredPosition;
    }

    void Start ()
    {
        const float fMove = 200.0f;
        const float fTime = 0.75f;
        
        Vector3 min = new Vector3(0,0,0);
        Vector3 max = Vector3.zero;
        switch(_arrowDir)
        {
            case eArrowDir.UP:
                max = new Vector3(0, fMove, 0);
                break;
            case eArrowDir.DOWN:
                max = new Vector3(0, -fMove, 0);
                break;
            case eArrowDir.RIGHT:
                max = new Vector3(fMove, 0, 0);
                break;
            case eArrowDir.LEFT:
                max = new Vector3(-fMove, 0, 0);
                break;
        }
        TriangleWave<Vector3> triangleMove = TriangleWaveFactory.Vector3(min, max, fTime);
        this.UpdateAsObservable() 
            .Subscribe(_ =>
            {
                triangleMove.Progress();

                if (!triangleMove.IsAdd)
                    return;

                _rectTrans.anchoredPosition = _initPos + triangleMove.CurrentValue;
            });

        Image image = GetComponent<Image>();
        this.UpdateAsObservable()
            .Where(_ => !triangleMove.IsAdd)
            .Subscribe(_ =>
            {
                image.color -= new Color(0, 0, 0, 1) * (Time.deltaTime / (fTime - 0.1f));
            });

        triangleMove.OnReverseNowAdd.Subscribe(_ =>
        {
            image.color = Color.white;
        });
    }

    public IEnumerator OnWindow()
    {
        Image image = GetComponent<Image>();
        image.enabled = false;
        IsNext = false;
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
