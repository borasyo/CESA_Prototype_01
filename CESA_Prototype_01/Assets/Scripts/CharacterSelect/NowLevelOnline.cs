using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NowLevelOnline : NowLevel
{ 
	public void SetLevel(int number)
    {
        GameObject selectCanvas = GameObject.FindWithTag("SelectCanvas");
        selectCanvas.GetComponent<LevelSelectOnline>().SetLevel(this, number);
    }

    public override void OnClick()
    {
        if (!photonView.isMine)
            return;

        _nNowLevel--;
        if (_nNowLevel < 0)
            _nNowLevel += 3;
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // データの送信
            stream.SendNext(_nNowLevel);
        }
        else
        {
            // データの受信
            this._nNowLevel = (int)stream.ReceiveNext();
        }
    }
}
