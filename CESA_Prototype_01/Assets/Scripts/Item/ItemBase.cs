using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class ItemBase : FieldObjectBase
{
    protected bool _IsCollision = false;
    [SerializeField] float _fLife = 10.0f;

    public void Start()
    {
        //  CollisionCheck
        this.UpdateAsObservable()
            .Where(_ => !_IsCollision)
            .Subscribe(_ => {
                List<FieldObjectBase> charactors = FieldData.Instance.GetCharactors;

                foreach (FieldObjectBase chara in charactors)
                {
                    if(!chara)
                        continue;

                    if (chara.GetDataNumber() != GetDataNumber())
                        continue;

                    _IsCollision = true;
                    OnCollsion();
                    Run();
                }
            });
        
        //  DestroyCheck
        this.UpdateAsObservable()
            .Where(_ => this.enabled && !_IsCollision)
            .Subscribe(_ => {
                FieldObjectBase[] objs = FieldData.Instance.GetObjDataArray;

                foreach (FieldObjectBase obj in objs)
                {
                    if (!obj || obj.tag == "Charactor")
                        continue;

                    if (obj.GetDataNumber() != GetDataNumber())
                        continue;

                    Destroy (this.gameObject);
                    break;
                }
            });

        // LifeCheck
        this.UpdateAsObservable()
            .Where(_ => this.enabled && !_IsCollision)
            .Subscribe(_ => {
                _fLife -= Time.deltaTime;

                if(_fLife > 0.0f)
                    return;

                Destroy (this.gameObject);
            });
    }

    void OnCollsion()
    {
        FieldObjectBase obj = FieldData.Instance.GetObjData(GetDataNumber());
        transform.SetParent(obj.transform);
        transform.GetComponentInChildren<MeshRenderer>().enabled = false;
    }

    virtual public void Run()
    {
        //  継承先で記述
    }
}
