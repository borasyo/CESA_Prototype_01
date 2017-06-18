using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultStar : MonoBehaviour
{
    [SerializeField]
    int nNumber = 0;
    public int SetNumber { set { nNumber = value; } }

    [SerializeField]
    GameObject starPrefabs = null;

    void Start()
    {
        int nMaxRound = RoundAmount.GetRound();

        float range = 0;
        if (nMaxRound == 1)
            range = 0;
        else if (nMaxRound == 2)
            range = 32.5f;
        else
            range = 65.0f;

        float distance = 65.0f;
        if (nMaxRound >= 4)
            distance = 130.0f / (nMaxRound - 1);

        int nowRound = RoundCounter.nRoundCounter[nNumber];
        
        for (int i = 0; i < nMaxRound; i++)
        {
            GameObject obj = Instantiate(starPrefabs);
            obj.transform.SetParent(transform);
            obj.transform.localPosition = new Vector3((-distance * i) + range, 0, 0);
            obj.transform.localScale = new Vector3(1,1,1);

            if (i < RoundAmount.GetRound() - nowRound)
                continue;

            obj.GetComponent<Image>().sprite = Resources.Load<Sprite>("Texture/Result/hosi");
        }
    }
}
