using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCreatorOnline : ItemCreator
{
    protected override void CreateItem()
    {
        int number = Random.Range(0, _ItemPrefabs.Length);
        Vector3 pos = FieldData.Instance.GetNonObjPos();
        photonView.RPC("CreateOnline", PhotonTargets.All, number, pos);
    }

    [PunRPC]
    public void CreateOnline(int number, Vector3 pos)
    {
        GameObject item = Instantiate(_ItemPrefabs[number]);
        item.transform.position = pos;
    }
}
