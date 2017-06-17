using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldCreatorModeSelect : FieldCreator
{
    protected override void CreateChara()
    {
        //  なにもしない
    }

    protected override bool StageBlock(int x, int z)
    {
        return false;
    }
}
