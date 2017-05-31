using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputAIOnline : CharacterInputAI
{
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // データの送信
            stream.SendNext(_IsForawrd);
            stream.SendNext(_IsBack);
            stream.SendNext(_IsRight);
            stream.SendNext(_IsLeft);
            stream.SendNext(_IsPut);
            stream.SendNext(_IsBreak);
        }
        else
        {
            // データの受信
            _IsForawrd = (bool)stream.ReceiveNext();
            _IsBack = (bool)stream.ReceiveNext();
            _IsRight = (bool)stream.ReceiveNext();
            _IsLeft = (bool)stream.ReceiveNext();
            _IsPut = (bool)stream.ReceiveNext();
            _IsBreak = (bool)stream.ReceiveNext();
        }
    }
}
