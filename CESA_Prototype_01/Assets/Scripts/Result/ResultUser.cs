using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultUser : MonoBehaviour
{
    [SerializeField] int nNumber = 0;

    void Start()
    {
        if (LevelSelect.SelectLevel[nNumber] >= 0)
            return;

        Image image = GetComponent<Image>();
        image.sprite = Resources.Load<Sprite>("Texture/CharaSelect/player_" + (nNumber + 1).ToString() + "P");
    }
}
