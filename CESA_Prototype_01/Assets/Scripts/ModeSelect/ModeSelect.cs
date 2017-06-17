using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using System.Linq;

public class ModeSelect : MonoBehaviour
{
    void Awake()
    {
        LevelSelect.Reset();
        CharacterSelect.Reset();
    }

    public void Offline()
    {
        if (FadeManager.Instance.Fading)
            return;

        StartCoroutine(Anim(false));
    }

    public void Online()
    {
        if (FadeManager.Instance.Fading)
            return;

        StartCoroutine(Anim(true));
    }

    IEnumerator Anim(bool isOnline)
    {
        List<Animator> happyAnimList = new List<Animator>();
        List<Animator> sadAnimList = new List<Animator>();
        if (isOnline)
        {
            happyAnimList = GameObject.Find("OnlineChara").GetComponentsInChildren<Animator>().ToList();
            sadAnimList = GameObject.Find("OfflineChara").GetComponentsInChildren<Animator>().ToList();
        }
        else
        {
            happyAnimList = GameObject.Find("OfflineChara").GetComponentsInChildren<Animator>().ToList();
            sadAnimList = GameObject.Find("OnlineChara").GetComponentsInChildren<Animator>().ToList();
        }

        foreach (Animator happy in happyAnimList)
            happy.SetTrigger("Happy");

        foreach (Animator sad in sadAnimList)
            sad.SetTrigger("Sad");

        yield return new WaitForSeconds(2.0f);

        if(isOnline)
        {
            SceneChanger.Instance.ChangeScene("OnlineRoom", true);
        }
        else
        {
            SceneChanger.Instance.ChangeScene("CharacterSelect", true);
        }
    }
}
