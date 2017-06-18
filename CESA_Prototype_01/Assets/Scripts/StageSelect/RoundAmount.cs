using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;

public class RoundAmount : Photon.MonoBehaviour
{
    static protected int nRound = 3;
    //static protected int nRand = 0;
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

                buttonList[nRound - 1].sprite = _onSprite;
            });

        //nRand = Random.Range(0, 5);
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
        //nRand = Random.Range(1, 5);
    }

    static public int GetRound()
    {
        /*if (nRound == 5)
        {
            return nRand;
        }*/
        return nRound;
    }
}
