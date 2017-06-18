using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseWindow : Photon.MonoBehaviour
{
    Vector3 _initSize = Vector3.zero;
    float _fTimeScale = 0.0f;

    void Awake()
    {
        _initSize = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    public void On()
    {
        if (PhotonNetwork.inRoom && !PhotonNetwork.isMasterClient)
            return;

        if (FadeManager.Instance.Fading)
            return;

        if (Time.timeScale <= 0.0f)
            return;

        if (PhotonNetwork.inRoom)
        {
            photonView.RPC("OnlineOn", PhotonTargets.All);
        }
        else
        {
            _fTimeScale = Time.timeScale;
            Time.timeScale = 0.0f;
            StartCoroutine(OnWindow());
        }
    }

    [PunRPC]
    public void OnlineOn()
    {
        _fTimeScale = Time.timeScale;
        Time.timeScale = 0.0f;

        StartCoroutine(OnWindow());
    }

    public void OnNonPose()
    {
        if (PhotonNetwork.inRoom && !PhotonNetwork.isMasterClient)
            return;

        if (FadeManager.Instance.Fading)
            return;

        if (Time.timeScale <= 0.0f)
            return;

        if (PhotonNetwork.inRoom)
        {
            photonView.RPC("OnlineOnNonPose", PhotonTargets.All);
        }
        else
        {
            _fTimeScale = 1.0f;
            StartCoroutine(OnWindow());
        }
    }

    [PunRPC]
    public void OnlineOnNonPose()
    {
        _fTimeScale = 1.0f;
        StartCoroutine(OnWindow());
    }

    IEnumerator OnWindow()
    {
        yield return new WaitWhile(() => {

            transform.localScale += _initSize * (Time.unscaledDeltaTime / 0.25f);

            if (transform.localScale.x >= _initSize.x)
                return false;

            return true;
        });

        transform.localScale = _initSize;
    }

    public void Off()
    {
        if (PhotonNetwork.inRoom)
        {
            if (!PhotonNetwork.isMasterClient)
                return;

            photonView.RPC("OnlineOff", PhotonTargets.All);
        }
        else
        {
            StartCoroutine(OffWindow());
        }
    }


    [PunRPC]
    public void OnlineOff()
    {
        StartCoroutine(OffWindow());
    }

    IEnumerator OffWindow()
    {
        yield return new WaitWhile(() => {

            transform.localScale -= _initSize * (Time.unscaledDeltaTime / 0.25f);

            if (transform.localScale.x <= 0.0f)
                return false;

            return true;
        });

        transform.localScale = Vector2.zero;
        Time.timeScale = _fTimeScale;
    }

    void OnDestroy()
    {
        Time.timeScale = 1.0f;
    }
}
