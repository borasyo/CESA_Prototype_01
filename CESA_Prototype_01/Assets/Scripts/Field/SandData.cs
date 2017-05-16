using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    public struct tCheckData
    {
        public FieldObjectBase first;
        public FieldObjectBase second;
    };
    public struct tSandData
    {
        public int _number;
        public SandItem.eType _Type;
    };
    List<tSandData> _SandDataList = new List<tSandData>();   //  はさまれている箇所のリスト
    public List<tSandData> GetSandDataList { get { return _SandDataList; } }

    [SerializeField] bool _IsSlope = true;
    [SerializeField] bool _IsOverLapToSafe = true;

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
            SandCheck(FindSandObj(objDataArray, number, GameScaler._nWidth), number);   //  上下
            SandCheck(FindSandObj(objDataArray, number, 1), number);                    //  左右

            if (!_IsSlope)
                continue;
            
            SandCheck(FindSandObj(objDataArray, number, GameScaler._nWidth - 1), number);   //  左上右下
            SandCheck(FindSandObj(objDataArray, number, GameScaler._nWidth + 1), number);   //  右上左下
        }

        if (!_IsOverLapToSafe)
            return;
        
        OverLapDistinct();
    }

    // TODO : 処理効率が懸念材料....
    void OverLapDistinct()
    {
        for(int i = 0; i < GameScaler._nWidth * GameScaler._nHeight; i++) {
            List<tSandData> overLapList = _SandDataList.Where(_ => _._number == i).ToList();
            if (overLapList.Count <= 1)
                continue;

            // 全て同じタイプなら削除しない
            bool isUnique = false;
            for (int type = 0; type < (int)SandItem.eType.MAX; type++)
            {
                if (overLapList.Count != _SandDataList.Where(_ => _._number == i && _._Type == (SandItem.eType)type).Count())
                    continue;

                isUnique = true;
            }
            if(isUnique)
                continue;
            
            _SandDataList.RemoveAll(x => x._number == i);
        }
    }

    tCheckData FindSandObj(FieldObjectBase[] objDataArray, int number, int add)
    {
        tCheckData checkData;
        checkData.first = checkData.second = null;
        int nRemRange = GameScaler._nSandRange + 1;

        int nRoopCnt = 0;
        while (nRemRange > 0)
        {
            nRoopCnt++; 
            nRemRange--;
            checkData.first = objDataArray[number + (add * nRoopCnt)];

            if (!checkData.first)
                continue;

            if (checkData.first.tag == "Charactor")
                continue;

            break;
        }

        nRoopCnt = 0;
        while (nRemRange > 0)
        {
            nRoopCnt++; 
            nRemRange--;
            checkData.second = objDataArray[number - (add * nRoopCnt)];

            if (!checkData.second)
                continue;

            if (checkData.second.tag == "Charactor")
                continue;
            
            break;
        }

        return checkData;
    }
     
    bool SandCheck(tCheckData checkData, int number)
    {
        if (!checkData.first || !checkData.second)
            return false;

        if (checkData.first.tag == "Charactor" || checkData.second.tag == "Charactor")
            return false;

        tSandData sandData = new tSandData();

        if (checkData.first.tag == "Block")
        {
            if (checkData.second.tag == "Block")
                return false;

            SandItem sandItem = (SandItem)checkData.second;
            sandData._Type = sandItem.GetType;
        }
        else
        {
            SandItem firstSandItem  = (SandItem)checkData.first;
            if (checkData.second.tag == "Block")
            {
                sandData._Type = firstSandItem.GetType;
            }
            else
            {
                SandItem secondSandItem = (SandItem)checkData.second;
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