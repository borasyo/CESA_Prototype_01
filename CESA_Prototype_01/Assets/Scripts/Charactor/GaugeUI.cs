using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GaugeUI : MonoBehaviour {

    const float fMaxGaugeSize = 600.0f;

    [SerializeField] bool _IsPlayer = false;
    CharactorGauge _charaGauge = null;
    Image _image = null;
    [SerializeField] Color _NonGaugeColor = new Color (255,0,0,255);
    [SerializeField] Color _OnGaugeColor = new Color (0,255,0,255);
    [SerializeField] Color _MaxGaugeColor = new Color (255,255,0,255);

	// Use this for initialization
	void Start () 
    {
		_image = GetComponent<Image> ();

        if (_IsPlayer)  FindCharaGauge("Player");
        else            FindCharaGauge("Enemy");
	}

    void FindCharaGauge (string name)
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Charactor");
        for (int i = 0; i < objs.Length; i++)
        {
            if (!objs[i].name.Contains(name))
                continue;

            _charaGauge = objs[i].GetComponent<CharactorGauge>();
            break;
        }
    }
	
	// Update is called once per frame
	void Update () 
    {
        transform.localScale = new Vector3 (_charaGauge.GaugePercent, transform.localScale.y, transform.localScale.z);

        if (_charaGauge.GaugePercent < 0.2f)
        {
            _image.color = _NonGaugeColor;
        }
        else if(_charaGauge.GaugePercent < 1.0f)
        {
            _image.color = _OnGaugeColor;
        }
        else
        {
            _image.color = _MaxGaugeColor;
        }
	}
}
