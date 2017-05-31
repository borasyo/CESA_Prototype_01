using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldCreator : MonoBehaviour
{
    protected int _nWidth, _nHeight;
    protected FieldObjectBase[] _objBaseArray = null;
    GameObject _fieldHolder = null;
    protected GameObject _charaHolder = null;

    public FieldObjectBase[] Create (int w, int h)
    {
        _nWidth  = w;
        _nHeight = h;
        _objBaseArray = new FieldObjectBase[w * h];

        //  地形生成
        CreateField();

        //  キャラ生成
        AIData.Instance.Set();
        CreateChara();

        return _objBaseArray;
    }

    void CreateField()
    {
        //  入れ物生成 (初期生成した消えないオブジェクトのみ格納)
        _fieldHolder = new GameObject("InitFieldObjHolder");
        _charaHolder = new GameObject("CharaHolder");

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
                    CreateRandomBlock(BlockObj, createPos, x + (z * _nWidth));
                }

                GameObject tile = CreateObj(TileObj, createPos);
                tile.transform.eulerAngles = new Vector3(90,0,0);
#if DEBUG
                tile.GetComponentInChildren<TextMesh>().text = (z * GameScaler._nWidth + x).ToString();
#else
                Destroy(tile.transform.GetChild(0).gameObject);
#endif
            }
        } 
    }
     
    protected virtual void CreateChara()
    {
        Vector3 pos = Vector3.zero;
        GameObject obj = null;
        GameObject[] SelectCharas = CharacterSelect.SelectCharas;
        int[] SelectLevels = LevelSelect.SelectLevel;

        // 左下に生成
        if (!SelectCharas[0])
            SelectCharas[0] = Resources.Load<GameObject> ("Prefabs/Chara/Balance");

        pos = new Vector3(1.0f * GameScaler._fScale, 0.0f, 1.0f * GameScaler._fScale);
        obj = CreateObj(SelectCharas[0], pos);
        obj.name += ",1Player";
        obj.transform.eulerAngles = new Vector3(0, 90, 0);
        _objBaseArray[_nWidth + 1] = obj.GetComponent<FieldObjectBase>();
        obj.GetComponent<Character>().Init(0);

        //  右上に生成
        if (!SelectCharas[1])
            SelectCharas[1] = Resources.Load<GameObject> ("Prefabs/Chara/Balance");
        
        pos = new Vector3((_nWidth - 2.0f) * GameScaler._fScale, 0.0f, (_nHeight - 2.0f) * GameScaler._fScale);
        obj = CreateObj(SelectCharas[1], pos);
        obj.name += ",2Player" + ",CPU";
        obj.transform.eulerAngles = new Vector3(0, 270, 0);
        _objBaseArray[_nWidth * (_nHeight - 2) + _nWidth - 2] = obj.GetComponent<FieldObjectBase>();
        obj.GetComponent<Character>().Init(SelectLevels[1]);

        //  左上に生成
        if (SelectCharas[2])
        {
            pos = new Vector3(1.0f * GameScaler._fScale, 0.0f, (_nHeight - 2.0f) * GameScaler._fScale);
            obj = CreateObj(SelectCharas[2], pos);
            obj.name += ",3Player" + ",CPU";
            obj.transform.eulerAngles = new Vector3(0, 90, 0);
            _objBaseArray[1 + _nWidth * (_nHeight - 2)] = obj.GetComponent<FieldObjectBase>();
            obj.GetComponent<Character>().Init(SelectLevels[2]);
        }

        //  右上に生成
        if (SelectCharas[3])
        {
            pos = new Vector3((_nWidth - 2.0f) * GameScaler._fScale, 0.0f, 1.0f * GameScaler._fScale);
            obj = CreateObj(SelectCharas[3], pos);
            obj.name += ",4Player" + ",CPU";
            obj.transform.eulerAngles = new Vector3(0, 270, 0);
            _objBaseArray[(_nWidth - 2) + _nWidth] = obj.GetComponent<FieldObjectBase>();
            obj.GetComponent<Character>().Init(SelectLevels[3]);
        }
    }

    GameObject CreateObj(GameObject obj, Vector3 pos)
    {
        GameObject instance = (GameObject)Instantiate(obj, pos, obj.transform.rotation);

        if(instance.tag == "Character")
            instance.transform.SetParent(_charaHolder.transform);
        else
            instance.transform.SetParent(_fieldHolder.transform);

        return instance;
    }

    protected virtual void CreateRandomBlock(GameObject obj ,Vector3 pos, int idx)
    {
        GameObject block = CreateObj(obj, pos);
        _objBaseArray[idx] = block.GetComponent<FieldObjectBase>();
    }

    //  フィールドの外周かどうかをチェックする
    protected bool FenceCheck(float x, float z)
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

    protected bool RandomBlock(float x, float z)
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
