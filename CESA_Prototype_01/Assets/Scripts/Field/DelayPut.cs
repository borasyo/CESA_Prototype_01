using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class DelayPut : MonoBehaviour 
{  
    float _fDelayTime_Sec = 3.0f;
    float DangerTime { get { return 1.0f; } }

    MeshRenderer _meRend = null;
    Vector3 _InitScale = Vector3.zero;

    static GameObject effectPrefab = null;

    public SandItem.eType SetType { get; set; }

    public IEnumerator Init (int number)
    {
        _meRend = GetComponentInChildren<MeshRenderer>();
        DelayPut me = GetComponent<DelayPut>();

        int nSetNumber = number;

        yield return new WaitForSeconds(1.0f);

        _InitScale = transform.localScale;
        transform.localScale = Vector3.zero;

        FieldData.Instance.SetObjData(this.GetComponent<FieldObjectBase>(), nSetNumber);

        this.UpdateAsObservable()
            .Where(_ => me && enabled)
            .Subscribe(_ => 
            {
                _fDelayTime_Sec -= Time.deltaTime;

                if (_fDelayTime_Sec > 0.0f)
                    return;

                //  子も表示
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(true);
                }

                GetComponent<SandItem>().GetType = SetType;
                FieldData.Instance.ExceptionChangeField();
                SoundManager.Instance.PlaySE(SoundManager.eSeValue.PUT);
                Destroy(me);
            });

        this.ObserveEveryValueChanged(x => x._fDelayTime_Sec)
            .Where(_ => DangerTime >= this._fDelayTime_Sec)
            .Subscribe(_ => {
                _meRend.enabled = true;
            });

        if (!effectPrefab)
            effectPrefab = Resources.Load("Prefabs/Effect/DelayPutEffect") as GameObject;

        GameObject effect = Instantiate(effectPrefab, transform.position, transform.rotation);
        effect.GetComponent<ParticleSystem>().startColor = GetColor();
    }

    void Update ()
    {
        if (!_meRend.enabled)
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
            default:
                col = Color.gray;
                break;
        }

        return col;
    }
}
