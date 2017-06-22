using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarEffect : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        StartCoroutine(Run());
	}

    IEnumerator Run()
    {
        yield return new WaitForSeconds(0.5f);

        SoundManager.Instance.PlaySE(SoundManager.eSeValue.INTERVAL);
        float time = 0.0f;
        while (time < 1.0f)
        {
            time += Time.deltaTime / 0.5f;
            transform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(360.0f, 0.0f, time));
            yield return null;
        }
        /*while (true)
        {
            yield return new WaitForSeconds(1.5f);
        }*/
    }
}
