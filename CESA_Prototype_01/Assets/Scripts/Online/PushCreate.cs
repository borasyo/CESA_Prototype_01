using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PushCreate : MonoBehaviour
{
    public void OnClick()
    {
        GetComponent<Image>().sprite = Resources.Load<Sprite>("Texture/OnlineMode/create_on");
    }
}
