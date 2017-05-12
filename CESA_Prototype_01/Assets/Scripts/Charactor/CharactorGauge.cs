using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorGauge : MonoBehaviour
{
    public float _fGauge { get; private set; }
    public float GaugePercent { get { return _fGauge / _fMaxGauge; } }
    [SerializeField] float _fChargeSpeed = 1.0f;
    [SerializeField] float _fMaxGauge = 5.0f;

    [SerializeField] float _fPutGauge = 2.0f;
    [SerializeField] float _fBreakGauge = 2.0f;

    void Update()
    {
        _fGauge += Time.deltaTime / _fChargeSpeed;
        if (_fGauge > _fMaxGauge)
            _fGauge = _fMaxGauge;
        //Debug.Log(_fGauge);
    }

    public bool PutGaugeCheck()
    {
        return _fGauge >= _fPutGauge;
    }

    public void PutAction()
    {
        _fGauge -= _fPutGauge;
    }

    public bool BreakGaugeCheck()
    {
        return _fGauge >= _fBreakGauge;
    }

    public void BreakAction()
    {
        _fGauge -= _fBreakGauge;
    }
}
