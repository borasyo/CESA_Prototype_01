using UnityEngine;
using System.Collections;

public class BulletScript : Photon.MonoBehaviour {

	// ぶつかったものが、Playerタグだったらダメージを与える
	void OnCollisionEnter(Collision other){

		if (!photonView.isMine)
			return;

		if (other.gameObject.tag == "Player") {
			//other.gameObject.GetComponent<PlayerScript> ().Damage ();
		} else if (other.gameObject.tag == "Boss") {
			PhotonView BossPhotonView = other.gameObject.GetComponent<PhotonView> ();
			BossPhotonView.RPC("Damage", PhotonTargets.All);

			BossPhotonView.RPC("SetColor", PhotonTargets.All, Random.value, Random.value, Random.value);
		}
		PhotonNetwork.Destroy (this.gameObject);
	}
}
