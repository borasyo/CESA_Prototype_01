using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectMassSprite : MonoBehaviour 
{
    protected SpriteRenderer _SpRend = null;
    protected Character _character = null;
    protected CharacterGauge _charactorGauge = null;

    protected Sprite _defaultSprite = null;
    protected Sprite _notSprite = null;

	// Use this for initialization
	void Start () 
    {
        _SpRend = GetComponent<SpriteRenderer>();
        _character = GetComponentInParent<Character>();
        _charactorGauge = GetComponentInParent<CharacterGauge>();

        _defaultSprite = _SpRend.sprite;
        _notSprite = Resources.Load<Sprite>("Texture/GameMain/Not");
	}
	
	// Update is called once per frame
	void Update () 
    {
        SpriteCheck();
	}

    virtual protected void SpriteCheck()
    {
        int number = _character.GetDataNumberForDir();
        _SpRend.sprite = _defaultSprite;

        //  置ける、壊せる、何もできないを判定
        FieldObjectBase obj = FieldData.Instance.GetObjData(number);
        if (obj)
        {
            if (obj.GetSandType() != SandItem.eType.MAX && _charactorGauge.BreakGaugeCheck())
            {
                return;
            }
        }
        else if(_charactorGauge.PutGaugeCheck())
        {
            return;
        }

        _SpRend.sprite = _notSprite;
        transform.localScale = new Vector3(1,1,1);
    }
}
