using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultChecker : MonoBehaviour
{
    //  
	void Start ()
    {
        Text text = GetComponent<Text>();
        text.text = "ラウンド数 : " + RoundAmount.GetRound() + ", 1Pは" + RoundCounter.nRoundCounter[0] + ", 2Pは" + RoundCounter.nRoundCounter[1] + ", 3Pは" + RoundCounter.nRoundCounter[2] + ", 4Pは" + RoundCounter.nRoundCounter[3];
        //Debug.Log(RoundCounter.nRoundCounter[0] + "," + RoundCounter.nRoundCounter[1] + "," + RoundCounter.nRoundCounter[2] + "," + RoundCounter.nRoundCounter[3]);

        if (RoundCounter.nRoundCounter[0] >= RoundAmount.GetRound() ||
            RoundCounter.nRoundCounter[1] >= RoundAmount.GetRound() ||
            RoundCounter.nRoundCounter[2] >= RoundAmount.GetRound() ||
            RoundCounter.nRoundCounter[3] >= RoundAmount.GetRound())
        {
            StartCoroutine(GameEnd());
        }
        else
        {
            StartCoroutine(ReGameMain());
        }
	}

    IEnumerator ReGameMain()
    {
        yield return new WaitForSeconds(2.0f);

        if (PhotonNetwork.inRoom)
        {
            if (PhotonNetwork.isMasterClient)
                PhotonNetwork.DestroyAll();

            SceneManager.LoadScene("OnlineGameMain");
        }
        else
        {
            SceneManager.LoadScene("GameMain");
        }
    }

    IEnumerator GameEnd()
    {
        yield return new WaitForSeconds(2.0f);

        for (int i = 0; i < RoundCounter.nRoundCounter.Length; i++)
        {
            RoundCounter.nRoundCounter[i] = 0;
        }

        if (PhotonNetwork.inRoom)
        {
            if (PhotonNetwork.isMasterClient)
                PhotonNetwork.DestroyAll();

            SceneManager.LoadScene("OnlineRoom");
        }
        else
        {
            SceneManager.LoadScene("CharacterSelect");
        }
    }
}
