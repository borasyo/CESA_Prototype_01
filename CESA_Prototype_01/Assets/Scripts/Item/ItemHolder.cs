using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    #region Singleton

    private static ItemHolder instance;

    public static ItemHolder Instance
    {
        get
        {
            if (instance)
                return instance;

            instance = (ItemHolder)FindObjectOfType(typeof(ItemHolder));

            if (instance)
                return instance;

            GameObject obj = new GameObject("ItemHolder");
            obj.AddComponent<ItemHolder>();
            Debug.Log(typeof(ItemHolder) + "が存在していないのに参照されたので生成");

            return instance;
        }
    }

    #endregion

    List<ItemBase> _ItemList = new List<ItemBase>();
    public List<ItemBase> ItemList { get { return _ItemList; } }

    public void Add(ItemBase item)
    {
        _ItemList.Add(item);
        item.transform.SetParent(transform);
    }

    public void Remove(ItemBase item)
    {
        _ItemList.Remove(item);
    }

    public int GetDistanceForType(ItemBase.eItemType type)
    {
        int distance = 0;
        switch(type)
        {
            case ItemBase.eItemType.MOVEUP:
            case ItemBase.eItemType.GAUGEUP:
            case ItemBase.eItemType.INVINCIBLE:
                distance = 6;
                break;
            case ItemBase.eItemType.SPECIAL:
                distance = 9;
                break;
        }
        return distance;
    }

    /*void Update()
    {
        if (!Input.GetKeyDown(KeyCode.LeftShift))
            return;

        string debug = "ItemList : ";
        for(int i = 0; i < _ItemList.Count; i++)
        {
            debug += _ItemList[i].name + ", ";
        }
        Debug.Log(debug);
    }*/
}
