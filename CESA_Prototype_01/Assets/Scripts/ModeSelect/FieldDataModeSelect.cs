using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using UniRx;
using UniRx.Triggers;

public class FieldDataModeSelect : FieldData
{
    protected override void Init()
    {
        GameScaler._nWidth = 17;
        GameScaler._nHeight = 13;

        //  データ配列生成
        _ObjectDataArray = new FieldObjectBase[GameScaler._nWidth * GameScaler._nHeight];
        _ChangeDataList = new tChangeData[GameScaler._nWidth * GameScaler._nHeight];

        //  フィールドにオブジェクトを生成し、データを格納
        FieldCreatorModeSelect creator = new FieldCreatorModeSelect();
        creator.Create();

        UpdateStart();
        StartCoroutine(CharaSet());

        //GameObject readyGo = Resources.Load<GameObject>("Prefabs/GameMain/ReadyGo");
        //Instantiate(readyGo, readyGo.transform.position, Quaternion.identity);
    }
}
