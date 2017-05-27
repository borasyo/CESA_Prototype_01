using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Referee : MonoBehaviour
{
    List<FieldObjectBase> _charaList = new List<FieldObjectBase>();
    [SerializeField] GameObject _explosionPrefab = null;

    void Start()
    {
        GameObject[] charaObjects = GameObject.FindGameObjectsWithTag("Character");
        for(int i = 0; i < charaObjects.Length; i++) {
            _charaList.Add(charaObjects[i].GetComponent<FieldObjectBase>());
        }
    }

    void LateUpdate()
    {
        for (int i = 0; i < _charaList.Count; i++)
        {
            if (_charaList[i].name.Contains("Invincible"))
                continue;

            SandItem.eType charaType = CheckType(_charaList[i].name);
            SandItem.eType type = SandData.Instance.GetSandDataList[_charaList[i].GetDataNumber()];

            if (type == SandItem.eType.MAX || type == charaType)
                continue;

            FieldObjectBase obj =_charaList[i];
            _charaList.Remove(_charaList[i]);

            // 死ぬ判定にバグがある可能性があるのでチェック
            //EditorApplication.isPaused = true;

            CheckResult(obj, type);
            return;
        }
    }

    void CheckResult(FieldObjectBase obj, SandItem.eType type)
    {
        ReStart reStart = Instantiate(_explosionPrefab).GetComponent<ReStart>();
        reStart.GetComponentInChildren<TextMesh>().text = obj.GetComponent<Character>().GetPlayerNumber() + "Pは" + obj.GetDataNumber() + "マスで" + type + "に挟まれて死んだ！";
        //ReStart reStart = Instantiate(_explosionPrefab, obj.transform.position, Quaternion.identity).GetComponent<ReStart>();
        reStart._IsEnd= false;
        Destroy(obj.gameObject);

        if (_charaList.Count > 1)
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
