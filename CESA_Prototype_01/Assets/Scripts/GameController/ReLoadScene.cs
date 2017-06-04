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
            PhotonNetwork.LeaveRoom();      // 退室
            CharacterSelectOnline._nMyNumber = 0;
        }

        SceneManager.LoadScene("ModeSelect");
    }
    public void ReLoadCharaSelect()
    {
        if (!PhotonNetwork.inRoom)
        {
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
        SceneManager.LoadScene("OnlineRoom");
    }
}
