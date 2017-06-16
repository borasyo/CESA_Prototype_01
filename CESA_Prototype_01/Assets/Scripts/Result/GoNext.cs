using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoNext : MonoBehaviour
{
    // nNowWinerPlayerが0の時、最後のresultを生成
    void Start()
    {
        if (RoundCounter.nNowWinerPlayer < 0)
            return;

        if (RoundCounter.nRoundCounter[0] >= RoundAmount.GetRound() ||
            RoundCounter.nRoundCounter[1] >= RoundAmount.GetRound() ||
            RoundCounter.nRoundCounter[2] >= RoundAmount.GetRound() ||
            RoundCounter.nRoundCounter[3] >= RoundAmount.GetRound())
        {
            StartCoroutine(LastResult());
        }
        else
        {
            StartCoroutine(ReGameMain());
        }
    }

    public void OnClick()
    {
        if (PhotonNetwork.inRoom && !PhotonNetwork.isMasterClient)
            return;

        ReCharaSelect();
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

    // 最後のresultへ
    IEnumerator LastResult()
    {
        yield return new WaitForSeconds(2.0f);

        RoundCounter.nNowWinerPlayer = -1; 

        if (PhotonNetwork.inRoom)
        {
            SceneManager.LoadScene("OnlineResult");
        }
        else
        {
            SceneManager.LoadScene("Result");
        }
    }

    void ReCharaSelect()
    {
        for (int i = 0; i < RoundCounter.nRoundCounter.Length; i++)
        {
            RoundCounter.nRoundCounter[i] = 0;
        }

        if (PhotonNetwork.inRoom)
        {
            if (PhotonNetwork.isMasterClient)
                PhotonNetwork.DestroyAll();

            GetComponent<PhotonView>().RPC("OnlineReCharaSelect", PhotonTargets.All);
        }
        else
        {
            SceneManager.LoadScene("CharacterSelect");
        }
    }

    [PunRPC]
    public void OnlineReCharaSelect()
    {
        SceneManager.LoadScene("OnlineRoom");
    } 
}
