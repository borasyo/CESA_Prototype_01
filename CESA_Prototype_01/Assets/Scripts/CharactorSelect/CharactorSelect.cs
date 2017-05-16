﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;
using UnityEngine.SceneManagement;

public class CharactorSelect : MonoBehaviour 
{
    public static GameObject[] SelectCharas = new GameObject[4];
    [SerializeField] NowSelect[] _nowSelectDatas = null; 

	// Use this for initialization
	void Start () 
    {
        DontDestroyOnLoad(this);

        // Scene遷移
        this.UpdateAsObservable()
            .Where(_ => SceneManager.GetActiveScene().name == "CharactorSelect")
            .Subscribe(_ =>
            {
                if (!Input.GetKeyDown(KeyCode.Return))
                    return;

                SetChara();
                SceneManager.LoadScene("GameMain");
            });

        int nSelect = 0;
        ChangeActive(nSelect);
        this.UpdateAsObservable()
            .Where(_ => SceneManager.GetActiveScene().name == "CharactorSelect")
            .Subscribe(_ =>
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                    nSelect --;
                else if(Input.GetKeyDown(KeyCode.DownArrow))
                    nSelect ++;
                else return;

                if (nSelect < 0)
                    nSelect = _nowSelectDatas.Length - 1;
                else if (nSelect >= _nowSelectDatas.Length)
                    nSelect = 0;

                ChangeActive(nSelect);
            });

    }

    public enum eCharaType {
        NONE = 0,
        BALANCE,
        POWER,
        SPEED,
        TECHNICAL,
        MAX,
    };

    void SetChara()
    {
        GameObject BalanceObj   = Resources.Load<GameObject> ("Prefabs/Chara/Balance");
        GameObject PowerObj     = Resources.Load<GameObject> ("Prefabs/Chara/Power");
        GameObject SpeedObj     = Resources.Load<GameObject> ("Prefabs/Chara/Speed");
        GameObject TechnicalObj = Resources.Load<GameObject> ("Prefabs/Chara/Technical");

        for (int i = 0; i < _nowSelectDatas.Length; i++)
        {
            switch (_nowSelectDatas[i].GetCharaType)
            {
                case eCharaType.NONE:
                    SelectCharas[i] = null;
                    break;
                case eCharaType.BALANCE:
                    SelectCharas[i] = BalanceObj;
                    break;
                case eCharaType.POWER:
                    SelectCharas[i] = PowerObj;
                    break;
                case eCharaType.SPEED:
                    SelectCharas[i] = SpeedObj;
                    break;
                case eCharaType.TECHNICAL:
                    SelectCharas[i] = TechnicalObj;
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
}