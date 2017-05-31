using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputUserOnline : CharacterInputUser
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

            if (_IsPut)
            {
                Debug.Log("置くフラグを受け取った");
            }
            if (_IsBreak)
            {
                Debug.Log("破壊フラグを受け取った");
            }
        }
    }

    /*[PunRPC]
    public void SetMove(bool isForward, bool isBack, bool isRight, bool isLeft)
    {
        _IsForawrd = isForward;
        _IsBack = isBack;
        _IsRight = isRight;
        _IsLeft = isLeft;
        Debug.Log("Forward : " + _IsForawrd + ", Back : " + _IsBack + ", Right : " + _IsRight + ", Left : " + _IsLeft);
    }

    [PunRPC]
    public void SetAction(bool isPut, bool isBreak)
    {
        _IsPut = isPut;
        _IsBreak = isBreak;
        Debug.Log("Put : " + _IsPut + ", Break : " + _IsBreak);
    }*/
}
