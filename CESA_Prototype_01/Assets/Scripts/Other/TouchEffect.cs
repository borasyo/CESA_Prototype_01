using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class TouchEffect : MonoBehaviour
{
    void Awake()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            return;

        Destroy(gameObject);
    }

	// Use this for initialization
	void Start ()
    {
        ParticleSystem particle = GetComponent<ParticleSystem>();
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    if (Input.touchCount <= 0 || Input.GetTouch(0).phase != TouchPhase.Began)
                        return;

                    Vector3 pos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                    pos.z = 0.0f;
                    transform.position = pos;
                }
                else
                {
                    if (!Input.GetMouseButtonDown(0))
                        return;

                    Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    pos.z = 0.0f;
                    transform.position = pos;
                }

                StartCoroutine(Emit(particle));
            });
	}

    IEnumerator Emit(ParticleSystem particle)
    {
        for(int i = 0; i < 1; i++)
        {
            particle.Emit(1);
            yield return new WaitForSeconds(0.05f);
        }
    }
}
