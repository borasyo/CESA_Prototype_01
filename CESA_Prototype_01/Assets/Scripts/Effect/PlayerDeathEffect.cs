using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathEffect : MonoBehaviour
{
	public void Init(string type)
    {
        Color setCol = GetTypeColor(type);
        //Debug.Log(setCol);

        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<ParticleSystem>().startColor = setCol;
        }
    }

    Color GetTypeColor(string type)
    {
        const float add = 0.2f;
        switch(type)
        {
            case "1P":
                return Color.red + new Color(0, add, add, 0);
            case "2P":
                return Color.blue + new Color(add, add, 0, 0); ;
            case "3P":
                return Color.green + new Color(add, 0, add, 0); ;
            case "4P":
                return Color.yellow + new Color(0, 0, add, 0); ;
        }
        return Color.clear;
    }
}
