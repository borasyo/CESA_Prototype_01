using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelect : MonoBehaviour
{
    public static int[] SelectLevel = new int[3] {2,2,2};
    [SerializeField]
    NowLevel[] LevelList = null;

    void Awake()
    {
        for (int i = 0; i < SelectLevel.Length; i++)
        {
            LevelList[i].nNowLevel = SelectLevel[i];
        }
    }

    void OnDisable()
    {
        for(int i = 0; i < SelectLevel.Length; i++)
        {
            SelectLevel[i] = LevelList[i].nNowLevel;
        }
    }
}
