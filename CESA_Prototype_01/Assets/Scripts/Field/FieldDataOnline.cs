using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class FieldDataOnline : FieldData
{
    GameObject _BlockObj = null;

    protected override void Init()
    {
        //  データ配列生成
        _ObjectDataArray = new FieldObjectBase[GameScaler._nWidth * GameScaler._nHeight];
        _ChangeDataList = new tChangeData[GameScaler._nWidth * GameScaler._nHeight];

        //  フィールドにオブジェクトを生成し、データを格納
        FieldCreatorOnline creator = new FieldCreatorOnline();
        //_ObjectDataArray = 
        creator.Create(GameScaler._nWidth, GameScaler._nHeight);

        _CharaList = _ObjectDataArray.Where(_ => _ && _.tag == "Character").Select(_ => _.GetComponent<Character>()).ToList();
    }

    [PunRPC]
    public void CreateBlock(Vector3 pos)
    {
        if(!_BlockObj)
            _BlockObj = Resources.Load<GameObject>("Prefabs/Field/Block");

        GameObject obj = (GameObject)Instantiate(_BlockObj, pos, _BlockObj.transform.rotation);
    }
}
