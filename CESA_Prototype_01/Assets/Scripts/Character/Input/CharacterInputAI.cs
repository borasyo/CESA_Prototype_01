using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputAI : CharacterInput
{
    public EnemyAI _enemyAI { get; private set; }

    void Awake()
    {
        if (PhotonNetwork.inRoom && !photonView.isMine)
            return;

        _enemyAI = this.gameObject.AddComponent<EnemyAI>();
    }

    override protected void InputCheck()
    {
        _IsForawrd = _enemyAI.GetMove(Character.eDirection.FORWARD);
        _IsBack = _enemyAI.GetMove(Character.eDirection.BACK);
        _IsRight = _enemyAI.GetMove(Character.eDirection.RIGHT);
        _IsLeft = _enemyAI.GetMove(Character.eDirection.LEFT);
        _IsPut = _enemyAI.GetAction(Character.eAction.PUT);
        _IsBreak = _enemyAI.GetAction(Character.eAction.BREAK);
    }
}
