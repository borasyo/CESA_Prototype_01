using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReCharaSelect : MonoBehaviour 
{	
	// Update is called once per frame
	void Update () 
    {
        if (!Input.GetKeyDown(KeyCode.Backspace))
            return;

        SceneManager.LoadScene("CharacterSelect");
	}

    public void ReChara()
    {
        SceneManager.LoadScene("CharacterSelect");
    }
}
