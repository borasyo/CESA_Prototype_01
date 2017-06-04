using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;

public class StageScaler : Photon.MonoBehaviour
{
    public static int StageScale = 1;

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
}
