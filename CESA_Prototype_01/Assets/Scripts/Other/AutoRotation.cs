using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class AutoRotation : MonoBehaviour
{
    [SerializeField]
    float fAngleAmount_Sec = 360.0f;

    void Start()
    {
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                transform.eulerAngles += new Vector3(0.0f, 0.0f, fAngleAmount_Sec * Time.deltaTime);
            });
    }
}
