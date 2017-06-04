using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class AutoParticleDestroyer : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        ParticleSystem particle = GetComponentInChildren<ParticleSystem>();
        this.UpdateAsObservable()
            .Where(_ => !particle.isPlaying)
            .Subscribe(_ => 
            {
                Destroy(gameObject);
            });
	}
}
