using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;
using System.Linq;

public class EnemyAI : MonoBehaviour 
{
    public class AIInput
    { 
        public AIInput()
        {
            _direction = Charactor.eDirection.MAX;
            _action    = Charactor.eAction.MAX;
        }

        public Charactor.eDirection _direction;
        public Charactor.eAction    _action;
    };
    
    AIInput _nowInput = new AIInput();

    public struct tDistanceData
    {
        public int _nIdx;
        public int _nDistance;
    };
    public tDistanceData[] _DistanceDatas { get; private set; }    //  各マスの自分から見た移動距離を保持する
    Charactor _charactor = null;
    FieldObjectBase _fieldObjBase = null;

    public enum eState
    {
        WAIT = 0,
        WALK,
        PUT,
        BREAK,

        MAX,
    }
    eState _state = eState.WAIT;
    public eState GetState { get { return _state; } }

    //  AI選択用メソッド
    int _nLevel = 0;                                        //  低いほど頭が良くなる (0 : 強, 1 : 中, 2 : 弱)
    bool _IsDanger = false;                                 //  危険状態かどうか trueの時AI変更
    int[] _nActionRatio        = new int[(int)eState.MAX];  //  通常時行動比率 (合計で100になるようにする)
    int[] _nSpecialActionRatio = new int[(int)eState.MAX];  //  特殊時行動比率 (合計で100になるようにする)

    //  各行動AI
    MoveAI _moveAI = null;
    PutAI _putAI = null;
    BreakAI _breakAI = null;

    #region Init

    //  キャラのタイプ、CPUのレベルによって設定する情報
    public void Set(int level, Charactor.eCharaType type)
    {
        _nLevel = level;
        _charactor = GetComponent<Charactor>();
        _fieldObjBase = GetComponent<FieldObjectBase>();

        _nActionRatio = AIData.Instance.GetRatio(level, type, false);
        _nSpecialActionRatio = AIData.Instance.GetRatio(level, type, true);
        AIData.RiskData riskData = AIData.Instance.GetRisk(level, type);
        SetRiskData(riskData.maxRisk, riskData.riskRange);

        Debug.Log(this.name + "は");
        Debug.Log("Wait : " + _nActionRatio[0] + ", Walk : " + _nActionRatio[1] + ", Put : " + _nActionRatio[2] + ", Break : " + _nActionRatio[3]);
        Debug.Log("SpecialWait : " + _nSpecialActionRatio[0] + ", SpecialWalk : " + _nSpecialActionRatio[1] + ", SpecialPut : " + _nSpecialActionRatio[2] + ", SpecialBreak : " + _nSpecialActionRatio[3]);
        Debug.Log("許容リスク : " + riskData.maxRisk + ", 探索範囲 : " + riskData.riskRange);

        // 各行動AIを生成
        if(_charactor._charaType != Charactor.eCharaType.TECHNICAL)
            _moveAI = gameObject.AddComponent<MoveAI>();
        else
            _moveAI = gameObject.AddComponent<TechnicalTypeMoveAI>();
        _moveAI.Init(level, type);
        _putAI = gameObject.AddComponent<PutAI>();
        _putAI.Init(level);
        _breakAI = gameObject.AddComponent<BreakAI>();
        _breakAI.Init(level);
    }

