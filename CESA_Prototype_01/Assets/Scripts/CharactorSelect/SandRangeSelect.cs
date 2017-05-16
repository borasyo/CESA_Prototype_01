using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class SandRangeSelect : MonoBehaviour
{
    Text _text = null;
    [SerializeField] int _size = 1;
    [SerializeField] StageScaleSelect _width;
    [SerializeField] StageScaleSelect _height;

    float _fNowInterval = 0.0f;
    float _fInterval = 0.2f;

    // Use this for initialization
    void Start () 
    {
        _text = GetComponent<Text>();
        _fNowInterval = _fInterval;
    }

    // Update is called once per frame
    void Update () 
    {
        //  動的に変更するため毎回チェック
        int max = (_width.GetSize > _height.GetSize ? _width.GetSize : _height.GetSize);
        if (_size > max)
            _size = max;

        _text.text = "SandRange : " + _size;
        
        _fNowInterval += Time.deltaTime;
        if (_fNowInterval < _fInterval)
            return;
        
        if (Input.GetKey(KeyCode.A))
        {
            _size++;
            _fNowInterval = 0.0f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            _size--;
            _fNowInterval = 0.0f;
            if (_size < 1)
                _size = 1;
        }
    }

    void OnDisable()
    {
        GameScaler._nSandRange = _size;
    }
}
