using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class ResultChecker : Photon.MonoBehaviour
{
    #region SceneLoadInit

    private bool _isInitialized = false;
    private readonly string ReadyStateKey = "Result";
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

    [SerializeField]
    GameObject intervalCanvas = null;
    [SerializeField]
    GameObject lastCanvas = null;

    //  
    void Start ()
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
        GameObject canvas = null;

        //  ここでIntervalかLastか判断
        if (RoundCounter.nNowWinerPlayer >= 0)
        {
            canvas = intervalCanvas;
            //  flameも変更
        }
        else
        {
            canvas = lastCanvas;
            //  flameも変更
        }

        if (PhotonNetwork.inRoom)
        {
            if (!PhotonNetwork.isMasterClient)
                return;

            PhotonNetwork.Instantiate("Prefabs/Result/" + canvas.name, canvas.transform.position, canvas.transform.rotation, 0);
        }
        else
        {
            Instantiate(canvas);
        }
    }
}
