using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldCreatorTutorial : FieldCreator
{
    protected override void CreateChara()
    {
        Vector3 pos = Vector3.zero;
        GameObject obj = null;
        GameObject[] SelectCharas = CharacterSelect.SelectCharas;
        int[] SelectLevels = LevelSelect.SelectLevel;

        // 左下に生成
        SelectCharas[0] = Resources.Load<GameObject>("Prefabs/Chara/Tutorial");
        pos = new Vector3(1.0f * GameScaler._fScale, 0.0f, (int)(1.0f * GameScaler._fScale) * GameScaler._nHeight / 2);
        obj = CreateObj(SelectCharas[0], pos);
        obj.name += ",1Player";
        obj.transform.eulerAngles = new Vector3(0, 90, 0);
        _objBaseArray[_nWidth + 1] = obj.GetComponent<FieldObjectBase>();
        obj.GetComponent<Character>().Init(0);

        //  右上に生成
        SelectCharas[1] = Resources.Load<GameObject>("Prefabs/Chara/Balance");
        pos = new Vector3((_nWidth - 2.0f) * GameScaler._fScale, 0.0f, (int)(1.0f * GameScaler._fScale) * GameScaler._nHeight / 2);
        obj = CreateObj(SelectCharas[1], pos);
        obj.name += ",2Player" + ",CPU";
        obj.transform.eulerAngles = new Vector3(0, 270, 0);
        _objBaseArray[_nWidth * (_nHeight - 2) + _nWidth - 2] = obj.GetComponent<FieldObjectBase>();
        obj.GetComponent<Character>().Init(-1);
    }

    protected override bool StageBlock(int x, int z)
    {
        return StageTwo(x, z);
    }

}
