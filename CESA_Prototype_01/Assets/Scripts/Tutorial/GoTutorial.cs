using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoTutorial : MonoBehaviour
{
    public void OnClick()
    {
        SceneManager.LoadScene("Tutorial");
    }
}
