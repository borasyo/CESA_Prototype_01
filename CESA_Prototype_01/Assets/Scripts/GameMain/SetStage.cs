using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetStage : MonoBehaviour
{
    void Start()
    {
        float fScale = StageScaler.GetMagni(); // (true);
        fScale *= fScale;

        //  スケールに合わせて拡大
        transform.localScale *= fScale;

        ParticleSystem[] particleList = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem particle in particleList)
        {
            particle.startSize *= fScale;
        }
    }
}
