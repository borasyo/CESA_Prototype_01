using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;
using UnityEngine.SceneManagement;
using System.Linq;

public class CharacterSelectOnline : CharacterSelect
{
    GameObject _selectCanvas = null;
    public static int _nMyNumber = 0;

    // オンライン時のキャラセレクト

    //  どのプレイヤー番号が空いているかを確認
    public void SetPlayerNumber()
    {
        if (PhotonNetwork.isMasterClient)
            return;

        for (int i = 0; i < _nowSelectDatas.Length; i++)
        {
            if (_nowSelectDatas[i].transform.parent.childCount < 2)
                continue;

            _nMyNumber = i;
            break;
        }
    }

    public void SetNowSelect(NowSelectOnline nowSelect, int idx)
    {
        if (!_selectCanvas)
            _selectCanvas = GameObject.FindWithTag("SelectCanvas");

        //  元あるものを破壊して自分を入れる
        nowSelect.transform.parent.SetParent(_selectCanvas.transform);
        nowSelect.transform.parent.position = _nowSelectDatas[idx].transform.parent.position;
        _nowSelectDatas[idx] = nowSelect;
    }

    public bool InstanceCheck(GameObject obj)
    {
        foreach (NowSelect now in _nowSelectDatas)
        {
            if (now.gameObject != obj)
                continue;

            return true;
        }

        return false;
    }

    public override void GameStart()
    {
        SetChara();

        //  2キャラ以上いない場合は進まない
        if (SelectCharas.Where(_ => _).Count() < 2)
            return;

        SceneManager.LoadScene("OnlineGameMain");
    }
}
