using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class InvincibleItem : ItemBase
{
    [SerializeField] float _fDuration_Sec = 5.0f;
    //Character _character = null;

    void Start()
    {
        _itemType = ItemBase.eItemType.INVINCIBLE;
        base.Start();
    }

    override public void Run()
    {
        //_character = this.GetComponentInParent<Character>();
        transform.parent.name += ",Invincible"; //  無敵開始

        this.UpdateAsObservable()
            .Subscribe(_ => {
                _fDuration_Sec -= Time.deltaTime;

                if(_fDuration_Sec > 0.0f)
                    return;
                
                transform.parent.name = transform.parent.name.Replace(",Invincible", ""); // 無敵終了
                Destroy(this.gameObject);

            });
    }
}
