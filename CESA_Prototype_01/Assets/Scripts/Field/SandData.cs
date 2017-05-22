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
    public struct tHalfSandData
    {
        public tSandData _data;
        public Charactor.eDirection _dir; 
    };
    List<tSandData> _SandDataList = new List<tSandData>();               //  はさまれている箇所のリスト
    public List<tSandData> GetSandDataList { get { return _SandDataList; } }

    List<tHalfSandData> _HalfSandDataList = new List<tHalfSandData>();   //  半分はさまれている箇所のリスト(重複も可)
    public List<tHalfSandData> GetHalfSandDataList { get { return _HalfSandDataList; } }
    

    [SerializeField] bool _IsSlope = true;
    [SerializeField] bool _IsOverLapToSafe = true;

    void Start()
    {
        SandUpdate();
    }

    void Update()
    {
        //  Fieldに変化があったかを確認
        if (!FieldData.Instance.ChangeField)
            return;

        SandUpdate();
    }

    void SandUpdate()
    {
        SandDataCheck();
        HalfSandDataCheck();

        if (!_IsOverLapToSafe)
            return;

        OverLapDistinct();
    }

    #region SandCheck

    void SandDataCheck()
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
            Sand(FindSandObj(objDataArray, number, GameScaler._nWidth), number);       //  上下
            Sand(FindSandObj(objDataArray, number, 1), number);                        //  左右

            if (!_IsSlope)
                continue;

            Sand(FindSandObj(objDataArray, number, GameScaler._nWidth - 1), number);   //  左上右下
            Sand(FindSandObj(objDataArray, number, GameScaler._nWidth + 1), number);   //  右上左下
        }
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

    //  判定を少し改善した。バグが出る可能性アリ
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

            if (checkData.first && checkData.first.tag != "Charactor")
                break;

            checkData.first = null;
        }

        nRoopCnt = 0;
        while (nRemRange > 0)
        {
            nRoopCnt++; 
            nRemRange--;
            checkData.second = objDataArray[number - (add * nRoopCnt)];

            if (checkData.second && checkData.second.tag != "Charactor")
                break;

            checkData.second = null;
        }

        return checkData;
    }
     
    bool Sand(tCheckData checkData, int number)
    {
        if (!checkData.first || !checkData.second)
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

    #endregion

    #region HalfSandCheck

    void HalfSandDataCheck()
    {
        _HalfSandDataList.Clear();

        FieldObjectBase[] objDataArray = FieldData.Instance.GetObjDataArray;
        for (int number = 0; number < objDataArray.Length; number++)
        {
            if (objDataArray[number] && objDataArray[number].tag != "Charactor")
                continue;

            //  すでに挟まれているか
            if (OnSand(number))
                continue;

            HalfSand(number, 1);
            HalfSand(number, GameScaler._nWidth);
        }

        Debug.Log("半分はさまれリスト");
        for(int i = 0; i < _HalfSandDataList.Count; i++)
        {
            Debug.Log("番号 : " + _HalfSandDataList[i]._data._number + ", タイプ : " + _HalfSandDataList[i]._data._Type + ", 置く所 : " + _HalfSandDataList[i]._dir);
        }
    }

    bool OnSand(int number)
    {
        foreach (tSandData data in _SandDataList)
        {
            if (data._number != number)
                continue;

            return true;
        }

        return false;
    }
    bool HalfSand(int number, int add)
    {
        FieldObjectBase first = null;
        FieldObjectBase second = null;

        int nRoopCnt = 0;
        int nRemRange = GameScaler._nSandRange;
        while (nRemRange > 0)
        {
            nRoopCnt++;
            nRemRange--;
            first = FieldData.Instance.GetObjDataArray[number + (add * nRoopCnt)];

            if (first && first.tag != "Charactor")
                break;

            first = null;
        }

        nRoopCnt = 0;
        nRemRange = GameScaler._nSandRange;
        while (nRemRange > 0)
        {
            nRoopCnt++;
            nRemRange--;
            second = FieldData.Instance.GetObjDataArray[number - (add * nRoopCnt)];

            if (second && second.tag != "Charactor")
                break;

            second = null;
        }

        //  二つ見つかるか(はさまれてはいるが違うのにはさまれている) or 何もない場合は失敗!
        if ((first && second) || (!first && !second))
            return false;

        //  追加
        tHalfSandData halfSandData = new tHalfSandData();
        halfSandData._data._number = number;
        FieldObjectBase obj = first ? first: second;
        if (obj.tag == "Block")
        {
            halfSandData._data._Type = SandItem.eType.BLOCK;
        }
        else
        {
            SandItem item = (SandItem)obj;
            halfSandData._data._Type = item.GetType;
        }

        //  はさむ位置をチェック
        if (first)
        {
            if (add == 1)
                halfSandData._dir = Charactor.eDirection.LEFT;      //  左に置けばはさめる
            else
                halfSandData._dir = Charactor.eDirection.BACK;      //  下に置けばはさめる
        }
        else
        {
            if (add == 1)
                halfSandData._dir = Charactor.eDirection.RIGHT;     //  右に置けばはさめる
            else
                halfSandData._dir = Charactor.eDirection.FORWARD;   //  上に置けばはさめる
        }
        _HalfSandDataList.Add(halfSandData);

        return true;
    }

    #endregion
}