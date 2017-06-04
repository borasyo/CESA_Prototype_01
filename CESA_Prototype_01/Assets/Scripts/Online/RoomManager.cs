using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RoomManager : Photon.MonoBehaviour
{
    #region Singleton

    private static RoomManager instance;

    public static RoomManager Instance
    {
        get
        {
            if (instance)
                return instance;

            instance = (RoomManager)FindObjectOfType(typeof(RoomManager));

            if (instance)
                return instance;

            GameObject obj = new GameObject();
            obj.AddComponent<RoomManager>();
            Debug.Log(typeof(RoomManager) + "が存在していないのに参照されたので生成");

            return instance;
        }
    }

    #endregion

    private static readonly int LimitRoomCount = 5;	// Room上限数

	public GameObject lobbyUI;					// lobbyのUI

	//public GameObject playerNameInputPanel;		// player名入力panel
	//public InputField playerNameInputField;		// player名入力領域

	public GameObject roomSelectPanel;			// room選択panel
	public InputField roomNameInputField;		// room名入力領域

	public Transform roomButtonParent;			// roomButtonの親
	private GameObject roomButtonPrefab;		// roomButtonのPrefab

	private int preRoomCount = 0;				// 直前のroom数
	private List<GameObject> roomButtonPool;	// roomButtonのPool

	private GameObject charaSelectObj;
    private bool isJoinLobby = false;

	void Start ()
    { 	
		// サーバー接続
		// 接続成功時に自動的にlobbyに参加する
		PhotonNetwork.ConnectUsingSettings ("0.1");

		roomSelectPanel.SetActive (true);	// room選択panelの非表示

		roomButtonPrefab = Resources.Load ("Prefabs/Online/roomButton") as GameObject;	// Resourceからprefabを取得
        PhotonNetwork.playerName = "User";

        // roomButtonの作成
        roomButtonPool = new List<GameObject> ();
		for (int loopCnt = 0; loopCnt < LimitRoomCount; ++loopCnt) {
			GameObject roomButtonObj = (GameObject)Instantiate (roomButtonPrefab);
			roomButtonObj.transform.SetParent (roomButtonParent, false);
			roomButtonObj.name = "roomButton_" + loopCnt.ToString ("d2");
			roomButtonObj.SetActive (false);
			roomButtonObj.GetComponent<RoomButton> ().roomMgr = this;
			roomButtonPool.Add(roomButtonObj);
		}
	}

	void Update()
    {
		// lobby内にいないとき、処理しない
		if (!PhotonNetwork.insideLobby)
			return;

		// rooom内にいるとき、処理しない
		if (PhotonNetwork.inRoom)
			return;
		
		RoomInfo[] roomInfo = PhotonNetwork.GetRoomList ();	// room情報の取得

		// roo数が更新されたとき、
		if (roomInfo.Length != preRoomCount) {

			// roomButtonを必要数表示
			for (int index = 0; index < LimitRoomCount; ++index)
				roomButtonPool [index].SetActive (index < roomInfo.Length ? true : false);

			// room情報をbuttonに反映
			for(int index = 0 ; index < roomInfo.Length ; ++index){

				RoomInfo room = roomInfo [index];

				// buttonに自身のindexを保持させる(別のいい方法があるかも)
				roomButtonPool[index].GetComponent<RoomButton> ().buttonIndex = index;

				// buttonの文字列を更新
				Text buttonText = roomButtonPool[index].transform.GetChild(0).GetComponent<Text> ();
				buttonText.text = room.name + " player(" + room.playerCount + "/" + 4 + ")";// room.maxPlayers+")";
            }
		} 

		// room数の更新がないとき、
		else {
			
			// room情報の更新
			for(int index = 0 ; index < roomInfo.Length ; ++index){

				RoomInfo room = roomInfo [index];

				// buttonの文字列を更新
				Text buttonText = roomButtonPool[index].transform.GetChild(0).GetComponent<Text> ();
                buttonText.text = room.name + " player(" + room.playerCount + "/" + 4 + ")";// room.maxPlayers+")";
			}
		}

		preRoomCount = roomInfo.Length;	// room数の取得
	}

	// room作成buttonが押されたときの処理
	public void OnPressCreateRoomButton()
    {
        if (!isJoinLobby)
            return;

		// 何も入力がされていないとき、処理しない
		if (roomNameInputField.text == "") {
			Debug.Log ("Failed to create the room. : Room's Name has not been entered.");
			return;
		}

		// 同じ名前のroomがすでに存在していたとき、処理しない
		for (int index = 0; index < PhotonNetwork.GetRoomList ().Length; ++index) {
			if (roomNameInputField.text == PhotonNetwork.GetRoomList () [index].name) {
				Debug.Log ("Failed to create the room. : Already the room of the same name exists.");
				return;
			}
		}

		RoomOptions roomOpt = new RoomOptions();	// roomoption(roomの詳細設定)の作成

		// 詳細設定
		roomOpt.MaxPlayers = 4;	// 最大プレイヤー数 10
		roomOpt.IsOpen = true;		// roomへの入室が可能
		roomOpt.IsVisible = true;	// lobby内のroom一覧に表示される

		PhotonNetwork.CreateRoom(roomNameInputField.text, roomOpt, null);	// Roomを自分で作って参加する

		lobbyUI.SetActive (false);		// lobbyUIの非表示
	}

	// roombuttonを押したときの処理
	// RoomButtonから呼ばれる
	// int index : roomInfo配列のindex
	public void OnPressRoomButton(int index)
    {
		RoomInfo room = PhotonNetwork.GetRoomList () [index];	// 指定room情報の取得

		// 部屋が満員であるとき、処理しない
		if (room.playerCount >= 4) { // room.maxPlayers) {
			Debug.Log ("The room is packed.");
			return;
		}
        
        PhotonNetwork.JoinRoom (room.name);	// roomに参加

		lobbyUI.SetActive (false);		// lobbyUIの非表示
	}

	// 退室buttonが押されたときの処理
	public void OnPressLeaveRoomButton()
    {
        //PhotonNetwork.Destroy (charaSelectObj);	// playerの削除
        //  マスターが退室したら強制解散
        if (PhotonNetwork.isMasterClient)
        {
            photonView.RPC("LeaveRoom", PhotonTargets.All);
        }
        else
        {
            LeaveRoom();
        }
    }

    //  Masterが退室した時
    void OnMasterClientSwitched()
    {
        LeaveRoom();
    }

    [PunRPC]
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();      // 退室

        lobbyUI.SetActive(true);    // lobbyのUIを表示
        //playerNameInputPanel.SetActive(true);   // playername入力panelを表示
        roomSelectPanel.SetActive(true);       // room選択panelの非表示

        //playerNameInputField.text = "PlayerName";   // player名入力領域の初期化
        roomNameInputField.text = "RoomName";		// room名入力領域の初期化
    }

	// Lobbyに参加した時に呼ばれる
	void OnJoinedLobby()
    {
		Debug.Log ("Joined lobby");
        isJoinLobby = true;
    }

	// Lobbyに参加した時、Roomが作成されていなかった時に呼ばれる
	void OnPhotonRandomJoinFailed()
    {
		Debug.Log ("Joined Failed");
	}

	// Room参加成功時に呼ばれる
	void OnJoinedRoom()
    {
		Debug.Log ("Joined Room");

		RoomInit ();	// Player作成
	}

	// Player作成
	void RoomInit()
    {
		Debug.Log("RoomInit");

        // 自分がMasterCliantであれば、Sceneに属するBossの生成
        if (PhotonNetwork.isMasterClient)
        {
            CharacterSelectOnline charaSele = PhotonNetwork.Instantiate("Prefabs/CharacterSelect/SelectCanvas", Vector3.zero, Quaternion.identity, 0).GetComponentInChildren<CharacterSelectOnline>();
            PhotonNetwork.Instantiate("Prefabs/CharacterSelect/1P"    , Vector3.zero, Quaternion.identity, 0);
            PhotonNetwork.Instantiate("Prefabs/CharacterSelect/2P_CPU", Vector3.zero, Quaternion.identity, 0);
            PhotonNetwork.Instantiate("Prefabs/CharacterSelect/3P_CPU", Vector3.zero, Quaternion.identity, 0);
            PhotonNetwork.Instantiate("Prefabs/CharacterSelect/4P_CPU", Vector3.zero, Quaternion.identity, 0);
        }
        else
        {
            PhotonNetwork.Instantiate("Prefabs/CharacterSelect/CharaSelect", Vector3.zero, Quaternion.identity, 0);
        }
	}
		
	void OnGUI()
    {
		GUILayout.Label (PhotonNetwork.connectionStateDetailed.ToString());
	}
}
