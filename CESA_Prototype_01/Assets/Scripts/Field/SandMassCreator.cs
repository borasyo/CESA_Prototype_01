using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandMassCreator : MonoBehaviour
{
    int _nWidth, _nHeight;
    //SandMass[] _sandMassArray = null;
    GameObject _sandMassHolder = null;

    public void Create (int w, int h)
    {
        _nWidth  = w;
        _nHeight = h;

        //  地形生成
        CreateSandMass();
    }

    void CreateSandMass()
    {
        //  入れ物生成 (初期生成した消えないオブジェクトのみ格納)
        _sandMassHolder = new GameObject("InitSandMassHolder");

        //  リソース取得
        GameObject SandMass = Resources.Load<GameObject> ("Prefabs/Field/SandMass");

        //  
        for (int x = 0; x < _nWidth; x ++)
        {
            for (int z = 0; z < _nHeight; z ++)
            {
                if (FenceCheck(x, z))
                    continue;

                Vector3 createPos = new Vector3(x * GameScaler._fScale, 0.0f, z * GameScaler._fScale);

                GameObject sandMass = CreateObj(SandMass, createPos);
                sandMass.transform.eulerAngles = new Vector3(0, 90, 0);
                //_sandMassArray[x + (z * _nWidth)] = sandMass.GetComponent<SandMass>();
                sandMass.GetComponent<SandMass>().SetSandDir = SandData.eSandDir.VERTICAL;

                sandMass = CreateObj(SandMass, createPos);
                sandMass.transform.eulerAngles = new Vector3(0, 0, 0);
                //_sandMassArray[x + (z * _nWidth)] = sandMass.GetComponent<SandMass>();
                sandMass.GetComponent<SandMass>().SetSandDir = SandData.eSandDir.HORIZONTAL;
            }
        } 
    }

    GameObject CreateObj(GameObject obj, Vector3 pos)
    {
        GameObject instance = (GameObject)Instantiate(obj, pos, Quaternion.identity);
        instance.transform.SetParent(_sandMassHolder.transform);

        return instance;
    }

    //  フィールドの外周かどうかをチェックする
    bool FenceCheck(float x, float z)
    {
        if (x % _nWidth == 0)
            return true;

        if (x % _nWidth == _nWidth - 1)
            return true;

        if (z == 0)
            return true;

        if (z == _nHeight - 1)
            return true;

        return false;
    }
}
