using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectMassSprite : MonoBehaviour 
{
    protected SpriteRenderer _SpRend = null;
    protected Charactor _charactor = null;
    protected CharactorGauge _charactorGauge = null;

    protected Sprite _defaultSprite = null;
    protected Sprite _notSprite = null;

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

    virtual protected void SpriteCheck()
    {
        int number = _charactor.GetDataNumberForDir();
        _SpRend.sprite = _defaultSprite;

        //  置ける、壊せる、何もできないを判定
        FieldObjectBase obj = FieldData.Instance.GetObjData(number);
        if (obj)
        {
            if (obj.tag == "SandItem" && _charactorGauge.BreakGaugeCheck())
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
