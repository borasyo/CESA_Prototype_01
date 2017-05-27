using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectMassColor : MonoBehaviour
{
    #region Singleton

    private static SelectMassColor instance;

    public static SelectMassColor Instance
    {
        get
        {
            if (instance)
                return instance;

            instance = (SelectMassColor)FindObjectOfType(typeof(SelectMassColor));

            if (instance)
                return instance;

            GameObject obj = new GameObject("SelectMassColor");
            obj.AddComponent<SelectMassColor>();
            //Debug.Log(typeof(FieldDataChecker) + "が存在していないのに参照されたので生成");

            return instance;
        }
    }

    #endregion

    [SerializeField]
    Color NotColor_1P = Color.white;
    [SerializeField]
    Color PutColor_1P = Color.white;
    [SerializeField]
    Color BreakColor_1P = Color.white;

    [SerializeField]
    Color NotColor_2P = Color.white;
    [SerializeField]
    Color PutColor_2P = Color.white;
    [SerializeField]
    Color BreakColor_2P = Color.white;

    [SerializeField]
    Color NotColor_3P = Color.white;
    [SerializeField]
    Color PutColor_3P = Color.white;
    [SerializeField]
    Color BreakColor_3P = Color.white;

    [SerializeField]
    Color NotColor_4P = Color.white;
    [SerializeField]
    Color PutColor_4P = Color.white;
    [SerializeField]
    Color BreakColor_4P = Color.white;

    public Color GetNotColor(string playerName)
    {
        Color col = Color.white;
        string player = playerName[playerName.IndexOf("Player") - 1].ToString();
        switch (player)
        {
            case "1":
                col = NotColor_1P;
                break;
            case "2":
                col = NotColor_2P;
                break;
            case "3":
                col = NotColor_3P;
                break;
            case "4":
                col = NotColor_4P;
                break;
        }
        return col;
    }

    public Color GetPutColor(string playerName)
    {
        Color col = Color.white;
        string player = playerName[playerName.IndexOf("Player") - 1].ToString();
        switch (player)
        {
            case "1":
                col = PutColor_1P;
                break;
            case "2":
                col = PutColor_2P;
                break;
            case "3":
                col = PutColor_3P;
                break;
            case "4":
                col = PutColor_4P;
                break;
        }
        return col;
    }

    public Color GetBreakColor(string playerName)
    {
        Color col = Color.white;
        string player = playerName[playerName.IndexOf("Player") - 1].ToString();
        switch (player)
        {
            case "1":
                col = BreakColor_1P;
                break;
            case "2":
                col = BreakColor_2P;
                break;
            case "3":
                col = BreakColor_3P;
                break;
            case "4":
                col = BreakColor_4P;
                break;
        }
        return col;
    }
}
