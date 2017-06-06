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
        transform.localPosition = new Vector3(0, 0, 0);
    }

	void Start()
    {
        Ready.nReadyCnt = 0;

        Button button = GetComponent<Button>();
        button.enabled = false;

        Transform setParent = transform.parent;
        transform.parent = null;

        this.ObserveEveryValueChanged(_ => Ready.nReadyCnt)
            .Subscribe(_ =>
            {
                if (Ready.nReadyCnt == PhotonNetwork.playerList.Length)
                {
                    button.enabled = true; ;
                    transform.parent = null;
                    transform.SetParent(setParent);
                }
                else
                {
                    button.enabled = false;
                    transform.parent = null;
                }
            });

        this.ObserveEveryValueChanged(_ => PhotonNetwork.playerList.Length)
            .Subscribe(_ =>
            {
                if (Ready.nReadyCnt == PhotonNetwork.playerList.Length)
                {
                    button.enabled = true;
                    transform.parent = null;
                    transform.SetParent(setParent);
                }
                else
                {
                    button.enabled = false;
                    transform.parent = null;
                }
            });
    }

    void Update()
    {
        transform.localPosition = new Vector3(0,0,0);
    }

    void OnPhotonPlayerConnected()
    {
        Ready.nReadyCnt = 0;
    }

    void OnPhotonPlayerDisconnecte()
    {
        Ready.nReadyCnt = 0;
    }
}
