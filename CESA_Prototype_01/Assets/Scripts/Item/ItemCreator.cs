using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class ItemCreator : Photon.MonoBehaviour 
{
    [SerializeField] protected GameObject[] _ItemPrefabs = null;

    float _fNowInteval = 0.0f;
    [SerializeField] float _fInterval = 15.0f;

	// Use this for initialization
	void Start () 
    {
        //  ステセレで指定した量に変更
        if (ItemAmount.GetAmount() == 0)
            return;
        
        switch(ItemAmount.GetAmount())
        {
            case 1:
                _fInterval *= 1.5f;
                break;
            case 2:
                //  変更なし
                break;
            case 3:
                _fInterval *= 0.5f;
                break;
        }

        ResetInterval();

        this.UpdateAsObservable()
            .Subscribe(_ => {
                _fNowInteval += Time.deltaTime;

                if(_fNowInteval < _fInterval
                    #if DEBUG
                    && !Input.GetKeyDown(KeyCode.Return)
                    #endif               
                )
                    return;

                CreateItem();
                ResetInterval();
            });
	}

    void ResetInterval()
    {
        _fNowInteval = Random.Range(0.0f, _fInterval / 2.0f);
    }

    protected virtual void CreateItem()
    {
        int number = Random.Range(0, _ItemPrefabs.Length);
        Vector3 pos = FieldData.Instance.GetNonObjPos();
        GameObject item = Instantiate(_ItemPrefabs[number]);
        item.transform.position = pos + new Vector3(0.0f, 0.35f, 0.0f);
        SoundManager.Instance.PlaySE(SoundManager.eSeValue.ITEM);
    }
}
