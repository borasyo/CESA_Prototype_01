﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCameraPos : MonoBehaviour 
{
	// Use this for initialization
	public void Init () 
    {
        float x = (float)(GameScaler._nWidth - 1) / 2.0f * GameScaler._fScale;
        float z = ((float)(GameScaler._nWidth - GameScaler._nHeight) * 0.1f * GameScaler._fScale);
        if (StageScaler.GetScale() == 1)
            z += 0.5f;
        float y = (GameScaler._nWidth + GameScaler._nHeight) / 10.0f * (GameScaler._fScale * 4.5f);
        Camera.main.transform.position = new Vector3(x,y,z);

        Camera.main.transform.eulerAngles = new Vector3(70, 0, 0);
   	}

    public void AdjustmentPos(Vector3 pos)
    {
        Camera.main.transform.position += pos;
    }
}
