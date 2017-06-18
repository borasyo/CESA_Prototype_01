using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultRank : MonoBehaviour
{
    [SerializeField]
    int nNumber = 0;
    public int SetNumber { set { nNumber = value; } }

    // Use this for initialization
    void Start ()
    {
        int[] roundList = RoundCounter.nRoundCounter;
        int myCount = roundList[nNumber];
        int rank = 1;

        for(int i = 0; i < roundList.Length; i++)
        {
            if (myCount >= roundList[i])
                continue;

            rank++;
        }

        GetComponent<Image>().sprite = Resources.Load<Sprite>("Texture/Result/rank_" + rank.ToString());
    }
}
