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
    int[] _DistanceDatas = null;    //  各マスの自分から見た移動距離を保持する
    FieldObjectBase _fieldObjBase = null;

    //  各行動AI
    MoveAI moveAI = null;

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

        moveAI = new MoveAI();
        moveAI.Start(gameObject.AddComponent<AStar>(), _fieldObjBase);
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (moveAI.NowMove)
        {
            _nowInput._direction = moveAI.GetMoveData();
            //Debug.Log ("Move : " + _nowInput._direction);
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            //Debug.Log("Search");
            int rand = 0;
            while (true)
            {
                rand = Random.Range(0, GameScaler._nHeight * GameScaler._nWidth);
                if (FieldData.Instance.GetObjData(rand))
                    continue;

                List<SandData.tSandData> dataList = SandData.Instance.GetSandDataList.FindAll(_ => _._number == rand);
                if (dataList.Count > 0 && !TypeCheck(dataList[0]._Type))
                    continue;

                break;
            }
            moveAI.SearchRoute(rand);
        }
    }

    bool TypeCheck(SandItem.eType type)
    {
        switch(type)
        {
            case SandItem.eType.ONE_P:
                if (this.name.Contains("1P"))
                    return true;
                break;
            case SandItem.eType.TWO_P:
                if (this.name.Contains("2P"))
                    return true;
                break;
            case SandItem.eType.THREE_P:
                if (this.name.Contains("3P"))
                    return true;
                break;
            case SandItem.eType.FOUR_P:
                if (this.name.Contains("4P"))
                    return true;
                break;
        }

        return false;
    }

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
        if (dir == Charactor.eDirection.RIGHT && Input.GetKey(KeyCode.RightArrow))
            return true;
        else if (dir == Charactor.eDirection.LEFT && Input.GetKey(KeyCode.LeftArrow))
            return true;
        else if (dir == Charactor.eDirection.FORWARD && Input.GetKey(KeyCode.UpArrow))
            return true;
        else if (dir == Charactor.eDirection.BACK && Input.GetKey(KeyCode.DownArrow))
            return true;

        if (_nowInput._direction != dir)
            return false;

        Charactor.eDirection nowDir = _nowInput._direction;
        _nowInput._direction = Charactor.eDirection.MAX;
        return true;
    }

    public bool GetAction(Charactor.eAction act)
    {
        Charactor.eAction nowAct = _nowInput._action;
        _nowInput._action = Charactor.eAction.MAX;
        return (nowAct == act);
    }
}
