using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendRateChecker : MonoBehaviour
{
	// Update is called once per frame
	void Update ()
    {
        PhotonNetwork.sendRateOnSerialize = PhotonNetwork.sendRate = (int)(1.0f / Time.deltaTime);
    }
}
