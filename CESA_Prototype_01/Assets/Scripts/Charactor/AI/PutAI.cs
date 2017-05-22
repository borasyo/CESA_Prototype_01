using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PutAI : MonoBehaviour
{
    EnemyAI _enemyAI;
    MoveAI _moveAI;

    void Start()
    {
        _enemyAI = GetComponent<EnemyAI>();
        _moveAI = GetComponent<MoveAI>();
    }

    public bool OnPut()
    {
        return RandomPut();
    }

    #region AI

    //  ランダムにアイテムを配置する
    bool RandomPut()
    {
        int rand = RandomPutMass();
        if (rand < 0)
            return false;

        _moveAI.SearchRoute(rand, 1);
        return true;

    }

    //  キャラの目の前にアイテムを配置する
    bool CharaPut()
    {
        List<FieldObjectBase> charas = FieldData.Instance.GetCharactors;
        _moveAI.SearchRoute(charas[Random.Range(0, charas.Count)].GetDataNumber(), 2);

        return true;
    }

    #endregion

    int RandomPutMass()
    {
        List<int> nullMassList = new List<int>();
        FieldObjectBase[] objList = FieldData.Instance.GetObjDataArray;
        for (int i = 0; i < objList.Length; i++)
        {
            if (objList[i])
                continue;

            nullMassList.Add(i);
        }

        if (nullMassList.Count <= 0)
            return -1;

        return nullMassList[Random.Range(0, nullMassList.Count)];
    }
}
