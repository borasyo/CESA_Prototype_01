﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterOnline : Character
{
    protected override void SetInput(int level)
    {
        // Input生成
        if (this.name.Contains("CPU"))
        {
            CharacterInputAI ai = this.gameObject.GetComponent<CharacterInputAIOnline>();
            _charactorInput = ai;

            if (!ai._enemyAI)
                return;

            ai._enemyAI.Set(level, _charaType);
        }
        else
        {
            _charactorInput = this.gameObject.GetComponent<CharacterInputUserOnline>();
        }
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
        if (!photonView.isMine)
            return;

        base.Update();
    }

    public void OnlineUpdate()
    {
        Debug.Log("OnlineUpdate : " + transform.name);

        _nOldNumber = GetDataNumber();

        //MoveUpdate(); //  移動は座標同期
        MoveCheck(eDirection.FORWARD);
        MoveCheck(eDirection.BACK);
        MoveCheck(eDirection.RIGHT);
        MoveCheck(eDirection.LEFT);

        DirUpdate();
        NumberUpdate();

        //  アクション
        //ItemPut();
        //ItemBreak();
    }

    override protected void ItemPut()
    {
        if (!_charactorGauge.PutGaugeCheck() || !_charactorInput.GetActionInput(eAction.PUT))
            return;

        int dirNumber = GetDataNumberForDir();
        if (dirNumber < 0 || GameScaler.GetRange < dirNumber)
            return;

        FieldObjectBase obj = FieldData.Instance.GetObjData(dirNumber);
        if (obj)
            return;

        Vector3 pos = GetPosForNumber(dirNumber);
        photonView.RPC("OnlineItemPut", PhotonTargets.All, pos, dirNumber, true);
    }
   
    override protected void ItemBreak()
    {
        if (!_charactorGauge.BreakGaugeCheck() || !_charactorInput.GetActionInput(eAction.BREAK))
            return;

        int dirNumber = GetDataNumberForDir();
        FieldObjectBase obj = FieldData.Instance.GetObjData(dirNumber);

        if (!obj || obj.GetSandType() == SandItem.eType.MAX)
            return;

        photonView.RPC("OnlineItemBreak", PhotonTargets.All, dirNumber);
    }

    [PunRPC]
    public virtual void OnlineItemPut(Vector3 pos, int dirNumber, bool isPostProcess)
    {   
        GameObject item = (GameObject)Instantiate(_sandItem, pos, Quaternion.identity);
        FieldData.Instance.SetObjData(item.GetComponent<FieldObjectBase>(), dirNumber);

        if (!isPostProcess)
            return;

        _charactorGauge.PutAction();
        _fNotMoveTime = 0.0f;
    } 

    [PunRPC]
    public virtual void OnlineItemBreak(int dirNumber)
    { 
        FieldObjectBase obj = FieldData.Instance.GetObjData(dirNumber);

        FieldData.Instance.SetObjData(null,dirNumber);
        FieldData.Instance.ExceptionChangeField();
        Destroy(obj.gameObject);
        _charactorGauge.BreakAction();
        _fNotMoveTime = 0.0f;
    }

}
