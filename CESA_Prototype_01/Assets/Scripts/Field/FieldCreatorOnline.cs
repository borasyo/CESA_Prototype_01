using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldCreatorOnline : FieldCreator
{
    protected override void CreateRandomBlock(GameObject obj, Vector3 pos, int idx)
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        FieldData.Instance.GetComponent<PhotonView>().RPC("CreateBlock", PhotonTargets.All, pos);
    }

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
            obj.GetComponent<PhotonView>().RPC("Create", PhotonTargets.All, ",1Player", 90, _nWidth + 1, 0);

            //  右上に生成
            if (SelectCharas[1] && SelectLevels[1] >= 0)
            {
                pos = new Vector3((_nWidth - 2.0f) * GameScaler._fScale, 0.0f, (_nHeight - 2.0f) * GameScaler._fScale);
                obj = CreateCPUCharaObj(SelectCharas[1], pos);
                obj.GetComponent<PhotonView>().RPC("Create", PhotonTargets.All, ",2Player" + ",CPU", 270, _nWidth * (_nHeight - 2) + _nWidth - 2, SelectLevels[1]);
            }

            //  左上に生成
            if (SelectCharas[2] && SelectLevels[2] >= 0)
            {
                pos = new Vector3(1.0f * GameScaler._fScale, 0.0f, (_nHeight - 2.0f) * GameScaler._fScale);
                obj = CreateCPUCharaObj(SelectCharas[2], pos);
                obj.GetComponent<PhotonView>().RPC("Create", PhotonTargets.All, ",3Player" + ",CPU", 90, 1 + _nWidth * (_nHeight - 2), SelectLevels[2]);
            }

            //  右上に生成
            if (SelectCharas[3] && SelectLevels[3] >= 0)
            {
                pos = new Vector3((_nWidth - 2.0f) * GameScaler._fScale, 0.0f, 1.0f * GameScaler._fScale);
                obj = CreateCPUCharaObj(SelectCharas[3], pos);
                obj.GetComponent<PhotonView>().RPC("Create", PhotonTargets.All, ",4Player" + ",CPU", 270, (_nWidth - 2) + _nWidth, SelectLevels[3]);
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
                    obj.GetComponent<PhotonView>().RPC("Create", PhotonTargets.All, ",2Player", 270, _nWidth * (_nHeight - 2) + _nWidth - 2, 0);
                    break;
                case 2:
                    pos = new Vector3(1.0f * GameScaler._fScale, 0.0f, (_nHeight - 2.0f) * GameScaler._fScale);
                    obj = CreateCharaObj(SelectCharas[2], pos);
                    obj.GetComponent<PhotonView>().RPC("Create", PhotonTargets.All, ",3Player", 90, 1 + _nWidth * (_nHeight - 2), 0);
                    break;
                case 3:
                    pos = new Vector3((_nWidth - 2.0f) * GameScaler._fScale, 0.0f, 1.0f * GameScaler._fScale);
                    obj = CreateCharaObj(SelectCharas[3], pos);
                    obj.GetComponent<PhotonView>().RPC("Create", PhotonTargets.All, ",4Player", 270, (_nWidth - 2) + _nWidth, 0);
                    break;
            }
        }
    }

    GameObject CreateCharaObj(GameObject obj, Vector3 pos)
    {
        GameObject instance = PhotonNetwork.Instantiate("Prefabs/Chara/" + obj.name + "_Online", pos, obj.transform.rotation, 0);
        return instance;
    }
    GameObject CreateCPUCharaObj(GameObject obj, Vector3 pos)
    {
        GameObject instance = PhotonNetwork.Instantiate("Prefabs/Chara/" + obj.name + "_Online,CPU", pos, obj.transform.rotation, 0);
        return instance;
    }
}
