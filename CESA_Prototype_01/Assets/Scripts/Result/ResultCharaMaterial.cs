using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ResultCharaMaterial : MonoBehaviour
{
    [SerializeField]
    Character.eCharaType type = Character.eCharaType.MAX;

    [SerializeField]
    int nNumber = 0;

    List<SkinnedMeshRenderer> _sMeRendList = new List<SkinnedMeshRenderer>();
    List<MeshRenderer> _meRendList = new List<MeshRenderer>();

    void Awake()
    {
        SetMaterial(FindObjectOfType<CharacterSelect>());

        Animator anim = GetComponent<Animator>();
        if(nNumber == RoundCounter.nNowWinerPlayer)
        {
            anim.SetBool("Win", true);
        }
        else
        {
            anim.SetBool("Win", false);
        }
    }

    void SetMaterial(CharacterSelect charaSele)
    {
        string materialName = "Materials/Chara/";
        switch (type)
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
        materialName += (nNumber + 1).ToString();

        _sMeRendList = GetComponentsInChildren<SkinnedMeshRenderer>().ToList();
        _meRendList = GetComponentsInChildren<MeshRenderer>().ToList();
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
        foreach (MeshRenderer meRend in _meRendList)
        {
            meRend.enabled = isActive;
        }
    }
}
