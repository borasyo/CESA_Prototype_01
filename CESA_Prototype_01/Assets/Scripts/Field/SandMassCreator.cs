using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandMassCreator : MonoBehaviour
{
    int _nWidth, _nHeight;
    SandMass[] _sandMassArray = null;
    GameObject _sandMassHolder = null;

    public SandMass[] Create (int w, int h)
    {
        _nWidth  = w;
        _nHeight = h;
        _sandMassArray = new SandMass[w * h];

        //  地形生成
        CreateSandMass();

        return _sandMassArray;
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
                Vector3 createPos = new Vector3(x * GameScaler._fScale, 0.0f, z * GameScaler._fScale);

                GameObject sandMass = CreateObj(SandMass, createPos);
                sandMass.transform.eulerAngles = new Vector3(90,0,0);
                _sandMassArray[x + (z * _nWidth)] = sandMass.GetComponent<SandMass>();
            }
        } 
    }

    GameObject CreateObj(GameObject obj, Vector3 pos)
    {
        GameObject instance = (GameObject)Instantiate(obj, pos, Quaternion.identity);
        instance.transform.SetParent(_sandMassHolder.transform);

        return instance;
    }
}
