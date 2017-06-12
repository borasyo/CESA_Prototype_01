using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaMaterial : MonoBehaviour
{
    [SerializeField]
    Character.eCharaType type = Character.eCharaType.MAX;

    void Start()
    {
        string materialName = "Materials/Chara/";
        switch(type)
        {
            case Character.eCharaType.BALANCE:
                materialName += "Balance_";
                break;
            case Character.eCharaType.POWER:
                materialName += "Power_";
                break;
            case Character.eCharaType.SPEED:
                materialName += "Speed_";
                break;
            case Character.eCharaType.TECHNICAL:
                materialName += "Technique_";
                break;
        }
        materialName += (FindObjectOfType<CharacterSelect>().InstanceCheck(transform.parent.gameObject) + 1).ToString();

        foreach (MeshRenderer meRend in GetComponentsInChildren<MeshRenderer>())
        {
             meRend.material = Resources.Load<Material>(materialName);
        }
    }
}
