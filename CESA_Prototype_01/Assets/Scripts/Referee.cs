using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Referee : MonoBehaviour
{
    void LateUpdate()
    {
        List<SandData.tSandData> sandDataList = SandData.Instance.GetSandDataList;

        FieldObjectBase player = FieldData.Instance.GetCharaData("Player");
        MeshRenderer msRend = player.gameObject.GetComponent<MeshRenderer>();
        msRend.material.color = new Color(1, 1, 1, 1);
        for (int i = 0; i < sandDataList.Count; i++)
        {
            //Debug.Log(sandDataList[i]._number + "," +  sandDataList[i]._Type);
            if (sandDataList[i]._number != player.GetDataNumber())
                continue;

            if (sandDataList[i]._Type == SandItem.eType.PLAYER)
                continue;

            msRend.material.color = new Color(0, 0, 1, 1);
           // EditorApplication.isPaused = true;
        }
            
        FieldObjectBase enemy  = FieldData.Instance.GetCharaData("Enemy");
        msRend = enemy.gameObject.GetComponent<MeshRenderer>();
        msRend.material.color = new Color(1, 1, 1, 1);
        for (int i = 0; i < sandDataList.Count; i++)
        {
            if (sandDataList[i]._number != enemy.GetDataNumber())
                continue;

            if (sandDataList[i]._Type == SandItem.eType.ENEMY)
                continue;

            msRend.material.color = new Color(1, 0, 0, 1);
            //EditorApplication.isPaused = true;
        }
    }
}
