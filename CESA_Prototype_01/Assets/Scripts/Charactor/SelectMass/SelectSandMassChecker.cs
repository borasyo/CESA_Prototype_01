using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class SelectSandMassChecker : MonoBehaviour
{
    void Start()
    {
        Color initCol = SelectMassColor.Instance.GetPutColor(transform.parent.name);
        SpriteRenderer[] selectSandMassList = new SpriteRenderer[transform.childCount];

        for(int i = 0; i < transform.childCount; i++)
        {
            selectSandMassList[i] = transform.GetChild(i).GetComponent<SpriteRenderer>();
            selectSandMassList[i].GetComponent<SelectSandMass>().Init(initCol);
        }

        Charactor charactor = GetComponentInParent<Charactor>();
        CharactorGauge charactorGauge = GetComponentInParent<CharactorGauge>();
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                for(int i = 0; i < selectSandMassList.Length; i++)
                    selectSandMassList[i].enabled = false;

                int dirNumber = charactor.GetDataNumberForDir();
                if (FieldData.Instance.GetObjData(dirNumber) || !charactorGauge.PutGaugeCheck())
                    return;

                string player = charactor.GetPlayerNumber();
                transform.position = charactor.GetPosForNumber(dirNumber);

                if (SelectSand(dirNumber + GameScaler._nWidth, player, Charactor.eDirection.BACK))
                {
                    selectSandMassList[0].enabled = true;
                    selectSandMassList[0].transform.position = transform.position + new Vector3(0, 0, GameScaler._fScale);
                }
                if (SelectSand(dirNumber - GameScaler._nWidth, player, Charactor.eDirection.FORWARD))
                {
                    selectSandMassList[1].enabled = true;
                    selectSandMassList[1].transform.position = transform.position - new Vector3(0, 0, GameScaler._fScale);
                }
                if (SelectSand(dirNumber + 1, player, Charactor.eDirection.LEFT))
                {
                    selectSandMassList[2].enabled = true;
                    selectSandMassList[2].transform.position = transform.position + new Vector3(GameScaler._fScale, 0, 0);
                }
                if (SelectSand(dirNumber - 1, player, Charactor.eDirection.RIGHT))
                {
                    selectSandMassList[3].enabled = true;
                    selectSandMassList[3].transform.position = transform.position - new Vector3(GameScaler._fScale, 0, 0);
                }
            });
    }

    bool SelectSand(int idx, string player, Charactor.eDirection dir)
    {
        SandData.HalfSandData data = SandData.Instance.GetHalfSandDataList[idx];

        for(int i = 0; i < 2; i++)
        {
            if (data._type[i] == SandItem.eType.MAX)
                continue;

            if (data._type[i] == SandItem.eType.BLOCK && data._dir[i] == dir)
                return true;

            switch (player)
            {
                case "1":
                    if (data._type[i] == SandItem.eType.ONE_P && data._dir[i] == dir)
                        return true;
                    break;
                case "2":
                    if (data._type[i] == SandItem.eType.TWO_P && data._dir[i] == dir)
                        return true;
                    break;
                case "3":
                    if (data._type[i] == SandItem.eType.THREE_P && data._dir[i] == dir)
                        return true;
                    break;
                case "4":
                    if (data._type[i] == SandItem.eType.FOUR_P && data._dir[i] == dir)
                        return true;
                    break;
            }
        }
        return false;
    }
}
