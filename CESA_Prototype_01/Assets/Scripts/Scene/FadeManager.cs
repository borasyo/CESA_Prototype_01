using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
	/// <summary>
	/// 概要 : シーン遷移時のフェードイン・アウトを制御するためのクラス .
	/// Author : 大洞祥太
	/// </summary>

    #region Singleton

    private static FadeManager instance;

    public static FadeManager Instance {
        get {
            if (instance)
                return instance;

            instance = (FadeManager)FindObjectOfType(typeof(FadeManager));

            if (instance)
                return instance;

            GameObject obj = new GameObject();
            obj.AddComponent<FadeManager>();
            Debug.Log(typeof(FadeManager) + "が存在していないのに参照されたので生成");

            return instance;
        }
    }

    #endregion Singleton

    /// フェード中の透明度
    float fadeAlpha = 0;
    /// フェード中かどうか
    bool isFading = false;
    public bool Fading { get { return isFading; } }
    public bool HalfFading { get; private set; }

    /// フェード色
	[SerializeField]
	Color fadeColor = Color.black;


    public void Awake() {
        if (this != Instance) {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
        HalfFading = false;
    }

    public void OnGUI()
    {
        if (this.isFading)
        {
            //色と透明度を更新して白テクスチャを描画 .
            this.fadeColor.a = this.fadeAlpha;
            GUI.color = this.fadeColor;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        }
    }

    /// <param name='scene'>シーン名</param>
    /// <param name='interval'>暗転にかかる時間(秒)</param>
	public void LoadLevel(string scene, float interval, bool bStopBgm)
    {
        if (isFading) return;
		this.isFading = true;
        HalfFading = true;

        StartCoroutine(TransScene(scene, interval, bStopBgm));
    }

    /// <param name='scene'>シーン名</param>
    /// <param name='interval'>暗転にかかる時間(秒)</param>
	private IEnumerator TransScene(string scene, float interval, bool bStopBgm)
    {
        //だんだん暗く .
//        this.isFading = true;
        float time = 0;
        while (time <= interval)
        {
            this.fadeAlpha = Mathf.Lerp(0.0f, 1.0f, time / interval);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

		if (bStopBgm) {
			SoundManager.Instance.StopBGM ();
		}

        HalfFading = false;

        yield return null;

        // キャッシュの開放（場合に合わせて行った方がいいと思う
        //Resources.UnloadUnusedAssets();
        //シーン名を確認し、あればシーン切替 .
        if (scene != "")
            SceneManager.LoadScene(scene);

        //yield return new WaitWhile(() => Time.unscaledDeltaTime > 0.1f);

        //だんだん明るく .
        time = 0;
		while (this.fadeAlpha >= 0.2f)
        {
            this.fadeAlpha = Mathf.Lerp(1.0f, 0.0f, time / interval);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        this.isFading = false;
    }
}

