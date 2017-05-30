using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldCreatorOnline : FieldCreator
{
    protected override void CreateChara()
    {
        Vector3 pos = Vector3.zero;
        GameObject obj = null;
        GameObject[] SelectCharas = CharacterSelect.SelectCharas;
        int[] SelectLevels = LevelSelect.SelectLevel;

        if (PhotonNetwork.isMasterClient)
        {
            // 左下に生成
            pos = new Vector3(1.0f * GameScaler._fScale, 0.0f, 1.0f * GameScaler._fScale);
            obj = CreateCharaObj(SelectCharas[0], pos);
            obj.name += ",1Player";
            obj.transform.eulerAngles = new Vector3(0, 90, 0);
            _objBaseArray[_nWidth + 1] = obj.GetComponent<FieldObjectBase>();
            obj.GetComponent<Character>().Init(0);

            //  右上に生成
            if (SelectCharas[1] && SelectLevels[1] >= 0)
            {
                pos = new Vector3((_nWidth - 2.0f) * GameScaler._fScale, 0.0f, (_nHeight - 2.0f) * GameScaler._fScale);
                obj = CreateCharaObj(SelectCharas[1], pos);
                obj.name += ",2Player" + ",CPU";
                obj.transform.eulerAngles = new Vector3(0, 270, 0);
                _objBaseArray[_nWidth * (_nHeight - 2) + _nWidth - 2] = obj.GetComponent<FieldObjectBase>();
                obj.GetComponent<Character>().Init(SelectLevels[1]);
                Debug.Log("a");
            }

            //  左上に生成
            if (SelectCharas[2] && SelectLevels[2] >= 0)
            {
                pos = new Vector3(1.0f * GameScaler._fScale, 0.0f, (_nHeight - 2.0f) * GameScaler._fScale);
                obj = CreateCharaObj(SelectCharas[2], pos);
                obj.name += ",3Player" + ",CPU";
                obj.transform.eulerAngles = new Vector3(0, 90, 0);
                _objBaseArray[1 + _nWidth * (_nHeight - 2)] = obj.GetComponent<FieldObjectBase>();
                obj.GetComponent<Character>().Init(SelectLevels[2]);
            }

            //  右上に生成
            if (SelectCharas[3] && SelectLevels[3] >= 0)
            {
                pos = new Vector3((_nWidth - 2.0f) * GameScaler._fScale, 0.0f, 1.0f * GameScaler._fScale);
                obj = CreateCharaObj(SelectCharas[3], pos);
                obj.name += ",4Player" + ",CPU";
                obj.transform.eulerAngles = new Vector3(0, 270, 0);
                _objBaseArray[(_nWidth - 2) + _nWidth] = obj.GetComponent<FieldObjectBase>();
                obj.GetComponent<Character>().Init(SelectLevels[3]);
            }
        }
        else
        {
            int number = CharacterSelectOnline._nMyNumber;
            switch(number)
            {
                case 1:
                    pos = new Vector3((_nWidth - 2.0f) * GameScaler._fScale, 0.0f, (_nHeight - 2.0f) * GameScaler._fScale);
                    obj = CreateCharaObj(SelectCharas[1], pos);
                    obj.name += ",2Player";
                    obj.transform.eulerAngles = new Vector3(0, 270, 0);
                    _objBaseArray[_nWidth * (_nHeight - 2) + _nWidth - 2] = obj.GetComponent<FieldObjectBase>();
                    obj.GetComponent<Character>().Init(0);
                    Debug.Log("b");
                    break;
                case 2:
                    pos = new Vector3(1.0f * GameScaler._fScale, 0.0f, (_nHeight - 2.0f) * GameScaler._fScale);
                    obj = CreateCharaObj(SelectCharas[2], pos);
                    obj.name += ",3Player";
                    obj.transform.eulerAngles = new Vector3(0, 90, 0);
                    _objBaseArray[1 + _nWidth * (_nHeight - 2)] = obj.GetComponent<FieldObjectBase>();
                    obj.GetComponent<Character>().Init(0);
                    break;
                case 3:
                    pos = new Vector3((_nWidth - 2.0f) * GameScaler._fScale, 0.0f, 1.0f * GameScaler._fScale);
                    obj = CreateCharaObj(SelectCharas[3], pos);
                    obj.name += ",4Player";
                    obj.transform.eulerAngles = new Vector3(0, 270, 0);
                    _objBaseArray[(_nWidth - 2) + _nWidth] = obj.GetComponent<FieldObjectBase>();
                    obj.GetComponent<Character>().Init(0);
                    break;
            }
        }
    }

    GameObject CreateCharaObj(GameObject obj, Vector3 pos)
    {
        Debug.Log(obj.name);
        GameObject instance = PhotonNetwork.Instantiate("Prefabs/Chara/" + obj.name + "_Online", pos, obj.transform.rotation, 0);
        instance.transform.SetParent(_charaHolder.transform);   //  各自で親にする
        return instance;
    }
}
