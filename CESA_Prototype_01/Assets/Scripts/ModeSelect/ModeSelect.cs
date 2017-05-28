using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class ModeSelect : MonoBehaviour
{
    public void Offline()
    {
        SceneManager.LoadScene("CharacterSelect");
    }

    public void Online()
    {
        SceneManager.LoadScene("OnlineCharaSelectRoom");
    }
}
