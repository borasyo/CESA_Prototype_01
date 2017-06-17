using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class GoGame : Photon.MonoBehaviour
{
    private bool _IsGoGame = false;

    #region Ready

    private bool _isInitialized = false;
    private readonly string ReadyStateKey = "StageSelect";
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

    void Start()
    {
        Button button = GetComponent<Button>();
        ColorBlock colBlock = new ColorBlock();
        colBlock = button.colors;
        colBlock.normalColor *= 0.5f;
        colBlock.highlightedColor *= 0.5f;
        colBlock.pressedColor *= 0.5f;
        button.colors = colBlock;

        Ready();
        CheckAllPlayerState();
    }


    void Init()
    {
        _IsGoGame = true;

        Button button = GetComponent<Button>();
        ColorBlock colBlock = new ColorBlock();
        colBlock = button.colors;
        colBlock.normalColor = Color.white;
        colBlock.highlightedColor = Color.white;
        colBlock.pressedColor = new Color(0.79f, 0.79f, 0.79f, 1.0f);
        button.colors = colBlock;
    }

    public void GameStart()
    {
        if (FadeManager.Instance.Fading)
            return;

        if (!_IsGoGame)
            return;

        if (PhotonNetwork.inRoom)
        {
            if (!PhotonNetwork.isMasterClient)
                return;

            photonView.RPC("LoadGameMain", PhotonTargets.All);
        }
        else
        {
            SceneChanger.Instance.ChangeScene("GameMain", true);
            //SceneManager.LoadScene("GameMain");
        }
    }

    [PunRPC]
    public void LoadGameMain()
    {
        SceneChanger.Instance.ChangeScene("OnlineGameMain", true);
        //SceneManager.LoadScene("OnlineGameMain");
    }
}
