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
            _direction = Character.eDirection.MAX;
            _action    = Character.eAction.MAX;
        }

        public Character.eDirection _direction;
        public Character.eAction    _action;
    };
    
    AIInput _nowInput = new AIInput();

    public struct tDistanceData
    {
        public int _nIdx;
        public int _nDistance;
    };
    public tDistanceData[] _DistanceDatas { get; private set; }    //  各マスの自分から見た移動距離を保持する
    Character _character = null;
    CharacterGauge _characterGauge = null;
    FieldObjectBase _fieldObjBase = null;

    public enum eState
    {
        WAIT = 0,
        MOVE,
        PUT,
        BREAK,

        MAX,
    }
    eState _state = eState.WAIT;
    public eState GetState { get { return _state; } }

    //  AIを思考するか
    bool _IsAI = true;

    //  AI選択用メソッド
    int _nLevel = 0;                                        //  低いほど頭が良くなる (0 : 強, 1 : 中, 2 : 弱)
    int[] _nActionRatio = new int[(int)eState.MAX];         //  通常時行動比率 (合計で100になるようにする)
    int[] _nSpecialActionRatio = new int[(int)eState.MAX];  //  特殊時行動比率 (合計で100になるようにする)

    //  
    bool _IsDanger = false;     //  危険状態かどうか trueの時AI変更
    bool _IsOnRisk = true;      //  リスクチェックするかどうか    
    public void OffRiskCheck() { _IsOnRisk = false; }

    //  各行動AI
    MoveAI _moveAI = null;
    PutAI _putAI = null;
    BreakAI _breakAI = null;

    #region Init

    //  キャラのタイプ、CPUのレベルによって設定する情報
    public void Set(int level, Character.eCharaType type)
    {
        if (level < 0)
        {
            _IsAI = false;
            level = 0;
        }

        _nLevel = level;
        _character = GetComponent<Character>();
        _characterGauge = GetComponent<CharacterGauge>();
        _fieldObjBase = GetComponent<FieldObjectBase>();

        _nActionRatio = AIData.Instance.GetRatio(level, type, false);
        _nSpecialActionRatio = AIData.Instance.GetRatio(level, type, true);
        AIData.RiskData riskData = AIData.Instance.GetRisk(level, type);
        SetRiskData(riskData.maxRisk, riskData.riskRange);

        //Debug.Log(this.name + "はLevel" + level);
        //Debug.Log("Wait : " + _nActionRatio[0] + ", Walk : " + _nActionRatio[1] + ", Put : " + _nActionRatio[2] + ", Break : " + _nActionRatio[3]);
        //Debug.Log("SpecialWait : " + _nSpecialActionRatio[0] + ", SpecialWalk : " + _nSpecialActionRatio[1] + ", SpecialPut : " + _nSpecialActionRatio[2] + ", SpecialBreak : " + _nSpecialActionRatio[3]);
        //Debug.Log("許容リスク : " + riskData.maxRisk + ", 探索範囲 : " + riskData.riskRange);

        // 各行動AIを生成
        if(_character._charaType != Character.eCharaType.TECHNICAL)
            _moveAI = gameObject.AddComponent<MoveAI>();
        else
            _moveAI = gameObject.AddComponent<TechnicalTypeMoveAI>();
        _moveAI.Init(level, type);

        _putAI = gameObject.AddComponent<PutAI>();
        _putAI.Init(level);

        if (_character._charaType != Character.eCharaType.POWER)
            _breakAI = gameObject.AddComponent<BreakAI>();
        else
            _breakAI = gameObject.AddComponent<PowerTypeBreakAI>();
        _breakAI.Init(level);
    }

    // Use this for initialization
    void Start()
    {
#if DEBUG
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                //Debug.Log(_state);
                if (!Input.GetKeyDown(KeyCode.LeftShift))
                    return;

                _IsAI = !_IsAI;
            });
