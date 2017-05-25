using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelect : MonoBehaviour
{
    public static int[] SelectLevel = new int[3];
    [SerializeField]
    NowLevel[] LevelList = null;

    void OnDisable()
    {
        for(int i = 0; i < SelectLevel.Length; i++)
        {
            SelectLevel[i] = LevelList[i].nNowLevel;
        }
    }
}
