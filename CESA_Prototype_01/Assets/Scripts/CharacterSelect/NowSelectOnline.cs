using System.Collections;
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
    int _nInstanceNumber = 0;

    void Start()
    {
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        GameObject selectCanvas = null;
        yield return new WaitWhile(() => (selectCanvas = GameObject.FindWithTag("SelectCanvas")) == null);
        
        _charaSele = selectCanvas.GetComponent<CharacterSelectOnline>();
        _rectTrans = transform.parent.GetComponent<RectTransform>();
        _photonView = GetComponent<PhotonView>();

        if (_IsInit)
            //Set();
            StartCoroutine(Set());
        else
        {
            _charaSele.SetNowSelect(this, _nInitNumber);
            SetDestroyCheck();
        }

        //  CPUなら
        if (transform.parent.childCount >= 2)
        {
            StartCoroutine(SetLevel());
        }

        //StartCoroutine(SetDestroyCheck());
        //SetDestroyCheck();

        this.UpdateAsObservable()
            .Where(_ => !transform.parent.parent)
            .Subscribe(_ =>
            {
                transform.parent.SetParent(_charaSele.transform);
            });

        this.UpdateAsObservable()
            .Where(_ => _photonView.isMine)
            .Subscribe(_ =>
            {
                _photonView.RPC("SetPosition", PhotonTargets.All, _rectTrans.localPosition.x, _rectTrans.localPosition.y);
            });
    }

    IEnumerator SetLevel()
    {
        Debug.Log("c : " + _charaSele.InstanceCheck(gameObject));
        yield return new WaitWhile(() => _charaSele.InstanceCheck(gameObject) <= -1);
        Debug.Log("d : " + _charaSele.InstanceCheck(gameObject));
        transform.parent.GetComponentInChildren<NowLevelOnline>().SetLevel(_charaSele.InstanceCheck(gameObject));
    }

    //IEnumerator SetDestroyCheck()
    void SetDestroyCheck()
    {
        //yield return null;

        _nInstanceNumber = _charaSele.InstanceCheck(gameObject);
        this.UpdateAsObservable()
            .Where(_ => !_IsInit && _photonView.isMine && _charaSele.InstanceCheck(gameObject) < 0)
            .Subscribe(_ =>
            {
                PhotonNetwork.Destroy(transform.parent.gameObject);
            });
    }

    [PunRPC]
    public void SetPosition(float x, float y)
    {
        if (!_rectTrans)
            return;

        //Debug.Log("SetPosition");
        _rectTrans.localPosition = new Vector2(x,y);
    }

    [PunRPC]
    public void CPUInit(int idx)
    {
        if (!_charaSele)
        {
            GameObject obj = GameObject.FindWithTag("SelectCanvas");
            if (!obj)
                return;
            _charaSele = obj.GetComponent<CharacterSelectOnline>();
        }

        _charaSele.SetNowSelect(this, idx);
    }

    IEnumerator Set()
    //void Set()
    {
        yield return new WaitWhile(() => _charaSele.EndInit() == false);

        Debug.LogError("asd");
        if (!_charaSele)
            _charaSele = GameObject.FindWithTag("SelectCanvas").GetComponent<CharacterSelectOnline>();
        _charaSele.SetPlayerNumber();
        
        _charaSele.SetNowSelect(this, _charaSele.GetCreateNumber());

        _IsInit = false;

        SetDestroyCheck();
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
    }

    void OnEnable()
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        if (transform.parent.childCount >= 2)
            return;

        //  CPUを作成する
        GameObject obj = PhotonNetwork.Instantiate("Prefabs/CharacterSelect/CPUSelect", Vector3.zero, Quaternion.identity, 0);
        if (!obj || !obj.transform.Find("TypeText"))
            return;

        PhotonView nowSelect = obj.transform.Find("TypeText").GetComponent<PhotonView>();
        nowSelect.RPC("CPUInit", PhotonTargets.All, _nInstanceNumber);
    }
}
