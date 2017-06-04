using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : FieldObjectBase
{
    [SerializeField]
    GameObject _breakEffect = null;

    void Awake()
    {
        FieldData.Instance.SetObjData(this, GetDataNumber());
    }

    void Update ()
    {
        DataUpdate();
    }

    public void Init()
    {
        transform.SetParent(GameObject.Find("InitFieldObjHolder").transform);
    }

    public void Break()
    {
        FieldData.Instance.SetObjData(null, GetDataNumber());
        FieldData.Instance.ExceptionChangeField();
        Instantiate(_breakEffect, transform.position, _breakEffect.transform.rotation);
        Destroy(this.gameObject);
    }
}
