using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NowSelect : MonoBehaviour 
{
    CharactorSelect.eCharaType _charaType = CharactorSelect.eCharaType.BALANCE;
    public CharactorSelect.eCharaType CharaType {  get { return _charaType; } set { _charaType = value; } }

    Text _text = null;

    [SerializeField] bool _IsOnNone = true;

	// Use this for initialization
	void Awake () 
    {
        if (!_IsOnNone)
            _charaType = CharactorSelect.eCharaType.BALANCE;
        else
            _charaType = CharactorSelect.eCharaType.NONE;

        _text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        //  テキスト更新
        TextUpdate();

        //  Input
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _charaType++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _charaType--;
        }
        else
        {
            return;
        }

        // 範囲外処理
        if (_IsOnNone)
        {
            if (_charaType >= CharactorSelect.eCharaType.MAX)
            {
                _charaType = (CharactorSelect.eCharaType)0;
            }
            else if (_charaType < (CharactorSelect.eCharaType)0)
            {
                _charaType = (CharactorSelect.eCharaType)(CharactorSelect.eCharaType.MAX - 1);
            }
        }
        else
        {
            if (_charaType >= CharactorSelect.eCharaType.MAX)
            {
                _charaType = (CharactorSelect.eCharaType)1;
            }
            else if (_charaType < (CharactorSelect.eCharaType)1)
            {
                _charaType = (CharactorSelect.eCharaType)(CharactorSelect.eCharaType.MAX - 1);
            }
        }
	}

    public void TextUpdate()
    {
        switch (_charaType)
        {
            case CharactorSelect.eCharaType.NONE:
                _text.text = "None";
                break;
            case CharactorSelect.eCharaType.BALANCE:
                _text.text = "Balance";
                break;
            case CharactorSelect.eCharaType.POWER:
                _text.text = "Power";
                break;
            case CharactorSelect.eCharaType.SPEED:
                _text.text = "Speed";
                break;
            case CharactorSelect.eCharaType.TECHNICAL:
                _text.text = "Technical";
                break;
        }
    }

    void OnEnable()
    {
        _text.color = Color.red;
    }

    void OnDisable()
    {
        _text.color = Color.black;
    }
}
