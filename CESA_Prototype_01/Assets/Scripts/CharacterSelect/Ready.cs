using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Ready : Photon.MonoBehaviour
{
    public static int nReadyCnt = 0;
    bool _isReady = false;
    Image _image = null;
    Button _CharaChangeButton = null;

    List<Animator> _animList = new List<Animator>();

    static Sprite _onSprite = null;
    static Sprite _offSprite = null;

    void Awake()
    {
        _image = GetComponentInChildren<Image>();
        _CharaChangeButton = transform.parent.GetComponent<Button>();
        _animList = transform.parent.GetComponentsInChildren<Animator>().ToList();

        if (!_onSprite)
            _onSprite = Resources.Load<Sprite>("Texture/CharaSelect/ready_ON");
        if (!_offSprite)
            _offSprite = Resources.Load<Sprite>("Texture/CharaSelect/ready_OFF");
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
            OffReady();
        }
        else
        {
            OnReady();
        }
    }

    void ChangeAnim(List<Animator> animList, bool isOK)
    {
        foreach (Animator anim in animList)
            anim.SetBool("OK", isOK);
    }

    void OnReady()
    {
        _isReady = true;
        nReadyCnt++;
        _image.sprite = _onSprite;
        _CharaChangeButton.enabled = false;

        ChangeAnim(_animList, true);
        SoundManager.Instance.PlaySE(SoundManager.eSeValue.DECISION);

        CharacterSelectOnline charaSele = FindObjectOfType<CharacterSelectOnline>();

        if (charaSele.InstanceCheck(transform.parent.GetComponentInChildren<NowSelect>().gameObject) != 0)
            return;

        for (int i = PhotonNetwork.playerList.Length; i < 4; i++)
        {
            ChangeAnim(charaSele.GetNowSelect(i).transform.parent.GetComponentsInChildren<Animator>().ToList(), true);
            foreach (Button button in charaSele.GetNowSelect(i).transform.parent.GetComponentsInChildren<Button>().ToList())
                button.enabled = false;
        }
    }

    void OffReady()
    {
        _isReady = false;
        nReadyCnt--;
        _image.sprite = _offSprite;
        _CharaChangeButton.enabled = true;

        ChangeAnim(_animList, false);
        SoundManager.Instance.PlaySE(SoundManager.eSeValue.OFFWINDOW);

        CharacterSelectOnline charaSele = FindObjectOfType<CharacterSelectOnline>();

        if (charaSele.InstanceCheck(transform.parent.GetComponentInChildren<NowSelect>().gameObject) != 0)
            return;

        for (int i = PhotonNetwork.playerList.Length; i < 4; i++)
        {
            ChangeAnim(charaSele.GetNowSelect(i).transform.parent.GetComponentsInChildren<Animator>().ToList(), false);
            foreach (Button button in charaSele.GetNowSelect(i).transform.parent.GetComponentsInChildren<Button>().ToList())
                button.enabled = true;
        }
    }

    void OnPhotonPlayerConnected()
    {
        OffReady();
        nReadyCnt = 0;
    }

    void OnPhotonPlayerDisconnected()
    {
        OffReady();
        nReadyCnt = 0;
    }
}
