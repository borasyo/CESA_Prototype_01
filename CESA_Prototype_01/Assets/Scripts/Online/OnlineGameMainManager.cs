using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnlineGameMainManager : Photon.MonoBehaviour
{
    void OnPhotonPlayerDisconnected()
    {
        PhotonNetwork.LeaveRoom();      // 退室
        SceneManager.LoadScene("ModeSelect");
    }
}
