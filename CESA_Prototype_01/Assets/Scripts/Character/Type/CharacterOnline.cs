using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class CharacterOnline : Character
{
    protected bool _IsPutWait = false;

    protected override void SetInput(int level)
    {
        // Input生成
        if (name.Contains("CPU"))
        {
            CharacterInputAI ai = gameObject.GetComponent<CharacterInputAIOnline>();
            _charactorInput = ai;

            if (!ai._enemyAI)
                return;

            ai._enemyAI.Set(level, _charaType);
        }
        else
        {
            _charactorInput = gameObject.GetComponent<CharacterInputUserOnline>();
        }

        //  Synchronizeを設定
        GetComponent<PhotonTransformView>().SetSynchronizedValues(new Vector3(_moveAmount_Sec, 0.0f, _moveAmount_Sec), 0);
    }

    [PunRPC]
    public void Create(string player, int angle, int idx, int level)
    {
        transform.SetParent(GameObject.Find("CharaHolder").transform);
        name += player;
        transform.eulerAngles = new Vector3(0, angle, 0);
        GetComponent<Character>().Init(level);
    }

    void Update()
    {
        NumberUpdate();
        _nOldNumber = GetDataNumber();
    }

    public void OnlineMoveUpdate()
    {
        if (_IsPutWait || _animator.GetBool("Put") || _animator.GetBool("Break") || _animator.GetBool("Happy"))
            return;

        //if (PhotonNetwork.isMasterClient)
        if (photonView.isMine)
        {
            MoveUpdate();
            //photonView.RPC("SerializePos", PhotonTargets.Others, transform.position);
        }
        else
        {
            MoveCheck(eDirection.FORWARD);
            MoveCheck(eDirection.BACK);
            MoveCheck(eDirection.RIGHT);
            MoveCheck(eDirection.LEFT);
        }
        DirUpdate();
        NotMoveUpdate();
    }

    public void OnlineActionUpdate()
    {
        if (_animator.GetBool("Put") || _animator.GetBool("Break"))
            return;

        //  アクション
        ItemPut();
        ItemBreak();
    }

    public void OnlineActionCheck(bool isPut) //, bool isBreak)
    {
        if (!_charactorGauge.PutGaugeCheck() || !isPut)
            return;

        int dirNumber = GetDataNumberForDir();
        if (dirNumber < 0 || GameScaler.GetRange < dirNumber)
            return;

        FieldObjectBase obj = FieldData.Instance.GetObjData(dirNumber);
        if (obj) // && obj.gameObject != gameObject)
            return;

        _IsPutWait = true;
    }

    protected override void ItemPut()
    {
        if (!_charactorGauge.PutGaugeCheck() || !_charactorInput.GetActionInput(eAction.PUT))
        {
            photonView.RPC("OffPutWait", PhotonTargets.All);
            return;
        }

        int dirNumber = GetDataNumberForDir();
        if (dirNumber < 0 || GameScaler.GetRange < dirNumber)
        {
            photonView.RPC("OffPutWait", PhotonTargets.All);
            return;
        }

        FieldObjectBase obj = FieldData.Instance.GetObjData(dirNumber);
        if (obj) // && obj.gameObject != gameObject)
        {
            photonView.RPC("OffPutWait", PhotonTargets.All);
            return;
        }
        
        Vector3 pos = GetPosForNumber(dirNumber);
        photonView.RPC("OnlineItemPut", PhotonTargets.All, pos, dirNumber, true);
    }
   
    protected override void ItemBreak()
    {
        if (!_charactorGauge.BreakGaugeCheck() || !_charactorInput.GetActionInput(eAction.BREAK))
            return;

        int dirNumber = GetDataNumberForDir();
        FieldObjectBase obj = FieldData.Instance.GetObjData(dirNumber);

        if (!obj || obj.GetSandType() == SandItem.eType.MAX)
            return;

        _animator.SetBool("Break", true);
        photonView.RPC("OnlineItemBreak", PhotonTargets.All, dirNumber);
    }

    [PunRPC]
    public virtual void OnlineItemPut(Vector3 pos, int dirNumber, bool isPostProcess)
    {
        FieldObjectBase obj = FieldData.Instance.GetObjData(dirNumber);
        if (obj) // && obj.gameObject != gameObject)
        {
            _IsPutWait = false;
            return;
        }

        Instantiate(_sandItem, pos, Quaternion.identity);

        if (!isPostProcess)
            return;

        _IsPutWait = false;
        _charactorGauge.PutAction();
        _fNotMoveTime = 0.0f;
        _animator.SetBool("Put", true);
    } 

    [PunRPC]
    public virtual void OnlineItemBreak(int dirNumber)
    { 
        FieldObjectBase obj = FieldData.Instance.GetObjData(dirNumber);

        if (!obj)
            return;

        StartCoroutine(Break(obj));
        _animator.SetBool("Break", true);
    }

    [PunRPC]
    public void OffPutWait()
    {
        _IsPutWait = false;
    }
}
