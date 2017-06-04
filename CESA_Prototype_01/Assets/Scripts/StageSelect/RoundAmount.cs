using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;

public class RoundAmount : Photon.MonoBehaviour
{
    public static int nRound = 3;

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

        //this.ObserveEveryValueChanged(_ => nRound)
        this.UpdateAsObservable()  
            .Subscribe(_ =>
            {
                for (int i = 0; i < buttonList.Count; i++)
                {
                    buttonList[i].color = offColor;
                }

                buttonList[nRound - 1].color = onColor;
            });
    }

    public virtual void None()
    {
        nRound = 1;
    }
    public virtual void Few()
    {
        nRound = 2;
    }
    public virtual void Normal()
    {
        nRound = 3;
    }
    public virtual void Many()
    {
        nRound = 4;
    }
}
