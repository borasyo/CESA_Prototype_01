using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;

public class ReStart : Photon.MonoBehaviour 
{    
    public bool _IsEnd { get; set; }

    void Start()
    {
        ParticleSystem particle = GetComponentInChildren<ParticleSystem>();

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (particle.isPlaying)
                    return;

                StartCoroutine(DestroyReStart());
            });
    }

    IEnumerator DestroyReStart()
    {
        yield return new WaitForSeconds(2.0f);

        if (_IsEnd)
        {
            if(PhotonNetwork.inRoom)
                SceneManager.LoadScene("OnlineGameMain");
            else
                SceneManager.LoadScene("GameMain");
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
