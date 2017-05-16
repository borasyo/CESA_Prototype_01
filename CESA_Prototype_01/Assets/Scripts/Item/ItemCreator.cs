using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class ItemCreator : MonoBehaviour 
{
    [SerializeField] GameObject[] _ItemPrefabs = null;

	// Use this for initialization
	void Start () 
    {
        this.UpdateAsObservable()
            .Subscribe(_ => {
                if(!Input.GetKeyDown(KeyCode.Return) && Random.Range(0,300) != 0)
                    return;
                
                int number = Random.Range(0,_ItemPrefabs.Length);
                Vector3 pos = FieldData.Instance.GetNonObjPos();
                GameObject item = Instantiate(_ItemPrefabs[number]);
                item.transform.position = pos;
            });

	}
}
