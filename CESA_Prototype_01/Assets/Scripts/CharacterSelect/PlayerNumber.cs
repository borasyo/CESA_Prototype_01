using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNumber : MonoBehaviour
{	
    public void Set(int number)
    {
        GetComponentInChildren<Text>().text = (number + 1) + "P";
    }
}
