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
    static public int nNowWinerPlayer = 0;

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

            nNowWinerPlayer = idx;

            // CPUが残っていれば死亡させる
            GameObject CPUDeath = Resources.Load<GameObject>("Prefabs/Effect/CPUDeath");
            List<Character> charaList = FieldData.Instance.GetCharactors;
            for (int i = 0; i < charaList.Count; i++)
            {
                //Debug.Log(charaList[i].name);
                if (nNowWinerPlayer == charaList[i].GetPlayerNumberToInt() - 1)
                {
                    //  TODO : 勝利演出
                    charaList[i].Win();
                }
                else
                {
                    GameObject effect = Instantiate(CPUDeath, charaList[i].transform.position + new Vector3(0.0f, GameScaler._fScale * 0.5f, 0.0f), Quaternion.identity);
                    effect.GetComponent<PlayerDeathEffect>().Init(charaList[i].GetPlayerNumber() + "P");
                    Destroy(charaList[i].gameObject);
                }
            }
            charaList.Clear();

            //  リザルトへ
            StartCoroutine(GoResult());
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

        nNowWinerPlayer = idx;

        // CPUが残っていれば死亡させる
        GameObject CPUDeath = Resources.Load<GameObject>("Prefabs/Effect/CPUDeath");
        List<Character> charaList = FieldData.Instance.GetCharactors;
        for (int i = 0; i < charaList.Count; i++)
        {
            //Debug.Log(charaList[i].name);
            if (nNowWinerPlayer == charaList[i].GetPlayerNumberToInt() - 1)
            {
                //  TODO : 勝利演出
                charaList[i].Win();
            }
            else
            {
                GameObject effect = Instantiate(CPUDeath, charaList[i].transform.position + new Vector3(0.0f, GameScaler._fScale * 0.5f, 0.0f), Quaternion.identity);
                effect.GetComponent<PlayerDeathEffect>().Init(charaList[i].GetPlayerNumber() + "P");
                Destroy(charaList[i].gameObject);
            }
        }
        charaList.Clear();

        //  リザルトへ
        StartCoroutine(GoResult());
    }

    IEnumerator GoResult()
    {
        yield return new WaitForSeconds(3.0f);

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
}
