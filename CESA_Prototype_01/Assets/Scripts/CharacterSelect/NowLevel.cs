using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UniRx;
using UniRx.Triggers;

public class NowLevel : Photon.MonoBehaviour
{
    protected int _nNowLevel = 2;
    public int nNowLevel { get { return _nNowLevel; } set { _nNowLevel = value; } }

	void Start ()
    {
        Text text = GetComponentInChildren<Text>();
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                switch(_nNowLevel)
                {
                    case 0:
                        text.text = "強";
                        break;
                    case 1:
                        text.text = "普";
                        break;
                    case 2:
                        text.text = "弱";
                        break;
                }
            });
	}

    public virtual void OnClick()
    {
        _nNowLevel --;
        if (_nNowLevel < 0)
            _nNowLevel += 3;
    }
}
