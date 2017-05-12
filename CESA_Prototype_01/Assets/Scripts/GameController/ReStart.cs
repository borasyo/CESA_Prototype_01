using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class ReStart : MonoBehaviour 
{    
    ParticleSystem _particle = null;

    void Start()
    {
        _particle = GetComponent<ParticleSystem>();
    }

	void Update () 
    {
        if (_particle.isPlaying)
            return;

        SceneManager.LoadScene("Prototype");
	}
}
