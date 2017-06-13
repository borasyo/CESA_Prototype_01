using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharaMaterial : MonoBehaviour
{
    [SerializeField]
    Character.eCharaType type = Character.eCharaType.MAX;

    List<SkinnedMeshRenderer> _sMeRendList = new List<SkinnedMeshRenderer>();

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

        _sMeRendList = GetComponentsInChildren<SkinnedMeshRenderer>().ToList();
        foreach (SkinnedMeshRenderer sMeRend in _sMeRendList)
        {
            sMeRend.material = Resources.Load<Material>(materialName);
        }
    }

    public void SetMeshActive(bool isActive)
    {
        foreach (SkinnedMeshRenderer sMeRend in _sMeRendList)
        {
            sMeRend.enabled = isActive;
        }
    }
}
