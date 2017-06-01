using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputAIOnline : CharacterInputAI
{
    CharacterOnline _characterOnline = null;

    override protected void InputCheck()
    {
        if (!photonView.isMine)
            return;

        _IsForawrd = _enemyAI.GetMove(Character.eDirection.FORWARD);
        _IsBack = _enemyAI.GetMove(Character.eDirection.BACK);
        _IsRight = _enemyAI.GetMove(Character.eDirection.RIGHT);
        _IsLeft = _enemyAI.GetMove(Character.eDirection.LEFT);
        _IsPut = _enemyAI.GetAction(Character.eAction.PUT);
        _IsBreak = _enemyAI.GetAction(Character.eAction.BREAK);

        //  同期する
        //photonView.RPC("Set", PhotonTargets.All, _IsForawrd, _IsBack, _IsRight, _IsLeft, _IsPut, _IsBreak);
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(_IsForawrd);
            stream.SendNext(_IsBack);
            stream.SendNext(_IsRight);
            stream.SendNext(_IsLeft);
            //stream.SendNext(_IsPut);
            //stream.SendNext(_IsBreak);
        }
        else
        {
            this._IsForawrd = (bool)stream.ReceiveNext();
            this._IsBack = (bool)stream.ReceiveNext();
            this._IsRight = (bool)stream.ReceiveNext();
            this._IsLeft = (bool)stream.ReceiveNext();
            //this._IsPut = (bool)stream.ReceiveNext();
            //this._IsBreak = (bool)stream.ReceiveNext();

            if (!_characterOnline)
                _characterOnline = GetComponent<CharacterOnline>();

            _characterOnline.OnlineUpdate();
        }
    }

    /*[PunRPC]
    public void Set(bool isForward, bool isBack, bool isRight, bool isLeft, bool isPut, bool isBreak)
    {
        if (photonView.isMine)
            return;

        _IsForawrd = isForward;
        _IsBack = isBack;
        _IsRight = isRight;
        _IsLeft = isLeft;
        _IsPut = isPut;
        _IsBreak = isBreak;
        Debug.Log("Set");

        if (!_characterOnline)
            _characterOnline = GetComponent<CharacterOnline>();

        //  更新
        _characterOnline.OnlineUpdate();
        // Debug.Log("Forward : " + _IsForawrd + ", Back : " + _IsBack + ", Right : " + _IsRight + ", Left : " + _IsLeft);
    }*/
}
