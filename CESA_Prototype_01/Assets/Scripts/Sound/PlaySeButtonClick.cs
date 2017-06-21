using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySeButtonClick : Photon.MonoBehaviour
{
    [SerializeField]
    SoundManager.eSeValue eSeValue;

    public void OnClick()
    {
        if (PhotonNetwork.inRoom)
        {
            photonView.RPC("OnlinePlaySe", PhotonTargets.All);
        }
        else
        {
            SoundManager.Instance.PlaySE(eSeValue);
        }
    }

    [PunRPC]
    public void OnlinePlaySe()
    {
        SoundManager.Instance.PlaySE(eSeValue);
    }
}