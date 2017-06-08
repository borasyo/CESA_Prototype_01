using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldCreator : MonoBehaviour
{
    protected int _nWidth, _nHeight;
    protected FieldObjectBase[] _objBaseArray = null;
    GameObject _fieldHolder = null;
    protected GameObject _charaHolder = null;

    public FieldObjectBase[] Create ()
    {
        _nWidth  = GameScaler._nWidth;
        _nHeight = GameScaler._nHeight;
        _objBaseArray = new FieldObjectBase[_nWidth * _nHeight];

        //  地形生成
        CreateField();

        //  キャラ生成
        AIData.Instance.Set();
        CreateChara();

        return _objBaseArray;
    }

    void CreateField()
    {
        //  外回りのステージ生成
        GameObject stage = Resources.Load<GameObject>("Prefabs/Field/Stage");
        Instantiate(stage, new Vector3((float)GameScaler._nWidth / 2.0f - 0.5f, -1.25f, (float)GameScaler._nHeight / 2.0f - 0.5f), stage.transform.rotation);

        //  入れ物生成 (初期生成した消えないオブジェクトのみ格納)
        _fieldHolder = new GameObject("InitFieldObjHolder");
        _charaHolder = new GameObject("CharaHolder");

        //  リソース取得
        GameObject BlockObj = Resources.Load<GameObject>("Prefabs/Field/Block");
        GameObject WallObj = Resources.Load<GameObject>("Prefabs/Field/Wall");       
        GameObject CornerWallObj = Resources.Load<GameObject>("Prefabs/Field/Corner");
        GameObject TileObj  = Resources.Load<GameObject> ("Prefabs/Field/Tile");

        for (int x = 0; x < _nWidth; x ++)
        {
            for (int z = 0; z < _nHeight; z ++)
            {
                Vector3 createPos = new Vector3(x * GameScaler._fScale, 0.65f, z * GameScaler._fScale);

                if (FenceCheck(x, z))
                {
                    GameObject wall = null;
                    
                    //  角かチェック
                    if ((x == 0 || x == GameScaler._nWidth - 1) && (z == 0 || z == GameScaler._nHeight - 1))
                    {
                        wall = CreateObj(CornerWallObj, createPos);
                        SetCornerWallRot(wall, x, z);
                    }
                    else
                    {
                        wall = CreateObj(WallObj, createPos);
                        SetWallRot(wall, x, z);
                    }
                    wall.name += ",Fence";

                    _objBaseArray[x + (z * _nWidth)] = wall.GetComponent<FieldObjectBase>();
                }
                else if (StageBlock(x, z))
                {
                    CreateRandomBlock(BlockObj, createPos, x + (z * _nWidth));
                }

                GameObject tile = CreateObj(TileObj, createPos);
                tile.transform.localPosition += new Vector3(0.0f, -1.35f, 0.0f); //  モデルの原点がズレているため一旦補正　(リテイク後消す)
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

    void SetCornerWallRot(GameObject wall, int x, int z)
    {
        if (x == 0)  //  右向き
        {
            if (z == 0)  //  上向き
            {
                wall.transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else if (z == GameScaler._nHeight - 1)  //  下向き
            {
                wall.transform.eulerAngles = new Vector3(0, 90, 0);
            }
        }
        else if (x == GameScaler._nWidth - 1)  //  左向き
        {
            if (z == 0)  //  上向き
            {
                wall.transform.eulerAngles = new Vector3(0, 270, 0);
            }
            else if (z == GameScaler._nHeight - 1)  //  下向き
            {
                wall.transform.eulerAngles = new Vector3(0, 180, 0);
            }
        }

    }
    void SetWallRot(GameObject wall, int x, int z)
    {
        if (x == 0)  //  右向き
        {
            wall.transform.eulerAngles = new Vector3(0, 270, 0);
        }
        else if (x == GameScaler._nWidth - 1)  //  左向き
        {
            wall.transform.eulerAngles = new Vector3(0, 90, 0);
        }
        else if (z == 0)  //  上向き
        {
            wall.transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else if (z == GameScaler._nHeight - 1)  //  下向き
        {
            wall.transform.eulerAngles = new Vector3(0, 0, 0);
        }
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

    bool StageBlock(int x, int z)
    {
        //  ステージ番号取得
        int nStageNumber = SelectStage.GetStageNumber();

        switch (nStageNumber)
        {
            case 0:
                return StageOne(x, z);
            case 1:
                return StageTwo(x, z);
            case 2:
                return false;   //  まっさらステージ
        }

        return false;
    }

    bool StageOne(int x, int z)
    {
        int min = StageScaler.GetScale() == 0 ? 2 : 3;
        int max = StageScaler.GetScale() == 0 ? 3 : 4;

        if (x == GameScaler._nWidth  / 2 && z >= min && z <= GameScaler._nHeight - max)
            return true;

        if (z == GameScaler._nHeight / 2 && x >= 3 && x <= GameScaler._nWidth  - 4)
            return true;

        return false;
    }

    // TODO : returnの統一する
    bool StageTwo(int x, int z)
    {
        int min = StageScaler.GetScale() == 0 ? 2 : 3;
        int max = StageScaler.GetScale() == 0 ? 3 : 4;

        if ((x == min || x == GameScaler._nWidth - max) && (z == min || z == GameScaler._nHeight - max))
            return false;

        if ((x == GameScaler._nWidth - max || x == min) && (z <= GameScaler._nHeight - max && z >= min) && z != GameScaler._nHeight / 2)
            return true;

        if ((z == GameScaler._nHeight - max || z == min) && (x <= GameScaler._nWidth - max && x >= min) && x != GameScaler._nWidth  / 2)
            return true;

        return false;
    }
}
