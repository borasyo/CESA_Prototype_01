using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : FieldObjectBase
{
    #region Enum

    public enum eCharaType 
    {
        BALANCE = 0,
        POWER,
        SPEED,
        TECHNICAL,
        MAX
    };

    public enum eDirection
    {
        FORWARD = 0,
        RIGHT,
        BACK,
        LEFT,

        MAX,
    };

    public enum eAction
    {
        PUT = 0,
        BREAK,

        MAX,
    };

    #endregion

    #region Member

    protected CharacterInput _charactorInput = null;
    protected CharacterGauge _charactorGauge = null;

    [SerializeField] protected float _moveAmount_Sec = 0.5f;
    protected int _nOldNumber = 0;

    public eCharaType _charaType { get; protected set; }
    protected eDirection _nowDirection = eDirection.FORWARD;
    protected GameObject _sandItem = null;

    protected bool _IsSpecialMode = false;
    public bool GetSpecialModeFlg { get { return _IsSpecialMode; } }

    bool _IsNotMove = false;
    protected float _fNotMoveTime = 0.0f;
    public bool NotMove { get { return _IsNotMove; } }

    # endregion

    #region Event

    // Use this for initialization
    public virtual void Init(int level)
    {
        SetInput(level);

        _charactorGauge = GetComponent<CharacterGauge>();

        string charaName = this.name[this.name.IndexOf("Player") - 1].ToString();
        _sandItem = Resources.Load<GameObject>("Prefabs/SandItem/SandItem" + charaName);


        _nowDirection = (eDirection)(transform.eulerAngles.y / 90);
        //FieldData.Instance.CharaSet(this);
        FieldData.Instance.SetObjData(this, GetDataNumber());
    }

    protected virtual void SetInput(int level)
    {    
        // Input生成
        if (this.name.Contains("CPU"))
        {
            CharacterInputAI ai = this.gameObject.AddComponent<CharacterInputAI>();
            ai._enemyAI.Set(level, _charaType);
            _charactorInput = ai;
        }
        else
        {
            _charactorInput = this.gameObject.AddComponent<CharacterInputUser>();
        }
    }

    // Update is called once per frame
    protected void Update()
    {
        _nOldNumber = GetDataNumber();

        MoveUpdate();

        DirUpdate();
        NumberUpdate();

        //  アクション
        ItemPut();
        ItemBreak();
    }

    #endregion

    #region Move

    protected void MoveUpdate()
    {
        //  移動
        if (MoveCheck(eDirection.FORWARD))
        {
            transform.position += new Vector3(0,0, _moveAmount_Sec) * Time.deltaTime;
            _fNotMoveTime = 0.0f;
            return;
        }
        else if(MoveCheck(eDirection.BACK)) 
        {
            transform.position -= new Vector3(0,0, _moveAmount_Sec) * Time.deltaTime;
            _fNotMoveTime = 0.0f;
            return;
        }
        else if(MoveCheck(eDirection.RIGHT)) 
        {
            transform.position += new Vector3(_moveAmount_Sec, 0,0) * Time.deltaTime;
            _fNotMoveTime = 0.0f;
            return;
        }
        else if(MoveCheck(eDirection.LEFT)) 
        {
            transform.position -= new Vector3(_moveAmount_Sec, 0,0) * Time.deltaTime;
            _fNotMoveTime = 0.0f;
            return;
        }

        _fNotMoveTime += Time.deltaTime;
        _IsNotMove = _fNotMoveTime >= 1.0f;
    }

    virtual protected bool MoveCheck(eDirection dir)
    {
        if (!_charactorInput.GetMoveInput(dir))
            return false;
        
        _nowDirection = dir;
        int number = GetDataNumber();

        int check = 0;
        switch(dir) 
        {
            case eDirection.FORWARD:
                check = number + GameScaler._nWidth;
                break;
            case eDirection.BACK:
                check = number - GameScaler._nWidth;
                break;
            case eDirection.RIGHT:
                check = number + 1;
                break;
            case eDirection.LEFT:
                check = number - 1;
                break;
        }

        if (check < 0 || GameScaler.GetRange <= check)
            return false;

        FieldObjectBase checkData = FieldData.Instance.GetObjData(check);
        if (checkData && checkData.tag != "Character")
        {
            float distance = 0.0f;
            switch (dir)
            {
                case eDirection.FORWARD:
                    distance = checkData.GetPosForNumber().z - transform.position.z;
                    break;
                case eDirection.BACK:
                    distance = transform.position.z - checkData.GetPosForNumber().z;
                    break;
                case eDirection.RIGHT:
                    distance = checkData.GetPosForNumber().x - transform.position.x;
                    break;
                case eDirection.LEFT:
                    distance = transform.position.x - checkData.GetPosForNumber().x;
                    break;
            }
            //  障害物があって進めない!
            if (distance <= GameScaler._fScale)
                return false;
        }

        return true;
    }
      
    public void ChangeSpeed(float per)
    {
        _moveAmount_Sec *= per;
    }

    public void SetSpeed(float speed)
    {
        _moveAmount_Sec = speed;
    }

    #endregion

    #region Action

    protected virtual void ItemPut()
    {
        if (!_charactorGauge.PutGaugeCheck() || !_charactorInput.GetActionInput(eAction.PUT))
            return;

        int dirNumber = GetDataNumberForDir();
        if (dirNumber < 0 || GameScaler.GetRange < dirNumber)
            return;

        FieldObjectBase obj = FieldData.Instance.GetObjData(dirNumber);
        if (obj)
            return;

        GameObject item = (GameObject)Instantiate(_sandItem, GetPosForNumber(dirNumber), Quaternion.identity);
        FieldData.Instance.SetObjData(item.GetComponent<FieldObjectBase>(), dirNumber);
        _charactorGauge.PutAction();
        _fNotMoveTime = 0.0f;
    }

    protected virtual void ItemBreak()
    {
        if (!_charactorGauge.BreakGaugeCheck() || !_charactorInput.GetActionInput(eAction.BREAK))
            return;

        FieldObjectBase obj = FieldData.Instance.GetObjData(GetDataNumberForDir());

        if (!obj || obj.GetSandType() == SandItem.eType.MAX)
            return;

        obj.GetComponent<SandItem>().Break();
        _charactorGauge.BreakAction();
        _fNotMoveTime = 0.0f;
    }

    #endregion

    #region Other

    protected void DirUpdate()
    {
        switch (_nowDirection)
        {
            case eDirection.FORWARD:
                transform.eulerAngles = new Vector3(0,0,0);
                break;
            case eDirection.BACK:
                transform.eulerAngles = new Vector3(0,180,0);
                break;
            case eDirection.RIGHT:
                transform.eulerAngles = new Vector3(0,90,0);
                break;
            case eDirection.LEFT:
                transform.eulerAngles = new Vector3(0,270,0);
                break;
        }
    }

    protected virtual void NumberUpdate()
    {
        int nowNumber = GetDataNumber();
        if (_nOldNumber == nowNumber)
            return;

        FieldData.Instance.SetObjData(null, _nOldNumber);
        FieldData.Instance.SetObjData(this, nowNumber);
    }
        
    //  向いている方向を元にデータ番号を取得
    public int GetDataNumberForDir()
    {
        int number = GetDataNumber();
        switch(_nowDirection)
        {
            case eDirection.FORWARD:
                number += GameScaler._nWidth;
                break;
            case eDirection.BACK:
                number -= GameScaler._nWidth;
                break;
            case eDirection.RIGHT:
                number += 1;
                break;
            case eDirection.LEFT:
                number -= 1;
                break;
        }

        return number;
    }

    public string GetPlayerNumber()
    {
        return name[this.name.IndexOf("Player") - 1].ToString();
    }

    #endregion

    #region VirtualMethod

    public virtual bool RunSpecialMode(bool IsRun)
    {
        //  継承先で記述
        return false;
    }

    #endregion
}
