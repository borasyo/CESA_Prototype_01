using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoTutorial : MonoBehaviour
{
    public void OnClick()
    {
        if (FadeManager.Instance.Fading)
            return;

        SceneChanger.Instance.ChangeScene("Tutorial", true);
        //SceneManager.LoadScene("Tutorial");
    }
}
