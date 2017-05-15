using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class MoveSpeedUp : ItemBase 
{
    [SerializeField] float _fUpAmountPer = 2.0f;
    [SerializeField] float _fDuration_Sec = 5.0f;

    Charactor _charactor = null;

    override public void Run()
    {
        _charactor = this.GetComponentInParent<Charactor>();
        _charactor.ChangeSpeed(_fUpAmountPer);

        this.UpdateAsObservable()
            .Subscribe(_ => {
                _fDuration_Sec -= Time.deltaTime;

                if(_fDuration_Sec > 0.0f)
                    return;
              
                _charactor.ChangeSpeed(1.0f / _fUpAmountPer);
                Destroy(this.gameObject);
            });
    }
}
