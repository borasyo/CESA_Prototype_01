using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UniRx;
using UniRx.Triggers;
using System.Linq;

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

    #region SceneLoadInit

    private bool _isInitialized = false;
    private readonly string ReadyStateKey = "Timer";
    /// <param name="data"></param>
    private void OnPhotonPlayerPropertiesChanged(object[] data)
    {
        //誰かのカスタムプロパティが書き換わるたびに確認
        CheckAllPlayerState();
    }

    private void CheckAllPlayerState()
    {
        if (_isInitialized) return;

        //全員のフラグが設定されているか？
        var isAllPlayerLoaded = PhotonNetwork.playerList
            .Select(x => x.customProperties)
            .All(x => x.ContainsKey(ReadyStateKey) && (bool)x[ReadyStateKey]);

        if (isAllPlayerLoaded)
        {
            //全員のフラグが設定されていたら初期化開始
            _isInitialized = true;
            ClearReadyStatus();
            Init();
            //Reset();
        }
    }

    private void Ready()
    {
        var cp = PhotonNetwork.player.customProperties;
        cp[ReadyStateKey] = true;
        PhotonNetwork.player.SetCustomProperties(cp);
    }

    private void ClearReadyStatus()
    {
        var cp = PhotonNetwork.player.customProperties;
        cp[ReadyStateKey] = null;
        PhotonNetwork.player.SetCustomProperties(cp);
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
        if(PhotonNetwork.inRoom)
        {
            Ready();
            CheckAllPlayerState();
        }
        else
        {
            Init();
        }
    }

    void Init()
    {
        _fTime = (float)TimeAmount.GetTime();
        Text text = GetComponent<Text>();
        Time.timeScale = 0.0f;

        if (_fTime < 0)
        {
            text.text = "∞";
            text.fontSize = (int)(text.fontSize * 1.5f);
            return;
        }
   
        this.UpdateAsObservable()
            .Where(_ => _fTime > 0.0f && FieldData.Instance.GetCharactors.Count > 0)
            .Subscribe(_ =>
            {
                _fTime -= Time.deltaTime;
#if DEBUG
                if (Input.GetKeyDown(KeyCode.Backspace))
                    _fTime = 0.0f;
#endif
                text.text = ((int)(_fTime + 0.1f)).ToString();

            });

        this.ObserveEveryValueChanged(_ => _fTime > 30.0f)
            .Where(_ => _fTime <= 30.0f)
            .Subscribe(_ =>
            {
                text.color = Color.red;
            });

        this.ObserveEveryValueChanged(_ => GameEnd)
            .Where(_ => _fTime <= 0.0f)
            .Subscribe(_ =>
            {
                _fTime = 0.0f;
                Instantiate(Resources.Load<GameObject>("Prefabs/GameMain/SuddenDeath")).transform.SetParent(transform.parent);

                StartCoroutine(StartFlash());
            });
    }

    IEnumerator StartFlash()
    {
        yield return null;

        Text text = GetComponent<Text>();
        Color min = text.color;
        Color max = text.color;
        max.a = 0.0f;
        TriangleWave<Color> triangleColor = TriangleWaveFactory.Color(min, max, 0.25f);
        this.UpdateAsObservable()
            .Subscribe(x =>
            {
                triangleColor.Progress();
                text.color = triangleColor.CurrentValue;
            });
    }
}
