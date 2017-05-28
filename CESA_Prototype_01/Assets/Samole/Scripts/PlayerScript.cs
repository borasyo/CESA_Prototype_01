using UnityEngine;
using System.Collections;

public class PlayerScript :Photon.MonoBehaviour {

	public float speed = 3.0f; 			// Playerの移動スピード
	public float bulletSpeed = 10.0f; 	// 弾のスピード

	Transform point; 		// 弾の発射の位置
	int playerHP; 			// PlayerのHP

	GameObject myCamera; 	// 自分のカメラのオブジェクト

	void Awake(){
		point = transform.Find ("Point"); // 発射ポイントを自分の子供から探す
		myCamera = transform.Find ("Main Camera").gameObject; // カメラを自分の子供から探す
	}

	// Use this for initialization
	void Start () {
		playerHP = 10; //体力を10に設定
	}

	// Update is called once per frame
	void Update () {
		if (photonView.isMine) {
			Move ();
			if (Input.GetKeyDown (KeyCode.Space)) {
				Shot ();
			}
			myCamera.SetActive (true);
		} else
			myCamera.SetActive (false);
	}

	void Move(){
		float h = Input.GetAxis ("Horizontal");
		float v = Input.GetAxis ("Vertical");
		transform.position += transform.forward * v * Time.deltaTime * speed;
		transform.Rotate (new Vector3 (0, 60f * h * Time.deltaTime, 0));
	}

	void Shot(){
		GameObject obj = PhotonNetwork.Instantiate ("bullet", point.position, transform.rotation, 0) as GameObject;
		obj.GetComponent<Rigidbody> ().velocity = transform.forward * bulletSpeed;
	}

	public void Damage(){
		playerHP -= 1;
		if (playerHP <= 0) {
			PhotonNetwork.Destroy (this.gameObject);
		}
	}
}
