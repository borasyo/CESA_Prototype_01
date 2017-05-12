using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectMassSprite : MonoBehaviour 
{
    SpriteRenderer _SpRend = null;
    Charactor _charactor = null;
    CharactorGauge _charactorGauge = null;

    Sprite _defaultSprite = null;
    Sprite _notSprite = null;

	// Use this for initialization
	void Start () 
    {
        _SpRend = GetComponent<SpriteRenderer>();
        _charactor = GetComponentInParent<Charactor>();
        _charactorGauge = GetComponentInParent<CharactorGauge>();

        _defaultSprite = _SpRend.sprite;
        _notSprite = Resources.Load<Sprite>("Texture/Not");
	}
	
	// Update is called once per frame
	void Update () 
    {
        SpriteCheck();
	}

    void SpriteCheck()
    {
        int number = _charactor.GetDataNumberForDir();

        //  置ける、壊せる、何もできないを判定
        FieldObjectBase obj = FieldData.Instance.GetObjData(number);
        Sprite setSprite = _defaultSprite;
        if (obj)
        {
            if (obj.tag != "SandItem" || !_charactorGauge.BreakGaugeCheck())
            {
                setSprite = _notSprite;
            }
        }
        else if(!_charactorGauge.PutGaugeCheck())
        {
            setSprite = _notSprite;
        }

        _SpRend.sprite = setSprite;
    }
}
