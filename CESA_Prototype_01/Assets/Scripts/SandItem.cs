using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandItem : FieldObjectBase
{
    static GameObject _SandItemHolder = null;

    public enum eType
    {
        PLAYER = 0,
        ENEMY,

        MAX,
    };
    [SerializeField] eType _Type;
    public eType GetType { get { return _Type; } private set { _Type = value; } }

    void Awake()
    {
        if (_SandItemHolder)
            return;
        
        _SandItemHolder = new GameObject("SandItemHolder");
    }

/*    public void SetType(string name)
    {
        if(name.Contains("Player"))
            _Type = eType.PLAYER;
        else if (name.Contains("Enemy"))
            _Type = eType.ENEMY;
    }*/

    void Start()
    {
        transform.SetParent(_SandItemHolder.transform);
    }
	
    void Update()
    {
		
    }
}
