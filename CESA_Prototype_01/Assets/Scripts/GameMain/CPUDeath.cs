using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class CPUDeath : MonoBehaviour
{
    void Start()
    {
        List<ParticleSystem> particleList = new List<ParticleSystem>();
        for (int i = 0; i < transform.childCount; i++)
            particleList.Add(transform.GetChild(i).GetComponent<ParticleSystem>());

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                foreach (ParticleSystem particle in particleList)
                    particle.Simulate(Time.unscaledDeltaTime, true, false);
            });
    }
}
