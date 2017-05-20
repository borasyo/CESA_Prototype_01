using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorInputAI : CharactorInput
{
    EnemyAI _enemyAI = null;

    void Awake()
    {
        _enemyAI = this.gameObject.AddComponent<EnemyAI>();
    }

    override protected void InputCheck()
    {    
        
    }

    override public bool GetMoveInput(Charactor.eDirection dir)
    {
        return _enemyAI.GetMove(dir);
    }

    override public bool GetActionInput(Charactor.eAction act)
    {
        return _enemyAI.GetAction(act);
    }
}
