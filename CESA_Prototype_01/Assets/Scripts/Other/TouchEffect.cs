using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class TouchEffect : MonoBehaviour
{
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

                particle.Emit(1);
            });
	}
}
