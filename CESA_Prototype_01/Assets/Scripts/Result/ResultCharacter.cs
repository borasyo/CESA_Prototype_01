using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultCharacter : MonoBehaviour
{
    [SerializeField] int nNumber = 0;
    const float _fDefaultHeight = 35.0f;
    static Sprite[] _typeSprite = new Sprite[(int)CharacterSelect.eCharaType.MAX];

    void Start ()
    {
        if (!_typeSprite[1])
        {
            _typeSprite[(int)CharacterSelect.eCharaType.BALANCE - 1] = Resources.Load<Sprite>("Texture/CharaSelect/BALANCE");
            _typeSprite[(int)CharacterSelect.eCharaType.POWER - 1] = Resources.Load<Sprite>("Texture/CharaSelect/POWER");
            _typeSprite[(int)CharacterSelect.eCharaType.SPEED - 1] = Resources.Load<Sprite>("Texture/CharaSelect/SPEED");
            _typeSprite[(int)CharacterSelect.eCharaType.TECHNICAL - 1] = Resources.Load<Sprite>("Texture/CharaSelect/TECHNIC");
            _typeSprite[(int)CharacterSelect.eCharaType.MAX - 1] = Resources.Load<Sprite>("Texture/CharaSelect/RANDOM");
        }

        if (!CharacterSelect.SelectCharas[nNumber])
        {
            Destroy(transform.parent.gameObject);
            return;
        }

        int type = 0;
        string charaName = CharacterSelect.SelectCharas[nNumber].name;
        Image image = GetComponent<Image>();
        RectTransform rectTrans = GetComponent<RectTransform>();

        image.color = Color.white;
        if (charaName.Contains("Balance"))
        {
            type = 0;
            image.sprite = _typeSprite[0];
            rectTrans.sizeDelta = new Vector2(200.0f, _fDefaultHeight);
        }
        else if (charaName.Contains("Power"))
        {
            type = 1;
            image.sprite = _typeSprite[1];
            rectTrans.sizeDelta = new Vector2(153.0f, _fDefaultHeight);
        }
        else if (charaName.Contains("Speed"))
        {
            type = 2;
            image.sprite = _typeSprite[2];
            rectTrans.sizeDelta = new Vector2(136.0f, _fDefaultHeight);
        }
        else if (charaName.Contains("Technical"))
        {
            type = 3;
            image.sprite = _typeSprite[3];
            rectTrans.sizeDelta = new Vector2(178.0f, _fDefaultHeight);
        }

        StartCoroutine(ChangeMesh(type));
    }

    IEnumerator ChangeMesh(int type)
    {
        yield return null;

        for (int i = 0; i < transform.childCount; i++)
        {
            if (i == type)
                continue;

            transform.GetChild(i).GetComponent<ResultCharaMaterial>().SetMeshActive(false);
        }

    }
}
