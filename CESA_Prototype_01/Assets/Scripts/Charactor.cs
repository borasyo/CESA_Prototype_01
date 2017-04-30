using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charactor : FieldObjectBase00
{
    CharactorInput _chatactorInput = null;
    [SerializeField] Vector3 _moveAmount_Sec = new Vector3(1,0,1);

    // Use this for initialization
    void Start()
    {
        _chatactorInput = GetComponent<CharactorInput>();
    }
	
    // Update is called once per frame
    void Update()
    {
        if (_chatactorInput.GetInput(CharactorInput.eDirection.FORWARD))
        {
            transform.position += new Vector3(0,0, _moveAmount_Sec.z) * Time.deltaTime;
        }
        if(_chatactorInput.GetInput(CharactorInput.eDirection.BACK)) 
        {
            transform.position -= new Vector3(0,0, _moveAmount_Sec.z) * Time.deltaTime;
        }
        if(_chatactorInput.GetInput(CharactorInput.eDirection.RIGHT)) 
        {
            transform.position += new Vector3(_moveAmount_Sec.x, 0,0) * Time.deltaTime;
        }
        if(_chatactorInput.GetInput(CharactorInput.eDirection.LEFT)) 
        {
            transform.position -= new Vector3(_moveAmount_Sec.x, 0,0) * Time.deltaTime;
        }
    }
}
