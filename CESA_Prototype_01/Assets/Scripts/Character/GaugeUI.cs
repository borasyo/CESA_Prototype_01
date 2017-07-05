using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using UniRx;
using UniRx.Triggers;

public class GaugeUI : MonoBehaviour
{
    
    [SerializeField] SandItem.eType _type = SandItem.eType.MAX;
    CharacterGauge _charaGauge = null;
    Image _image = null;
    [SerializeField] Color _NonGaugeColor = new Color (255,0,0,255);
    [SerializeField] Color _OnGaugeColor = new Color (0,255,0,255);
    [SerializeField] Color _MaxGaugeColor = new Color (255,255,0,255);

	// Use this for initialization
	void Start () 
    {
		_image = GetComponent<Image> ();

        int number = 0;
        switch (_type)
        {
            case SandItem.eType.ONE_P:
                number = 0;
                break;
            case SandItem.eType.TWO_P:
                number = 1;
                break;
            case SandItem.eType.THREE_P:
                number = 2;
                break;
            case SandItem.eType.FOUR_P:
                number = 3;
                break;
        }

        if (!CharacterSelect.SelectCharas[number])
        {
            Destroy(this.gameObject);
            return;
        }

        StartCoroutine(FindCharaGauge());
	}

    IEnumerator FindCharaGauge ()
    {
        string typeName = "";
        switch(_type)
        {
            case SandItem.eType.ONE_P:
                typeName = "1P";
                break;
            case SandItem.eType.TWO_P:
                typeName = "2P";
                break;
            case SandItem.eType.THREE_P:
                typeName = "3P";
                break;
            case SandItem.eType.FOUR_P:
                typeName = "4P";
                break;
        }

        yield return new WaitWhile(() => {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("Character");
            for (int i = 0; i < objs.Length; i++)
            {
                if (!objs[i].name.Contains(typeName))
                    continue;

                _charaGauge = objs[i].GetComponent<CharacterGauge>();
                return false;
            }
            return true;
        });

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                transform.localScale = new Vector3(_charaGauge.GaugePercent, transform.localScale.y, transform.localScale.z);

                if (_charaGauge.GaugePercent < 0.2f)
                {
                    _image.color = _NonGaugeColor;
                }
                else if (_charaGauge.GaugePercent < 1.0f)
                {
                    _image.color = _OnGaugeColor;
                }
                else
                {
                    _image.color = _MaxGaugeColor;
                }
            });
    }
}
