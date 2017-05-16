﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class AutoMoveObj : MonoBehaviour 
{
    FieldObjectBase _fieldObjBase = null;

    public void Init (Charactor.eDirection dir, Vector3 move)
    {
        _fieldObjBase = GetComponent<FieldObjectBase>();
        AutoMoveObj me = GetComponent<AutoMoveObj>(); 
        transform.name += "Kick";

        this.UpdateAsObservable()
            .Where(_ => me)
            .Subscribe(_ =>
            {
                if (!FieldData.Instance.GetObjData(GetDataNumberForDir(dir)))
                {
                    int oldNumber = _fieldObjBase.GetDataNumber(); 
                    transform.position += move * Time.deltaTime;
                    
                    if(oldNumber == _fieldObjBase.GetDataNumber())
                        return;

                    FieldData.Instance.SetObjData(null, oldNumber);
                }
                else
                {
                    transform.position = _fieldObjBase.GetPosForNumber();
                    transform.name.Replace("Kick", "");
                    Destroy(me);
                }
            });
    }

    //  向いている方向を元にデータ番号を取得
    public int GetDataNumberForDir(Charactor.eDirection dir)
    {
        int number = _fieldObjBase.GetDataNumber();
        switch(dir)
        {
            case Charactor.eDirection.FORWARD:
                number += GameScaler._nWidth;
                break;
            case Charactor.eDirection.BACK:
                number -= GameScaler._nWidth;
                break;
            case Charactor.eDirection.RIGHT:
                number += 1;
                break;
            case Charactor.eDirection.LEFT:
                number -= 1;
                break;
        }

        return number;
    }
}