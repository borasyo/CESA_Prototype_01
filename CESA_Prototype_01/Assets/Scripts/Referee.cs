using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Referee : MonoBehaviour
{
    void Update()
    {
        List<SandData.tSandData> sandDataList = SandData.Instance.GetSandDataList;

        FieldObjectBase player = FieldData.Instance.GetCharaData("Player");
        for (int i = 0; i < sandDataList.Count; i++)
        {
            Debug.Log(sandDataList[i]._number + "," +  sandDataList[i]._Type);
            if (sandDataList[i]._number != player.GetDataNumber())
                continue;

            if (sandDataList[i]._Type == SandItem.eType.PLAYER)
                continue;

            Debug.Log(player.name + "の負け");
            EditorApplication.isPaused = true;
        }
            
        FieldObjectBase enemy  = FieldData.Instance.GetCharaData("Enemy");
        for (int i = 0; i < sandDataList.Count; i++)
        {
            if (sandDataList[i]._number != enemy.GetDataNumber())
                continue;

            if (sandDataList[i]._Type == SandItem.eType.ENEMY)
                continue;

            Debug.Log(enemy.name + "の負け");
            EditorApplication.isPaused = true;
        }
    }
}
