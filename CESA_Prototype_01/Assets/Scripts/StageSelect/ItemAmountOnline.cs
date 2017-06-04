using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAmountOnline : ItemAmount
{
    public override void None()
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        base.None();
    }

    public override void Few()
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        base.Few();
    }

    public override void Normal()
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        base.Normal();
    }

    public override void Many()
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        base.Many();
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (PhotonNetwork.isMasterClient)
        {
            stream.SendNext(nItemAmount);
        }
        else
        {
            nItemAmount = (int)stream.ReceiveNext();
        }
    }
}
