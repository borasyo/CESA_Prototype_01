using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullsObject : MonoBehaviour 
{
    [SerializeField] float _fAmount = 0.5f;
    [SerializeField] float _fTime_Sec = 1.0f;
	
	// Update is called once per frame
	void Update () 
    {
        transform.position += new Vector3(0, _fAmount * (Time.deltaTime/_fTime_Sec) , 0);	
        transform.eulerAngles += new Vector3(0, 360 * (Time.deltaTime / _fTime_Sec), 0);
	}
}
