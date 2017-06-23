using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;
using UniRx.Triggers;

public class RoomButton : MonoBehaviour
{ 
	[SerializeField]
	public int buttonIndex = 0;			// 自身が対応しているroom情報のindex
	[SerializeField]
	public RoomManager roomMgr = null;	// RoomManagerの参照

    void Start()
    {
        this.ObserveEveryValueChanged(_ => PhotonNetwork.GetRoomList()[buttonIndex].playerCount)
            .Subscribe(_ =>
            {
                ChangeEnable();
            });

        this.ObserveEveryValueChanged(_ => PhotonNetwork.GetRoomList()[buttonIndex].open)
            .Subscribe(_ =>
            {
                ChangeEnable();
            });
    }


    void ChangeEnable()
    {
        Button button = GetComponent<Button>();
        ColorBlock colBlock = new ColorBlock();
        colBlock = button.colors;
        if (PhotonNetwork.GetRoomList()[buttonIndex].playerCount >= 4 || !PhotonNetwork.GetRoomList()[buttonIndex].open)
        {
            colBlock.normalColor = Color.gray;
            colBlock.highlightedColor = Color.gray;
            colBlock.pressedColor = new Color(Color.gray.r / 2.0f, Color.gray.g / 2.0f, Color.gray.b / 2.0f, 1.0f);
            button.colors = colBlock;
        }
        else
        {
            colBlock.normalColor = Color.white;
            colBlock.highlightedColor = Color.white;
            colBlock.pressedColor = new Color(0.79f, 0.79f, 0.79f, 1.0f);
            button.colors = colBlock;
        }
    }

	// 自身が押されたときの処理
	public void OnPressMyself()
    {

		// 参照が設定されているとき、roomButtonが押されたときの処理を行う
		if (roomMgr != null)
			roomMgr.OnPressRoomButton (buttonIndex);
		else
			Debug.Log ("Reference not be found.");
	}
}
