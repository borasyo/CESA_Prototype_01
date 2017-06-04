using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectStageOnline : SelectStage
{
    void Awake()
    {
        PhotonNetwork.sendRateOnSerialize = PhotonNetwork.sendRate = 60; 
    }

    public override void Add()
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        base.Add();
    }

    public override void Sub()
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        base.Sub();
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (PhotonNetwork.isMasterClient)
        {
            stream.SendNext(StageNumber);
        }
        else
        {
            StageNumber = (int)stream.ReceiveNext();
        }
    }
}
