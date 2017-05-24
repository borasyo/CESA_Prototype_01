using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorInputAI : CharactorInput
{
    public EnemyAI _enemyAI { get; private set; }

    void Awake()
    {
        _enemyAI = this.gameObject.AddComponent<EnemyAI>();
    }

    override protected void InputCheck()
    {
        _IsForawrd = _enemyAI.GetMove(Charactor.eDirection.FORWARD);
        _IsBack = _enemyAI.GetMove(Charactor.eDirection.BACK);
        _IsRight = _enemyAI.GetMove(Charactor.eDirection.RIGHT);
        _IsLeft = _enemyAI.GetMove(Charactor.eDirection.LEFT);
        _IsPut = _enemyAI.GetAction(Charactor.eAction.PUT);
        _IsBreak = _enemyAI.GetAction(Charactor.eAction.BREAK);
    }
}
