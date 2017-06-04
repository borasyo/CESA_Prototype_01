using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AllReady : Photon.MonoBehaviour
{
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

    public void OnClick()
    {
        if (!photonView.isMine)
            return;

        photonView.RPC("SceneNext", PhotonTargets.All);
    }

    [PunRPC]
    public void SceneNext()
    {
        SceneManager.LoadScene("OnlineGameMain");   //  ステージセレクトはさむ
    }
}
