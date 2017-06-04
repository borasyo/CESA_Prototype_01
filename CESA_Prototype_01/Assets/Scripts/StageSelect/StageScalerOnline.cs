using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageScalerOnline : StageScaler
{
    public override void Small()
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        base.Small();
    }
    public override void Normal()
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        base.Normal();
    }
    public override void Big()
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        base.Big();
    }
    
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (PhotonNetwork.isMasterClient)
        {
            stream.SendNext(StageScale);
        }
        else
        {
            StageScale = (int)stream.ReceiveNext();
        }
    }
}
