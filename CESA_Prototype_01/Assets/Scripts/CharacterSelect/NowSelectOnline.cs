﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UniRx;
using UniRx.Triggers;

public class NowSelectOnline : NowSelect
{
    CharacterSelectOnline _charaSele = null;
    RectTransform _rectTrans = null;
    Vector2 anchoredPos = Vector2.zero;
    PhotonView _photonView = null;

    [SerializeField] bool _IsInit = false;
    [SerializeField] int _nInitNumber = -1; 

    void Start()
    {
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        GameObject selectCanvas = null;
        yield return new WaitWhile(() => (selectCanvas = GameObject.FindWithTag("SelectCanvas")) == null);
        
        _charaSele = selectCanvas.GetComponent<CharacterSelectOnline>();    //  Missingバグ発生
        _rectTrans = transform.parent.GetComponent<RectTransform>();
        _photonView = GetComponent<PhotonView>();

        if (_IsInit)
            //Set();
            StartCoroutine(Set());
        else
        {
            if (_nInitNumber < 0)
            {
                _nInitNumber = _charaSele.GetNullIdx();
            }

            _charaSele.SetNowSelect(this, _nInitNumber);
            SetDestroyCheck();
            _charaType = _charaSele.GetCharaType(_nInitNumber);
            if (_charaType == CharacterSelect.eCharaType.NONE && !_IsOnNone)
            {
                _charaType = CharacterSelect.eCharaType.BALANCE;
            }
        }

        //  CPUなら
        if (transform.parent.name.Contains("CPU"))
        {
            StartCoroutine(SetLevel());
        }

        this.UpdateAsObservable()
            .Where(_ => !transform.parent.parent)
            .Subscribe(_ =>
            {
                transform.parent.SetParent(_charaSele.transform);
            });

        this.UpdateAsObservable()
            .Where(_ => _rectTrans)
            .Subscribe(_ =>
            {
                _rectTrans.localPosition = _charaSele.GetLocalPos(gameObject);
                transform.parent.localScale = Vector3.one;
            });

        PlayerNumber playerNumber = transform.parent.GetComponentInChildren<PlayerNumber>();
        if (playerNumber)
        {
            playerNumber.Set(_nInitNumber);
            this.ObserveEveryValueChanged(_ => _nInitNumber)
                .Subscribe(_ =>
                {
                    transform.parent.GetComponentInChildren<PlayerNumber>().Set(_nInitNumber);
                });
        }
    }

    IEnumerator Set()
    //void Set()
    {
        if (_photonView.isMine)
        {
            yield return null;

            if (!_charaSele)
                _charaSele = GameObject.FindWithTag("SelectCanvas").GetComponent<CharacterSelectOnline>();

            int idx = 0;
            if (CharacterSelectOnline._nMyNumber <= 0)
            {
                idx = RoomManager.Instance.nMyPlayerCount;
                //Debug.Log("Start : " + idx);
            }
            else
            {
                idx = CharacterSelectOnline._nMyNumber;
                //Debug.Log("Already : " + idx);
            }
            _photonView.RPC("AllSet", PhotonTargets.All, idx);
        }
    }

    void OnPhotonPlayerConnected()
    {
        if (!_photonView.isMine || transform.parent.name.Contains("CPU") || PhotonNetwork.isMasterClient)
            return;

        _photonView.RPC("AllSet", PhotonTargets.All, _nInitNumber);
    }


    [PunRPC]
    public void AllSet(int idx)
    {
        StartCoroutine(WaitSet(idx));
    }

    IEnumerator WaitSet(int idx)
    {
        yield return new WaitWhile(() => _charaSele == null);

        _nInitNumber = idx; // _charaSele.GetCreateNumber();

        _charaSele.SetNowSelect(this, _nInitNumber);
        SetDestroyCheck();
        _charaType = _charaSele.GetCharaType(_nInitNumber);
        if(_charaType == CharacterSelect.eCharaType.NONE && !_IsOnNone)
        {
            _charaType = CharacterSelect.eCharaType.BALANCE;
        }
    }

    IEnumerator SetLevel()
    {
        yield return new WaitWhile(() => _charaSele.InstanceCheck(gameObject) <= -1);
        transform.parent.GetComponentInChildren<NowLevelOnline>().SetLevel(_charaSele.InstanceCheck(gameObject));
    }

    //IEnumerator SetDestroyCheck()
    void SetDestroyCheck()
    {
        //yield return null;

        this.UpdateAsObservable()
            .Where(_ => _photonView.isMine && _charaSele.InstanceCheck(gameObject) < 0)
            .Subscribe(_ =>
            {
                PhotonNetwork.Destroy(transform.parent.gameObject);
            });
    }

    //  プレイヤーが退室し、変更がある場合実行
    public void PlayerChange(int idx)
    {
        _nInitNumber = idx;

        if (PhotonNetwork.isMasterClient)
            return;

        if (!PhotonNetwork.inRoom)
            return;

        CharacterSelectOnline._nMyNumber = idx;
    }

    public override void Add()
    {
        if (Ready.nReadyCnt == PhotonNetwork.playerList.Length)
            return;

        if (transform.parent.name.Contains("CPU"))
        {
            if (PhotonNetwork.isMasterClient)
                _charaType++;

            return;
        }

        if (_photonView.isMine)
            _charaType++;
    }
    public override void None()
    {
        if (!photonView.isMine)
            return;

        base.None();
    }

    // データの送受信
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (!transform.parent.name.Contains("CPU"))
        {
            if (stream.isWriting)
            {
                // データの送信
                stream.SendNext(_charaType);
            }
            else
            {
                // データの受信
                this._charaType = (CharacterSelect.eCharaType)stream.ReceiveNext();
            }
        }
        else
        {
            if (PhotonNetwork.isMasterClient)
            {
                // データの送信
                stream.SendNext(_charaType);
            }
            else
            {
                // データの受信
                this._charaType = (CharacterSelect.eCharaType)stream.ReceiveNext();
            }
        }
    }

    void OnDestroy()
    {
        if(photonView.isMine)
            _charaSele.SetPlayerNumber(_nInitNumber);

        if (transform.parent.name.Contains("CPU"))
            return;

        _charaSele.PlayerChange(_nInitNumber);

        if (!PhotonNetwork.isMasterClient)
            return;

        //  空いた箇所にCPUを作成する
        string player = (_charaSele.GetNullIdx() + 1).ToString();

        if (player == "0")
            return;

        GameObject obj = PhotonNetwork.Instantiate("Prefabs/CharacterSelect/" + player + "P_CPU", Vector3.zero, Quaternion.identity, 0);
    }
}