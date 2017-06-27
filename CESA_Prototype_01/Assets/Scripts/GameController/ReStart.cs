using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;

public class ReStart : Photon.MonoBehaviour 
{    
    public FieldObjectBase _winer { get; set; }

    void Start()
    {
        ParticleSystem particle = GetComponentInChildren<ParticleSystem>();

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (particle.isPlaying)
                    return;

                DestroyReStart();
                //StartCoroutine(DestroyReStart());
            });
        SoundManager.Instance.PlaySE(SoundManager.eSeValue.DEATH, 0.5f);
    }

    protected virtual void DestroyReStart()
    {
        if (_winer)
        {
            RoundCounter.Instance.WinCharacter(_winer);
            SoundManager.Instance.StopBGM();
        }

        Destroy(this.gameObject);

        //yield return new WaitForSeconds(2.0f);
    }
}
