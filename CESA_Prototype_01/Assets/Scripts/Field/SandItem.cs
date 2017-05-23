using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Awake()
    {
        if (_SandItemHolder)
            return;
        
        _SandItemHolder = new GameObject ("SandItemHolder");
    }

    void Start()
    {
        _sandItem = this;
        transform.SetParent(_SandItemHolder.transform);
    }
	
    void Update()
    {
        DataUpdate();
    }
}
