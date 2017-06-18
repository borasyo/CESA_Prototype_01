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
        offColBlock.normalColor *= new Color(0.5f, 0.5f, 0.5f, 1.0f);
        offColBlock.highlightedColor *= new Color(0.5f, 0.5f, 0.5f, 1.0f);
        offColBlock.pressedColor *= new Color(0.5f, 0.5f, 0.5f, 1.0f);
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
        TriangleWave<Color> triangleColor = TriangleWaveFactory.Color(new Color(0.5f, 0.5f, 0.5f,1.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.5f);
        this.UpdateAsObservable()
            .Where(_ => button.enabled)
            .Subscribe(_ =>
            {
                triangleColor.Progress();
                _image.color = triangleColor.CurrentValue;
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
