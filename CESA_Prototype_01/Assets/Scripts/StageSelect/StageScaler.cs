using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;

public class StageScaler : Photon.MonoBehaviour
{
    static protected int StageScale = 1;
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

        //this.ObserveEveryValueChanged(_ => StageScale)
        this.UpdateAsObservable()  
            .Subscribe(_ =>
            {
                for (int i = 0; i < buttonList.Count; i++)
                {
                    buttonList[i].color = offColor;
                }

                buttonList[StageScale].color = onColor;
            });

        nRand = Random.Range(0, 3);
    }

    public virtual void Small()
    {
        StageScale = 0;
    }
    public virtual void Normal()
    {
        StageScale = 1;
    }
    public virtual void Big()
    {
        StageScale = 2;
    }
    public virtual void Rand()
    {
        StageScale = 3;
        nRand = Random.Range(0, 3);
    }

    //  拡大比率を返す
    static public float GetMagni(bool isStage = false)
    {
        int scale = StageScale != 3 ? StageScale : nRand; 

        float fMagni = 0.0f;
        switch(scale)
        {
            case 0:
                if (isStage)
                    fMagni = 0.9f;
                else
                    fMagni = 0.75f;
                break;
            case 1:
                fMagni = 1.0f;
                break;
            case 2:
                if (isStage)
                    fMagni = 1.15f;
                else
                    fMagni = 1.25f;
                break;
        }
        return fMagni;
    }
}
