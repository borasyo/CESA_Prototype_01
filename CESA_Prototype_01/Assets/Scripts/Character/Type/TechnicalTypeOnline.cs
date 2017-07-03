using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class TechnicalTypeOnline : CharacterOnline
{
    [SerializeField]
    float _fKickMoveAmount_Sec = 1.0f;
    [SerializeField]
    bool _IsAuthBlock = false;

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

        if (obj.tag == "Character")
            return;

        if (obj.tag == "Block" && (!_IsAuthBlock || obj.name.Contains("Fence")))
            return;

        if (obj.GetSandType() == SandItem.eType.MAX)
            return;

        if (!PushObj())
            return;

        obj.gameObject.AddComponent<AutoMoveObj>().Init(_nowDirection, MoveAmount());
    }

    bool PushObj()
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
                move = new Vector3(0, 0, _fKickMoveAmount_Sec);
                break;
            case eDirection.BACK:
                move = new Vector3(0, 0, -_fKickMoveAmount_Sec);
                break;
            case eDirection.RIGHT:
                move = new Vector3(_fKickMoveAmount_Sec, 0, 0);
                break;
            case eDirection.LEFT:
                move = new Vector3(-_fKickMoveAmount_Sec, 0, 0);
                break;
        }

        return move;
    }

    [PunRPC]
    public override void OnlineItemPut(Vector3 pos, int dirNumber, bool isPostProcess)
    {
        GameObject item = Instantiate(_sandItem, pos, Quaternion.identity);
        StartCoroutine(item.AddComponent<DelayPut>().Init(dirNumber));

        if (!isPostProcess)
            return;

        _charactorGauge.PutAction();
        _fNotMoveTime = 0.0f;
        _animator.SetBool("Put", true);
    }

    override public bool RunSpecialMode(bool IsRun)
    {
        if (_IsSpecialMode == IsRun)
            return false;

        _IsSpecialMode = IsRun;
        //  今のところなし
        if (IsRun)
        {

        }
        else
        {

        }
        return true;
    }
}
