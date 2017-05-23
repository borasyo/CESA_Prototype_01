using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charactor : FieldObjectBase
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

    protected CharactorInput _charactorInput = null;
    protected CharactorGauge _charactorGauge = null;

    [SerializeField] float _moveAmount_Sec = 0.5f;
    protected int _nOldNumber = 0;

    public eCharaType _charaType { get; protected set; }
    protected eDirection _nowDirection = eDirection.FORWARD;
    protected GameObject _sandItem = null;
    protected bool _IsSpecialMode = false;
    public bool GetSpecialModeFlg { get { return _IsSpecialMode; } }

    # endregion

    #region Event

    // Use this for initialization
    public void Start()
    {
        // Input生成
        if (this.name.Contains("CPU")) 
            _charactorInput = this.gameObject.AddComponent<CharactorInputAI>();
        else
            _charactorInput = this.gameObject.AddComponent<CharactorInputUser>();
        
        _charactorGauge = GetComponent<CharactorGauge>();

        string charaName = this.name[this.name.IndexOf("Player") - 1].ToString();
        _sandItem = Resources.Load<GameObject>("Prefabs/SandItem/SandItem" + charaName);
        _nowDirection = (eDirection)(transform.eulerAngles.y / 90);
    }
	
    // Update is called once per frame
    void Update()
    {
        _nOldNumber = GetDataNumber();

        MoveUpdate();

        DirUpdate();
        DataUpdate();

        //  アクション
        ItemPut();
        ItemBreak();
    }

    #endregion

    #region Move

    void MoveUpdate()
    {
        //  移動
        if (MoveCheck(eDirection.FORWARD))
        {
            transform.position += new Vector3(0,0, _moveAmount_Sec) * Time.deltaTime;
        }
        else if(MoveCheck(eDirection.BACK)) 
        {
            transform.position -= new Vector3(0,0, _moveAmount_Sec) * Time.deltaTime;
        }
        else if(MoveCheck(eDirection.RIGHT)) 
        {
            transform.position += new Vector3(_moveAmount_Sec, 0,0) * Time.deltaTime;
        }
        else if(MoveCheck(eDirection.LEFT)) 
        {
            transform.position -= new Vector3(_moveAmount_Sec, 0,0) * Time.deltaTime;
        }
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

        if (check < 0 || GameScaler._nWidth * GameScaler._nHeight <= check)
            return false;

        FieldObjectBase checkData = FieldData.Instance.GetObjData(check);
        if (checkData && checkData != this)
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

    virtual protected void ItemPut()
    {
        if (!_charactorGauge.PutGaugeCheck() || 
            !_charactorInput.GetActionInput(eAction.PUT))
            return;

        int dirNumber = GetDataNumberForDir();
        if (dirNumber < 0 || GameScaler._nWidth * GameScaler._nHeight < dirNumber)
            return;

        FieldObjectBase obj = FieldData.Instance.GetObjData(dirNumber);
        if (obj)
            return;

        GameObject item = (GameObject)Instantiate(_sandItem, GetPosForNumber(dirNumber), Quaternion.identity);
        FieldData.Instance.SetObjData(item.GetComponent<FieldObjectBase>(), dirNumber);
        _charactorGauge.PutAction();
    }

    virtual protected void ItemBreak()
    {
        if (!_charactorGauge.BreakGaugeCheck() || 
            !_charactorInput.GetActionInput(eAction.BREAK))
            return;

        FieldObjectBase obj = FieldData.Instance.GetObjData(GetDataNumberForDir());

        if (!obj || obj.GetSandType() == SandItem.eType.MAX)
            return;

        FieldData.Instance.SetObjData(null, GetDataNumberForDir());
        FieldData.Instance.ExceptionChangeField();
        Destroy(obj.gameObject);
        _charactorGauge.BreakAction();
    }

    #endregion

    #region Other

    void DirUpdate()
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

    protected virtual void DataUpdate()
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

    #endregion

    #region VirtualMethod

    virtual public void RunSpecialMode(bool IsRun)
    {
        //  継承先で記述
    }

    #endregion
}
