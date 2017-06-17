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

    protected bool _IsNotMove = false;
    protected float _fNotMoveTime = 0.0f;
    public bool NotMove { get { return _IsNotMove; } }
    protected bool _IsNowMove = false;

    public int Level { get; private set; }

    protected Animator _animator = null;

    # endregion

    #region Event

    // Use this for initialization
    public virtual void Init(int level)
    {
        SetInput(level);
        Level = level;

        _charactorGauge = GetComponent<CharacterGauge>();
        _animator = GetComponent<Animator>();

        string charaName = this.name[this.name.IndexOf("Player") - 1].ToString();
        _sandItem = Resources.Load<GameObject>("Prefabs/SandItem/SandItem" + charaName);

        _nowDirection = (eDirection)(transform.eulerAngles.y / 90);
        //FieldData.Instance.CharaSet(this);
        FieldData.Instance.SetObjData(this, GetDataNumber());

        SetMaterial();
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

    void SetMaterial()
    {
        string materialName = "Materials/Chara/";
        switch(_charaType)
        {
            case eCharaType.BALANCE:
                materialName += "Balance_";
                break;
            case eCharaType.POWER:
                materialName += "Power_";
                break;
            case eCharaType.SPEED:
                materialName += "Speed_";
                break;
            case eCharaType.TECHNICAL:
                materialName += "Technique_";
                break;
        }
        materialName += GetPlayerNumber();

        SkinnedMeshRenderer[] all = GetComponentsInChildren<SkinnedMeshRenderer>();
        Material mat = Resources.Load(materialName) as Material;
        foreach (SkinnedMeshRenderer meRend in all)
            meRend.material = mat;
    }

    // Update is called once per frame
    protected void Update()
    {
        _nOldNumber = GetDataNumber();
        
        if (_animator.GetBool("Put") || _animator.GetBool("Break"))
            return;

        MoveUpdate();

        DirUpdate();
        NumberUpdate();
        NotMoveUpdate();

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
            OnMove();
            return;
        }
        else if(MoveCheck(eDirection.BACK)) 
        {
            transform.position -= new Vector3(0,0, _moveAmount_Sec) * Time.deltaTime;
            OnMove();
            return;
        }
        else if(MoveCheck(eDirection.RIGHT)) 
        {
            transform.position += new Vector3(_moveAmount_Sec, 0,0) * Time.deltaTime;
            OnMove();
            return;
        }
        else if(MoveCheck(eDirection.LEFT)) 
        {
            transform.position -= new Vector3(_moveAmount_Sec, 0,0) * Time.deltaTime;
            OnMove();
            return;
        }
    }

    void OnMove()
    {
        _fNotMoveTime = 0.0f;
        _animator.SetBool("Walk", true);
    }

    protected void NotMoveUpdate()
    {
        if (_IsNowMove)
        {
            _IsNowMove = false;
            return;
        }

        _fNotMoveTime += Time.deltaTime;
        _IsNotMove = _fNotMoveTime >= 1.0f;
        _animator.SetBool("Walk", false);
     
        _IsNowMove = false;
    }

    virtual protected bool MoveCheck(eDirection dir)
    {
        if (!_charactorInput.GetMoveInput(dir))
            return false;

        if (Time.deltaTime <= 0.0f)
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

        _IsNowMove = true;
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
        _charactorGauge.PutAction();
        _fNotMoveTime = 0.0f;
        _animator.SetBool("Put", true);
    }

    protected virtual void ItemBreak()
    {
        if (!_charactorGauge.BreakGaugeCheck() || !_charactorInput.GetActionInput(eAction.BREAK))
            return;

        FieldObjectBase obj = FieldData.Instance.GetObjData(GetDataNumberForDir());

        if (!obj || obj.GetSandType() == SandItem.eType.MAX)
            return;

        StartCoroutine(Break(obj));
        _animator.SetBool("Break", true);
    }

    protected IEnumerator Break(FieldObjectBase obj)
    {
        yield return new WaitForSeconds(0.75f);

        if (obj)
        {
            if (obj.tag == "SandItem")
                obj.GetComponent<SandItem>().Break();
            else
                obj.GetComponent<Block>().Break();

            _charactorGauge.BreakAction();
            _fNotMoveTime = 0.0f;
        }
    }

    #endregion

    #region Other

    protected virtual void DirUpdate()
    {
        if (_animator.GetBool("Put") || _animator.GetBool("Break"))
            return;

            switch (_nowDirection)
        {
            case eDirection.FORWARD:
                transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case eDirection.BACK:
                transform.eulerAngles = new Vector3(0, 180, 0);
                break;
            case eDirection.RIGHT:
                transform.eulerAngles = new Vector3(0, 90, 0);
                break;
            case eDirection.LEFT:
                transform.eulerAngles = new Vector3(0, 270, 0);
                break;
        }
    }

    protected virtual void NumberUpdate()
    {
        FieldData data = FieldData.Instance;
        int nowNumber = GetDataNumber();
        //if (_nOldNumber == nowNumber && 
        if(!FieldData.Instance.ChangeFieldWithChara)
            return;

        //  その場に何もなければキャラが登録されていないので登録
        FieldObjectBase obj = data.GetObjData(nowNumber);
        if (obj && obj.tag != "Character")
            return;

        data.SetObjData(null, _nOldNumber);
        data.SetObjData(this, nowNumber);
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

    public int GetPlayerNumberToInt()
    {
        int number = 0;
        switch(name[this.name.IndexOf("Player") - 1].ToString())
        {
            case "1":
                number = 1;
                break;
            case "2":
                number = 2;
                break;
            case "3":
                number = 3;
                break;
            case "4":
                number = 4;
                break;
        }
        return number;
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
