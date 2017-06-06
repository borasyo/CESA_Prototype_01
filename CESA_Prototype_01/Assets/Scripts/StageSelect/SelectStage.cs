using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;

public class SelectStage : Photon.MonoBehaviour
{
    static protected int StageNumber = 0;
    static protected int nRand = 0; 
    static int nMaxStage = 4;
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

        nRand = Random.Range(0, 3);
    }


    public virtual void Add()
    {
        StageNumber++;
        if (StageNumber >= nMaxStage)
            StageNumber -= nMaxStage;

        if(StageNumber == nMaxStage - 1)
        {
            nRand = Random.Range(0,3);
        }
    }

    public virtual void Sub()
    {
        StageNumber--;
        if (StageNumber < 0)
            StageNumber += nMaxStage;

        if (StageNumber == nMaxStage - 1)
        {
            nRand = Random.Range(0, 3);
        }
    }

    static public int GetStageNumber()
    {
        if (StageNumber == nMaxStage - 1)
        {
            return nRand;
        }
        return StageNumber;
    }
}
