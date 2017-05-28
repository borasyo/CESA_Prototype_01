using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIScript : Photon.MonoBehaviour {

	private static readonly int MessageLimitCount = 10;		// message上限数

	private Text messageLabel;				// messageを表示するlabel
	public InputField messageInputField;	// message入力領域

	PhotonView myPhotonView;				// 自身のPhotonView
	List<string> messageList;				// messageのlist

	// Use this for initialization
	void Start () {
		
		messageList = new List<string> ();	// Listの作成

		messageLabel = GetComponent<Text> ();		// 自身のTextを取得
		myPhotonView = GetComponent<PhotonView> ();	// 自身のPhotonviewを取得

		messageLabel.text = "";	// labelの初期化
	}

	// message送信buttonが押されたときの処理
	public void OnPressSendMessageButton(){

		// 何も入力されていないとき、処理しない
		if (messageInputField.text == "") {
			Debug.Log ("Failed to send the message. : Message has not been entered.");
			return;
		}

		// message受信イベントを全員が実行
		myPhotonView.RPC( "ReceiveMessage", PhotonTargets.All, (PhotonNetwork.playerName + " : " + messageInputField.text));

		messageInputField.text = "";	// message入力領域の初期化
	}

	// message受信
	// string message : 受信した文字列
	[PunRPC]
	void ReceiveMessage(string msgText){

		string text = "";	// messageLabelに表示する文字列

		messageList.Add ("\n" + msgText);	// listに追加

		// list長が上限数を上回っていたとき、先頭の要素を削除
		if (messageList.Count > MessageLimitCount)
			messageList.RemoveAt (0);

		// 文字列の作成
		for (int index = messageList.Count - 1; index >= 0; --index)
			text += messageList[index];
		
		messageLabel.text = text;	// 文字列の反映
	}
}