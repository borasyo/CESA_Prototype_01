using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputUserOnline : CharacterInputUser
{
    CharacterOnline _characterOnline = null;

    void Start()
    {
        if (!photonView.isMine)
            return;

        base.Start();
    }

    override protected void InputCheck()
    {
        if (!photonView.isMine)
            return;

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (!_moveButton.IsActiveAndMove)
            {
                _IsForawrd = _IsBack = _IsRight = _IsLeft = false;
            }
            else
            {
                int angle = (int)(_moveButton.GetMoveAngle);
                _IsForawrd = (angle < 135 && angle >= 45);
                _IsBack = (angle < -45 && angle >= -135);
                _IsRight = (angle < 45 && angle >= -45);
                _IsLeft = (angle < -135 || angle >= 135);
            }
        }
        else
        {
            _IsForawrd = (Input.GetAxisRaw("Vertical") >= 1.0f);// Input.GetKey(KeyCode.W);
            _IsBack = (Input.GetAxisRaw("Vertical") <= -1.0f);  //Input.GetKey(KeyCode.S);
            _IsRight = (Input.GetAxisRaw("Horizontal") >= 1.0f); //Input.GetKey(KeyCode.D);
            _IsLeft = (Input.GetAxisRaw("Horizontal") <= -1.0f); //Input.GetKey(KeyCode.A);
            _IsPut = _IsBreak = Input.GetButtonDown("Action"); // Input.GetKeyDown(KeyCode.T);
            photonView.RPC("SetAction", PhotonTargets.MasterClient, _IsPut, _IsBreak);
            _characterOnline.OnlineActionCheck(_IsPut);
        }
        
        photonView.RPC("SetMove", PhotonTargets.All, _IsForawrd, _IsBack, _IsRight, _IsLeft);
    }

    public override IEnumerator ActionClick()
    {
        _IsPut = true;
        _IsBreak = true;

        photonView.RPC("SetAction", PhotonTargets.MasterClient, _IsPut, _IsBreak);
        _characterOnline.OnlineActionCheck(_IsPut);

        yield return null;

        _IsPut = false;
        _IsBreak = false;
    }

    [PunRPC]
    public void SetMove(bool isForward, bool isBack, bool isRight, bool isLeft)
    {
        _IsForawrd = isForward;
        _IsBack = isBack;
        _IsRight = isRight;
        _IsLeft = isLeft;

        if (!_characterOnline)
            _characterOnline = GetComponent<CharacterOnline>();

        _characterOnline.OnlineMoveUpdate();
    }

    //  Masterに情報を送信
    [PunRPC]
    public void SetAction(bool isPut, bool isBreak)
    {
        _IsPut = isPut;
        _IsBreak = isBreak;

        // 追加
        /*if (!photonView.isMine)
            return;*/

        if (!_characterOnline)
            _characterOnline = GetComponent<CharacterOnline>();

        _characterOnline.OnlineActionUpdate();
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        /*if (stream.isWriting)
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
        }*/
    }
}
