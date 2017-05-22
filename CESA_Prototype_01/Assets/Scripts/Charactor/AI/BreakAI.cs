using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BreakAI : MonoBehaviour
{
    EnemyAI _enemyAI;
    MoveAI _moveAI;

    void Start()
    {
        _enemyAI = GetComponent<EnemyAI>();
        _moveAI = GetComponent<MoveAI>();
    }

    public bool OnBreak()
    {
        return RandomBreak();
    }

    #region AI

    bool RandomBreak()
    {
        int rand = RandomBreakMass();
        if (rand < 0)
            return false;

        _moveAI.SearchRoute(rand, 1);
        return true;
    }

    #endregion

    int RandomBreakMass()
    {
        FieldObjectBase[] sandItemList = FieldData.Instance.GetObjDataArray.Where(element => element && element.tag == "SandItem").ToArray();

        if (sandItemList.Length <= 0)
            return -1;

        return sandItemList[Random.Range(0, sandItemList.Length)].GetDataNumber();
    }
}
