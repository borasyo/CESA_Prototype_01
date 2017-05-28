using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UniRx;
using UniRx.Triggers;

public class NowSelectOnline : NowSelect
{
    CharacterSelectOnline _charaSele = null;

    [SerializeField] bool _IsInit = false;
    PhotonView _photonView = null;

    void Start()
    {
        _charaSele = GameObject.FindWithTag("SelectCanvas").GetComponent<CharacterSelectOnline>();
        _photonView = GetComponent<PhotonView>();

        if (_IsInit)
            _photonView.RPC("Init", PhotonTargets.All);
   
        StartCoroutine(SetDestroyCheck());
    }

    IEnumerator SetDestroyCheck()
    {
        yield return null;

        this.UpdateAsObservable()
            .Where(_ => !_IsInit && _photonView.isMine && !_charaSele.InstanceCheck(gameObject))
            .Subscribe(_ =>
            {
                PhotonNetwork.Destroy(transform.parent.gameObject);
            });
    }

    [PunRPC]
    public void Init()
    {
        StartCoroutine(Set());
    }

    IEnumerator Set()
    {
        yield return null;
        
        if (!_charaSele)
            _charaSele = GameObject.FindWithTag("SelectCanvas").GetComponent<CharacterSelectOnline>();
        _charaSele.SetPlayerNumber();

        if (!PhotonNetwork.isMasterClient)
            _charaSele.SetNowSelect(this, CharacterSelectOnline._nMyNumber);

        _IsInit = false;
    }


    public override void Add()
    {
        if (transform.parent.childCount >= 2)
        {
            if (CharacterSelectOnline._nMyNumber == 0)
            {
                _charaType++;
            }
            return;
        }

        if (_photonView.isMine)
            _charaType++;
    }

    // データの送受信
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (transform.parent.childCount < 2)
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

        /*if (stream.isWriting)
        {
            stream.SendNext(transform.parent);
            stream.SendNext(transform.position);
        }
        else
        {
            transform.SetParent((Transform)stream.ReceiveNext());
            transform.position = (Vector3)stream.ReceiveNext();
        }*/
    }
}
