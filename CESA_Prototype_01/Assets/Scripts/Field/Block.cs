using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : FieldObjectBase 
{
    void Awake()
    {
        FieldData.Instance.SetObjData(this, GetDataNumber());
    }

    void Update ()
    {
        DataUpdate();
    }
}
