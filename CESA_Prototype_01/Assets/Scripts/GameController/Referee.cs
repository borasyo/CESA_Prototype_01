using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Referee : MonoBehaviour
{
    [SerializeField] GameObject _explosionPrefab = null;

    void LateUpdate()
    {
        List<SandData.tSandData> sandDataList = SandData.Instance.GetSandDataList;

        FieldObjectBase player = FieldData.Instance.GetCharaData("Player");
        //MeshRenderer msRend = player.gameObject.GetComponent<MeshRenderer>();
        //msRend.material.color = new Color(1, 1, 1, 1);
        for (int i = 0; i < sandDataList.Count; i++)
        {
            //Debug.Log(sandDataList[i]._number + "," +  sandDataList[i]._Type);
            if (sandDataList[i]._number != player.GetDataNumber())
                continue;

            if (sandDataList[i]._Type == SandItem.eType.PLAYER)
                continue;

            StartResult(player.gameObject);
            //msRend.material.color = new Color(0, 0, 1, 1);
            // EditorApplication.isPaused = true;
        }
            
        FieldObjectBase enemy  = FieldData.Instance.GetCharaData("Enemy");
        //msRend = enemy.gameObject.GetComponent<MeshRenderer>();
        //msRend.material.color = new Color(1, 1, 1, 1);
        for (int i = 0; i < sandDataList.Count; i++)
        {
            if (sandDataList[i]._number != enemy.GetDataNumber())
                continue;

            if (sandDataList[i]._Type == SandItem.eType.ENEMY)
                continue;

            StartResult(enemy.gameObject);
            //msRend.material.color = new Color(1, 0, 0, 1);
            //EditorApplication.isPaused = true;
        }
    }

    void StartResult(GameObject obj)
    {
        Instantiate(_explosionPrefab, obj.transform.position, Quaternion.identity);
        Camera.main.gameObject.AddComponent<PullsObject>();
        Destroy(obj);
        this.enabled = false;
    }
}
