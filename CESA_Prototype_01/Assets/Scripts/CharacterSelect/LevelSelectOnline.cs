using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectOnline : LevelSelect
{
    public void SetLevel(NowLevelOnline level, int number)
    {
        if (number < 0)
            return;

        LevelList[number] = level;
    }
}
