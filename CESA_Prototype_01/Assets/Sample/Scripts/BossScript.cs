using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BossScript : Photon.MonoBehaviour {

	public int BossHP = 500;	// 残HP
	public Text HPLabel;		// 残HP表示用ラベル
	Renderer _renderer;			// レンダラ

	// Use this for initialization
	void Start () {
		
		_renderer = GetComponent<Renderer> ();	// レンダラの取得
	}
	
	// Update is called once per frame
	void Update () {
		
		HPLabel.text = "HP:" + BossHP.ToString ();	// ラベルの更新
	}

	// ダメージ処理
	[PunRPC]
	public void Damage(){

		BossHP -= 1;
		if (BossHP <= 0) {	// 0以下になったら破壊
			PhotonNetwork.Destroy (this.gameObject);
		}
	}

	// 自身のマテリアルの色設定
	[PunRPC]
	public void SetColor(float r,float g,float b){
		_renderer.material.color = new Color (r,g,b,1.0f);
	}

	// データの送受信
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){

		if (stream.isWriting) {
			// データの送信
			stream.SendNext (BossHP);
			stream.SendNext (_renderer.material.color.r);
			stream.SendNext (_renderer.material.color.g);
			stream.SendNext (_renderer.material.color.b);
		} 
		else {
			// データの受信
			this.BossHP = (int)stream.ReceiveNext ();
			float r = (float)stream.ReceiveNext ();
			float g = (float)stream.ReceiveNext ();
			float b = (float)stream.ReceiveNext ();

			SetColor (r,g,b);	// マテリアルの色変更
		}
	}
}
