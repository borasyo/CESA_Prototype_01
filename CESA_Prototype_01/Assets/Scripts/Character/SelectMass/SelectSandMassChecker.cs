using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class SelectSandMassChecker : MonoBehaviour
{
    [SerializeField]
    bool _IsBalance = false;

    void Start()
    {
        Color initCol = SelectMassColor.Instance.GetPutColor(transform.parent.name);
        SpriteRenderer[] selectSandMassList = new SpriteRenderer[transform.childCount];

        for(int i = 0; i < transform.childCount; i++)
        {
            selectSandMassList[i] = transform.GetChild(i).GetComponent<SpriteRenderer>();
            selectSandMassList[i].GetComponent<SelectSandMass>().Init(initCol);
        }

        if (_IsBalance)
            BalanceSelectSandMass(selectSandMassList);
        else
            NormalSelectSandMass(selectSandMassList);

        transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
    }

    void NormalSelectSandMass(SpriteRenderer[] selectSandMassList)
    {
        Character Character = GetComponentInParent<Character>();
        CharacterGauge CharacterGauge = GetComponentInParent<CharacterGauge>();
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                for (int i = 0; i < selectSandMassList.Length; i++)
                    selectSandMassList[i].enabled = false;

                int dirNumber = Character.GetDataNumberForDir();
                if (FieldData.Instance.GetObjData(dirNumber) || !CharacterGauge.PutGaugeCheck())
                    return;

                string player = Character.GetPlayerNumber();
                transform.position = Character.GetPosForNumber(dirNumber);

                if (SelectSand(dirNumber + GameScaler._nWidth, player, Character.eDirection.BACK))
                {
                    selectSandMassList[0].enabled = true;
                    selectSandMassList[0].transform.position = transform.position + new Vector3(0, 0, GameScaler._fScale);
                }
                if (SelectSand(dirNumber - GameScaler._nWidth, player, Character.eDirection.FORWARD))
                {
                    selectSandMassList[1].enabled = true;
                    selectSandMassList[1].transform.position = transform.position - new Vector3(0, 0, GameScaler._fScale);
                }
                if (SelectSand(dirNumber + 1, player, Character.eDirection.LEFT))
                {
                    selectSandMassList[2].enabled = true;
                    selectSandMassList[2].transform.position = transform.position + new Vector3(GameScaler._fScale, 0, 0);
                }
                if (SelectSand(dirNumber - 1, player, Character.eDirection.RIGHT))
                {
                    selectSandMassList[3].enabled = true;
                    selectSandMassList[3].transform.position = transform.position - new Vector3(GameScaler._fScale, 0, 0);
                }
            });
    }

    void BalanceSelectSandMass(SpriteRenderer[] selectSandMassList)
    {
        Character Character = GetComponentInParent<Character>();
        CharacterGauge CharacterGauge = GetComponentInParent<CharacterGauge>();
        this.UpdateAsObservable()
            .Where(_ => !Character.GetSpecialModeFlg)
            .Subscribe(_ =>
            {
                for (int i = 0; i < selectSandMassList.Length; i++)
                    selectSandMassList[i].enabled = false;

                int dirNumber = Character.GetDataNumberForDir();
                if (FieldData.Instance.GetObjData(dirNumber) || !CharacterGauge.PutGaugeCheck())
                    return;

                string player = Character.GetPlayerNumber();
                transform.position = Character.GetPosForNumber(dirNumber);

                if (SelectSand(dirNumber + GameScaler._nWidth, player, Character.eDirection.BACK))
                {
                    selectSandMassList[0].enabled = true;
                    selectSandMassList[0].transform.position = transform.position + new Vector3(0, 0, GameScaler._fScale);
                }
                if (SelectSand(dirNumber - GameScaler._nWidth, player, Character.eDirection.FORWARD))
                {
                    selectSandMassList[1].enabled = true;
                    selectSandMassList[1].transform.position = transform.position - new Vector3(0, 0, GameScaler._fScale);
                }
                if (SelectSand(dirNumber + 1, player, Character.eDirection.LEFT))
                {
                    selectSandMassList[2].enabled = true;
                    selectSandMassList[2].transform.position = transform.position + new Vector3(GameScaler._fScale, 0, 0);
                }
                if (SelectSand(dirNumber - 1, player, Character.eDirection.RIGHT))
                {
                    selectSandMassList[3].enabled = true;
                    selectSandMassList[3].transform.position = transform.position - new Vector3(GameScaler._fScale, 0, 0);
                }
            });

        this.UpdateAsObservable()
            .Where(_ => Character.GetSpecialModeFlg)
            .Subscribe(_ =>
            {
                for (int i = 0; i < selectSandMassList.Length; i++)
                    selectSandMassList[i].enabled = false;

                int dirNumber = Character.GetDataNumberForDir();
                if (FieldData.Instance.GetObjData(dirNumber) || !CharacterGauge.PutGaugeCheck())
                    return;

                int number = Character.GetDataNumber();
                string player = Character.GetPlayerNumber();
                transform.position = Character.GetPosForNumber(dirNumber);

                if (1 == Mathf.Abs(number - dirNumber))
                {
                    if (SelectSand(dirNumber + GameScaler._nWidth * 2, player, Character.eDirection.BACK))
                    {
                        selectSandMassList[0].enabled = true;
                        selectSandMassList[0].transform.position = transform.position + new Vector3(0, 0, GameScaler._fScale * 2);
                    }
                    if (SelectSand(dirNumber - GameScaler._nWidth * 2, player, Character.eDirection.FORWARD))
                    {
                        selectSandMassList[1].enabled = true;
                        selectSandMassList[1].transform.position = transform.position - new Vector3(0, 0, GameScaler._fScale * 2);
                    }
                    if (SelectSand(dirNumber + 1, player, Character.eDirection.LEFT))
                    {
                        selectSandMassList[2].enabled = true;
                        selectSandMassList[2].transform.position = transform.position + new Vector3(GameScaler._fScale, 0, 0);
                    }
                    if (SelectSand(dirNumber - 1, player, Character.eDirection.RIGHT))
                    {
                        selectSandMassList[3].enabled = true;
                        selectSandMassList[3].transform.position = transform.position - new Vector3(GameScaler._fScale, 0, 0);
                    }

                    if (SelectSand(dirNumber + GameScaler._nWidth + 1, player, Character.eDirection.LEFT))
                    {
                        selectSandMassList[4].enabled = true;
                        selectSandMassList[4].transform.position = transform.position + new Vector3(GameScaler._fScale, 0, GameScaler._fScale);
                    }
                    if (SelectSand(dirNumber + GameScaler._nWidth - 1, player, Character.eDirection.RIGHT))
                    {
                        selectSandMassList[5].enabled = true;
                        selectSandMassList[5].transform.position = transform.position + new Vector3(-GameScaler._fScale, 0, GameScaler._fScale);
                    }

                    if (SelectSand(dirNumber - GameScaler._nWidth + 1, player, Character.eDirection.LEFT))
                    {
                        selectSandMassList[6].enabled = true;
                        selectSandMassList[6].transform.position = transform.position + new Vector3(GameScaler._fScale, 0, -GameScaler._fScale);
                    }
                    if (SelectSand(dirNumber - GameScaler._nWidth - 1, player, Character.eDirection.RIGHT))
                    {
                        selectSandMassList[7].enabled = true;
                        selectSandMassList[7].transform.position = transform.position + new Vector3(-GameScaler._fScale, 0, -GameScaler._fScale);
                    }
                }
                else
                {
                    if (SelectSand(dirNumber + GameScaler._nWidth, player, Character.eDirection.BACK))
                    {
                        selectSandMassList[0].enabled = true;
                        selectSandMassList[0].transform.position = transform.position + new Vector3(0, 0, GameScaler._fScale);
                    }
                    if (SelectSand(dirNumber - GameScaler._nWidth, player, Character.eDirection.FORWARD))
                    {
                        selectSandMassList[1].enabled = true;
                        selectSandMassList[1].transform.position = transform.position - new Vector3(0, 0, GameScaler._fScale);
                    }
                    if (SelectSand(dirNumber + 2, player, Character.eDirection.LEFT))
                    {
                        selectSandMassList[2].enabled = true;
                        selectSandMassList[2].transform.position = transform.position + new Vector3(GameScaler._fScale * 2, 0, 0);
                    }
                    if (SelectSand(dirNumber - 2, player, Character.eDirection.RIGHT))
                    {
                        selectSandMassList[3].enabled = true;
                        selectSandMassList[3].transform.position = transform.position - new Vector3(GameScaler._fScale * 2, 0, 0);
                    }

                    if (SelectSand(dirNumber + GameScaler._nWidth + 1, player, Character.eDirection.BACK))
                    {
                        selectSandMassList[4].enabled = true;
                        selectSandMassList[4].transform.position = transform.position + new Vector3(GameScaler._fScale, 0, GameScaler._fScale);
                    }
                    if (SelectSand(dirNumber + GameScaler._nWidth - 1, player, Character.eDirection.BACK))
                    {
                        selectSandMassList[5].enabled = true;
                        selectSandMassList[5].transform.position = transform.position + new Vector3(-GameScaler._fScale, 0, GameScaler._fScale);
                    }

                    if (SelectSand(dirNumber - GameScaler._nWidth + 1, player, Character.eDirection.FORWARD))
                    {
                        selectSandMassList[6].enabled = true;
                        selectSandMassList[6].transform.position = transform.position + new Vector3(GameScaler._fScale, 0, -GameScaler._fScale);
                    }
                    if (SelectSand(dirNumber - GameScaler._nWidth - 1, player, Character.eDirection.FORWARD))
                    {
                        selectSandMassList[7].enabled = true;
                        selectSandMassList[7].transform.position = transform.position + new Vector3(-GameScaler._fScale, 0, -GameScaler._fScale);
                    }
                }
            });
    }

    bool SelectSand(int idx, string player, Character.eDirection dir)
    {
        if (idx < 0 || GameScaler.GetRange <= idx)
            return false;

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
