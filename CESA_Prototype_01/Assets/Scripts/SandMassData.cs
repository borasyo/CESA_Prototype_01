using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandMassData : MonoBehaviour
{
    ///<summary>
    /// 
    /// はさんでいる位置を表示するオブジェクトのデータ配列
    /// 
    /// </summary>

    #region Singleton

    private static SandMassData instance;

    public static SandMassData Instance
    {
        get
        {
            if (instance)
                return instance;

            instance = (SandMassData)FindObjectOfType(typeof(SandMassData));

            if (instance)
                return instance;

            GameObject obj = new GameObject("SandMassData");
            obj.AddComponent<SandMassData>();
            Debug.Log(typeof(SandMassData) + "が存在していないのに参照されたので生成");

            return instance;
        }
    }

    #endregion

    SandMass[] _ObjectDataArray = null;
    public SandMass[] GetObjDataArray { get { return _ObjectDataArray; } }

    void Awake()
    {
        //  データ配列生成
        _ObjectDataArray = new SandMass[GameScaler._nWidth * GameScaler._nHeight];

        //  フィールドにオブジェクトを生成し、データを格納
        SandMassCreator creator = new SandMassCreator();
        _ObjectDataArray = creator.Create(GameScaler._nWidth, GameScaler._nHeight);

        //DebugCheck();
    }

    //  データを格納
    public void SetObjData(SandMass objBase, int number)
    {
        _ObjectDataArray[number] = objBase;
    }

    //  データを取得
    public SandMass GetObjData(int number)
    {
        if (0 > number || number > GameScaler._nWidth * GameScaler._nHeight)
            return null;

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
