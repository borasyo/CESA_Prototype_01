using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using UniRx;
using UniRx.Triggers;

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

    public enum eSandDir
    {
        VERTICAL = 0,
        HORIZONTAL,
        NONE,
    }
    public struct tData
    {        
        // 上下と左右があるため２つずつ用意する
        public SandItem.eType[] _type;
        public eSandDir[] _sandDir;    //  縦横どちらで挟んだか

        public void Init()
        {
            _type = new SandItem.eType[2];
            _sandDir = new eSandDir[2];
            for (int i = 0; i < 2; i++)
            {
                _type[i] = SandItem.eType.MAX;
                _sandDir[i] = eSandDir.NONE;
            }
        }
    };
    tData[] _SandDataList = null;      //  はさまれている箇所のリスト
    public tData[] GetSandDataList { get { return _SandDataList; } }

    public struct HalfSandData
    {
        // 上下と左右があるため２つずつ用意する
        public SandItem.eType[] _type;
        public Character.eDirection[] _dir;

        public void Init()
        {
            _type = new SandItem.eType[2];
            _dir = new Character.eDirection[2];
            for (int i = 0; i < 2; i++)
            {
                _type[i] = SandItem.eType.MAX;
                _dir[i]  = Character.eDirection.MAX;
            }
        }
    };
    HalfSandData[] _HalfSandDataList = null;   //  半分はさまれている箇所のリスト(重複も可)
    public HalfSandData[] GetHalfSandDataList { get { return _HalfSandDataList; } }
   
    [SerializeField] bool _IsSlope = true;
    [SerializeField] bool _IsOverLapToSafe = true;

    void Awake()
    {
        _SandDataList = new tData[GameScaler.GetRange];
        for (int i = 0; i < _SandDataList.Length; i++)
            _SandDataList[i].Init();

        _HalfSandDataList = new HalfSandData[GameScaler.GetRange];
        for (int i = 0; i < _HalfSandDataList.Length; i++)
            _HalfSandDataList[i].Init();

        StartCoroutine(StartUpdate());
    }

    IEnumerator StartUpdate()
    {
        Referee referee = GameObject.FindWithTag("Referee").GetComponent<Referee>();
        referee.enabled = false;

        //  Fieldの生成が終わるまで待つ
        yield return new WaitWhile(() => FieldData.Instance.IsStart == false);

        SandUpdate();

        this.UpdateAsObservable()
            .Where(_ => FieldData.Instance.ChangeField)
            .Subscribe(_ =>
            {
                SandUpdate();
            });

        referee.enabled = true;
    }

    void SandUpdate()
    {
        SandDataCheck();
        HalfSandDataCheck();
    }

    #region SandCheck

    void SandDataCheck()
    {
        //  初期化
        //_SandDataList.Clear();

        //  チェックする
        FieldObjectBase[] objDataArray = FieldData.Instance.GetObjDataArray;
        
        for (int number = 0; number < objDataArray.Length; number++)
        {
            for (int i = 0; i < _SandDataList[number]._type.Length; i++)
            {
                _SandDataList[number]._type[i] = SandItem.eType.MAX;
                _SandDataList[number]._sandDir[i] = eSandDir.NONE;
            }

            //  既に何か配置されていて、キャラクターではないなら
            if (objDataArray[number] && objDataArray[number].tag != "Character")
                continue;

            // はさまれていないかチェック(4パターン)
            Sand(FindSandObj(objDataArray, number, GameScaler._nWidth), number);       //  上下
            Sand(FindSandObj(objDataArray, number,                  1), number);       //  左右

            if (!_IsSlope)
                continue;

            Sand(FindSandObj(objDataArray, number, GameScaler._nWidth - 1), number);   //  左上右下
            Sand(FindSandObj(objDataArray, number, GameScaler._nWidth + 1), number);   //  右上左下
        }
    }

    //  判定を少し改善した。バグが出る可能性アリ
    tCheckData FindSandObj(FieldObjectBase[] objDataArray, int number, int add)
    {
        tCheckData checkData;
        checkData.first = checkData.second = null;
        int nRemRange = GameScaler._nSandRange + 1;

        int nRoopCnt = 1;
        while (nRemRange > 0)
        {
            int idx = number + (add * nRoopCnt);
            if (idx < 0 || GameScaler.GetRange <= idx)
                break;

            nRoopCnt++; 
            nRemRange--;
            checkData.first = objDataArray[idx];

            if (checkData.first && checkData.first.tag != "Character")
                break;

            checkData.first = null;
        }

        nRoopCnt = 1;
        while (nRemRange > 0)
        {
            int idx = number - (add * nRoopCnt);
            if (idx < 0 || GameScaler.GetRange <= idx)
                break;

            nRoopCnt++; 
            nRemRange--;
            checkData.second = objDataArray[idx];

            if (checkData.second && checkData.second.tag != "Character")
                break;

            checkData.second = null;
        }
        return checkData;
    }
     
    bool Sand(tCheckData checkData, int number)
    {
        if (!checkData.first || !checkData.second)
            return false;

        SandItem.eType type = SandItem.eType.MAX;

        if (checkData.first.tag == "Block")
        {
            if (checkData.second.tag == "Block")
                return false;

            SandItem sandItem = (SandItem)checkData.second;
            type = sandItem.GetType;
        }
        else
        {
            SandItem firstSandItem  = (SandItem)checkData.first;
            if (checkData.second.tag == "Block")
            {
                type = firstSandItem.GetType;
            }
            else
            {
                SandItem secondSandItem = (SandItem)checkData.second;
                if (firstSandItem.GetType != secondSandItem.GetType)
                    return false;

                type = firstSandItem.GetType;
            }
        }

        //  ※修正箇所※ はさまれているが重複しているので削除
        if (checkData.first.GetDataNumber() - number >= GameScaler._nWidth) // && _SandDataList[number]._type[0] != type)
        {
            _SandDataList[number]._type[0] = type;
            _SandDataList[number]._sandDir[0] = eSandDir.VERTICAL;
        }
        else
        {
            _SandDataList[number]._type[1] = type;
            _SandDataList[number]._sandDir[1] = eSandDir.HORIZONTAL;
        }
        return true;
    }

    #endregion

    #region HalfSandCheck

    void HalfSandDataCheck()
    {
        //_HalfSandDataList.Clear();

        FieldObjectBase[] objDataArray = FieldData.Instance.GetObjDataArray;
        for (int number = 0; number < objDataArray.Length; number++)
        {
            _HalfSandDataList[number]._type[0] = SandItem.eType.MAX;
            _HalfSandDataList[number]._type[1] = SandItem.eType.MAX;

            FieldObjectBase obj = objDataArray[number];
            if (obj && obj.tag != "Character")
                continue;

            //  すでに挟まれているか
            if (OnSand(number))
                continue;

            HalfSand(number, 1);
            HalfSand(number, GameScaler._nWidth);
        }

#if DEBUG
        /*Debug.Log("半分はさまれリスト");
        for(int i = 0; i < _HalfSandDataList.Length; i++)
        {
            Debug.Log("番号 : " + i + ", タイプ : " + _HalfSandDataList[i]._type[0] + ", 置く所 : " + _HalfSandDataList[i]._dir[0]);
            Debug.Log("番号 : " + i + ", タイプ : " + _HalfSandDataList[i]._type[1] + ", 置く所 : " + _HalfSandDataList[i]._dir[1]);
        }*/
#endif
    }

    bool OnSand(int number)
    {
        for (int i = 0; i < _SandDataList[number]._type.Length; i++)
        {
            if (_SandDataList[number]._type[i] == SandItem.eType.MAX)
                continue;

            return true;
        }

        return false;
    }

    bool HalfSand(int number, int add)
    {
        FieldObjectBase first = null;
        FieldObjectBase second = null;
        int idx = add == 1 ? 0 : 1; 

        int nRoopCnt = 0;
        int nRemRange = GameScaler._nSandRange + 1;
        while (nRemRange > 0)
        {
            int seacrhNumber = number + (add * nRoopCnt);
            if (seacrhNumber < 0 || GameScaler.GetRange <= seacrhNumber)
                break;

            nRoopCnt++;
            nRemRange--;
            first = FieldData.Instance.GetObjDataArray[seacrhNumber];

            if (first && first.tag != "Character")
                break;

            first = null;
        }

        nRoopCnt = 0;
        nRemRange = GameScaler._nSandRange + 1;
        while (nRemRange > 0)
        {
            int seacrhNumber = number - (add * nRoopCnt);
            if (seacrhNumber < 0 || GameScaler.GetRange <= seacrhNumber)
                break;

            nRoopCnt++;
            nRemRange--;
            second = FieldData.Instance.GetObjDataArray[seacrhNumber];

            if (second && second.tag != "Character")
                break;

            second = null;
        }

        //  二つ見つかるか(はさまれてはいるが違うのにはさまれている) or 何もない場合は失敗!
        if ((first && second) || (!first && !second))
            return false;

        //  追加
        FieldObjectBase obj = first ? first: second;
        if (obj.tag == "Block")
        {
            _HalfSandDataList[number]._type[idx] = SandItem.eType.BLOCK;
        }
        else
        {
            SandItem item = (SandItem)obj;
            _HalfSandDataList[number]._type[idx] = item.GetType;
        }

        //  はさむ位置をチェック
        if (first)
        {
            if (add == 1)
                _HalfSandDataList[number]._dir[idx] = Character.eDirection.LEFT;      //  左に置けばはさめる
            else
                _HalfSandDataList[number]._dir[idx] = Character.eDirection.BACK;      //  下に置けばはさめる
        }
        else
        {
            if (add == 1)
                _HalfSandDataList[number]._dir[idx] = Character.eDirection.RIGHT;     //  右に置けばはさめる
            else
                _HalfSandDataList[number]._dir[idx] = Character.eDirection.FORWARD;   //  上に置けばはさめる
        }

        return true;
    }

    #endregion
}