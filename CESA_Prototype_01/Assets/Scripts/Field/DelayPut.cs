using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class DelayPut : MonoBehaviour 
{
    int _nSetNumber = 0;
    SandItem.eType _SetType;
   
    float _fDelayTime_Sec = 3.0f;
    //float DangerTime { get { return 1.0f; } }

    MeshRenderer _MeRend = null;
    //TriangleWave<Vector3> _scaleChanger = null;

    public void Init (int number)
    {
        _nSetNumber = number;
        _MeRend = GetComponentInChildren<MeshRenderer>();
        _MeRend.enabled = false;
        _SetType = GetComponent<SandItem>().GetType;
        GetComponent<SandItem>().GetType = SandItem.eType.MAX;
        FieldData.Instance.SetObjData(this.GetComponent<FieldObjectBase>(), _nSetNumber);

        this.UpdateAsObservable().Subscribe(_ => 
            {
                _fDelayTime_Sec -= Time.deltaTime;        

                if(_fDelayTime_Sec > 0.0f)
                    return;

  //              FieldData.Instance.SetObjData(this.GetComponent<FieldObjectBase>(), _nSetNumber);
                _MeRend.enabled = true;
                GetComponent<SandItem>().GetType = _SetType;
                //Destroy(GetComponent<DelayPut>());
            });

        /*this.ObserveEveryValueChanged(x => x._fDelayTime_Sec)
            .Where(_ => DangerTime > this._fDelayTime_Sec)
            .Subscribe(_ => {
                _scaleChanger = TriangleWaveFactory.Vector3(Vector3.zero, Vector3.zero, 0.01f);
                _MeRend.enabled = true;

                //  点滅処理
                this.UpdateAsObservable()
                    .Where(i => _fDelayTime_Sec <= DangerTime)
                    .Subscribe(i => {
                        float nowMax = 1.0f - _fDelayTime_Sec;
                        _scaleChanger.SetRange(Vector3.zero, new Vector3(nowMax, nowMax, nowMax));
                        _scaleChanger.Progress();
                        transform.localScale = _scaleChanger.CurrentValue;
                    });
            });*/
    }

}
