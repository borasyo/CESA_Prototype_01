using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGauge : MonoBehaviour
{
    public float _fGauge { get; private set; }
    public float GaugePercent { get { return _fGauge / _fMaxGauge; } }
    [SerializeField] float _fChargeSpeed = 1.0f;
    [SerializeField] float _fMaxGauge    = 5.0f;

    [SerializeField] float _fPutGauge   = 2.0f;
    public float GetPutGauge { get { return _fPutGauge; } }
    [SerializeField] float _fBreakGauge = 2.0f;
    public float GetBreakGauge { get { return _fBreakGauge; } }

#if DEBUG
    bool isDebug = false;
#endif

    void Update()
    {
        _fGauge += Time.deltaTime * _fChargeSpeed;
        if (_fGauge > _fMaxGauge)
            _fGauge = _fMaxGauge;
        //Debug.Log(_fGauge);

#if DEBUG
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            isDebug = !isDebug;
            if (isDebug)
                GaugeMax();
        }
#endif
    }

    #region Gauge

    public void GaugeMax()
    {
        _fGauge = _fMaxGauge;
    }

    public void ChangeChargeSpeed(float per)
    {
        _fChargeSpeed *= per;
    }

    public void SetChargeSpeed(float speed)
    {
        _fChargeSpeed = speed;
    }

    #endregion

    #region PutGauge

    public bool PutGaugeCheck()
    {
        return _fGauge >= _fPutGauge;
    }

    public void PutAction()
    {
#if DEBUG
        if (isDebug)
            return;
#endif
        _fGauge -= _fPutGauge;
    }

    public void ChangePutGauge(float per)
    {
        _fPutGauge *= per;
    }

    public void SetPutGauge(float speed)
    {
        _fPutGauge = speed;
    }

    #endregion

    #region BreakGauge

    public bool BreakGaugeCheck()
    {
        return _fGauge >= _fBreakGauge;
    }

    public void BreakAction()
    {
#if DEBUG
        if (isDebug)
            return;
#endif
        _fGauge -= _fBreakGauge;
    }

    public void ChangeBreakGauge(float per)
    {
        _fBreakGauge *= per;
    }

    public void SetBreakGauge(float speed)
    {
        _fBreakGauge = speed;
    }

    #endregion
}
