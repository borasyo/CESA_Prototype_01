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
        Vector3 initScale = transform.localScale;
        while (time < 1.0f)
        {
            time += Time.deltaTime / 0.5f;
            transform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(360.0f, 0.0f, time));

            if (time < 0.5f)
            {
                transform.localScale = initScale * Mathf.Lerp(1.0f, 1.5f, time * 2.0f);
            }
            else
            {
                transform.localScale = initScale * Mathf.Lerp(1.5f, 1.0f, (time - 0.5f) * 2.0f);
            }
            yield return null;
        }
    }
}
