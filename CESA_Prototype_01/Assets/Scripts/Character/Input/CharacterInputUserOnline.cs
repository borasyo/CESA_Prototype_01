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

        if (Application.platform == RuntimePlatform.Android)
        {
            if (!_moveButton.IsActiveAndMove)
            {
                _IsForawrd = _IsBack = _IsRight = _IsLeft = false;
                return;
            }

            int angle = (int)(_moveButton.GetMoveAngle);
            _IsForawrd = (angle < 135 && angle >= 45);
            _IsBack = (angle < -45 && angle >= -135);
            _IsRight = (angle < 45 && angle >= -45);
            _IsLeft = (angle < -135 || angle >= 135);
        }
        else
        {
            _IsForawrd = Input.GetKey(KeyCode.W);
            _IsBack = Input.GetKey(KeyCode.S);
            _IsRight = Input.GetKey(KeyCode.D);
            _IsLeft = Input.GetKey(KeyCode.A);
            _IsPut = Input.GetKeyDown(KeyCode.T);
            _IsBreak = Input.GetKeyDown(KeyCode.T);
        }
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
}
