using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultStar : MonoBehaviour
{
    [SerializeField]
    int nNumber = 0;

    [SerializeField]
    GameObject starPrefabs = null;

    void Start()
    {
        int nMaxRound = RoundAmount.GetRound();
        float distance = (float)130 / (nMaxRound - 1);

        int nowRound = RoundCounter.nRoundCounter[nNumber];
        
        for (int i = 0; i < nMaxRound; i++)
        {
            GameObject obj = Instantiate(starPrefabs);
            obj.transform.SetParent(transform);
            obj.transform.localPosition = new Vector3((distance * i) - 65.0f, 0, 0);
            obj.transform.localScale = new Vector3(1,1,1);

            if (i >= nowRound)
                continue;

            obj.GetComponent<Image>().sprite = Resources.Load<Sprite>("Texture/Result/hosi");
        }
    }
}
