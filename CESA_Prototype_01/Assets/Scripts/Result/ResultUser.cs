using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultUser : MonoBehaviour
{
    [SerializeField] int nNumber = 0;
    [SerializeField] bool isColorChange = false;

    struct tNumber
    {
        public int _nUser;
        public int _Round;
    };

    void Awake()
    {
        if (!isColorChange)
            return;

        Image image = GetComponent<Image>();

        tNumber[] numberList = new tNumber[4];
        int[] roundList = RoundCounter.nRoundCounter;
        for (int i = 0; i < roundList.Length; i++)
        {
            tNumber num = new tNumber();
            num._nUser = i; // User番号
            num._Round = roundList[i];
            numberList[i] = num;
        }

        for (int i = 0; i < numberList.Length - 1; i++)
        {
            for (int j = numberList.Length - 1; j > i; j--)
            {
                if (numberList[j]._Round > numberList[j - 1]._Round)
                {
                    tNumber temp = numberList[j];
                    numberList[j] = numberList[j - 1];
                    numberList[j - 1] = temp;
                }
            }
        }

        //  指定されている順位のUser番号を格納
        //Debug.Log((nNumber + 1) + "位は" + (numberList[nNumber]._nUser + 1) + "Player!");
        nNumber = numberList[nNumber]._nUser;
        transform.parent.GetComponentInChildren<ResultCharacter>().SetNumber = nNumber;
        transform.parent.GetComponentInChildren<ResultStar>().SetNumber = nNumber;
        transform.parent.GetComponentInChildren<ResultRank>().SetNumber = nNumber;
        foreach(ResultCharaMaterial charaMat in transform.parent.GetComponentsInChildren<ResultCharaMaterial>())
        {
            charaMat.SetNumber = nNumber;
        }

        switch (nNumber)
        {
            case 0:
                image.color = Color.red;
                break;
            case 1:
                image.color = Color.blue;
                break;
            case 2:
                image.color = Color.green;
                break;
            case 3:
                image.color = Color.yellow;
                break;
        }
    }

    void Start()
    {
        if (LevelSelect.SelectLevel[nNumber] >= 0)
            return;

        Image image = GetComponent<Image>();
        image.sprite = Resources.Load<Sprite>("Texture/CharaSelect/player_" + (nNumber + 1).ToString() + "P");
    }
}
