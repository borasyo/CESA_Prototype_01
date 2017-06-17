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

        if (FadeManager.Instance.Fading)
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

            SceneChanger.Instance.ChangeScene("OnlineGameMain", true);
            //SceneManager.LoadScene("OnlineGameMain");
        }
        else
        {
            SceneChanger.Instance.ChangeScene("GameMain", true);
            //SceneManager.LoadScene("GameMain");
        }
    }

    // 最後のresultへ
    IEnumerator LastResult()
    {
        yield return new WaitForSeconds(2.0f);

        RoundCounter.nNowWinerPlayer = -1; 

        if (PhotonNetwork.inRoom)
        {
            SceneChanger.Instance.ChangeScene("OnlineResult", true);
            //SceneManager.LoadScene("OnlineResult");
        }
        else
        {
            SceneChanger.Instance.ChangeScene("Result", true);
            //SceneManager.LoadScene("Result");
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
            SceneChanger.Instance.ChangeScene("CharacterSelect", true);
            //SceneManager.LoadScene("CharacterSelect");
        }
    }

    [PunRPC]
    public void OnlineReCharaSelect()
    {
        SceneChanger.Instance.ChangeScene("OnlineRoom", true);
        //SceneManager.LoadScene("OnlineRoom");
    } 
}
