using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PutAI : MonoBehaviour
{
    EnemyAI _enemyAI;
    MoveAI _moveAI;
    FieldObjectBase _fieldObjBase = null;

    public void Init(int level)
    {
        _enemyAI = GetComponent<EnemyAI>();
        _moveAI = GetComponent<MoveAI>();
        _fieldObjBase = GetComponent<FieldObjectBase>();
    }

    public bool OnPut()
    {
        switch(Random.Range(0,4))
        {
            case 0:
                return RandomPut();
            case 1:
                return CharaPut();
            case 2:
                return HalfSandPut(false);
            case 3:
                return HalfSandPut(true);
        }
        return false;
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
        List<FieldObjectBase> charas = FieldData.Instance.GetCharactors.Where(x => x && x != _fieldObjBase).ToList();
        if (charas.Count <= 0)
            return false;

        _moveAI.SearchRoute(charas[Random.Range(0, charas.Count)].GetDataNumber(), 2);

        return true;
    }

    bool HalfSandPut(bool isNear)
    {
        SandData.tHalfSandData[] dataList = SandData.Instance.GetHalfSandDataList; //.Where(_ => TypeCheck(_._type)).ToArray();
        SandData.tHalfSandData data = new SandData.tHalfSandData();
        int number = 0;

        if (isNear)
        {   //  最も近いはさめる箇所を探索
            int min = 1000;
            int nowNumber = _fieldObjBase.GetDataNumber();
            int x = nowNumber % GameScaler._nWidth;
            int z = nowNumber / GameScaler._nWidth;

            int element = 0;
            dataList = dataList.Where(_ =>
            {
                int dis = Mathf.Abs(x - element % GameScaler._nWidth) + Mathf.Abs(z - element / GameScaler._nWidth);
                element++;
                if (SandData.Instance.GetHalfSandDataList[element - 1]._type == SandItem.eType.MAX)
                    return false;
                if(dis >= min)
                    return false;

                min = dis;
                number = element - 1;
                return true;
            }).ToArray();
            data = dataList[dataList.Length - 1];
        }
        else
        {
            data = dataList[Random.Range(0, dataList.Length)];
        }

        switch(data._dir)
        {
            case Charactor.eDirection.FORWARD:
                number += GameScaler._nWidth;
                break;
            case Charactor.eDirection.BACK:
                number -= GameScaler._nWidth;
                break;
            case Charactor.eDirection.RIGHT:
                number ++;
                break;
            case Charactor.eDirection.LEFT:
                number --;
                break;
        }
        _moveAI.SearchRoute(number, 1);

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

    //  自分で挟むことが出来るか
    bool TypeCheck(SandItem.eType type)
    {
        if(type == SandItem.eType.BLOCK)
        {
            return true;
        }
        else if (name.Contains("1P") && type == SandItem.eType.ONE_P)
        {
            return true;
        }
        else if (name.Contains("2P") && type == SandItem.eType.TWO_P)
        {
            return true;
        }
        else if (name.Contains("3P") && type == SandItem.eType.THREE_P)
        {
            return true;
        }
        else if (name.Contains("4P") && type == SandItem.eType.FOUR_P)
        {
            return true;
        }

        return false;
    }
}
