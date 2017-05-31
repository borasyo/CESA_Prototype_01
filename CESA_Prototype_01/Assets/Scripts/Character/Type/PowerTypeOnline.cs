using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerTypeOnline : CharacterOnline
{
    float _InitBreakGauge;

    public override void Init(int level)
    {
        _charaType = eCharaType.POWER;
        base.Init(level);
    }

    override protected void ItemBreak()
    {
        if (!_charactorGauge.BreakGaugeCheck() ||
            !_charactorInput.GetActionInput(eAction.BREAK))
            return;

        FieldObjectBase obj = FieldData.Instance.GetObjData(GetDataNumberForDir());

        if (_IsSpecialMode)
        {
            if (!obj)
                return;

            if (obj.GetSandType() == SandItem.eType.MAX && obj.tag != "Block")
                return;

            if (obj.name.Contains("Fence"))
                return;
        }
        else
        {
            if (!obj || obj.GetSandType() == SandItem.eType.MAX)
                return;
        }

        FieldData.Instance.SetObjData(null, GetDataNumberForDir());
        FieldData.Instance.ExceptionChangeField();
        Destroy(obj.gameObject);
        _charactorGauge.BreakAction();
    }

    override public bool RunSpecialMode(bool IsRun)
    {
        if (_IsSpecialMode == IsRun)
            return false;

        _IsSpecialMode = IsRun;
        if (IsRun)
        {
            _InitBreakGauge = _charactorGauge.GetBreakGauge;
            _charactorGauge.SetBreakGauge(0.0f);
        }
        else
        {
            _charactorGauge.SetBreakGauge(_InitBreakGauge);
        }
        return true;
    }

}
