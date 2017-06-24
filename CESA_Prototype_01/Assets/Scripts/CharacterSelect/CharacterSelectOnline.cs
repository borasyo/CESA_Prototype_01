using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UniRx;
using UniRx.Triggers;
using UnityEngine.SceneManagement;
using System.Linq;

public class CharacterSelectOnline : CharacterSelect
{
    public static int _nMyNumber = 0;
    [SerializeField] Vector3[] _nowSelectPos = new Vector3[4];
    bool _IsChange = true;

    // オンライン時のキャラセレクト
    void Awake()
    {
        //_nMyNumber = 0;
        for (int idx = 0; idx < 4; idx++)
        {
            _nowSelectPos[idx] = _nowSelectDatas[idx].transform.parent.GetComponent<RectTransform>().localPosition;
            _nowSelectDatas[idx].transform.parent.gameObject.SetActive(false);
            _nowSelectDatas[idx] = null;
        }

        if (!PhotonNetwork.isMasterClient)
            return;

        PhotonNetwork.room.IsOpen = true;
    }

    void Start()
    {
        // baseのStartを呼ばせないため
    }
    
    //  自身のプレイヤー番号をセット 
    public void SetPlayerNumber(int number)
    {
        if (!PhotonNetwork.inRoom)
            return;

        _nMyNumber = number;
        //Debug.Log("SetPlayerNumber" + _nMyNumber);
    }

    //  新たに生成したキャラセレパネルはどこに配置すべきかを返す
    public int GetCreateNumber()
    {
        int number = 0;
        for (int i = 0; i < _nowSelectDatas.Length; i++)
        {
            if (_nowSelectDatas[i] && !_nowSelectDatas[i].transform.parent.name.Contains("CPU"))
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
        RectTransform rectTrans = nowSelect.transform.parent.GetComponent<RectTransform>();
        rectTrans.localPosition = _nowSelectPos[idx];

        _nowSelectDatas[idx] = nowSelect;
    }

    public NowSelect GetNowSelect(int idx)
    {
        if (idx < 0 || _nowSelectDatas.Length <= idx)
            return null;

        return _nowSelectDatas[idx];
    }

    public int GetNullIdx()
    {
        for(int idx = 0; idx < _nowSelectDatas.Length; idx++)
        {
            if (_nowSelectDatas[idx])
                continue;

            return idx;
        }
        //  空きはない
        return -1;
    }

    public Vector3 GetLocalPos(GameObject obj)
    {
        for(int i = 0; i < _nowSelectDatas.Length; i++)
        {
            NowSelect now = _nowSelectDatas[i];
            if (!now || now.gameObject != obj)
                continue;

            return _nowSelectPos[i];
        }

        return Vector3.zero;
    }
    public eCharaType GetCharaType(int idx)
    {
        if (!SelectCharas[idx])
            return eCharaType.NONE;

        return SearchCharaType(SelectCharas[idx], IsRandom[idx]);
    }

    //public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    public void PlayerChange(int number)
    { 
        if(number <= 0 || !_IsChange)
            return;

        //  詰める
        int idx = 0;
        for (idx = number; idx < _nowSelectDatas.Length - 1; idx++)
        {
            NowSelectOnline nowSelect = null;
            if (_nowSelectDatas[idx + 1])
            {
                nowSelect = _nowSelectDatas[idx + 1].GetComponent<NowSelectOnline>();

                if (nowSelect.tag != "CPU")
                {
                    //  1つ前へ
                    nowSelect.PlayerChange(idx);
                    _nowSelectDatas[idx] = _nowSelectDatas[idx + 1];
                    continue;
                }

                //  CPUが見つかったので終了
                break;
            }
            else
            {
                //  Nullのオブジェクトがあれば重複実行なので無視
                return;
            }
        }        
        //  最後にキャラを発見した箇所をNullにする
        _nowSelectDatas[idx] = null;
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
        _IsChange = false;
        SetChara();
        GetComponent<LevelSelect>().SetLevel();
        PhotonNetwork.room.IsOpen = false;
        StartCoroutine(Next());
    }

    protected override IEnumerator Next()
    {
        foreach (Button button in FindObjectsOfType<Button>())
            button.enabled = false;

        yield return null;

        SceneChanger.Instance.ChangeScene("OnlineStageSelect", true);
        //SceneManager.LoadScene("OnlineStageSelect");
    }
}