    // Use this for initialization
    void Start()
    {
        // State.WAIT
        this.UpdateAsObservable()
            .Where(_ => _state == eState.WAIT && !_IsDanger)
            .Subscribe(_ =>
            {
                AIUpdate();
            });

        // State.WALK
        this.UpdateAsObservable()
            .Where(_ => _state == eState.WALK)
            .Subscribe(_ =>
            {
                if (MoveUpdate())
                    return;

                _state = eState.WAIT;
            });

        // State.PUT
        this.UpdateAsObservable()
            .Where(_ => _state == eState.PUT)
            .Subscribe(_ =>
            {
                if (MoveUpdate())
                    return;

                _nowInput._action = Charactor.eAction.PUT;
                _state = eState.WAIT;
            });

        // State.BREAK
        this.UpdateAsObservable()
            .Where(_ => _state == eState.BREAK)
            .Subscribe(_ =>
            {
                if (MoveUpdate())
                    return;

                _nowInput._action = Charactor.eAction.BREAK;
                _state = eState.WAIT;
            });

        //  DistanceData
        _DistanceDatas = new tDistanceData[GameScaler._nWidth * GameScaler._nHeight];
        for (int idx = 0; idx < _DistanceDatas.Length; idx++)
            _DistanceDatas[idx]._nIdx = idx;

        int oldNumber = -1;
        this.UpdateAsObservable()
            .Subscribe(_DistanceDatas =>
            {
                if (oldNumber == _fieldObjBase.GetDataNumber())
                {
                    oldNumber = _fieldObjBase.GetDataNumber();
                    return;
                }

                CheckDistanceData();
                oldNumber = _fieldObjBase.GetDataNumber();
            });
    }

    #endregion

    #region AI

    bool AIUpdate(bool isDanger = false)
    {
        bool isResult = false;
        _state = GetNextState(isDanger);
        //Debug.Log("AI : " + _state);
        switch (_state)
        {
            case eState.WAIT:
                break;
            case eState.WALK:
                isResult = _moveAI.OnMove();
                //Debug.Log("Walk");
                break;
            case eState.PUT:
                isResult = _putAI.OnPut();
                //Debug.Log("Put");
                break;
            case eState.BREAK:
                isResult = _breakAI.OnBreak();
                //Debug.Log("Break");
                break;
        }

        if(!isResult)
            _state = eState.WAIT;

        return isResult;
    }

    eState GetNextState(bool isDanger)
    {
        eState next = eState.WAIT;

        // Debug
        if (isDanger)
            return eState.WALK; // Random.Range(0, 2) == 0 ? eState.WALK : eState.BREAK;
        else
            return eState.WALK; // (eState)Random.Range(0, (int)eState.MAX);

        int nRand = 0;
        int[] ratio = _charactor.GetSpecialModeFlg ? _nSpecialActionRatio : _nActionRatio;
        if (isDanger)
        {
            nRand = Random.Range(0, ratio[(int)eState.WALK] + ratio[(int)eState.BREAK]);
            if (nRand <= ratio[(int)eState.WALK])
                next = eState.WALK;
            else
                next = eState.BREAK;
        }
        else
        {
            nRand = Random.Range(0, ratio.Sum());
            for (int i = 0; i < ratio.Length; i++)
            {
                nRand -= ratio[i];
                if (nRand > 0)
                    continue;
                next = (eState)i;
                break;
            }
        }
        return next;
    }

    # endregion

    #region Move

    bool MoveUpdate()
    {
        // 移動終了か行動不能で次のAIへ
        if (!_moveAI.StateWalk || _moveAI.StateStop)
            return false;

        _nowInput._direction = _moveAI.GetMoveData();
        return true;
    }

    #endregion

    #region DistanceData
    /// <summary>
    /// 自分の位置から何マスでそのマスまで行けるかを保持
    /// </summary>

    void CheckDistanceData()
    {
        int nowNumber = _fieldObjBase.GetDataNumber();
       
        for (int number = 0; number < _DistanceDatas.Length; number++)
        {
            int step = 0;
            int distance = Mathf.Abs(nowNumber - number);

            while (distance >= GameScaler._nWidth)
            {
                distance -= GameScaler._nWidth;
                step++;
            }

            //  左上と右下のマスには補正をかける
            if (CheckRivRange(nowNumber, number))
                distance -= (GameScaler._nWidth + 1);

            _DistanceDatas[number]._nDistance = step + Mathf.Abs(distance);
        }
    }

