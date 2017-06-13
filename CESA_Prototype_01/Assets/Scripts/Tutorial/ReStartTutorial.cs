using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReStartTutorial : ReStart
{
    protected override IEnumerator DestroyReStart()
    {
        yield return new WaitForSeconds(2.0f);

        if (_winer)
        {
            RoundCounter.Instance.WinCharacter(_winer);
        }

        SceneManager.LoadScene("ModeSelect");
        Destroy(this.gameObject);
    }
}
