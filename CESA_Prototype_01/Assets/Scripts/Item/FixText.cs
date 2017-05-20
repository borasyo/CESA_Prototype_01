using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class FixText : MonoBehaviour 
{   
	// Update is called once per frame
	void Start () 
    {
        this.UpdateAsObservable()
            .Where(_ => transform.parent.parent && transform.parent.parent.tag == "Charactor")
            .Subscribe(_ => {
                transform.position = transform.parent.parent.position;
                transform.eulerAngles = new Vector3(90, 0, 0);
            });
	}
}