    bool CheckRivRange(int me, int opp)
    {
        int meHeight  = me  / GameScaler._nWidth;
        int oppHeight = opp / GameScaler._nWidth;
        if (meHeight == oppHeight)
            return false;

        int meWidth  = me  % GameScaler._nWidth;
        int oppWidth = opp % GameScaler._nWidth;
        if (meWidth == oppWidth)
            return false;

        if (meHeight > oppHeight)
        {
            if (meWidth > oppWidth)
                return false;
        }
        else if (meHeight < oppHeight)
        {
            if (meWidth < oppWidth)
                return false;
        }

        return true;
    }

    #endregion

    #region RiskCheck 

    //  キャラタイプとCPUレベルによって変える
    public void SetRiskData(int maxRisk, int riskRange)
    {
        FieldData.Instance.ExceptionChangeField();  // 開始1F時は計算する

        this.UpdateAsObservable()
            .Where(_ => _IsDanger)
            .Subscribe(_ =>
            {
                //  成功するまで繰り返す
                _IsDanger = !AIUpdate(true);
            });

        //  RiskCheck
        this.UpdateAsObservable()
            .Where(_ => FieldData.Instance.ChangeFieldWithChara)
            .Subscribe(_ =>
            {
                int risk = 0;

                //  自分の位置のリスク計算
                risk += RiskCheck(null, _fieldObjBase.GetDataNumber());

                //  周りの状態からリスクを追加
                List<tDistanceData> distanceList = _DistanceDatas.Where(x => x._nDistance > 0 && x._nDistance <= riskRange).ToList();
                string debug = " = ";
                foreach (tDistanceData data in distanceList)
                {
                    //  マスの状態と近さでリスク計算
                    int add = RiskCheck(FieldData.Instance.GetObjData(data._nIdx), data._nIdx); // / data._nDistance;
                    debug += add + " + ";
                    risk += add;

                    if (risk < maxRisk)
                        continue;

                    // AI強制切り替え (歩行or破壊のどちらかで)
                    _IsDanger = true;
                    //Debug.Log("現在リスク : " + risk + debug);
                }
            });
    }

    protected virtual int RiskCheck(FieldObjectBase obj, int idx)
    {
        if (obj)
        {
            switch (obj.tag)
            {
                case "Charactor":
                    return RiskData.nChara;

                case "SandItem":
                    SandItem item = (SandItem)obj;
                    if (FieldDataChecker.Instance.TypeCheck(name, item.GetType))
                        return RiskData.nMySandItem;
                    else
                        return RiskData.nEnemySandItem;

                case "Block":
                    return RiskData.nBlock;
            }
        }
        else
        {
            SandItem.eType type = SandData.Instance.GetSandDataList[idx];
            if (type != SandItem.eType.MAX)
            {
                if (FieldDataChecker.Instance.TypeCheck(name, type))
                    return RiskData.nMySand;
                else
                    return RiskData.nEnemySand;
            }

            type = SandData.Instance.GetHalfSandDataList[idx]._type;
            if (type != SandItem.eType.MAX)
            {
                //  Blockによる半はさまれは危険として処理
                if (FieldDataChecker.Instance.TypeCheck(name, type))
                    return RiskData.nMyHalfSand;
                else
                    return RiskData.nEnemyHalfSand;
            }
        }

        return 0;
    }
    #endregion

    #region GetInput

    //  取得時にリセットをかける
    //  Trueを返す側ではリセットしない。送り続けるだけ
    public bool GetMove(Charactor.eDirection dir)
    {
#if DEBUG
        if (DebugMoveInput(dir))
            return true;
#endif
        if (_nowInput._direction != dir)
            return false;

        Charactor.eDirection nowDir = _nowInput._direction;
        _nowInput._direction = Charactor.eDirection.MAX;
        return true;
    }

    public bool GetAction(Charactor.eAction act)
    {
#if DEBUG
        if (DebugActionInput(act))
            return true;
#endif
        if (_nowInput._action != act)
            return false;

        Charactor.eAction nowAct = _nowInput._action;
        _nowInput._action = Charactor.eAction.MAX;
        return true;
    }

    #endregion


#if DEBUG
    #region DebugInput

