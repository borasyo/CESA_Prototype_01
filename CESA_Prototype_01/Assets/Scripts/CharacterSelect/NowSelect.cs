using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NowSelect : MonoBehaviour
{
    protected CharacterSelect.eCharaType _charaType = CharacterSelect.eCharaType.BALANCE;
    public CharacterSelect.eCharaType CharaType {  get { return _charaType; } set { _charaType = value; } }

    Text _text = null;

    [SerializeField] bool _IsOnNone = true;
    

	// Use this for initialization
	void Awake ()
    {
        if (!_IsOnNone)
            _charaType = CharacterSelect.eCharaType.BALANCE;
        else
            _charaType = CharacterSelect.eCharaType.NONE;

        _text = GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () 
    {
        //  テキスト更新
        TextUpdate();

        // 範囲外処理
        if (_IsOnNone)
        {
            if (_charaType >= CharacterSelect.eCharaType.MAX)
            {
                _charaType = (CharacterSelect.eCharaType)0;
            }
            else if (_charaType < (CharacterSelect.eCharaType)0)
            {
                _charaType = (CharacterSelect.eCharaType)(CharacterSelect.eCharaType.MAX - 1);
            }
        }
        else
        {
            if (_charaType >= CharacterSelect.eCharaType.MAX)
            {
                _charaType = (CharacterSelect.eCharaType)1;
            }
            else if (_charaType < (CharacterSelect.eCharaType)1)
            {
                _charaType = (CharacterSelect.eCharaType)(CharacterSelect.eCharaType.MAX - 1);
            }
        }
	}

    public void TextUpdate()
    {
        switch (_charaType)
        {
            case CharacterSelect.eCharaType.NONE:
                _text.text = "None";
                break;
            case CharacterSelect.eCharaType.BALANCE:
                _text.text = "Balance";
                break;
            case CharacterSelect.eCharaType.POWER:
                _text.text = "Power";
                break;
            case CharacterSelect.eCharaType.SPEED:
                _text.text = "Speed";
                break;
            case CharacterSelect.eCharaType.TECHNICAL:
                _text.text = "Technical";
                break;
        }
    }

    public virtual void Add()
    {
        _charaType ++;
    }
}
