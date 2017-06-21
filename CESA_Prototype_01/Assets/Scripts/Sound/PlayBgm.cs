using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBgm : MonoBehaviour
{
    [SerializeField]
    SoundManager.eBgmValue eBgmValue;

    void Start()
    {
        SoundManager.Instance.PlayBGM(eBgmValue);
    }
}
