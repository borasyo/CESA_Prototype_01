using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandItem : FieldObjectBase
{
    static GameObject _SandItemHolder = null;

    void Awake()
    {
        if (_SandItemHolder)
            return;
        
        _SandItemHolder = new GameObject("SandItemHolder");
    }

    void Start()
    {
        transform.SetParent(_SandItemHolder.transform);
    }
	
    void Update()
    {
		
    }
}
