using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoNext : Photon.MonoBehaviour
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
            SceneChanger.Instance.ChangeScene("OnlineGameMain", true);
            //SceneManager.LoadScene("OnlineGameMain");
            StartCoroutine(Cleanup());
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
        if (PhotonNetwork.inRoom)
        {
            photonView.RPC("OnlineReCharaSelect", PhotonTargets.All);
            StartCoroutine(Cleanup());
        }
        else
        {
            for (int i = 0; i < RoundCounter.nRoundCounter.Length; i++)
            {
                RoundCounter.nRoundCounter[i] = 0;
            }

            SceneChanger.Instance.ChangeScene("CharacterSelect", true);
            //SceneManager.LoadScene("CharacterSelect");
        }
    }

    [PunRPC]
    public void OnlineReCharaSelect()
    {
        for (int i = 0; i < RoundCounter.nRoundCounter.Length; i++)
        {
            RoundCounter.nRoundCounter[i] = 0;
        }

        SceneChanger.Instance.ChangeScene("OnlineRoom", true);
        //SceneManager.LoadScene("OnlineRoom");
    } 

    IEnumerator Cleanup()
    {
        yield return new WaitWhile(() => FadeManager.Instance.HalfFading);

        if (PhotonNetwork.isMasterClient)
            PhotonNetwork.DestroyAll();
    }
}
