using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FieldData : MonoBehaviour
{
    ///<summary>
    /// 
    /// フィールドのデータ配列
    /// 何も配置されていない場合はnullが入っている
    /// 
    /// </summary>

    #region Singleton

    private static FieldData instance;

    public static FieldData Instance
    {
        get
        {
            if (instance)
                return instance;

            instance = (FieldData)FindObjectOfType(typeof(FieldData));

            if (instance)
                return instance;

            GameObject obj = new GameObject("FieldData");
            obj.AddComponent<FieldData>();
            Debug.Log(typeof(FieldData) + "が存在していないのに参照されたので生成");

            return instance;
        }
    }

    #endregion

    FieldObjectBase[] _ObjectDataArray = null;
    public FieldObjectBase[] GetObjDataArray { get { return _ObjectDataArray; } }

    bool _IsChangeField = false;            //  Fieldに変更があったか
    bool _IsChangeFieldWithChara = false;   //  キャラを含めたFieldに変更があったか
    FieldObjectBase[] _OldObjectDataArray = null;

    List<FieldObjectBase> _CharaList = new List<FieldObjectBase>();
    public List<FieldObjectBase> GetCharactors { get { return _CharaList; } } 

    #region Init

    void Awake()
    {
        //  データ配列生成
        _ObjectDataArray = _OldObjectDataArray = new FieldObjectBase[GameScaler._nWidth * GameScaler._nHeight];

        //  フィールドにオブジェクトを生成し、データを格納
        FieldCreator creator = new FieldCreator();
        _ObjectDataArray = _OldObjectDataArray = creator.Create(GameScaler._nWidth, GameScaler._nHeight);
      
        CreateCharaList();
    }

    void CreateCharaList()
    {
        //  キャラデータのリストを作成
        foreach (FieldObjectBase obj in _ObjectDataArray)
        {
            if (!obj)
                continue;

            if (obj.tag != "Charactor")
                continue;

            GetCharactors.Add(obj);
        }
    }

    #endregion

    void Update()
    {
        //  Fieldに変化があったかをチェック
        _IsChangeFieldWithChara = !_ObjectDataArray.SequenceEqual(_OldObjectDataArray);

        //  Charaを除いたFieldに変化があったかをチェック
        for(int i = 0; i < _ObjectDataArray.Length; i++)
        {
            if (!_ObjectDataArray[i])
                continue;

            if (_ObjectDataArray[i].tag == "Charactor" || _OldObjectDataArray[i].tag == "Charactor")
                continue;

            if (_ObjectDataArray[i] == _OldObjectDataArray[i])
                continue;

            _IsChangeField = true;
            break;
        }
        Debug.Log("ChangeField : " + _IsChangeField);
        Debug.Log("ChangeFieldWithChara : " + _IsChangeFieldWithChara);

        //  保存
        _OldObjectDataArray = _ObjectDataArray;
    }

    //  データを格納
    public void SetObjData(FieldObjectBase objBase, int number)
    {
        _ObjectDataArray[number] = objBase;
    }

    //  データを取得
    public FieldObjectBase GetObjData(int number)
    {
        if (0 > number || number > GameScaler._nWidth * GameScaler._nHeight)
            return null;

        return _ObjectDataArray[number];
    }

    //  キャラクターを取得する時のみ使用する
    public FieldObjectBase GetCharaData(string name)
    {
        foreach (FieldObjectBase obj in _ObjectDataArray)
        {
            if (!obj)
                continue;

            if (obj.tag != "Charactor")
                continue;

            if (!obj.name.Contains(name))
                continue;

            return obj; 
        }

        return null;    //  失敗
    }

    public Vector3 GetNonObjPos()
    {
        Vector3 pos = Vector3.zero;
        while (true)
        {
            int number = Random.Range(0, _ObjectDataArray.Length);
            FieldObjectBase obj = GetObjData(number);
        
            if (obj)
                continue;

            pos = GetPosForNumber(number);
            break;
        }
        
        return pos;
    }

    Vector3 GetPosForNumber(int number)
    {
        float x, z;
        x = (float)((number % GameScaler._nWidth) * GameScaler._fScale);
        z = (float)((number / GameScaler._nWidth) * GameScaler._fScale);

        return new Vector3(x,0,z);
    }

    #if DEBUG

    void DebugCheck()
    {
        for (int mass = 0; mass < _ObjectDataArray.Length; mass++)
        {
            if (!_ObjectDataArray[mass])
            {
                //Debug.Log("空");
                Debug.Log(mass + "は空");
                continue;
            }

            //Debug.Log(_ObjectDataArray[i].name);
            Debug.Log(mass + "は" + _ObjectDataArray[mass].name);
        }
    }

    #endif
}
