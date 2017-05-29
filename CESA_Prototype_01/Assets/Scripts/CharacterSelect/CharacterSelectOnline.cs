using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;
using UnityEngine.SceneManagement;
using System.Linq;

public class CharacterSelectOnline : CharacterSelect
{
    public static int _nMyNumber = 0;

    // オンライン時のキャラセレクト
    void Awake()
    {
        _nMyNumber = 0;
    }
    
    //  自身のプレイヤー番号をセット 
    public void SetPlayerNumber()
    {
        if (PhotonNetwork.isMasterClient)
            return;

        for (int i = 0; i < _nowSelectDatas.Length; i++)
        {
            if (!_nowSelectDatas[i] || _nowSelectDatas[i].transform.parent.childCount < 2)
                continue;

            _nMyNumber = i;
            break;
        }
    }

    //  新たに生成したキャラセレパネルはどこに配置すべきかを返す
    public int GetCreateNumber()
    {
        int number = 0;
        for (int i = 0; i < _nowSelectDatas.Length; i++)
        {
            if (!_nowSelectDatas[i] || _nowSelectDatas[i].transform.parent.childCount < 2)
                continue;

            number = i;
            break;
        }
        return number;
    }
    
    public void SetNowSelect(NowSelectOnline nowSelect, int idx)
    {
        if (idx < 0 || _nowSelectDatas.Length <= idx)
            return;

        //  自分の位置に挿入
        nowSelect.transform.parent.SetParent(this.transform);
        nowSelect.transform.parent.GetComponent<RectTransform>().position = _nowSelectDatas[idx].transform.parent.GetComponent<RectTransform>().position;
        _nowSelectDatas[idx] = nowSelect;
    }

    public int InstanceCheck(GameObject obj)
    {
        int number = 0;
        foreach (NowSelect now in _nowSelectDatas)
        {
            if (!now || now.gameObject != obj)
            {
                number++;
                continue;
            }

            //  発見した
            return number;
        }
        //  失敗
        return -1;
    }


    public bool EndInit()
    {
        foreach(NowSelect now in _nowSelectDatas)
        {
            if (now.isActiveAndEnabled)
                continue;

            return false;
        }
        return true;
    }

    public override void GameStart()
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        SetChara();

        //  2キャラ以上いない場合は進まない
        if (SelectCharas.Where(_ => _).Count() < 2)
            return;

        photonView.RPC("LoadOnlineGameMain", PhotonTargets.All);
    }

    [PunRPC]
    public void LoadOnlineGameMain()
    {
        SceneManager.LoadScene("OnlineGameMain");
    }
}
