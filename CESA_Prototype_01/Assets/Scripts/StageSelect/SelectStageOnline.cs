using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectStageOnline : SelectStage
{
    void Awake()
    {
        PhotonNetwork.sendRateOnSerialize = PhotonNetwork.sendRate = 65; 
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
            stream.SendNext(nRand);
        }
        else
        {
            StageNumber = (int)stream.ReceiveNext();
            nRand = (int)stream.ReceiveNext();
        }
    }
}
