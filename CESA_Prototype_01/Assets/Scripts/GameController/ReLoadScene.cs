using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReLoadScene : Photon.MonoBehaviour 
{
    public void ReLoadModeSelect()
    {
        if (FadeManager.Instance.Fading)
            return;

        if (!PhotonNetwork.inRoom)
        {
            for (int i = 0; i < RoundCounter.nRoundCounter.Length; i++)
                RoundCounter.nRoundCounter[i] = 0;
            SceneChanger.Instance.ChangeScene("ModeSelect", true);
            //SceneManager.LoadScene("ModeSelect");
        }
        else
        {
            if (!PhotonNetwork.isMasterClient && SceneManager.GetActiveScene().name != "OnlineRoom")
                return;

            if (PhotonNetwork.isMasterClient)
                PhotonNetwork.DestroyAll();

            photonView.RPC("OnlineReLoadModeSelect", PhotonTargets.All);
        }
    }

    [PunRPC]
    public void OnlineReLoadModeSelect()
    {
        for (int i = 0; i < RoundCounter.nRoundCounter.Length; i++)
            RoundCounter.nRoundCounter[i] = 0;

        PhotonNetwork.LeaveRoom();      // 退室
        CharacterSelectOnline._nMyNumber = 0;
        SceneChanger.Instance.ChangeScene("ModeSelect", true);
    }

    public void ReLoadCharaSelect()
    {
        if (FadeManager.Instance.Fading)
            return;

        if (!PhotonNetwork.inRoom)
        {
            for (int i = 0; i < RoundCounter.nRoundCounter.Length; i++)
                RoundCounter.nRoundCounter[i] = 0;

            SceneChanger.Instance.ChangeScene("CharacterSelect", true);
            //SceneManager.LoadScene("CharacterSelect");
        }
        else
        {
            if (!PhotonNetwork.isMasterClient)
                return;

            PhotonNetwork.DestroyAll();
            photonView.RPC("LoadOnlineRoom", PhotonTargets.All);
        }
    }

    [PunRPC]
    public void LoadOnlineRoom()
    {
        if (FadeManager.Instance.Fading)
            return;

        for (int i = 0; i < RoundCounter.nRoundCounter.Length; i++)
            RoundCounter.nRoundCounter[i] = 0;

        SceneChanger.Instance.ChangeScene("OnlineRoom", true);
        //SceneManager.LoadScene("OnlineRoom");
    }

    public void Retry()
    {
        if (FadeManager.Instance.Fading)
            return;

        if (!PhotonNetwork.inRoom)
        {
            for (int i = 0; i < RoundCounter.nRoundCounter.Length; i++)
                RoundCounter.nRoundCounter[i] = 0;

            SceneChanger.Instance.ChangeScene("GameMain", true);
            //SceneManager.LoadScene("CharacterSelect");
        }
        else
        {
            if (!PhotonNetwork.isMasterClient)
                return;

            PhotonNetwork.DestroyAll();
            photonView.RPC("LoadGameMain", PhotonTargets.All);
        }
    }

    [PunRPC]
    public void LoadGameMain()
    {
        if (FadeManager.Instance.Fading)
            return;

        for (int i = 0; i < RoundCounter.nRoundCounter.Length; i++)
            RoundCounter.nRoundCounter[i] = 0;

        SceneChanger.Instance.ChangeScene("OnlineGameMain", true);
        //SceneManager.LoadScene("OnlineRoom");
    }
}
