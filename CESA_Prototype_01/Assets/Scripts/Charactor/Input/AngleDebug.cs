using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;

public class AngleDebug : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        Text text = GetComponent<Text>();
        MoveButton moveButton = transform.parent.Find("Move").GetComponent<MoveButton>();
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (moveButton.IsActive)
                    text.text = "角度" + (int)(moveButton.GetMoveAngle);
                else
                    text.text = "なし";
            });	
	}
}
