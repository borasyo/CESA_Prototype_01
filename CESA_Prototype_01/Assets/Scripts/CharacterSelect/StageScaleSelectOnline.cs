using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageScaleSelectOnline : StageScaleSelect
{
    protected override void UpDown()
    {
        _fNowInterval += Time.deltaTime;
        if (_fNowInterval < _fInterval)
            return;

        if (_IsWidth)
        {
            if (PhotonNetwork.isMasterClient)
            {
                if (Input.GetKey(KeyCode.Z))
                {
                    _size++;
                    _fNowInterval = 0.0f;
                    if (_size > 30)
                        _size = 30;
                }
                else if (Input.GetKey(KeyCode.X))
                {
                    _size--;
                    _fNowInterval = 0.0f;
                    if (_size < 6)
                        _size = 6;
                }
            }

            // 毎回チェック
            if (_size < GameScaler._nHeight - 2)
                _size = GameScaler._nHeight - 2;

            GameScaler._nWidth = _size + 2;
            _text.text = "Width : " + _size;
        }
        else
        {
            if (PhotonNetwork.isMasterClient)
            {
                if (Input.GetKey(KeyCode.C))
                {
                    _size++;
                    _fNowInterval = 0.0f;
                    if (_size > 25)
                        _size = 25;
                }
                else if (Input.GetKey(KeyCode.V))
                {
                    _size--;
                    _fNowInterval = 0.0f;
                    if (_size < 5)
                        _size = 5;
                }
            }

            GameScaler._nHeight = _size + 2;
            _text.text = "Height : " + _size;
        }
    }

    public override void UpSize()
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        if (_IsWidth)
        {
            _size++;
            _fNowInterval = 0.0f;
            if (_size > 30)
                _size = 30;
        }
        else
        {
            _size++;
            _fNowInterval = 0.0f;
            if (_size > 25)
                _size = 25;
        }
    }

    public override void DownSize()
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        if (_IsWidth)
        {
            _size--;
            _fNowInterval = 0.0f;
            if (_size < 6)
                _size = 6;
        }
        else
        {
            _size--;
            _fNowInterval = 0.0f;
            if (_size < 5)
                _size = 5;
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // データの送信
            stream.SendNext(_size);
        }
        else
        {
            // データの受信
            this._size = (int)stream.ReceiveNext();
        }
    }
}