    bool DebugMoveInput(Charactor.eDirection dir)
    {
        if (transform.name.Contains("1P"))
        {
            switch (dir)
            {
                case Charactor.eDirection.FORWARD:
                    if (!Input.GetKey(KeyCode.W))
                        return false;
                    break;
                case Charactor.eDirection.BACK:
                    if (!Input.GetKey(KeyCode.S))
                        return false;
                    break;
                case Charactor.eDirection.RIGHT:
                    if (!Input.GetKey(KeyCode.D))
                        return false;
                    break;
                case Charactor.eDirection.LEFT:
                    if (!Input.GetKey(KeyCode.A))
                        return false;
                    break;
            }
        }
        else if ((transform.name.Contains("2P")))
        {
            switch (dir)
            {
                case Charactor.eDirection.FORWARD:
                    if (!Input.GetKey(KeyCode.UpArrow))
                        return false;
                    break;
                case Charactor.eDirection.BACK:
                    if (!Input.GetKey(KeyCode.DownArrow))
                        return false;
                    break;
                case Charactor.eDirection.RIGHT:
                    if (!Input.GetKey(KeyCode.RightArrow))
                        return false;
                    break;
                case Charactor.eDirection.LEFT:
                    if (!Input.GetKey(KeyCode.LeftArrow))
                        return false;
                    break;
            }
        }
        else if ((transform.name.Contains("3P")))
        {
            switch (dir)
            {
                case Charactor.eDirection.FORWARD:
                    if (!Input.GetKey(KeyCode.F))
                        return false;
                    break;
                case Charactor.eDirection.BACK:
                    if (!Input.GetKey(KeyCode.V))
                        return false;
                    break;
                case Charactor.eDirection.RIGHT:
                    if (!Input.GetKey(KeyCode.B))
                        return false;
                    break;
                case Charactor.eDirection.LEFT:
                    if (!Input.GetKey(KeyCode.C))
                        return false;
                    break;
            }

        }
        else if ((transform.name.Contains("4P")))
        {
            switch (dir)
            {
                case Charactor.eDirection.FORWARD:
                    if (!Input.GetKey(KeyCode.Alpha7))
                        return false;
                    break;
                case Charactor.eDirection.BACK:
                    if (!Input.GetKey(KeyCode.U))
                        return false;
                    break;
                case Charactor.eDirection.RIGHT:
                    if (!Input.GetKey(KeyCode.I))
                        return false;
                    break;
                case Charactor.eDirection.LEFT:
                    if (!Input.GetKey(KeyCode.Y))
                        return false;
                    break;
            }
        }

        return true;
    }
    bool DebugActionInput(Charactor.eAction act)
    {
        if (transform.name.Contains("1P"))
        {
            switch (act)
            {
                case Charactor.eAction.PUT:
                    if (!Input.GetKey(KeyCode.R))
                        return false;
                    break;
                case Charactor.eAction.BREAK:
                    if (!Input.GetKey(KeyCode.T))
                        return false;
                    break;
            }
        }
        else if ((transform.name.Contains("2P")))
        {
            switch (act)
            {
                case Charactor.eAction.PUT:
                    if (!Input.GetKey(KeyCode.O))
                        return false;
                    break;
                case Charactor.eAction.BREAK:
                    if (!Input.GetKey(KeyCode.P))
                        return false;
                    break;
            }
        }
        else if ((transform.name.Contains("3P")))
        {
            switch (act)
            {
                case Charactor.eAction.PUT:
                    if (!Input.GetKey(KeyCode.H))
                        return false;
                    break;
                case Charactor.eAction.BREAK:
                    if (!Input.GetKey(KeyCode.J))
                        return false;
                    break;
            }
        }
        else if ((transform.name.Contains("4P")))
        {
            switch (act)
            {
                case Charactor.eAction.PUT:
                    if (!Input.GetKey(KeyCode.Alpha9))
                        return false;
                    break;
                case Charactor.eAction.BREAK:
                    if (!Input.GetKey(KeyCode.Alpha0))
                        return false;
                    break;
            }
        }

        return true;
    }
    #endregion
#endif

}
