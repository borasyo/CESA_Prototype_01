﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetCameraToCanvas : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
	}
}