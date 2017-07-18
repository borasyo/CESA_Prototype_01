using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnTitle : MonoBehaviour 
{
	public void OnClick()
	{
		SceneChanger.Instance.ChangeScene("Title", true);
	}
}
