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

                StartCoroutine(DestroyReStart());
            });
    }

    IEnumerator DestroyReStart()
    {
        yield return new WaitForSeconds(2.0f);

        if (_winer)
        {
            RoundCounter.Instance.WinCharacter(_winer);
        }

        Destroy(this.gameObject);
    }
}
