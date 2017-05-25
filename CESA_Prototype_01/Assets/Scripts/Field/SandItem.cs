using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class SandItem : FieldObjectBase
{
    static GameObject _SandItemHolder = null;

    public enum eType
    {
        ONE_P = 0,
        TWO_P,
        THREE_P,
        FOUR_P,
        BLOCK,

        MAX,
    };
    [SerializeField] eType _Type;
    public eType GetType { get { return _Type; } set { _Type = value; } }
    [SerializeField]
    float _fMaxLife_Sec = 15.0f; 

    void Awake()
    {
        if (_SandItemHolder)
            return;
        
        _SandItemHolder = new GameObject ("SandItemHolder");
    }

    void Start()
    {
        _sandItemData = this;
        transform.SetParent(_SandItemHolder.transform);

        float life = _fMaxLife_Sec;
        MeshRenderer meRend = GetComponentInChildren<MeshRenderer>();
        Color initColor = meRend.material.color;
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                life -= Time.deltaTime;

                float per = life / _fMaxLife_Sec;
                if (per <= 1.0f /3.0f)
                {
                    per *= 3.0f;
                    initColor.a = 1.0f * per;
                    meRend.sharedMaterial.color = initColor;
                }

                if (life > 0.0f)
                    return;

                FieldData.Instance.SetObjData(null, GetDataNumber());
                FieldData.Instance.ExceptionChangeField();
                Destroy(this.gameObject);
            });
    }
	
    void Update()
    {
        DataUpdate();
    }
}
