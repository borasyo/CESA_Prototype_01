﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageScaleSelect : Photon.MonoBehaviour 
{
    [SerializeField] protected bool _IsWidth = true;
    protected Text _text = null;
    protected int _size = 0;
    public int GetSize { get { return _size; } }

    protected float _fNowInterval = 0.0f;
    protected float _fInterval = 0.2f;

    // Use this for initialization
    void Start () 
    {
        if (_IsWidth)
            _size = GameScaler._nWidth  - 2;
        else
            _size = GameScaler._nHeight - 2;
        _text = GetComponent<Text>();
        _fNowInterval = _fInterval;
	}

    // Update is called once per frame
    void Update()
    {
        UpDown();
    }

    protected virtual void UpDown()
    {  
        _fNowInterval += Time.deltaTime;
        if (_fNowInterval < _fInterval)
            return;

        if (_IsWidth)
        {
            if (Input.GetKey(KeyCode.Z))
            {
                _size++;
                _fNowInterval = 0.0f;
                if (_size > 30)
                    _size = 30;
            }
            else if (Input.GetKey(KeyCode.X))
            {
                _size--;
                _fNowInterval = 0.0f;
                if (_size < 6)
                    _size = 6;
            }

            // 毎回チェック
            if (_size < GameScaler._nHeight - 2)
                _size = GameScaler._nHeight - 2;

            GameScaler._nWidth = _size + 2;
            _text.text = "Width : " + _size;
        }
        else
        {
            if (Input.GetKey(KeyCode.C))
            {
                _size++;
                _fNowInterval = 0.0f;
                if (_size > 25)
                    _size = 25;
            }
            else if (Input.GetKey(KeyCode.V))
            {
                _size--;
                _fNowInterval = 0.0f;
                if (_size < 5)
                    _size = 5;
            }

            GameScaler._nHeight = _size + 2;
            _text.text = "Height : " + _size;
        }
	}

    void OnDisable()
    {
        // + 2 は外を囲う分
        if (_IsWidth)
        {
            GameScaler._nWidth = _size + 2;
        }
        else
        {
            GameScaler._nHeight = _size + 2;
        }
    }

    public virtual void UpSize()
    {
        if (_IsWidth)
        {
            _size++;
            _fNowInterval = 0.0f;
            if (_size > 30)
                _size = 30;
        }
        else
        {
            _size++;
            _fNowInterval = 0.0f;
            if (_size > 25)
                _size = 25;
        }
    }

    public virtual void DownSize()
    {
        if (_IsWidth)
        {
            _size--;
            _fNowInterval = 0.0f;
            if (_size < 6)
                _size = 6;
        }
        else
        {
            _size--;
            _fNowInterval = 0.0f;
            if (_size < 5)
                _size = 5;
        }
    }
}
