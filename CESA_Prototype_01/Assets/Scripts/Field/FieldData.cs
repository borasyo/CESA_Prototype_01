using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Awake()
    {
        //  データ配列生成
        _ObjectDataArray = new FieldObjectBase[GameScaler._nWidth * GameScaler._nHeight];

        //  フィールドにオブジェクトを生成し、データを格納
        FieldCreator creator = new FieldCreator();
        _ObjectDataArray = creator.Create(GameScaler._nWidth, GameScaler._nHeight);

        //DebugCheck();
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
        for (int i = 0; i < _ObjectDataArray.Length; i++)
        {
            if (!_ObjectDataArray[i])
                continue;

            if (_ObjectDataArray[i].tag != "Charactor")
                continue;

            if (!_ObjectDataArray[i].name.Contains(name))
                continue;

            return _ObjectDataArray[i]; 
        }

        return null;    //  失敗
    }

    #if DEBUG

    void DebugCheck()
    {
        for (int i = 0; i < _ObjectDataArray.Length; i++)
        {
            if (!_ObjectDataArray[i])
            {
                //Debug.Log("空");
                Debug.Log(i + "は空");
                continue;
            }

            //Debug.Log(_ObjectDataArray[i].name);
            Debug.Log(i + "は" + _ObjectDataArray[i].name);
        }
    }

    #endif
}
