using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundCounter : Photon.MonoBehaviour
{
    #region Singleton

    private static RoundCounter instance;

    public static RoundCounter Instance
    {
        get
        {
            if (instance)
                return instance;

            instance = (RoundCounter)FindObjectOfType(typeof(RoundCounter));

            if (instance)
                return instance;

            GameObject obj = new GameObject("RoundCounter");
            obj.AddComponent<RoundCounter>();
            Debug.Log(typeof(RoundCounter) + "が存在していないのに参照されたので生成");

            return instance;
        }
    }

    #endregion

    static public int[] nRoundCounter = new int[4];

    /// <summary>
    ///     勝者が確定した場合に呼び、リザルトへ行く
    /// </summary>
    /// <param name="obj"></param>
    public void WinCharacter(FieldObjectBase obj)
    {
        if (PhotonNetwork.inRoom && !PhotonNetwork.isMasterClient)
            return;

        //Debug.Log("Win");
        List<Character> charaData = FieldData.Instance.GetCharactors;

        if (!obj.name.Contains("CPU"))
        {
            foreach (Character chara in charaData)
            {
                if (chara.gameObject != obj.gameObject)
                    continue;

                //  勝者のカウントを1つ増加
                CountUp(chara.GetPlayerNumberToInt() - 1);
            }
        }
        else
        {
            Character data = charaData[0];
            foreach (Character chara in charaData)
            {
                //  同じならランダムで決定
                if (chara.Level == data.Level && Random.Range(0, 2) == 0)
                {
                    data = chara;
                    continue;
                }

                //  そのキャラの方がレベルが高いか
                if (chara.Level < data.Level && Random.Range(0, (int)(3 / Mathf.Abs(chara.Level - data.Level))) == 0)
                {
                    data = chara;
                    continue;
                }

                //  キャラの方がレベルが低い場合、たまに勝てる
                if (chara.Level > data.Level && Random.Range(0, Mathf.Abs(chara.Level - data.Level) * 4) != 0)
                {
                    data = chara;
                    continue;
                }
            }

            CountUp(data.GetPlayerNumberToInt() - 1);
        }
    }

    void CountUp(int idx)
    {
        if (!PhotonNetwork.inRoom)
        {
            //  勝者のカウントを1つ増加
            nRoundCounter[idx]++;

            //  リザルトへ
            GoResult();
        }
        else
        {
            photonView.RPC("OnlineCountUp", PhotonTargets.All, idx);
        }
    }

    [PunRPC]
    public void OnlineCountUp(int idx)
    {
        nRoundCounter[idx]++;

        //  リザルトへ
        GoResult();
    }

    void GoResult()
    {
        if (PhotonNetwork.inRoom)
        {
            SceneManager.LoadScene("OnlineResult");
        }
        else
        {
            SceneManager.LoadScene("Result");
        }
    }
}
