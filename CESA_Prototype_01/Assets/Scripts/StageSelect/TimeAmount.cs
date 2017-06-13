using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;

public class TimeAmount : Photon.MonoBehaviour
{
    static protected int nTime_Sec = 2;
    static protected int nRand = 0;

    // Use this for initialization
    void Start()
    {
        List<Image> buttonList = new List<Image>();
        Transform stick = transform.GetChild(0);
        for (int i = 0; i < stick.childCount; i++)
        {
            buttonList.Add(stick.GetChild(i).GetComponent<Image>());
        }

        Color onColor = Color.black;
        Color offColor = Color.white;

        //this.ObserveEveryValueChanged(_ => nTime_Sec)
        this.UpdateAsObservable()  
            .Subscribe(_ =>
            {
                for (int i = 0; i < buttonList.Count; i++)
                {
                    buttonList[i].color = offColor;
                }

                buttonList[nTime_Sec].color = onColor;
            });

        nRand = Random.Range(0, 4);
    }

    public virtual void None()
    {
        nTime_Sec = 0;
    }
    public virtual void Few()
    {
        nTime_Sec = 1;
    }
    public virtual void Normal()
    {
        nTime_Sec = 2;
    }
    public virtual void Many()
    {
        nTime_Sec = 3;
    }
    public virtual void Rand()
    {
        nTime_Sec = 4;
        nRand = Random.Range(0,4);
    }

    static public int GetTime()
    {
        if (nTime_Sec == 4)
        {
            return (nRand + 1) * 60;
        }
        if(nTime_Sec == 3)
        {
            return -1;  //  ∞
        }

        return (nTime_Sec + 1) * 60;
    }
}
