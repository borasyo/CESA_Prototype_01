using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

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
    public int[] _DistanceDatas { get; private set; }    //  各マスの自分から見た移動距離を保持する
    FieldObjectBase _fieldObjBase = null;

    enum eState
    {
        WAIT = 0,
        WALK,
        PUT,
        BREAK,

        MAX,
    }
    eState _state = eState.WAIT;
    int[] _nActionRatio = new int[(int)eState.MAX]; //  行動比率 (合計で100になるようにする)
    //  各行動AI
    MoveAI _moveAI = null;
    PutAI _putAI = null;
    BreakAI _breakAI = null;

    // Use this for initialization
    void Start ()
    {
        _DistanceDatas = new int[GameScaler._nWidth * GameScaler._nHeight];
        _fieldObjBase = GetComponent<FieldObjectBase>();

        //  DistanceData
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

        _moveAI = gameObject.AddComponent<MoveAI>();
        _putAI = gameObject.AddComponent<PutAI>();
        _breakAI = gameObject.AddComponent<BreakAI>();
    }
	
	// Update is called once per frame
	void Update () 
    {
        switch (_state)
        {
            case eState.WAIT:
                AIUpdate();
                break;
            case eState.WALK:
                if (!MoveUpdate())
                    _state = eState.WAIT;
                break;
            case eState.PUT:
                if (!MoveUpdate())
                {
                    _nowInput._action = Charactor.eAction.PUT;
                    _state = eState.WAIT;
                }
                break;
            case eState.BREAK:
                if (!MoveUpdate())
                {
                    _nowInput._action = Charactor.eAction.BREAK;
                    _state = eState.WAIT;
                }
                break;
        }
    }


    #region AI

    void AIUpdate()
    {
        _state = GetNextState();
        //Debug.Log("AI : " + _state);
        switch (_state)
        {
            case eState.WAIT:
                break;
            case eState.WALK:
                if (!_moveAI.OnMove())
                    _state = eState.WAIT;
                Debug.Log("Walk");
                break;
            case eState.PUT:
                if (!_putAI.OnPut())
                    _state = eState.WAIT;
                Debug.Log("Put");
                break;
            case eState.BREAK:
                if (!_breakAI.OnBreak())
                    _state = eState.WAIT;
                Debug.Log("Break");
                break;
        }
    }

    eState GetNextState()
    {
        int nRand = Random.Range(0, 100);
        eState next = eState.WAIT;
        if(Input.GetKeyDown(KeyCode.RightShift))
        {
            next = eState.PUT; // (eState)Random.Range(1, (int)eState.MAX);
        }
        /*for (int i = 0; i < _nActionRatio.Length; i++)
        {
            nRand -= _nActionRatio[i];
            if (nRand > 0)
                continue;
            next = (eState)i;
            break;
        }*/
        return next;
    }

    # endregion

    #region Move

    bool MoveUpdate()
    {
        if (!_moveAI.StateWalk)
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

            _DistanceDatas[number] = step + Mathf.Abs(distance);
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



#if DEBUG
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

#endif
}
