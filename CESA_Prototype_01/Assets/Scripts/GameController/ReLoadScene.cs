using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReLoadScene : Photon.MonoBehaviour 
{
    public void ReLoadModeSelect()
    {
        if (PhotonNetwork.inRoom)
        {
            if(PhotonNetwork.isMasterClient)
                PhotonNetwork.DestroyAll();

            PhotonNetwork.LeaveRoom();      // 退室
            CharacterSelectOnline._nMyNumber = 0;
        }

        SceneManager.LoadScene("ModeSelect");
    }
    public void ReLoadCharaSelect()
    {
        if (!PhotonNetwork.inRoom)
        {
            for (int i = 0; i < RoundCounter.nRoundCounter.Length; i++)
                RoundCounter.nRoundCounter[i] = 0;

            SceneManager.LoadScene("CharacterSelect");
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
        for (int i = 0; i < RoundCounter.nRoundCounter.Length; i++)
            RoundCounter.nRoundCounter[i] = 0;
        
        SceneManager.LoadScene("OnlineRoom");
    }
}
