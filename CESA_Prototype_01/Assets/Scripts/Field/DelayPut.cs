using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class DelayPut : MonoBehaviour 
{  
    float _fDelayTime_Sec = 3.0f;
    float DangerTime { get { return 1.0f; } }

    MeshRenderer _MeRend = null;
    Vector3 _InitScale = Vector3.zero;

    static GameObject effectPrefab = null;

    public void Init (int number)
    {
        DelayPut me = GetComponent<DelayPut>();

        int nSetNumber = number;
        SandItem.eType SetType = GetComponent<SandItem>().GetType;
        GetComponent<SandItem>().GetType = SandItem.eType.MAX;

        _MeRend = GetComponentInChildren<MeshRenderer>();
        _MeRend.enabled = false;

        //  子も隠す
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        _InitScale = transform.localScale;
        transform.localScale = Vector3.zero;

        FieldData.Instance.SetObjData(this.GetComponent<FieldObjectBase>(), nSetNumber);

        this.UpdateAsObservable()
            .Where(_ => me)
            .Subscribe(_ => 
            {
                _fDelayTime_Sec -= Time.deltaTime;        

                if(_fDelayTime_Sec > 0.0f)
                    return;

                //  子も表示
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(true);
                }

                GetComponent<SandItem>().GetType = SetType;
                FieldData.Instance.ExceptionChangeField();
                Destroy(me);
            });

        this.ObserveEveryValueChanged(x => x._fDelayTime_Sec)
            .Where(_ => DangerTime >= this._fDelayTime_Sec)
            .Subscribe(_ => {
                _MeRend.enabled = true;
            });

        if (!effectPrefab)
            effectPrefab = Resources.Load("Prefabs/Effect/DelayPutEffect") as GameObject;

        GameObject effect = Instantiate(effectPrefab, transform.position, transform.rotation);
        effect.GetComponent<ParticleSystem>().startColor = GetColor();
    }

    void Update ()
    {
        if (!_MeRend.enabled)
            return;

        transform.localScale += _InitScale * (Time.deltaTime / DangerTime);
    }

    Color GetColor()
    {
        Color col = Color.black;

        switch(name[name.IndexOf("(") - 1].ToString())
        {
            case "1":
                col = Color.red;
                break;
            case "2":
                col = Color.blue;
                break;
            case "3":
                col = Color.green;
                break;
            case "4":
                col = Color.yellow;
                break;
        }

        return col;
    }
}
