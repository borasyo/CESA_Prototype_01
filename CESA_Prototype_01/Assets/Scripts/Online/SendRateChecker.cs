using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendRateChecker : MonoBehaviour
{
    void Update()
    {
        PhotonNetwork.sendRateOnSerialize = PhotonNetwork.sendRate = (int)(1.0f / Time.unscaledDeltaTime);
    }

    /*void Update()
    {
        //PhotonNetwork.sendRateOnSerialize = PhotonNetwork.sendRate = (int)(1.0f / Time.deltaTime);
    }*/
}
