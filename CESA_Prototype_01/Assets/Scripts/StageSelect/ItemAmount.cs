using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;

public class ItemAmount : Photon.MonoBehaviour
{
    static protected int nItemAmount = 2;

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

    static public int GetAmount()
    {
        return nItemAmount;
    }
}
