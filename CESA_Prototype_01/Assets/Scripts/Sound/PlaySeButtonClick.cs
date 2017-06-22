using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySeButtonClick : Photon.MonoBehaviour
{
    [SerializeField]
    SoundManager.eSeValue eSeValue;

    [SerializeField]
    float Volume = 1.0f;

    public void OnClick()
    {
        if (PhotonNetwork.inRoom)
        {
            photonView.RPC("OnlinePlaySe", PhotonTargets.All);
        }
        else
        {
            SoundManager.Instance.PlaySE(eSeValue, Volume);
        }
    }

    [PunRPC]
    public void OnlinePlaySe()
    {
        SoundManager.Instance.PlaySE(eSeValue, Volume);
    }
}