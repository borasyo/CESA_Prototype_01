using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;

public class RoundAmount : Photon.MonoBehaviour
{
    static protected int nRound = 3;
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

        nRand = Random.Range(0, 5);
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
    public virtual void Rand()
    {
        nRound = 5;
        nRand = Random.Range(1, 5);
    }

    static public int GetRound()
    {
        if (nRound == 5)
        {
            return nRand;
        }
        return nRound;
    }
}
