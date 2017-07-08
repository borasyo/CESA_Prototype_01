using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NowLevelOnline : NowLevel
{ 
	public void SetLevel(int number)
    {
        GameObject selectCanvas = GameObject.FindWithTag("SelectCanvas");
        selectCanvas.GetComponent<LevelSelectOnline>().SetLevel(this, number);
        //_nNowLevel = LevelSelectOnline.SelectLevel[number];
    }

    public override void OnClick()
    {
        if (Ready.nReadyCnt == PhotonNetwork.playerList.Length)
            return;

        if (!photonView.isMine)
            return;

        _nNowLevel--;
        if (_nNowLevel < 0)
            _nNowLevel += 3;
    }

    public void WaitSet()
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        photonView.RPC("Set", PhotonTargets.Others, _nNowLevel);
    }

    [PunRPC] 
    public void Set(int level)
    {
        StartCoroutine(SetWait(_nNowLevel));
    }

    IEnumerator SetWait(int level)
    {
        yield return new WaitWhile(() => !FindObjectOfType<CharacterSelectOnline>());

        _nNowLevel = level;
        Debug.Log(transform.name + "," + _nNowLevel);
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
