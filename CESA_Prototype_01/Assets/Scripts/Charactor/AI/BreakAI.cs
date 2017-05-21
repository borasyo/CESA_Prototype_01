using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakAI : MonoBehaviour
{
    EnemyAI _enemyAI;

	void Start ()
    {
        _enemyAI = GetComponent<EnemyAI>();
    }

    void Update()
    {

    }

    public bool OnBreak(MoveAI moveAI)
    {
        int rand = RandomBreakMass();
        if (rand < 0)
            return false;
        
        moveAI.SearchRoute(rand, true);
        return true;
    }

    int RandomBreakMass()
    {
        int rand = 0;
        int loopCnt = 0;
        while (loopCnt < 100)
        {
            rand = Random.Range(0, GameScaler._nHeight * GameScaler._nWidth);
            loopCnt++;
            FieldObjectBase obj = FieldData.Instance.GetObjData(rand);
            if (!obj || obj.tag != "SandItem")
                continue;

            break;
        }
        if (loopCnt >= 100)
            return -1;

        return rand;
    }
}
