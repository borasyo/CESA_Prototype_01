using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class ItemBase : FieldObjectBase
{
    public enum eItemType
    {
        MOVEUP = 0,
        GAUGEUP,
        INVINCIBLE,
        SPECIAL,
    };
    protected eItemType _itemType;
    public eItemType GetItemType { get { return _itemType; } }

    protected bool _IsCollision = false;
    [SerializeField] float _fLife = 10.0f;

    public void Start()
    {
        // Holderに追加
        ItemHolder.Instance.Add(this);

        //  CollisionCheck
        this.UpdateAsObservable()
            .Where(_ => !_IsCollision)
            .Subscribe(_ => {
                List<Character> charactors = FieldData.Instance.GetCharactors;

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
                    if (!obj || obj.tag == "Character")
                        continue;

                    if (obj.GetDataNumber() != GetDataNumber())
                        continue;

                    Destroy();
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

                Destroy ();
            });
    }

    void OnCollsion()
    {
        FieldObjectBase obj = FieldData.Instance.GetObjData(GetDataNumber());
        transform.SetParent(obj.transform);
        transform.GetComponentInChildren<MeshRenderer>().enabled = false;
        ItemHolder.Instance.Remove(this);
    }

    protected void Destroy()
    {
        ItemHolder.Instance.Remove(this);
        Destroy(gameObject);
    }

    virtual public void Run()
    {
        //  継承先で記述
    }
}
