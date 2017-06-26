using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if DEBUG
using UnityEditor;
#endif

public class Referee : Photon.MonoBehaviour
{
    [SerializeField] protected GameObject _deathEffectPrefab = null;
    private bool _IsEnd = false;

    void LateUpdate()
    {
        if (!FieldData.Instance.IsStart)
            return;

        List<Character> charaList = FieldData.Instance.GetCharactors;
        for (int i = 0; i < charaList.Count; i++)
        {
            if (_IsEnd)
                return;

            if (!charaList[i])
                continue;

            if (charaList[i].name.Contains("Invincible"))
                continue;

            SandItem.eType charaType = CheckType(charaList[i].name);
            
            //  どちらかに挟まれていれば死亡
            SandItem.eType type = SandData.Instance.GetSandDataList[charaList[i].GetDataNumber()]._type[0];
            if (type == SandItem.eType.MAX || type == charaType)
            {
                type = SandData.Instance.GetSandDataList[charaList[i].GetDataNumber()]._type[1];
                if (type == SandItem.eType.MAX || type == charaType)
                    continue;
            }

            Character obj = charaList[i];
            charaList.Remove(obj);

#if DEBUG
            // 死ぬ判定にバグがある可能性があるのでチェック
            //EditorApplication.isPaused = true;
#endif

            CheckResult(obj, GetTypeText(type), charaList);
            return;
        }
    }

    protected void CheckResult(FieldObjectBase obj, string type, List<Character> charaList)
    {
        GameObject effect = (GameObject)Instantiate(_deathEffectPrefab, obj.transform.position + new Vector3(0.0f, GameScaler._fScale * 0.5f, 0.0f), Quaternion.identity);
        effect.GetComponent<PlayerDeathEffect>().Init(type);
        ReStart reStart = effect.GetComponent<ReStart>();
        //reStart.GetComponentInChildren<TextMesh>().text = obj.GetComponent<Character>().GetPlayerNumber() + "Pは" + obj.GetDataNumber() + "マスで" + type + "に挟まれて死んだ！";
        
        Destroy(obj.gameObject);

        //  キャラが2人以上いて、Userもまだ居たら終了じゃない
        if (charaList.Count > 1 && charaList.Where(x => !x.name.Contains("CPU")).ToList().Count() > 0)
            return;

        reStart._winer = charaList[0];
        //enabled = false;
        _IsEnd = true;
    }

    protected SandItem.eType CheckType(string name)
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

    protected string GetTypeText(SandItem.eType type)
    {
        string text = "";
        switch(type)
        {
            case SandItem.eType.ONE_P:
                text = "1P";
                break;
            case SandItem.eType.TWO_P:
                text = "2P";
                break;
            case SandItem.eType.THREE_P:
                text = "3P";
                break;
            case SandItem.eType.FOUR_P:
                text = "4P";
                break;
        }
        return text;
    }
}
