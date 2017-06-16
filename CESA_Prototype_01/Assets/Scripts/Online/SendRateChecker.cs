using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendRateChecker : MonoBehaviour
{
    void Start()
    {
        PhotonNetwork.sendRateOnSerialize = PhotonNetwork.sendRate = 30;
    }

    /*void Update()
    {
        PhotonNetwork.sendRateOnSerialize = PhotonNetwork.sendRate = (int)(1.0f / Time.deltaTime);
    }*/
}
