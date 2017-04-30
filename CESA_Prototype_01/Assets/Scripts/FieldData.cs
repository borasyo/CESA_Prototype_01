﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldData : MonoBehaviour
{
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

            GameObject obj = new GameObject();
            obj.AddComponent<FieldData>();
            Debug.Log(typeof(FieldData) + "が存在していないのに参照されたので生成");

            return instance;
        }
    }

    #endregion

    FieldObjectBase[] _ObjectDataArray = null;

    [SerializeField] int _nWidth = 12;
    [SerializeField] int _nHeight = 10;

    void Awake()
    {
        //  データ配列生成
        _ObjectDataArray = new FieldObjectBase[_nWidth * _nHeight];

        //  フィールドにオブジェクトを生成し、データを格納
        FieldCreator creator = new FieldCreator();
        _ObjectDataArray = creator.Create(_nWidth, _nHeight);

        DebugCheck();
    }

    //  データを格納
    public void SetObjData(FieldObjectBase objBase, int number)
    {
        _ObjectDataArray[number] = objBase;
    }

    //  データを取得
    public FieldObjectBase GetObjData(int number)
    {
        return _ObjectDataArray[number];
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
