using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;

public class SelectStage : Photon.MonoBehaviour
{
    public static int StageNumber = 0;
    int nMaxStage = 3;
    [SerializeField] List<Sprite> stageSpriteList = new List<Sprite>();

    void Start()
    {
        Image myImage = GetComponent<Image>();

        //this.ObserveEveryValueChanged(_ => StageNumber)
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                myImage.sprite = stageSpriteList[StageNumber];
            });
    }


    public virtual void Add()
    {
        StageNumber++;
        if (StageNumber >= nMaxStage)
            StageNumber -= nMaxStage;
    }

    public virtual void Sub()
    {
        StageNumber--;
        if (StageNumber < 0)
            StageNumber += nMaxStage; 
    }
}
