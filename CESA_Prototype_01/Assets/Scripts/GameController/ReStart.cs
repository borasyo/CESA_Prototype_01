using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class ReStart : MonoBehaviour 
{    
    ParticleSystem _particle = null;
    public bool _IsEnd { get; set; }

    void Start()
    {
        _particle = GetComponent<ParticleSystem>();
    }

	void Update () 
    {
        if (_particle.isPlaying)
            return;

        if (_IsEnd)
        {
            SceneManager.LoadScene("GameMain");
        }
        else
        {
            Destroy(this.gameObject);
        }
	}
}
