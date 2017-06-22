using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UniRx;
using UniRx.Triggers;
using System.Linq;

public class ReadyGoOnline : Photon.MonoBehaviour
{
    #region SceneLoadInit

    private bool _isInitialized = false;
    private readonly string ReadyStateKey = "Reset";
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
            StartCoroutine(SetReady());
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

    // Use this for initialization
    void Start ()
    {
        Time.timeScale = 0.0f;
        Ready();
        CheckAllPlayerState();
        //StartCoroutine(SetReady());
    }

    IEnumerator SetReady()
    {
        SoundManager.Instance.PlaySE(SoundManager.eSeValue.GAMESTART);
        Image ready = transform.Find("Ready").GetComponent<Image>();
        Image go = transform.Find("Go").GetComponent<Image>();

        ready.transform.localScale = Vector3.zero;
        go.transform.localScale = Vector3.zero;

        yield return new WaitWhile(() => FadeManager.Instance.Fading);

        this.UpdateAsObservable()
            .Where(_ => ready.transform.localScale.x < 1.0f)
            .Subscribe(_ =>
            {
                ready.transform.localScale += Vector3.one * (Time.unscaledDeltaTime / 0.5f);
            });

        yield return new WaitWhile(() => ready.transform.localScale.x <= 1.0f);

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

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                go.color -= new Color(0, 0, 0, 1) * (Time.unscaledDeltaTime / 0.5f);
            });

        yield return new WaitWhile(() => go.color.a < 1.0f);

        Reset();
    }

    void Reset()
    {
        Time.timeScale = 1.0f;
        gameObject.SetActive(false);
        SoundManager.Instance.PlayBGM(SoundManager.eBgmValue.GAMEMAIN, 0.25f);
    }

    void OnDestroy()
    {
        Time.timeScale = 1.0f;
    }
}
