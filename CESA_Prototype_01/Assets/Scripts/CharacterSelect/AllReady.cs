using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AllReady : Photon.MonoBehaviour
{
    void Awake()
    {
        //transform.localPosition = new Vector3(0, 0, -100);
    }

	void Start()
    {
        Ready.nReadyCnt = 0;

        Button button = GetComponent<Button>();
        button.enabled = false;

        ColorBlock onColBlock = new ColorBlock();
        onColBlock = button.colors;

        ColorBlock offColBlock = new ColorBlock();
        offColBlock = button.colors;
        offColBlock.normalColor *= 0.75f;
        offColBlock.highlightedColor *= 0.75f;
        offColBlock.pressedColor *= 0.75f;
        //Transform setParent = transform.parent;
        //transform.parent = null;

        this.ObserveEveryValueChanged(_ => Ready.nReadyCnt)
            .Subscribe(_ =>
            {
                if (Ready.nReadyCnt == PhotonNetwork.playerList.Length)
                {
                    button.enabled = true;
                    button.colors = onColBlock;
                    //transform.parent = null;
                    //transform.SetParent(setParent);
                }
                else
                {
                    button.enabled = false;
                    button.colors = offColBlock;
                    //transform.parent = null;
                }
            });

        this.ObserveEveryValueChanged(_ => PhotonNetwork.playerList.Length)
            .Subscribe(_ =>
            {
                if (Ready.nReadyCnt == PhotonNetwork.playerList.Length)
                {
                    button.enabled = true;
                    button.colors = onColBlock;
                    //transform.parent = null;
                    //transform.SetParent(setParent);
                }
                else
                {
                    button.enabled = false;
                    button.colors = offColBlock;
                    //transform.parent = null;
                }
            });

        Image _image = GetComponent<Image>();
        TriangleWave<float> triangleAlpha = TriangleWaveFactory.Float(0.5f, 1.0f, 0.25f);
        this.UpdateAsObservable()
            .Where(_ => button.enabled)
            .Subscribe(_ =>
            {
                triangleAlpha.Progress();
                _image.color = new Color(1,1,1, triangleAlpha.CurrentValue);
            });
    }

    void Update()
    {
        //transform.localPosition = new Vector3(0, 0, -100);
    }

    void OnPhotonPlayerConnected()
    {
        Ready.nReadyCnt = 0;
    }

    void OnPhotonPlayerDisconnected()
    {
        Ready.nReadyCnt = 0;
    }
}
