﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using UniRx;
using UniRx.Triggers;

public class FieldData : Photon.MonoBehaviour
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

    protected bool _IsStart = false;
    public bool IsStart { get { return _IsStart; } private set { _IsStart = value; } }     //  FIeldを生成し終わっているかの判定  

    protected FieldObjectBase[] _ObjectDataArray = null;
    public FieldObjectBase[] GetObjDataArray { get { return _ObjectDataArray; } }

    #region ChangeData
    protected struct tChangeData
    {
        public bool _IsChange;
        public FieldObjectBase _obj;

        public void Set(FieldObjectBase obj)
        {
            _IsChange = true;
            _obj = obj;
        }
    };
    protected tChangeData[] _ChangeDataList = null;  //  Field情報に変更があった場合、その情報を一時保存する
    #endregion

    bool _IsChangeField = false;            //  Fieldに変更があったか
    public bool ChangeField { get { return _IsChangeField; } }
    bool _IsChangeFieldWithChara = false;   //  キャラを含めたFieldに変更があったか
    public bool ChangeFieldWithChara { get { return _IsChangeFieldWithChara; } }
    bool _IsExceptionChangeField = false;
    public void ExceptionChangeField() { _IsExceptionChangeField = true; }

    protected List<Character> _CharaList = new List<Character>();
    public List<Character> GetCharactors { get { return _CharaList; } }
    public List<Character> GetCharactorsNonMe(GameObject obj) { return _CharaList.Where(x => x && x.gameObject != obj).ToList(); }    // 自分を除いたキャラにリストを返す
    public void ResetCharactors() { _CharaList.Clear(); }

    #region Init

    void Awake()
    {
        //  スケールに合わせて拡大
        // TODO : よくわからない
        if (RoundCounter.nRoundCounter.Where(count => count == 0).ToList().Count == 4)
        {
            GameScaler._nWidth = StageScaler.GetWidth(); // (int)(GameScaler._nBaseWidth * StageScaler.GetMagni());
            GameScaler._nHeight = StageScaler.GetHeight(); // (int)(GameScaler._nBaseHeight * StageScaler.GetMagni());
        }

        if(Camera.main.GetComponent<SetCameraPos>())
            Camera.main.GetComponent<SetCameraPos>().Init();

        SandItem._IsTutorial = false;
        Init();
    }

    protected virtual void Init()
    {
        //  データ配列生成
        _ObjectDataArray = new FieldObjectBase[GameScaler._nWidth * GameScaler._nHeight];
        _ChangeDataList = new tChangeData[GameScaler._nWidth * GameScaler._nHeight];

        //  フィールドにオブジェクトを生成し、データを格納
        FieldCreator creator = new FieldCreator(); 
        creator.Create();

        UpdateStart();
        StartCoroutine(CharaSet());

        GameObject readyGo = Resources.Load<GameObject>("Prefabs/GameMain/ReadyGo");
        Instantiate(readyGo, readyGo.transform.position, Quaternion.identity);
    }

    protected void UpdateStart()
    {
        _IsStart = true;    //  生成終了

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                //  Field情報に変更があったかをチェック
                _IsChangeField = _IsChangeFieldWithChara = false;

                if (_IsExceptionChangeField)
                {
                    _IsExceptionChangeField = false;
                    _IsChangeField = _IsChangeFieldWithChara = true;
                    return;
                }

                for (int i = 0; i < _ChangeDataList.Length; i++)
                {
                    if (!_ChangeDataList[i]._IsChange)
                        continue;

                    FieldObjectBase obj = _ObjectDataArray[i];
                    FieldObjectBase oldObj = _ChangeDataList[i]._obj;
                    if (obj != oldObj)
                    {
                        _IsChangeFieldWithChara = true;

                        if (!_IsChangeField &&
                           (!obj || obj.tag != "Character") &&
                           (!oldObj || oldObj.tag != "Character"))    //  キャラの場合は変更しない
                            _IsChangeField = true;
                    }
                    _ChangeDataList[i]._IsChange = false;
                }
            });

        //  歩行音をチェック
        bool isWalk = false;
        this.ObserveEveryValueChanged(_ =>
        {
            if(Time.timeScale <= 0.0f)
            {
                isWalk = false;
                return isWalk;
            }

            // isWalkが変化したら音も変える
            foreach (Character chara in _CharaList)
            {
                if (!chara)
                    continue;

                isWalk = chara.GetAnimator.GetBool("Walk");
                if (!isWalk)
                    continue;

                break;
            }
            return isWalk;
        }).Subscribe(_ =>
            {
                if(isWalk)
                {
                    SoundManager.Instance.PlayBGM(SoundManager.eBgmValue.WALK);
                }
                else
                {
                    SoundManager.Instance.StopBGM(SoundManager.eBgmValue.WALK);
                }
            });
    }

    #endregion

    //  データを格納
    public void SetObjData(FieldObjectBase setObj, int number)
    {
        _ChangeDataList[number].Set(_ObjectDataArray[number]);  //  変更前の情報を一時退避
        _ObjectDataArray[number] = setObj;
    }

    //  データを取得
    public FieldObjectBase GetObjData(int number)
    {
        if (0 > number || number > GameScaler._nWidth * GameScaler._nHeight)
            return null;

        return _ObjectDataArray[number];
    }

    protected virtual IEnumerator CharaSet()
    {
        GameObject[] charas = GameObject.FindGameObjectsWithTag("Character");

        foreach (GameObject obj in charas)
            _CharaList.Add(obj.GetComponent<Character>());

        yield return null;
    }

    //  キャラクターを取得する時のみ使用する
    public FieldObjectBase GetCharaData(string name)
    {
        return _ObjectDataArray.Where(_ => _ && _.tag == "Character" && _.name.Contains(name)).First();
    }

    public Vector3 GetNonObjPos()
    {
        List<int> nullMassList = new List<int>();
        for (int i = 0; i < _ObjectDataArray.Length; i++)
        {
            if (_ObjectDataArray[i])
                continue;

            nullMassList.Add(i);
        }

        if (nullMassList.Count <= 0)
            return -Vector3.one;

        return GetPosForNumber(nullMassList[Random.Range(0, nullMassList.Count)]);
    }

    public Vector3 GetNonObjPosForMostLeave()
    {
        List<int> nullMassList = new List<int>();
        for (int i = 0; i < _ObjectDataArray.Length; i++)
        {
            if (_ObjectDataArray[i])
                continue;

            nullMassList.Add(i);
        }

        if (nullMassList.Count <= 0)
            return -Vector3.one;

        Vector2 center = new Vector3(GameScaler._nWidth / 2, GameScaler._nHeight / 2);
        int idx = 0;
        int minDistance = 1000;
        for(int i = 0; i < nullMassList.Count; i++)
        {
            int x = nullMassList[i] % GameScaler._nWidth;
            int y = nullMassList[i] / GameScaler._nWidth;
            //int distance = (int)Mathf.Abs(center.x - x) + (int)Mathf.Abs(center.y - y);
            Vector2 distance = new Vector2(center.x < x ? (GameScaler._nWidth - 1) - x : x, center.y < y ? (GameScaler._nHeight - 1) - y : y);
            int min = (int)(distance.x < distance.y ? distance.x : distance.y);

            if (min >= minDistance)
                continue;

            minDistance = min;
            idx = nullMassList[i];
        }
        return GetPosForNumber(idx);
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
