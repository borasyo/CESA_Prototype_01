using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageScaleSelect : MonoBehaviour 
{
    [SerializeField] bool _IsWidth = true;
    Text _text = null;
    [SerializeField] int _size = 0;

	// Use this for initialization
	void Start () 
    {
        _text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (_IsWidth)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                _size++;
                if (_size > 30)
                    _size = 30;
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                _size--;
                if (_size < 6)
                    _size = 6;
            }

            _text.text = "Width : " + _size;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                _size++;
                if (_size > 25)
                    _size = 25;
            }
            else if (Input.GetKeyDown(KeyCode.V))
            {
                _size--;
                if (_size < 5)
                    _size = 5;
            }

            _text.text = "Height : " + _size;
        }
	}

    void OnDisable()
    {
        if (_IsWidth)
        {
            GameScaler._nWidth = _size;
        }
        else
        {
            GameScaler._nHeight = _size;
        }
    }
}
