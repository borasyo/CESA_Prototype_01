using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;

public class TimeAmount : Photon.MonoBehaviour
{
    static protected int nTime_Sec = 1;
    static Sprite _onSprite = null;
    static Sprite _offSprite = null;

    // Use this for initialization
    void Start()
    {
        List<Image> buttonList = new List<Image>();
        Transform stick = transform.GetChild(0);
        for (int i = 0; i < stick.childCount; i++)
        {
            buttonList.Add(stick.GetChild(i).GetComponent<Image>());
        }

        if (!_onSprite)
            _onSprite = Resources.Load<Sprite>("Texture/StageSelect/Check");
        if (!_offSprite)
            _offSprite = Resources.Load<Sprite>("Texture/StageSelect/Box");

        //this.ObserveEveryValueChanged(_ => nItemAmount)
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                for (int i = 0; i < buttonList.Count; i++)
                {
                    buttonList[i].sprite = _offSprite;
                }

                buttonList[nTime_Sec].sprite = _onSprite;
            });
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

    static public int GetTime()
    {
        if(nTime_Sec == 3)
        {
            return -1;  //  ∞
        }

        return (nTime_Sec + 1) * 60;
    }
}
