using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class TechnicalType : Charactor 
{
    [SerializeField] float _fKickMoveAmount_Sec = 1.0f;
    [SerializeField] bool _IsAuthBlock = false;

    public override void Init(int level)
    {
        _charaType = eCharaType.TECHNICAL;
        base.Init(level);

        this.UpdateAsObservable()
            .Where(_ => _IsSpecialMode)
            .Subscribe(_ => {
                KickCheck();
            });
    }

    void KickCheck()
    {
        int dirNumber = GetDataNumberForDir();
        //Debug.Log("KickCheck : " + dirNumber);
        if (dirNumber < 0 || GameScaler.GetRange < dirNumber)
            return;

        //  キックできないオブジェクトを判定
        FieldObjectBase obj = FieldData.Instance.GetObjData(dirNumber);
        if (!obj)
            return;

        if (obj.name.Contains("Move"))
            return;

        if (obj.tag == "Charactor")
            return;

        if (obj.tag == "Block" && (!_IsAuthBlock || obj.name.Contains("Fence")))
            return;

        if (!PushObj())
            return;
     
        obj.gameObject.AddComponent<AutoMoveObj>().Init(_nowDirection, MoveAmount());
    }

    bool PushObj ()
    {
        bool bResult = false;
        switch (_nowDirection)
        {
            case eDirection.FORWARD:
                bResult = _charactorInput.GetMoveInput(eDirection.FORWARD);
                break;
            case eDirection.BACK:
                bResult = _charactorInput.GetMoveInput(eDirection.BACK);
                break;
            case eDirection.RIGHT:
                bResult = _charactorInput.GetMoveInput(eDirection.RIGHT);
                break;
            case eDirection.LEFT:
                bResult = _charactorInput.GetMoveInput(eDirection.LEFT);
                break;
        }
        return bResult;
    }

    Vector3 MoveAmount() 
    {
        Vector3 move = Vector3.zero;

        switch (_nowDirection)
        {
            case eDirection.FORWARD: 
                move = new Vector3(0, 0,  _fKickMoveAmount_Sec);
                break;
            case eDirection.BACK: 
                move = new Vector3(0, 0, -_fKickMoveAmount_Sec);
                break;
            case eDirection.RIGHT: 
                move = new Vector3( _fKickMoveAmount_Sec, 0, 0);
                break;
            case eDirection.LEFT: 
                move = new Vector3(-_fKickMoveAmount_Sec, 0, 0);
                break;
        }

        return move;
    }

    override protected void ItemPut()
    {
        if (!_charactorGauge.PutGaugeCheck() || 
            !_charactorInput.GetActionInput(eAction.PUT))
            return;

        int dirNumber = GetDataNumberForDir();
        if (dirNumber < 0 || GameScaler._nWidth * GameScaler._nHeight < dirNumber)
            return;

        FieldObjectBase obj = FieldData.Instance.GetObjData(dirNumber);
        if (obj)
            return;

        GameObject item = (GameObject)Instantiate(_sandItem, GetPosForNumber(dirNumber), Quaternion.identity);
        item.AddComponent<DelayPut>().Init(dirNumber);
        _charactorGauge.PutAction();
    }

    override public void RunSpecialMode(bool IsRun)
    {
        if (_IsSpecialMode == IsRun)
            return;

        _IsSpecialMode = IsRun;

        //  今のところなし
        if (IsRun)
        {
            
        }
        else
        {
            
        }
    }
}
