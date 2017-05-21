using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutAI : MonoBehaviour
{
    EnemyAI _enemyAI;

    void Start()
    {
        _enemyAI = GetComponent<EnemyAI>();
    }

    void Update ()
    {
		
	}

    public bool OnPut(MoveAI moveAI)
    {
        int rand = RandomPutMass();
        if (rand < 0)
            return false;
        
        moveAI.SearchRoute(rand, true);
        return true;
    }

    int RandomPutMass()
    {
        int rand = 0;
        int loopCnt = 0;
        while (loopCnt < 100)
        {
            rand = Random.Range(0, GameScaler._nHeight * GameScaler._nWidth);
            loopCnt ++;
            if (FieldData.Instance.GetObjData(rand))
                continue;

            break;
        }
        if (loopCnt >= 100)
            return -1;

        return rand;
    }
}
