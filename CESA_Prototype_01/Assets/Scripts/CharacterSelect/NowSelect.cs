using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NowSelect : Photon.PunBehaviour
{
    protected CharacterSelect.eCharaType _charaType = CharacterSelect.eCharaType.BALANCE;
    public CharacterSelect.eCharaType CharaType {  get { return _charaType; } set { _charaType = value; } }
    protected CharacterSelect.eCharaType _oldCharaType = CharacterSelect.eCharaType.BALANCE;

    Text _text = null;

    [SerializeField] protected bool _IsOnNone = true;
   
	// Use this for initialization
	void Awake ()
    {
        if (!_IsOnNone)
            _charaType = CharacterSelect.eCharaType.BALANCE;
        else
            _charaType = CharacterSelect.eCharaType.NONE;

        _text = GetComponent<Text>();
        GetComponentInParent<Image>().color = Color.clear;
    }
	
	// Update is called once per frame
	void Update () 
    {
        //  テキスト更新
        CharaUpdate();

        // 範囲外処理
        if (_charaType == CharacterSelect.eCharaType.NONE)
            return;

        if (_charaType > CharacterSelect.eCharaType.MAX)
        {
            _charaType = (CharacterSelect.eCharaType)1;
        }
        else if (_charaType < (CharacterSelect.eCharaType)1)
        {
            _charaType = CharacterSelect.eCharaType.MAX;
        }
	}

    public void CharaUpdate()
    {
        if (!_text)
            return;

        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        switch (_charaType)
        {
            case CharacterSelect.eCharaType.NONE:
                _text.text = "";
                transform.GetChild(0).gameObject.SetActive(true);
                break;
            case CharacterSelect.eCharaType.BALANCE:
                _text.text = "Balance";
                transform.GetChild(1).gameObject.SetActive(true);
                break;
            case CharacterSelect.eCharaType.POWER:
                _text.text = "Power";
                transform.GetChild(2).gameObject.SetActive(true);
                break;
            case CharacterSelect.eCharaType.SPEED:
                _text.text = "Speed";
                transform.GetChild(3).gameObject.SetActive(true);
                break;
            case CharacterSelect.eCharaType.TECHNICAL:
                _text.text = "Technical";
                transform.GetChild(4).gameObject.SetActive(true);
                break;
            case CharacterSelect.eCharaType.MAX:
                _text.text = "???";
                transform.GetChild(5).gameObject.SetActive(true);
                break;
        }
    }

    public virtual void Add()
    {
        _oldCharaType = _charaType;
        _charaType ++;
    }

    public virtual void None()
    {
        if (Ready.nReadyCnt == PhotonNetwork.playerList.Length)
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
