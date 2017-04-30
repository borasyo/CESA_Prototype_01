using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandData : MonoBehaviour
{    
    ///<summary>
    /// 
    /// どのマスがはさまれているかを保持する
    /// 
    /// </summary>

    #region Singleton

    private static SandData instance;

    public static SandData Instance
    {
        get
        {
            if (instance)
                return instance;

            instance = (SandData)FindObjectOfType(typeof(SandData));

            if (instance)
                return instance;

            GameObject obj = new GameObject();
            obj.AddComponent<SandData>();
            Debug.Log(typeof(SandData) + "が存在していないのに参照されたので生成");

            return instance;
        }
    }

    #endregion

    public struct tSandData
    {
        public int _number;
        public SandItem.eType _Type;
    };
    List<tSandData> _SandDataList = new List<tSandData>();   //  はさまれている箇所のリスト
    public List<tSandData> GetSandDataList { get { return _SandDataList; } }

    void Update()
    {
        //  初期化
        _SandDataList.Clear();

        //  チェックする
        FieldObjectBase[] objDataArray = FieldData.Instance.GetObjDataArray;
        for (int number = 0; number < objDataArray.Length; number++)
        {
            //  既に何か配置されていて、キャラクターではないなら
            if (objDataArray[number] && objDataArray[number].tag != "Charactor")
                continue;

            // はさまれていないかチェック(4パターン)
            FieldObjectBase first; 
            FieldObjectBase second;

            first  = objDataArray[number + GameScaler._nWidth - 1];   //  左上
            second = objDataArray[number - GameScaler._nWidth + 1];   //  右下
            if (SandCheck(first, second, number))
                continue;

            first  = objDataArray[number + GameScaler._nWidth];   //  上
            second = objDataArray[number - GameScaler._nWidth];   //  下
            if (SandCheck(first, second, number))
                continue;

            first  = objDataArray[number + GameScaler._nWidth + 1];   //  右上
            second = objDataArray[number - GameScaler._nWidth - 1];   //  左下
            if (SandCheck(first, second, number))
                continue;

            first  = objDataArray[number + 1];   //  右
            second = objDataArray[number - 1];   //  左
            if (SandCheck(first, second, number))
                continue;
        }
    }
     
    bool SandCheck(FieldObjectBase first, FieldObjectBase second, int number)
    {
        if (!first || !second)
            return false;

        if (first.tag == "Charactor" || second.tag == "Charactor")
            return false;

        tSandData sandData = new tSandData();

        if (first.tag == "Block")
        {
            if (second.tag == "Block")
                return false;

            SandItem sandItem = (SandItem)second;
            sandData._Type = sandItem.GetType;
        }
        else
        {
            SandItem firstSandItem  = (SandItem)first;
            if (second.tag == "Block")
            {
                sandData._Type = firstSandItem.GetType;
            }
            else
            {
                SandItem secondSandItem = (SandItem)second;
                if (firstSandItem.GetType != secondSandItem.GetType)
                    return false;

                sandData._Type = firstSandItem.GetType;
            }
        }

        sandData._number = number;
        _SandDataList.Add(sandData);
        return true;
    }
}