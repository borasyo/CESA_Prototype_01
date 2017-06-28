using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using UniRx;
using UniRx.Triggers;

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

    public static readonly int LimitRoomCount = 5;	// Room上限数

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
    //private static bool isJoinLobby = false;

    public int nMyPlayerCount = 0;

    #region SceneLoadInit

    private bool _isInitialized = false;
    private readonly string ReadyStateKey = "Room";
    /// <param name="data"></param>
    private void OnPhotonPlayerPropertiesChanged(object[] data)
    {
        //誰かのカスタムプロパティが書き換わるたびに確認
        CheckAllPlayerState();
    }

    private void CheckAllPlayerState()
    {
        if (_isInitialized) return;

        //全員のフラグが設定されているか？
        var isAllPlayerLoaded = PhotonNetwork.playerList
            .Select(x => x.customProperties)
            .All(x => x.ContainsKey(ReadyStateKey) && (bool)x[ReadyStateKey]);

        if (isAllPlayerLoaded)
        {
            //全員のフラグが設定されていたら初期化開始
            _isInitialized = true;
            ClearReadyStatus();
            StartCoroutine(RoomInit());
        }
    }

    private void Ready()
    {
        var cp = PhotonNetwork.player.customProperties;
        cp[ReadyStateKey] = true;
        PhotonNetwork.player.SetCustomProperties(cp);
    }

    private void ClearReadyStatus()
    {
        var cp = PhotonNetwork.player.customProperties;
        cp[ReadyStateKey] = null;
        PhotonNetwork.player.SetCustomProperties(cp);
    }

    #endregion

    void Start ()
    {
        if (PhotonNetwork.inRoom)
        {
            roomSelectPanel.transform.parent.gameObject.SetActive(false);
            roomButtonPrefab = Resources.Load("Prefabs/Online/roomButton") as GameObject;	// Resourceからprefabを取得

            // roomButtonの作成
            roomButtonPool = new List<GameObject>();
            for (int loopCnt = 0; loopCnt < LimitRoomCount; ++loopCnt)
            {
                GameObject roomButtonObj = (GameObject)Instantiate(roomButtonPrefab);
                roomButtonObj.transform.SetParent(roomButtonParent, false);
                roomButtonObj.name = "roomButton_" + loopCnt.ToString("d2");
                roomButtonObj.SetActive(false);
                roomButtonObj.GetComponent<RoomButton>().roomMgr = this;
                roomButtonPool.Add(roomButtonObj);
            }

            _isInitialized = false;
            Ready();
            CheckAllPlayerState();
        }
        else
        {
            // サーバー接続
            // 接続成功時に自動的にlobbyに参加する
            PhotonNetwork.ConnectUsingSettings("0.1");

            roomSelectPanel.SetActive(true);    // room選択panelの非表示

            roomButtonPrefab = Resources.Load("Prefabs/Online/roomButton") as GameObject;	// Resourceからprefabを取得
            PhotonNetwork.playerName = "User";

            // roomButtonの作成
            roomButtonPool = new List<GameObject>();
            for (int loopCnt = 0; loopCnt < LimitRoomCount; ++loopCnt)
            {
                GameObject roomButtonObj = (GameObject)Instantiate(roomButtonPrefab);
                roomButtonObj.transform.SetParent(roomButtonParent, false);
                roomButtonObj.name = "roomButton_" + loopCnt.ToString("d2");
                roomButtonObj.SetActive(false);
                roomButtonObj.GetComponent<RoomButton>().roomMgr = this;
                roomButtonPool.Add(roomButtonObj);
            }
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
		if (roomInfo.Length != preRoomCount)
        {
			// roomButtonを必要数表示
			for (int index = 0; index < LimitRoomCount; ++index)
				roomButtonPool [index].SetActive (index < roomInfo.Length ? true : false);

			// room情報をbuttonに反映
			for(int index = 0 ; index < roomInfo.Length ; ++index)
            {
				RoomInfo room = roomInfo [index];

				// buttonに自身のindexを保持させる(別のいい方法があるかも)
				roomButtonPool[index].GetComponent<RoomButton> ().buttonIndex = index;

				// buttonの文字列を更新
				Text buttonText = roomButtonPool[index].transform.GetChild(0).GetComponent<Text> ();
				buttonText.text = room.name + " player(" + room.playerCount + "/" + 4 + ")";// room.maxPlayers+")";
            }
		} 

		// room数の更新がないとき、
		else
        {	
			// room情報の更新
			for(int index = 0 ; index < roomInfo.Length ; ++index)
            {
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
        if (!PhotonNetwork.insideLobby || FadeManager.Instance.Fading || PhotonNetwork.GetRoomList().Length >= LimitRoomCount)
        {
            SoundManager.Instance.PlaySE(SoundManager.eSeValue.OFFWINDOW);
            return;
        }

        // 何も入力がされていないとき、処理しない
        if (roomNameInputField.text == "")
        {
			Debug.Log ("Failed to create the room. : Room's Name has not been entered.");
            SoundManager.Instance.PlaySE(SoundManager.eSeValue.OFFWINDOW);
            return;
		}

		// 同じ名前のroomがすでに存在していたとき、処理しない
		for (int index = 0; index < PhotonNetwork.GetRoomList ().Length; ++index)
        {
			if (roomNameInputField.text == PhotonNetwork.GetRoomList () [index].name)
            {
                roomNameInputField.GetComponent<AlreadyRoomName>().Run();
                Debug.Log ("Failed to create the room. : Already the room of the same name exists.");
                SoundManager.Instance.PlaySE(SoundManager.eSeValue.OFFWINDOW);
                return;
			}
		}

		RoomOptions roomOpt = new RoomOptions();	// roomoption(roomの詳細設定)の作成

		// 詳細設定
		roomOpt.MaxPlayers = 4;	// 最大プレイヤー数 10
		roomOpt.IsOpen = true;		// roomへの入室が可能
		roomOpt.IsVisible = true;	// lobby内のroom一覧に表示される

		PhotonNetwork.CreateRoom(roomNameInputField.text, roomOpt, null);   // Roomを自分で作って参加する

        StartCoroutine(JoinRoomFade());
        SoundManager.Instance.PlaySE(SoundManager.eSeValue.DECISION);
    }

	// roombuttonを押したときの処理
	// RoomButtonから呼ばれる
	// int index : roomInfo配列のindex
	public void OnPressRoomButton(int index)
    {
        if (!PhotonNetwork.insideLobby || FadeManager.Instance.Fading)
        {
            SoundManager.Instance.PlaySE(SoundManager.eSeValue.OFFWINDOW);
            return;
        }

        RoomInfo room = PhotonNetwork.GetRoomList () [index];	// 指定room情報の取得

        // 部屋が満員であるとき、処理しない
        if (room.playerCount >= 4)
        {
			Debug.Log ("The room is packed.");
            SoundManager.Instance.PlaySE(SoundManager.eSeValue.OFFWINDOW);
            return;
		}

        if(!room.IsOpen)
        {
            Debug.Log("NotRoomOpen");
            SoundManager.Instance.PlaySE(SoundManager.eSeValue.OFFWINDOW);
            return;
        }

        PhotonNetwork.JoinRoom (room.name); // roomに参加
        nMyPlayerCount = room.playerCount;

        StartCoroutine(JoinRoomFade());
        SoundManager.Instance.PlaySE(SoundManager.eSeValue.DECISION);
    }

    IEnumerator JoinRoomFade()
    {
        SceneChanger.Instance.ChangeScene("", true);

        yield return new WaitForSeconds(1.0f);

        lobbyUI.SetActive(false);		// lobbyUIの非表示
    }

    // 退室buttonが押されたときの処理
    public void OnPressLeaveRoomButton()
    {
        if (GameObject.FindWithTag("SelectCanvas"))
        {
            LeaveRoom();
        }
        else
        {
            GetComponent<ReLoadScene>().ReLoadModeSelect();
        }
    }

    //  Masterが退室した時
    void OnMasterClientSwitched()
    {
        Instantiate(Resources.Load<GameObject>("Prefabs/Error/LeaveRoomCanvas")).GetComponentInChildren<Text>().text = "※ルームが解散となりました※";
    }

    [PunRPC]
    public void LeaveRoom()
    {
        StartCoroutine(LeaveRoomFade());
    }

    IEnumerator LeaveRoomFade()
    {
        SceneChanger.Instance.ChangeScene("", true);

        yield return new WaitForSeconds(1.0f);

        PhotonNetwork.LeaveRoom();      // 退室

        lobbyUI.SetActive(true);    // lobbyのUIを表示
        //playerNameInputPanel.SetActive(true);   // playername入力panelを表示
        roomSelectPanel.SetActive(true);       // room選択panelの非表示

        //playerNameInputField.text = "PlayerName";   // player名入力領域の初期化
        roomNameInputField.text = "RoomName";       // room名入力領域の初期

        CharacterSelectOnline._nMyNumber = 0;
    }

    // Lobbyに参加した時に呼ばれる
    void OnJoinedLobby()
    {
        //Debug.Log ("Joined lobby");
    }

	// Lobbyに参加した時、Roomが作成されていなかった時に呼ばれる
	void OnPhotonRandomJoinFailed()
    {
		//Debug.Log ("Joined Failed");
	}

	// Room参加成功時に呼ばれる
	void OnJoinedRoom()
    {
        StartCoroutine(Wait());
        StartCoroutine(RoomInit());	// Player作成

        //Debug.Log ("Joined Room");
    }

    void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        StartCoroutine(JoinRoomFailedError());

        //Debug.Log("Joined Room Failed");
    }

    IEnumerator JoinRoomFailedError()
    {
        yield return new WaitWhile(() => FadeManager.Instance.Fading);
        Instantiate(Resources.Load<GameObject>("Prefabs/Error/LeaveRoomCanvas")).GetComponentInChildren<Text>().text = "※ルームの入室に失敗しました※";
    }

    // Player作成
    IEnumerator RoomInit()
    {
        yield return new WaitWhile(() => FadeManager.Instance.HalfFading);

        //Debug.Log("RoomInit");

        // 自分がMasterCliantであれば、Sceneに属するBossの生成
        if (PhotonNetwork.isMasterClient)
        {
            CharacterSelectOnline charaSele = PhotonNetwork.Instantiate("Prefabs/CharacterSelect/SelectCanvas", Vector3.zero, Quaternion.identity, 0).GetComponentInChildren<CharacterSelectOnline>();
            PhotonNetwork.Instantiate("Prefabs/CharacterSelect/1P", Vector3.zero, Quaternion.identity, 0);
            PhotonNetwork.Instantiate("Prefabs/CharacterSelect/2P_CPU", Vector3.zero, Quaternion.identity, 0);
            PhotonNetwork.Instantiate("Prefabs/CharacterSelect/3P_CPU", Vector3.zero, Quaternion.identity, 0);
            PhotonNetwork.Instantiate("Prefabs/CharacterSelect/4P_CPU", Vector3.zero, Quaternion.identity, 0);
        }
        else
        {
            PhotonNetwork.Instantiate("Prefabs/CharacterSelect/CharaSelect", Vector3.zero, Quaternion.identity, 0);
        }
    }

    IEnumerator Wait()
    {
        PhotonNetwork.room.IsOpen = false;

        yield return new WaitForSeconds(2.0f);

        PhotonNetwork.room.IsOpen = true;
    }
}
