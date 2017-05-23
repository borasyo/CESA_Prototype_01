using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerType : Charactor
{
    float _InitBreakGauge;

    void Start()
    {
        _charaType = eCharaType.POWER;
        base.Start();
    }

    override protected void ItemBreak()
    {
        if (!_charactorGauge.BreakGaugeCheck() || 
            !_charactorInput.GetActionInput(eAction.BREAK))
            return;

        FieldObjectBase obj = FieldData.Instance.GetObjData(GetDataNumberForDir());

        if (_IsSpecialMode)
        {
            if (!obj || (obj.tag != "SandItem" && obj.tag != "Block"))
                return;

            if (obj.name.Contains("Fence"))
                return;
        }
        else
        {
            if (!obj ||  obj.tag != "SandItem")
                return;
        }

        FieldData.Instance.SetObjData(null, GetDataNumberForDir());
        FieldData.Instance.ExceptionChangeField();
        Destroy(obj.gameObject);
        _charactorGauge.BreakAction();
    }

    override public void RunSpecialMode(bool IsRun)
    {
        if (_IsSpecialMode == IsRun)
            return;

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
    }
}
