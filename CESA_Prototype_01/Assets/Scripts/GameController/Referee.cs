using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Referee : MonoBehaviour
{
    List<FieldObjectBase> _charaList = new List<FieldObjectBase>();
    [SerializeField] GameObject _explosionPrefab = null;

    void Start()
    {
        GameObject[] charaObjects = GameObject.FindGameObjectsWithTag("Charactor");
        for(int i = 0; i < charaObjects.Length; i++) {
            _charaList.Add(charaObjects[i].GetComponent<FieldObjectBase>());
        }
    }

    void LateUpdate()
    {
        List<SandData.tSandData> sandDataList = SandData.Instance.GetSandDataList;

        for (int i = 0; i < _charaList.Count; i++)
        {
            if (_charaList[i].name.Contains("Invincible"))
                continue;

            SandItem.eType type = CheckType(_charaList[i].name);

            for (int j = 0; j < sandDataList.Count; j++)
            {
                if (sandDataList[j]._number != _charaList[i].GetDataNumber())
                    continue;

                if (sandDataList[j]._Type == type)
                    continue;

                GameObject obj =_charaList[i].gameObject;
                _charaList.Remove(_charaList[i]);
                CheckResult(obj.gameObject);
                return;
            }
        }
    }

    void CheckResult(GameObject obj)
    {
        ReStart reStart = Instantiate(_explosionPrefab, obj.transform.position, Quaternion.identity).GetComponent<ReStart>();
        reStart._IsEnd= false;
        Destroy(obj);

        if (_charaList.Count > 1)
            return;

        reStart._IsEnd = true;
        Camera.main.gameObject.AddComponent<PullsObject>();
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
