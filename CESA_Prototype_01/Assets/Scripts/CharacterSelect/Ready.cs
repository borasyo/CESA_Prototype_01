using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Ready : Photon.MonoBehaviour {

    public static int nReadyCnt = 0;
    bool _isReady = false;
    Text _text = null;
    Button _CharaChangeButton = null;

    List<Animator> _animList = new List<Animator>();

    void Start()
    {
        _text = GetComponentInChildren<Text>();
        _CharaChangeButton = transform.parent.GetComponent<Button>();
        _animList = transform.parent.GetComponentsInChildren<Animator>().ToList();
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

            ChangeAnim(_animList, false);

            CharacterSelectOnline charaSele = FindObjectOfType<CharacterSelectOnline>();

            if (charaSele.InstanceCheck(transform.parent.GetComponentInChildren<NowSelect>().gameObject) != 0)
                return;

            for (int i = PhotonNetwork.playerList.Length; i < 4; i++)
            {
                ChangeAnim(charaSele.GetNowSelect(i).transform.parent.GetComponentsInChildren<Animator>().ToList(), false);
            }
        }
        else
        {
            _isReady = true;
            nReadyCnt++;
            _text.text = "OK!";
            _CharaChangeButton.enabled = false;

            ChangeAnim(_animList, true);

            CharacterSelectOnline charaSele = FindObjectOfType<CharacterSelectOnline>();

            if (charaSele.InstanceCheck(transform.parent.GetComponentInChildren<NowSelect>().gameObject) != 0)
                return;

            for (int i = PhotonNetwork.playerList.Length; i < 4; i++)
            {
                ChangeAnim(charaSele.GetNowSelect(i).transform.parent.GetComponentsInChildren<Animator>().ToList(), true);
            }
        }
    }

    void ChangeAnim(List<Animator> animList, bool isOK)
    {
        foreach (Animator anim in animList)
            anim.SetBool("OK", isOK);
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
