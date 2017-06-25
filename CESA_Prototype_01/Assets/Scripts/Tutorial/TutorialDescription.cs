using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialDescription : MonoBehaviour
{
    public bool IsNext { get; private set; }
    [SerializeField] [Multiline] string[] _Description = null;
    int _nCnt = 0;

    public IEnumerator OnWindow()
    {
        IsNext = false;
        gameObject.GetComponentInChildren<Text>().text = _Description[_nCnt];
        _nCnt++;

        float time = 0.0f;
        yield return new WaitWhile(() =>
        {
            time += Time.deltaTime / 0.5f;
            if (time > 1.0f)
                time = 1.0f;

            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, time);

            return (time < 1.0f);
        });

        yield return new WaitWhile(() =>
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                for(int i = 0; i < Input.touchCount; i++)
                {
                    if (Input.GetTouch(i).phase != TouchPhase.Began)
                        continue;

                    return false;
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Action"))
                    return false;
            }

            return true;
        });

        StartCoroutine(OffWindow());
    }

    public IEnumerator OffWindow()
    {
        IsNext = false;

        float time = 0.0f;
        yield return new WaitWhile(() =>
        {
            time += Time.deltaTime / 0.5f;
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, time);

            return (time < 1.0f);
        });
        
        IsNext = true;
    }
}
