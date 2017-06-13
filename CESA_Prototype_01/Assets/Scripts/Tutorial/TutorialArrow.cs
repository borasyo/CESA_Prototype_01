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

    void Awake()
    {
        _initPos = transform.position;
    }

    void Start ()
    {
        const float fMove = 240.0f;
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

                transform.position = _initPos + triangleMove.CurrentValue;
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

    void OnEnable()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            _initPos = Input.GetTouch(0).position - new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);
        }

        transform.position = _initPos;
    }
}
