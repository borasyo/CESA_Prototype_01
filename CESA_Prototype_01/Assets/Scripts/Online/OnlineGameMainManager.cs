using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnlineGameMainManager : Photon.MonoBehaviour
{
    void OnPhotonPlayerDisconnected()
    {
        SceneManager.LoadScene("ModeSelect");
    }
}
