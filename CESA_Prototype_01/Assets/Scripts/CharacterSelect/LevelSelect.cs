using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelect : MonoBehaviour
{
    public static int[] SelectLevel = new int[4] {2,2,2,2};
    [SerializeField] protected NowLevel[] LevelList = null;

    void Awake()
    {
        for (int i = 0; i < SelectLevel.Length; i++)
        {
            if (!LevelList[i])
                continue;

            LevelList[i].nNowLevel = SelectLevel[i];
        }
    }

    void OnDisable()
    {
        for(int i = 0; i < SelectLevel.Length; i++)
        {
            if (!LevelList[i])
            {
                SelectLevel[i] = -1;    //  ナシ
                continue;
            }

            SelectLevel[i] = LevelList[i].nNowLevel;
        }
    }
}
