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

                if (FenceCheck(x, z))
                {
                    GameObject block = CreateObj(BlockObj, createPos);
                    block.name += ",Fence";
                    _objBaseArray[x + (z * _nWidth)] = block.GetComponent<FieldObjectBase>();
                }
                else if (RandomBlock(x, z))
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
        GameObject BalanceObj = Resources.Load<GameObject> ("Prefabs/Chara/Balance");
        GameObject PowerObj = Resources.Load<GameObject> ("Prefabs/Chara/Power");
        GameObject TechnicalObj = Resources.Load<GameObject> ("Prefabs/Chara/Technical");
        GameObject SpeedObj = Resources.Load<GameObject> ("Prefabs/Chara/Speed");

        Vector3 pos = Vector3.zero;
        GameObject obj = null;

        // 左下に生成
        pos = new Vector3(1.0f  * GameScaler._fScale, 0.0f, 1.0f  * GameScaler._fScale);
        obj = CreateObj(BalanceObj, pos);
        obj.name += "1P";
        _objBaseArray[_nWidth + 1] = obj.GetComponent<FieldObjectBase>();

        //  左上に生成
        /*pos = new Vector3(1.0f * GameScaler._fScale, 0.0f, (_nHeight - 2.0f) * GameScaler._fScale);
        obj = CreateObj(PowerObj, pos);
        obj.name += "2P";
        _objBaseArray[1 + _nWidth * (_nHeight - 2)] = obj.GetComponent<FieldObjectBase>();

        //  右上に生成
        pos = new Vector3((_nWidth - 2.0f) * GameScaler._fScale, 0.0f, 1.0f  * GameScaler._fScale);
        obj = CreateObj(TechnicalObj, pos);
        obj.name += "3P";
        _objBaseArray[(_nWidth - 2) + _nWidth] = obj.GetComponent<FieldObjectBase>();*/

        //  右上に生成
        pos = new Vector3((_nWidth - 2.0f) * GameScaler._fScale, 0.0f, (_nHeight - 2.0f) * GameScaler._fScale);
        obj = CreateObj(BalanceObj, pos);
        obj.name += "4P";
        _objBaseArray[_nWidth * (_nHeight - 2) + _nWidth - 2] = obj.GetComponent<FieldObjectBase>();
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

        //  詰み状態回避処理
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
