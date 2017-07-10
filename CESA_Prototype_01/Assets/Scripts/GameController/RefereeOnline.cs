using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using UniRx.Triggers;

public class RefereeOnline : Referee
{
    [SerializeField]
    bool _IsNotDeath = false;

    void Start()
    {
        if (_IsNotDeath)
            return;

        if (!photonView.isMine)
            return;

        StartCoroutine(InitWait());   
    }

    IEnumerator InitWait()
    {
        yield return new WaitWhile(() => !FieldData.Instance.IsStart);

        this.LateUpdateAsObservable()
            .Subscribe(_ =>
            {
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

                    //  全体に通知
                    string player = charaList[i].name;
                    photonView.RPC("OnlineCheckResult", PhotonTargets.All, player, GetTypeText(type));
                    return;
                }
            });
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
