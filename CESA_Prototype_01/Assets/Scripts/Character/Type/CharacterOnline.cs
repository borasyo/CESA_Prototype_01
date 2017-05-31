using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterOnline : Character
{
    protected override void SetInput(int level)
    {
        // Input生成
        if (this.name.Contains("CPU"))
        {
            CharacterInputAI ai = this.gameObject.GetComponent<CharacterInputAIOnline>();
            _charactorInput = ai;

            if (!ai._enemyAI)
                return;

            ai._enemyAI.Set(level, _charaType);
        }
        else
        {
            _charactorInput = this.gameObject.GetComponent<CharacterInputUserOnline>();
        }
    }

    [PunRPC]
    public void Create(string player, int angle, int idx, int level)
    {
        transform.SetParent(GameObject.Find("CharaHolder").transform);
        name += player;
        transform.eulerAngles = new Vector3(0, angle, 0);
        GetComponent<Character>().Init(level);
    }

    void Update()
    {
        if (!photonView.isMine)
            return;

        base.Update();
    }

    public void OnlineUpdate()
    {
        Debug.Log("OnlineUpdate : " + transform.name);

        _nOldNumber = GetDataNumber();

        //MoveUpdate(); //  移動は座標同期
        MoveCheck(eDirection.FORWARD);
        MoveCheck(eDirection.BACK);
        MoveCheck(eDirection.RIGHT);
        MoveCheck(eDirection.LEFT);

        DirUpdate();
        NumberUpdate();

        //  アクション
        ItemPut();
        ItemBreak();
    }
}
