using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinEffect : MonoBehaviour
{
    [SerializeField]
    GameObject _intervalEffectPrefabs = null;
    [SerializeField]
    GameObject _lastEffectPrefabs = null;

    void Start()
    {
        if (RoundCounter.nNowWinerPlayer >= 0)
        {
            GameObject effect = Instantiate(_intervalEffectPrefabs);
            Vector3 pos = GameObject.Find("BackGround").transform.Find("waku").GetChild(RoundCounter.nNowWinerPlayer).position;
            pos.x -= 1.0f;
            pos.y = -1.5f;
            pos.z = 70.0f;
            effect.transform.position = pos;
        }
        else
        {
            GameObject effect = Instantiate(_lastEffectPrefabs);
            int nWinPlayer = 0;
            if (RoundCounter.nRoundCounter[1] >= RoundAmount.GetRound())
                nWinPlayer = 1;
            else if (RoundCounter.nRoundCounter[2] >= RoundAmount.GetRound())
                nWinPlayer = 2;
            else if (RoundCounter.nRoundCounter[3] >= RoundAmount.GetRound())
                nWinPlayer = 3;

            Vector3 pos = GameObject.Find("BackGround").transform.Find("waku").GetChild(nWinPlayer).position;
            pos.x -= 1.0f;
            pos.y = -1.5f;
            pos.z = 70.0f;
            effect.transform.position = pos;
        }
    }
}
