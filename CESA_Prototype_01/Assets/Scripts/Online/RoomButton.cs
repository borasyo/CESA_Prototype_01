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
        Button button = GetComponent<Button>();
        ColorBlock colBlock = new ColorBlock();
        colBlock = button.colors;
        this.UpdateAsObservable()
            .Where(_ => PhotonNetwork.GetRoomList().Length > buttonIndex)
            .Subscribe(_ =>
            {
                RoomInfo room = PhotonNetwork.GetRoomList()[buttonIndex];
                //Debug.Log(room.open);
                if (room.PlayerCount >= 4 || !room.IsOpen)
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
            });
    }

	// 自身が押されたときの処理
	public void OnPressMyself()
    {
        // 参照が設定されているとき、roomButtonが押されたときの処理を行う
        if (roomMgr)
        {
            roomMgr.OnPressRoomButton(buttonIndex);
        }
        else
        {
            Debug.Log("Reference not be found.");
        }
	}
}