#endif 
        StateInit();

        //  DistanceData
        _DistanceDatas = new tDistanceData[GameScaler._nWidth * GameScaler._nHeight];
        for (int idx = 0; idx < _DistanceDatas.Length; idx++)
            _DistanceDatas[idx]._nIdx = idx;
        
        this.ObserveEveryValueChanged(_ => _fieldObjBase.GetDataNumber())
            .Subscribe(_DistanceDatas =>
            {
                CheckDistanceData();
            });

        CharaInit();
    }

    void StateInit()
    {
        //  ItemCheck
        bool IsGetItem = false;
        this.ObserveEveryValueChanged(_ => _fieldObjBase.GetDataNumber())
            .Where(_ => !IsGetItem && ItemHolder.Instance.transform.childCount > 0)
            .Subscribe(_ =>
            {
                List<ItemBase> itemList = ItemHolder.Instance.ItemList;
                foreach (ItemBase item in itemList)
                {
                    //  特殊アイテムで、特殊状態なら無駄なので取りにいかない
                    if (item.GetItemType == ItemBase.eItemType.SPECIAL && _character.GetSpecialModeFlg)
                        continue;

                    int number = item.GetDataNumber();
                    int distance = ItemHolder.Instance.GetDistanceForType(item.GetItemType) / (_nLevel + 1);
                    if (_DistanceDatas[number]._nDistance > distance)
                        continue;

                    if (FieldDataChecker.Instance.SandCheck(number, name))
                        continue;

                    if (!_moveAI.SearchRoute(number, 0))
                        continue;

                    _state = eState.MOVE;
                    IsGetItem = true;
                    break;
                }
            });

        // State.WAIT
        this.UpdateAsObservable()
            .Where(_ => _state == eState.WAIT && !_IsDanger)
            .Subscribe(_ =>
            {
                AIUpdate();
            });

        // State.MOVE
        this.UpdateAsObservable()
            .Where(_ => _state == eState.MOVE)
            .Subscribe(_ =>
            {
                if (MoveUpdate())
                    return;

                IsGetItem = false;
                Reset();
            });

        // State.PUT
        this.UpdateAsObservable()
            .Where(_ => _state == eState.PUT)
            .Subscribe(_ =>
            {
                if (MoveUpdate())
                    return;

                _nowInput._action = Character.eAction.PUT;
                Reset();
            });

        // State.BREAK
        this.UpdateAsObservable()
            .Where(_ => _state == eState.BREAK)
            .Subscribe(_ =>
            {
                if (MoveUpdate())
                    return;

                _nowInput._action = Character.eAction.BREAK;
                Reset();
            });

    }

    //  キャラごとのAI設定
    void CharaInit()
    {
        //  SpeedTypeは特殊モード中リスク回避しないようにする
        if (_character._charaType == Character.eCharaType.SPEED)
        {
            this.UpdateAsObservable()
                .Where(_ => _character.GetSpecialModeFlg)
                .Subscribe(_ =>
                {
                    OffRiskCheck();
                });
        }
    }

    #endregion

    #region AI

    bool AIUpdate(bool isDanger = false)
    {
        if (!_IsAI)
            return false;

        if (isDanger)
        {
            bool isSuccess = DangerAI();
            if (isSuccess)
                return isSuccess;
        }

        if (_character.NotMove)
            return CharaNotMoveAI();

        int number = CheckStopEnemyChara();
        if(number >= 0)
            return EnemyStopAI(number);

        return NormalAI();
    }

    bool DangerAI()
    {
        bool isSuccess = false;
        if (_characterGauge.PutGaugeCheck() && Random.Range(0, (_nLevel + 1 * 3)) == 0)
        {
            isSuccess = _putAI.HalfSandPut(true);
            if (isSuccess)
                _state = eState.PUT;
        }
        else
        {
            isSuccess = _moveAI.SearchRoute(_moveAI.RandomNullMass(2 * (_nLevel + 1)), 0);
            if (isSuccess)
                _state = eState.MOVE;
        }
        //Debug.Log(name + "のDangerAI実行" + isSuccess);
        if (isSuccess)
            OffRiskCheck(); //  リスクAIに成功したらその行動はリスクチェックしない

        return isSuccess;
    }

    bool CharaNotMoveAI()
    {
        bool isSuccess = false;

        isSuccess = _breakAI.NearBlockBreak();
       
        if(isSuccess)
        {
            _state = eState.BREAK;
        }
        else
        {
            if (Random.Range(0, 60 * (_nLevel + 1)) == 0)
            {
                isSuccess = _moveAI.SearchRoute(_moveAI.RandomNullMass(2), 0);
                if (isSuccess)
                    _state = eState.MOVE;
                else
                    _state = eState.WAIT;
            }
            else
            {
                _state = eState.WAIT;
            }
        }
        //Debug.Log(name + "のCharaNotMoveAI実行" + isSuccess);
        return isSuccess;
    }

    bool EnemyStopAI(int number)
    {
        bool isSuccess = false;

        isSuccess = _putAI.PlacePutChara(number);   //  そのキャラの目の前に置くのを試みる

        if (isSuccess)
        {
            _state = eState.PUT;
        }
        else
        {
            isSuccess = _breakAI.NearBlockBreak(number);    //  置けないならそのキャラの近くのブロックを壊して置けるように試みる
            _state = isSuccess ? eState.BREAK : eState.WAIT; 
        }
        //Debug.Log(name + "のEnemyStopAI実行" + isSuccess);
        return isSuccess;
    }

    bool NormalAI()
    {
        bool isSuccess = false;
        _state = GetNextState();
        switch (_state)
        {
            case eState.WAIT:
                break;
            case eState.MOVE:
                isSuccess = _moveAI.OnMove();
                //Debug.Log("Walk");
                break;
            case eState.PUT:
                isSuccess = _putAI.OnPut();
                //Debug.Log("Put");
                break;
            case eState.BREAK:
                isSuccess = _breakAI.OnBreak();
                //Debug.Log("Break");
                break;
        }
        //Debug.Log("AI : " + _state + "," + isSuccess);

        if (!isSuccess)
            _state = eState.WAIT;

        return isSuccess;
    }

    eState GetNextState()
    {
        eState next = eState.WAIT;

        // Debug
        /*if (isDanger)
            return eState.WALK; // Random.Range(0, 2) == 0 ? eState.WALK : eState.BREAK;
        else
            return eState.WALK; // (eState)Random.Range(0, (int)eState.MAX);*/

        int nRand = 0;
        int[] ratio = _character.GetSpecialModeFlg ? _nSpecialActionRatio : _nActionRatio;
        nRand = Random.Range(0, ratio.Sum());
        for (int i = 0; i < ratio.Length; i++)
        {
            nRand -= ratio[i];
            if (nRand > 0)
                continue;
            next = (eState)i;
            break;
        }
        return next;
    }

    //  止まっている敵がいないかをチェックし、いたら場所を返す
    int CheckStopEnemyChara()
    {
        List<Character> charas = FieldData.Instance.GetCharactorsNonMe(gameObject);
        charas = charas.Where(_ =>
        {
            if (!_.NotMove)
                return false;

            if (_DistanceDatas[_.GetDataNumber()]._nDistance >= 3 * (3 - _nLevel))
                return false;

            return true;
        }).ToList();

        if (charas.Count <= 0)
            return -1;

        return charas[Random.Range(0, charas.Count)].GetDataNumber();
    }

    void Reset()
    {
        _state = eState.WAIT;
        _IsOnRisk = true;
    }

    # endregion

    #region Move

    bool MoveUpdate()
    {
        // 移動終了か行動不能で次のAIへ
        if (!_moveAI.StateWalk || _moveAI.StateStop)
            return false;

        _nowInput._direction = _moveAI.GetMoveData();

        //if(_nowInput._direction != Character.eDirection.MAX)
        //    Debug.Log(_nowInput._direction);    //  行動停止バグ監視用

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
            .Where(_ => _IsOnRisk && FieldData.Instance.ChangeFieldWithChara)
            .Subscribe(_ =>
            {
                int risk = 0;
                bool isChara = false;

                //  自分の位置のリスク計算
                risk += RiskCheck(null, _fieldObjBase.GetDataNumber(), ref isChara);

                //  周りの状態からリスクを追加
                List<tDistanceData> distanceList = _DistanceDatas.Where(x => x._nDistance > 0 && x._nDistance <= riskRange).ToList();
                string debug = " = ";
                foreach (tDistanceData data in distanceList)
                {
                    //  マスの状態と近さでリスク計算
                    int add = RiskCheck(FieldData.Instance.GetObjData(data._nIdx), data._nIdx, ref isChara);
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

    //  キャラのリスクは大きいので、2体以上被っていたら追加しない
    protected virtual int RiskCheck(FieldObjectBase obj, int idx, ref bool isChara)
    {
        if (obj)
        {
            switch (obj.tag)
            {
                case "Character":
                    if (isChara)
                        return 0;

                    isChara = true;
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
            SandItem.eType type = SandData.Instance.GetSandDataList[idx]._type;
            if (type != SandItem.eType.MAX)
            {
                if (FieldDataChecker.Instance.TypeCheck(name, type))
                    return RiskData.nMySand;
                else
                    return RiskData.nEnemySand;
            }

            foreach (SandItem.eType halfType in SandData.Instance.GetHalfSandDataList[idx]._type)
            {
                if (halfType != SandItem.eType.MAX)
                {
                    //  Blockによる半はさまれは危険として処理
                    if (FieldDataChecker.Instance.TypeCheck(name, halfType))
                        return RiskData.nMyHalfSand;
                    else
                        return RiskData.nEnemyHalfSand;
                }
            }
        }

        return 0;
    }
    #endregion

    #region GetInput

    //  取得時にリセットをかける
    //  Trueを返す側ではリセットしない。送り続けるだけ
    public bool GetMove(Character.eDirection dir)
    {
#if DEBUG
        if (DebugMoveInput(dir))
            return true;
#endif
        if (_nowInput._direction != dir)
            return false;

        Character.eDirection nowDir = _nowInput._direction;
        _nowInput._direction = Character.eDirection.MAX;
        return true;
    }

    public bool GetAction(Character.eAction act)
    {
#if DEBUG
        if (DebugActionInput(act))
            return true;
#endif
        if (_nowInput._action != act)
            return false;

        Character.eAction nowAct = _nowInput._action;
        _nowInput._action = Character.eAction.MAX;
        return true;
    }

    #endregion


#if DEBUG
    #region DebugInput

    bool DebugMoveInput(Character.eDirection dir)
    {
        if (transform.name.Contains("1P"))
        {
            switch (dir)
            {
                case Character.eDirection.FORWARD:
                    if (!Input.GetKey(KeyCode.W))
                        return false;
                    break;
                case Character.eDirection.BACK:
                    if (!Input.GetKey(KeyCode.S))
                        return false;
                    break;
                case Character.eDirection.RIGHT:
                    if (!Input.GetKey(KeyCode.D))
                        return false;
                    break;
                case Character.eDirection.LEFT:
                    if (!Input.GetKey(KeyCode.A))
                        return false;
                    break;
            }
        }
        else if ((transform.name.Contains("2P")))
        {
            switch (dir)
            {
                case Character.eDirection.FORWARD:
                    if (!Input.GetKey(KeyCode.UpArrow))
                        return false;
                    break;
                case Character.eDirection.BACK:
                    if (!Input.GetKey(KeyCode.DownArrow))
                        return false;
                    break;
                case Character.eDirection.RIGHT:
                    if (!Input.GetKey(KeyCode.RightArrow))
                        return false;
                    break;
                case Character.eDirection.LEFT:
                    if (!Input.GetKey(KeyCode.LeftArrow))
                        return false;
                    break;
            }
        }
        else if ((transform.name.Contains("3P")))
        {
            switch (dir)
            {
                case Character.eDirection.FORWARD:
                    if (!Input.GetKey(KeyCode.F))
                        return false;
                    break;
                case Character.eDirection.BACK:
                    if (!Input.GetKey(KeyCode.V))
                        return false;
                    break;
                case Character.eDirection.RIGHT:
                    if (!Input.GetKey(KeyCode.B))
                        return false;
                    break;
                case Character.eDirection.LEFT:
                    if (!Input.GetKey(KeyCode.C))
                        return false;
                    break;
            }

        }
        else if ((transform.name.Contains("4P")))
        {
            switch (dir)
            {
                case Character.eDirection.FORWARD:
                    if (!Input.GetKey(KeyCode.Alpha7))
                        return false;
                    break;
                case Character.eDirection.BACK:
                    if (!Input.GetKey(KeyCode.U))
                        return false;
                    break;
                case Character.eDirection.RIGHT:
                    if (!Input.GetKey(KeyCode.I))
                        return false;
                    break;
                case Character.eDirection.LEFT:
                    if (!Input.GetKey(KeyCode.Y))
                        return false;
                    break;
            }
        }

        return true;
    }
    bool DebugActionInput(Character.eAction act)
    {
        if (transform.name.Contains("1P"))
        {
            switch (act)
            {
                case Character.eAction.PUT:
                    if (!Input.GetKey(KeyCode.R))
                        return false;
                    break;
                case Character.eAction.BREAK:
                    if (!Input.GetKey(KeyCode.T))
                        return false;
                    break;
            }
        }
        else if ((transform.name.Contains("2P")))
        {
            switch (act)
            {
                case Character.eAction.PUT:
                    if (!Input.GetKey(KeyCode.O))
                        return false;
                    break;
                case Character.eAction.BREAK:
                    if (!Input.GetKey(KeyCode.P))
                        return false;
                    break;
            }
        }
        else if ((transform.name.Contains("3P")))
        {
            switch (act)
            {
                case Character.eAction.PUT:
                    if (!Input.GetKey(KeyCode.H))
                        return false;
                    break;
                case Character.eAction.BREAK:
                    if (!Input.GetKey(KeyCode.J))
                        return false;
                    break;
            }
        }
        else if ((transform.name.Contains("4P")))
        {
            switch (act)
            {
                case Character.eAction.PUT:
                    if (!Input.GetKey(KeyCode.Alpha9))
                        return false;
                    break;
                case Character.eAction.BREAK:
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
