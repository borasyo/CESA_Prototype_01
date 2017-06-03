using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RefereeOnline : Referee
{
    void LateUpdate()
    {
        if (!photonView.isMine)
            return;

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
            SandItem.eType type = SandData.Instance.GetSandDataList[charaList[i].GetDataNumber()]._type;

            if (type == SandItem.eType.MAX || type == charaType)
                continue;

            //  全体に通知
            string player = charaList[i].name;
            photonView.RPC("OnlineCheckResult", PhotonTargets.All, player, GetTypeText(type));
            return;
        }
    }

    [PunRPC]
    public void OnlineCheckResult(string player, string type)
    {
        List<Character> charaList = FieldData.Instance.GetCharactors;
        Character obj = charaList.Where(_ => _.name.Contains(player)).First();
        charaList.Remove(obj);

        CheckResult(obj, type, charaList);
    }
}
