using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;
using UnityEngine.SceneManagement;
using System.Linq;

public class CharacterSelect : Photon.PunBehaviour
{
    public static GameObject[] SelectCharas = new GameObject[4];
    public static bool[] IsRandom = new bool[4];
    [SerializeField] protected NowSelect[] _nowSelectDatas = null;

    // Use this for initialization
    public void Start()
    {
        for (int i = 0; i < SelectCharas.Length; i++)
        {
            if (!SelectCharas[i])
            {
                if(_nowSelectDatas[i])
                    _nowSelectDatas[i].TextUpdate();
                continue;
            }

            _nowSelectDatas[i].CharaType = SearchCharaType(SelectCharas[i], IsRandom[i]);
            _nowSelectDatas[i].TextUpdate();
        }
    }

    public enum eCharaType
    {
        NONE = 0,
        BALANCE,
        POWER,
        SPEED,
        TECHNICAL,
        MAX,
    };

    protected void SetChara()
    {
        GameObject BalanceObj = Resources.Load<GameObject>("Prefabs/Chara/Balance");
        GameObject PowerObj = Resources.Load<GameObject>("Prefabs/Chara/Power");
        GameObject SpeedObj = Resources.Load<GameObject>("Prefabs/Chara/Speed");
        GameObject TechnicalObj = Resources.Load<GameObject>("Prefabs/Chara/Technical");
        GameObject[] RandomObj = new GameObject[4] { BalanceObj, PowerObj, SpeedObj, TechnicalObj }; 

        for (int i = 0; i < _nowSelectDatas.Length; i++)
        {
            switch (_nowSelectDatas[i].CharaType)
            {
                case eCharaType.NONE:
                    SelectCharas[i] = null;
                    break;
                case eCharaType.BALANCE:
                    SelectCharas[i] = BalanceObj;
                    IsRandom[i] = false;
                    break;
                case eCharaType.POWER:
                    SelectCharas[i] = PowerObj;
                    IsRandom[i] = false;
                    break;
                case eCharaType.SPEED:
                    SelectCharas[i] = SpeedObj;
                    IsRandom[i] = false;
                    break;
                case eCharaType.TECHNICAL:
                    SelectCharas[i] = TechnicalObj;
                    IsRandom[i] = false;
                    break;
                case eCharaType.MAX:
                    SelectCharas[i] = RandomObj[Random.Range(0, RandomObj.Length)];
                    IsRandom[i] = true;
                    break;
            }
        }
    }

    void ChangeActive(int nSelect)
    {
        if (nSelect < 0 || nSelect >= _nowSelectDatas.Length)
            return;

        for (int i = 0; i < _nowSelectDatas.Length; i++)
        {
            _nowSelectDatas[i].enabled = false;
        }

        _nowSelectDatas[nSelect].enabled = true;
    }

    eCharaType SearchCharaType(GameObject charaData, bool IsRandom)
    {
        eCharaType type = eCharaType.NONE;

        if (IsRandom)
        {
            type = eCharaType.MAX;
        }
        else if (charaData.name.Contains("Balance"))
        { 
            type = eCharaType.BALANCE;
        }
        else if (charaData.name.Contains("Power"))
        {
            type = eCharaType.POWER;
        }
        else if (charaData.name.Contains("Speed"))
        {
            type = eCharaType.SPEED;
        }
        else if (charaData.name.Contains("Technical"))
        {
            type = eCharaType.TECHNICAL;
        }

        return type;
    }

    public virtual void GameStart()
    {
        SetChara();
        GetComponent<LevelSelect>().SetLevel();

        SceneManager.LoadScene("GameMain");
    }
}
