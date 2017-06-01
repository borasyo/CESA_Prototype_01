using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if DEBUG
using UnityEditor;
#endif

public class Referee : MonoBehaviour
{
    [SerializeField] GameObject _explosionPrefab = null;

    void LateUpdate()
    {
        if (!FieldData.Instance.IsStart)
            return;

        List<Character> charaList = FieldData.Instance.GetCharactors;
        for (int i = 0; i < charaList.Count; i++)
        {
            if (!charaList[i])
                continue;

            if (charaList[i].name.Contains("Invincible"))
                continue;

            SandItem.eType charaType = CheckType(charaList[i].name);
            SandItem.eType type = SandData.Instance.GetSandDataList[charaList[i].GetDataNumber()];

            if (type == SandItem.eType.MAX || type == charaType)
                continue;

            FieldObjectBase obj = charaList[i];
            charaList.Remove(charaList[i]);

#if DEBUG
            // 死ぬ判定にバグがある可能性があるのでチェック
            //EditorApplication.isPaused = true;
#endif

            CheckResult(obj, type, charaList.Count);
            return;
        }
    }

    void CheckResult(FieldObjectBase obj, SandItem.eType type, int length)
    {
        ReStart reStart = Instantiate(_explosionPrefab).GetComponent<ReStart>();
        reStart.GetComponentInChildren<TextMesh>().text = obj.GetComponent<Character>().GetPlayerNumber() + "Pは" + obj.GetDataNumber() + "マスで" + type + "に挟まれて死んだ！";
        //ReStart reStart = Instantiate(_explosionPrefab, obj.transform.position, Quaternion.identity).GetComponent<ReStart>();
        reStart._IsEnd= false;

        string name = obj.name;
        Destroy(obj.gameObject);

        if (length > 1 && name.Contains("CPU"))
            return;

        reStart._IsEnd = true;
        //Camera.main.gameObject.AddComponent<PullsObject>();
        this.enabled = false;
    }

    SandItem.eType CheckType(string name)
    {
        SandItem.eType type = SandItem.eType.MAX;
        if (name.Contains("1P"))
        {
            type = SandItem.eType.ONE_P;
        }
        else if ((name.Contains("2P")))
        {
            type = SandItem.eType.TWO_P;
        }
        else if ((name.Contains("3P")))
        {
            type = SandItem.eType.THREE_P;
        }
        else if ((name.Contains("4P")))
        {
            type = SandItem.eType.FOUR_P;
        }

        return type;
    }
}
