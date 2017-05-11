using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldCreator : MonoBehaviour
{
    int _nWidth, _nHeight;
    FieldObjectBase[] _objBaseArray = null;
    GameObject _fieldHolder = null;

    public FieldObjectBase[] Create (int w, int h)
    {
        _nWidth  = w;
        _nHeight = h;
        _objBaseArray = new FieldObjectBase[w * h];

        //  地形生成
        CreateField();

        //  キャラ生成
        CreateChara();

        return _objBaseArray;
    }

    void CreateField()
    {
        //  入れ物生成 (初期生成した消えないオブジェクトのみ格納)
        _fieldHolder = new GameObject("InitFieldHolder");

        //  リソース取得
        GameObject BlockObj = Resources.Load<GameObject> ("Prefabs/Field/Block");
        GameObject TileObj  = Resources.Load<GameObject> ("Prefabs/Field/Tile");

        for (int x = 0; x < _nWidth; x ++)
        {
            for (int z = 0; z < _nHeight; z ++)
            {
                Vector3 createPos = new Vector3(x * GameScaler._fScale, 0.0f, z * GameScaler._fScale);

                if (FenceCheck(x,z) || RandomBlock(x,z))
                {
                    GameObject block = CreateObj(BlockObj, createPos);
                    _objBaseArray[x + (z * _nWidth)] = block.GetComponent<FieldObjectBase>();
                }

                CreateObj(TileObj, createPos).transform.eulerAngles = new Vector3(90,0,0);
            }
        } 
    }
     
    void CreateChara()
    {
        //  リソース取得
        GameObject PlayerObj = Resources.Load<GameObject> ("Prefabs/Field/Player");
        GameObject EnemyObj  = Resources.Load<GameObject> ("Prefabs/Field/Enemy");

        // 左下に生成
        Vector3 ppos = new Vector3(1.0f  * GameScaler._fScale, 0.0f, 1.0f  * GameScaler._fScale);
        GameObject p = CreateObj(PlayerObj, ppos);
        _objBaseArray[_nWidth + 1] = p.GetComponent<FieldObjectBase>();

        //  右上に生成
        Vector3 epos = new Vector3((_nWidth - 2.0f) * GameScaler._fScale, 0.0f, (_nHeight - 2.0f) * GameScaler._fScale);
        GameObject e = CreateObj(EnemyObj, epos);
        _objBaseArray[(_nWidth - 2) * (_nHeight - 2)] = e.GetComponent<FieldObjectBase>();
    }

    GameObject CreateObj(GameObject obj, Vector3 pos)
    {
        GameObject instance = (GameObject)Instantiate(obj, pos, obj.transform.rotation);
        instance.transform.SetParent(_fieldHolder.transform);

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

    bool RandomBlock(float x, float z)
    {
        if (Random.Range(0, 10) != 0)
            return false;

        if (z < 2)
            return false;
        
        if (z >= _nHeight - 2)
            return false;
        
        if (x % _nWidth < 2)
            return false;

        if (x % _nWidth >= _nWidth - 2)
            return false;

        return true;
    }
}
