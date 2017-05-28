using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCameraPos : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
    {
        float x = (float)(GameScaler._nWidth - 1) / 2.0f * GameScaler._fScale;
        float z = -((float)(GameScaler._nWidth - GameScaler._nHeight) / 5.0f * GameScaler._fScale);
        float y = (GameScaler._nWidth + GameScaler._nHeight) / 10.0f * (GameScaler._fScale * 5.0f);
        Camera.main.transform.position = new Vector3(x,y,z);
   	}
}
