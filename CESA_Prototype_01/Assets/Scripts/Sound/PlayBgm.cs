using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBgm : MonoBehaviour
{
    [SerializeField]
    SoundManager.eBgmValue eBgmValue;

    [SerializeField]
    float Volume = 0.25f;

    void Start()
    {
        SoundManager.Instance.PlayBGM(eBgmValue, Volume);
    }
}
