using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PowerTypeBreakAI : BreakAI
{
    Character _character = null;

    public override void Init(int level)
    {
        _character = GetComponent<Character>();
        base.Init(level);
    }

    protected override List<FieldObjectBase> GetSandItemList()
    {
        if(!_character.GetSpecialModeFlg)
            return FieldData.Instance.GetObjDataArray.Where(x => x && x.tag == "SandItem" && x.GetSandType() != SandItem.eType.MAX).ToList();

        return FieldData.Instance.GetObjDataArray.Where(x => x && ((x.tag == "SandItem" && x.GetSandType() != SandItem.eType.MAX) || (x.tag == "Block" && !x.name.Contains("Fence")))).ToList();
    }

    protected override List<FieldObjectBase> GetSandItemList(string player)
    {
        if (!_character.GetSpecialModeFlg)
            return FieldData.Instance.GetObjDataArray.Where(x => x && x.tag == "SandItem" && x.name.Contains(player) && x.GetSandType() != SandItem.eType.MAX).ToList();

        return FieldData.Instance.GetObjDataArray.Where(x => x && ((x.tag == "SandItem" && x.name.Contains(player) && x.GetSandType() != SandItem.eType.MAX) || (x.tag == "Block" && !x.name.Contains("Fence")))).ToList();
    }
}
