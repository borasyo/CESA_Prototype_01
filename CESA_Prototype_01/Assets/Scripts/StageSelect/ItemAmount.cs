using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;

public class ItemAmount : Photon.MonoBehaviour
{
    static protected int nItemAmount = 2;
    static protected int nRand = 0;

	// Use this for initialization
	void Start ()
    {
        List<Image> buttonList = new List<Image>();
        Transform stick = transform.GetChild(0);
        for (int i = 0; i < stick.childCount; i++)
        {
            buttonList.Add(stick.GetChild(i).GetComponent<Image>());
        }

        Color onColor = Color.black;
        Color offColor = Color.white;

        //this.ObserveEveryValueChanged(_ => nItemAmount)
        this.UpdateAsObservable()  
            .Subscribe(_ =>
            {
                for (int i = 0; i < buttonList.Count; i++)
                {
                    buttonList[i].color = offColor;
                }

                buttonList[nItemAmount].color = onColor;
            });

        nRand = Random.Range(0, 4);
    }

    public virtual void None()
    {
        nItemAmount = 0;
    }
    public virtual void Few()
    {
        nItemAmount = 1;
    }
    public virtual void Normal()
    {
        nItemAmount = 2;
    }
    public virtual void Many()
    {
        nItemAmount = 3;
    }
    public virtual void Rand()
    {
        nItemAmount = 4;
        nRand = Random.Range(0,4);
    }

    static public int GetAmount()
    {
        if(nItemAmount == 4)
        {
            return nRand;
        }
        return nItemAmount;
    }
}
