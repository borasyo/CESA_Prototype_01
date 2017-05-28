using UnityEngine;
using System.Collections;

public class RoomButton : MonoBehaviour {

	[SerializeField]
	public int buttonIndex = 0;			// 自身が対応しているroom情報のindex
	[SerializeField]
	public RoomManager roomMgr = null;	// RoomManagerの参照

	// 自身が押されたときの処理
	public void OnPressMyself(){

		// 参照が設定されているとき、roomButtonが押されたときの処理を行う
		if (roomMgr != null)
			roomMgr.OnPressRoomButton (buttonIndex);
		else
			Debug.Log ("Reference not be found.");
	}
}
