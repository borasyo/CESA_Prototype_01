using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ready : Photon.MonoBehaviour {

    public static int nReadyCnt = 0;
    bool _isReady = false;
    Text _text = null;
    Button _CharaChangeButton = null;

    void Start()
    {
        _text = GetComponentInChildren<Text>();
        _CharaChangeButton = transform.parent.GetComponent<Button>();
    }

    public void OnClick()
    {
        if (!photonView.isMine)
            return;

        photonView.RPC("ChangeReady", PhotonTargets.All);
    }

    [PunRPC]
    public void ChangeReady()
    {
        if (_isReady)
        {
            _isReady = false;
            nReadyCnt--;
            _text.text = "Ready?";
            _CharaChangeButton.enabled = true;
        }
        else
        {
            _isReady = true;
            nReadyCnt++;
            _text.text = "OK!";
            _CharaChangeButton.enabled = false;
        }
    }

    void Reset()
    {
        _isReady = false;
        _text.text = "Ready?";
        _CharaChangeButton.enabled = true;
    }

    void OnPhotonPlayerConnected()
    {
        Reset();
    }

    void OnPhotonPlayerDisconnected()
    {
        Reset();
    }
}
