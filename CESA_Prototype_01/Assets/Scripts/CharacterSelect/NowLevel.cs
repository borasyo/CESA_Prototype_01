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

    static List<Sprite> cpuSpriteList = new List<Sprite>();

	void Start ()
    {
        if(cpuSpriteList.Count == 0)
        {
            cpuSpriteList.Add(Resources.Load<Sprite>("Texture/CharaSelect/CPULevel/CPU_hard"));
            cpuSpriteList.Add(Resources.Load<Sprite>("Texture/CharaSelect/CPULevel/CPU_normal"));
            cpuSpriteList.Add(Resources.Load<Sprite>("Texture/CharaSelect/CPULevel/CPU_easy"));
        }

        Image image = GetComponent<Image>();
        this.UpdateAsObservable()
            .Where(_ => cpuSpriteList.Count > _nNowLevel && _nNowLevel >= 0)
            .Subscribe(_ =>
            {
                image.sprite = cpuSpriteList[_nNowLevel];
            });

        if (transform.childCount <= 0)
            return;

        Destroy(transform.GetChild(0).gameObject);
	}

    public virtual void OnClick()
    {
        if (Ready.nReadyCnt == PhotonNetwork.playerList.Length)
            return;

        _nNowLevel --;
        if (_nNowLevel < 0)
            _nNowLevel += 3;
    }
}
