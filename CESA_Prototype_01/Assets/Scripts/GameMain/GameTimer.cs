using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UniRx;
using UniRx.Triggers;

public class GameTimer : MonoBehaviour
{
    #region Singleton

    private static GameTimer instance;

    public static GameTimer Instance
    {
        get
        {
            if (instance)
                return instance;

            instance = (GameTimer)FindObjectOfType(typeof(GameTimer));

            if (instance)
                return instance;

            GameObject obj = new GameObject("GameTimer");
            obj.AddComponent<GameTimer>();
            Debug.Log(typeof(GameTimer) + "が存在していないのに参照されたので生成");

            return instance;
        }
    }

    #endregion

    private float _fTime = 0.0f;

    //  残り時間表示などで使用
    public float fTime { get { return _fTime; } }
    
    //  ゲームが終了したか
    public bool GameEnd { get { return _fTime <= 0.0f;  } }

    // Event
    void Start()
    {
        _fTime = (float)TimeAmount.GetTime();
        Text text = GetComponent<Text>();

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                _fTime -= Time.deltaTime;
                text.text = "Time : " + (int)_fTime;

                if (_fTime < 0.0f)
                    _fTime = 0.0f;
            });
    }
}
