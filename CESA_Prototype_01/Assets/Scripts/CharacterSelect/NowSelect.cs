using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class NowSelect : Photon.PunBehaviour
{
    protected CharacterSelect.eCharaType _charaType = CharacterSelect.eCharaType.BALANCE;
    public CharacterSelect.eCharaType CharaType {  get { return _charaType; } set { _charaType = value; } }
    protected CharacterSelect.eCharaType _oldCharaType = CharacterSelect.eCharaType.BALANCE;

    Image _image = null;
    RectTransform _rectTrans = null;
    const float _fDefaultHeight = 35.0f;

    [SerializeField] protected bool _IsOnNone = true;
    List<CharaMaterial> _charaMatList = new List<CharaMaterial>();

    static Sprite[] _typeSprite = new Sprite[(int)CharacterSelect.eCharaType.MAX];
   
	// Use this for initialization
	void Awake ()
    {
        if (!_typeSprite[1])
        {
            _typeSprite[(int)CharacterSelect.eCharaType.BALANCE - 1] = Resources.Load<Sprite>("Texture/CharaSelect/BALANCE");
            _typeSprite[(int)CharacterSelect.eCharaType.POWER - 1] = Resources.Load<Sprite>("Texture/CharaSelect/POWER");
            _typeSprite[(int)CharacterSelect.eCharaType.SPEED - 1] = Resources.Load<Sprite>("Texture/CharaSelect/SPEED");
            _typeSprite[(int)CharacterSelect.eCharaType.TECHNICAL - 1] = Resources.Load<Sprite>("Texture/CharaSelect/TECHNIC");
            _typeSprite[(int)CharacterSelect.eCharaType.MAX - 1] = Resources.Load<Sprite>("Texture/CharaSelect/RANDOM");
        }

        if (!_IsOnNone)
            _charaType = CharacterSelect.eCharaType.BALANCE;
        else
            _charaType = CharacterSelect.eCharaType.NONE;

        _image = GetComponent<Image>();
        _rectTrans = GetComponent<RectTransform>();
        transform.parent.GetComponent<Image>().color = Color.clear;

        for (int i = 1; i < transform.childCount - 1; i++)
            _charaMatList.Add(transform.GetChild(i).GetComponent<CharaMaterial>());

        Transform cpu = transform.parent.Find("CPU");
        if (cpu)
        {
            //Image cpuSprite = cpu.GetComponent<Image>();
            AutoAlphaChanger cpuAlpha = cpu.GetComponent<AutoAlphaChanger>();
            if (!cpuAlpha)
                return;

            AutoAlphaChanger cpuLevelAlpha = transform.parent.GetChild(1).GetComponent<AutoAlphaChanger>();
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    switch (_charaType)
                    {
                        case CharacterSelect.eCharaType.NONE:
                            //cpuSprite.color = Color.gray;
                            cpuAlpha._IsOn = true;
                            cpuLevelAlpha._IsOn = false;
                            break;
                        default:
                            //cpuSprite.color = Color.white;
                            cpuAlpha._IsOn = false;
                            cpuLevelAlpha._IsOn = true;
                            break;
                    }
                });
        }
    }
	
	// Update is called once per frame
	void Update () 
    {
        //  テキスト更新
        CharaUpdate();

        // 範囲外処理
        //if (_charaType == CharacterSelect.eCharaType.NONE)
        //    return;
	}

    public void CharaUpdate()
    {
        if (!_image)
            return;

        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(5).gameObject.SetActive(false);
        foreach (CharaMaterial charaMat in _charaMatList)
        {
            charaMat.SetMeshActive(false);
        }

        _image.color = Color.white;
        switch (_charaType)
        {
            case CharacterSelect.eCharaType.NONE:
                _image.sprite = null;
                _image.color = Color.clear;
                transform.GetChild(0).gameObject.SetActive(true);
                break;
            case CharacterSelect.eCharaType.BALANCE:
                _image.sprite = _typeSprite[0];
                _rectTrans.sizeDelta = new Vector2(200.0f, _fDefaultHeight);
                _charaMatList[0].SetMeshActive(true);
                break;
            case CharacterSelect.eCharaType.POWER:
                _image.sprite = _typeSprite[1];
                _rectTrans.sizeDelta = new Vector2(153.0f, _fDefaultHeight);
                _charaMatList[1].SetMeshActive(true);
                break;
            case CharacterSelect.eCharaType.SPEED:
                _image.sprite = _typeSprite[2];
                _rectTrans.sizeDelta = new Vector2(136.0f, _fDefaultHeight);
                _charaMatList[2].SetMeshActive(true);
                break;
            case CharacterSelect.eCharaType.TECHNICAL:
                _image.sprite = _typeSprite[3];
                _rectTrans.sizeDelta = new Vector2(178.0f, _fDefaultHeight);
                _charaMatList[3].SetMeshActive(true);
                break;
            case CharacterSelect.eCharaType.MAX:
                _image.sprite = _typeSprite[4];
                _rectTrans.sizeDelta = new Vector2(183.0f, _fDefaultHeight);
                transform.GetChild(5).gameObject.SetActive(true);
                break;
        }
    }

    public virtual void Add()
    {
        _oldCharaType = _charaType;
        _charaType++;

        if (_charaType > CharacterSelect.eCharaType.MAX)
        {
            _charaType = (CharacterSelect.eCharaType)1;
        }
    }

    public virtual void None()
    {
        if (PhotonNetwork.inRoom && Ready.nReadyCnt == PhotonNetwork.playerList.Length)
            return;

        if (_charaType == CharacterSelect.eCharaType.NONE)
        {
            _charaType = _oldCharaType;
        }
        else
        {
            _oldCharaType = _charaType;
            _charaType = CharacterSelect.eCharaType.NONE;
        }
    }
}
