using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultCharacter : MonoBehaviour
{
    [SerializeField] int nNumber = 0;

	void Start ()
    {
        if(!CharacterSelect.SelectCharas[nNumber])
        {
            Destroy(transform.parent.gameObject);
            return;
        }

        int type = 0;
        string charaName = CharacterSelect.SelectCharas[nNumber].name;
        Text text = GetComponent<Text>();

        if (charaName.Contains("Balance"))
        {
            type = 0;
            text.text = "Balance";
        }
        else if (charaName.Contains("Power"))
        {
            type = 1;
            text.text = "Power";
        }
        else if (charaName.Contains("Speed"))
        {
            type = 2;
            text.text = "Speed";
        }
        else if (charaName.Contains("Technical"))
        {
            type = 3;
            text.text = "Technical";
        }

        for(int i = 0; i < transform.childCount; i++)
        {
            if (i == type)
                continue;

            transform.GetChild(i).GetComponent<ResultCharaMaterial>().SetMeshActive(false);
        }
    }
}
