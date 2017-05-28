using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScaler
{
    static public float _fScale = 1.0f;

    static public int _nWidth  = 12;
    static public int _nHeight = 10;
    static public int GetRange { get { return _nHeight * _nWidth; } }

    static public int _nSandRange = 1;  
}
