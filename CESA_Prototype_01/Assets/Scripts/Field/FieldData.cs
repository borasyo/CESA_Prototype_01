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
    public bool ChangeField { get { return _IsChangeField; } }
    bool _IsChangeFieldWithChara = false;   //  キャラを含めたFieldに変更があったか
    public bool ChangeFieldWithChara { get { return _IsChangeFieldWithChara; } }

    List<FieldObjectBase> _CharaList = new List<FieldObjectBase>();
    public List<FieldObjectBase> GetCharactors { get { return _CharaList; } } 

    #region Init

    void Awake()
    {
        //  データ配列生成
        _ObjectDataArray = new FieldObjectBase[GameScaler._nWidth * GameScaler._nHeight];

        //  フィールドにオブジェクトを生成し、データを格納
        FieldCreator creator = new FieldCreator();
        _ObjectDataArray = creator.Create(GameScaler._nWidth, GameScaler._nHeight);

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
        _IsChangeField = _IsChangeFieldWithChara = false;
    }

    //  データを格納
    public void SetObjData(FieldObjectBase setObj, int number)
    {
        FieldObjectBase obj = _ObjectDataArray[number];
        if (obj != setObj)
        {
            _IsChangeFieldWithChara = true;

            if (!_IsChangeField &&
               (!obj || obj.tag != "Charactor") &&
               (!setObj || setObj.tag != "Charactor"))    //  キャラの場合は変更しない
                _IsChangeField = true;
        }

        _ObjectDataArray[number] = setObj;
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
