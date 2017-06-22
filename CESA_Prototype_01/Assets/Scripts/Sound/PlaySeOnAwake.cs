using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySeOnAwake : MonoBehaviour
{
    [SerializeField]
    SoundManager.eSeValue eSeValue;

    [SerializeField]
    float Volume = 1.0f;

    void Awake()
    {
        SoundManager.Instance.PlaySE(eSeValue, Volume);
     
    }
}
