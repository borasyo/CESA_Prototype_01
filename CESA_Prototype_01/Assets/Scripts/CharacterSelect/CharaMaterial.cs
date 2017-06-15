using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharaMaterial : MonoBehaviour
{
    [SerializeField]
    Character.eCharaType type = Character.eCharaType.MAX;

    List<SkinnedMeshRenderer> _sMeRendList = new List<SkinnedMeshRenderer>();
    List<MeshRenderer> _meRendList = new List<MeshRenderer>();

    void Start()
    {
        if (PhotonNetwork.inRoom)
        {
            StartCoroutine(SetWait());
        }
        else
        {
            SetMaterial(FindObjectOfType<CharacterSelect>());
        }
    }

    IEnumerator SetWait()
    {
        CharacterSelect charaSele = null;
        yield return new WaitWhile(() =>
        {
            charaSele = FindObjectOfType<CharacterSelect>();

            if (charaSele)
                return false;

            return true;
        });

        yield return new WaitWhile(() => charaSele.InstanceCheck(transform.parent.gameObject) < 0);

        SetMaterial(charaSele);
    }

    void SetMaterial(CharacterSelect charaSele)
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
        materialName += (charaSele.InstanceCheck(transform.parent.gameObject) + 1).ToString();

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
