using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoGame : Photon.MonoBehaviour
{
	public void GameStart()
    {
        if (PhotonNetwork.inRoom)
        {
            if (!PhotonNetwork.isMasterClient)
                return;

            photonView.RPC("LoadGameMain", PhotonTargets.All);
        }
        else
        {
            SceneManager.LoadScene("GameMain");
        }
    }

    [PunRPC]
    public void LoadGameMain()
    {
        SceneManager.LoadScene("OnlineGameMain");
    }
}
