using UnityEngine;
using System.Collections;

// シーン遷移用
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour {

	/// <summary>
	/// 概要 : シーン遷移を管理
	/// Author : 大洞祥太
    /// </summary>

    #region Singleton
    
    private static SceneChanger instance;

	public static SceneChanger Instance {
		get {
			if (instance) 
                return instance;

			instance = (SceneChanger)FindObjectOfType(typeof(SceneChanger));

            if (instance) 
                return instance;

            GameObject obj = new GameObject();
            obj.AddComponent<SceneChanger>();
			Debug.Log(typeof(SceneChanger) + "が存在していないのに参照されたので生成");
            
            return instance;
		}
	}

    #endregion

    public void Awake() {
		if (this != Instance) {
			Destroy (this.gameObject);
			return;
		}

		DontDestroyOnLoad (this.gameObject);
	}

	public void ChangeScene(string sceneName, bool bNext, bool bStopBgm = true) {
		if (FadeManager.Instance.Fading) 
			return;

		FadeManager.Instance.LoadLevel(sceneName, 1.0f, bStopBgm);

		/*if (bNext) {
			SoundManager.Instance.PlaySE (SoundManager.eSeValue.SE_SCENECHANGE);
		} else {
			SoundManager.Instance.PlaySE (SoundManager.eSeValue.SE_OFFWINDOW);
		}*/
	}
}
