using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using UniRx;
using UniRx.Triggers;

public class SoundManager : MonoBehaviour {

	/// <summary>
	/// 概要 : サウンド管理
	/// Author : 大洞祥太
	/// </summary>

	protected static SoundManager instance;

	public static SoundManager Instance {
		get {
			if (instance == null) {
				instance = (SoundManager)FindObjectOfType(typeof(SoundManager));

				if (instance == null) {
					Debug.LogError("SoundManager Instance Error");
				}
			}

			return instance;
		}
	}

	[SerializeField]
	bool bUseSound = false;
	
	public enum eBgmValue
    {
        TITLE = 0,
        MODESELECT,
        CHARASELECT,
        STAGESELECT,
        GAMEMAIN,

        WALK,
        SPECIALMODE,
        GAMEMAIN_TIMEUP,

        MAX,
	};
	
	public enum eSeValue
    {
        PUT = 0,
		BREAK,
        THUNDER,
        ITEM,
        ITEMGET,
        DEATH,
        GAMESTART,
        GAMEEND,
        TOUCH,
        DECISION,
        CHARACHANGE,
        CPUCHANGE,
        INTERVAL,
        RESULT,
        RESULT_WIN,
        RESULT_LOSE,
        ONWINDOW,
        OFFWINDOW,

        MAX,
	};

	// 音量
	public SoundVolume volume = new SoundVolume();

	// 最大同時再生数
	[SerializeField] int MaxSyncBgmPlay = 5;
	[SerializeField] int MaxSyncSePlay = 20;
	[SerializeField] int MaxSyncVoicePlay = 1;

	AudioSource[] BGMsource;	// BGM
	AudioSource[] SEsources;	// SE
	AudioSource[] VoiceSources;	// 音声

	[SerializeField] AudioClip[] BGM;		// BGM
	[SerializeField] AudioClip[] SE;		// SE
	[SerializeField] AudioClip[] Voice;		// 音声

	void Awake()
    {
        GameObject[] obj = FindObjectsOfType<SoundManager>().Select(x => x.gameObject).ToArray();  //GameObject.FindGameObjectsWithTag("SoundManager");
		if (obj.Length > 1) {
			// 既に存在しているなら削除
			Destroy(gameObject);
		} else {
			// 音管理はシーン遷移では破棄させない
			DontDestroyOnLoad(gameObject);
		}

		// インスタンスの生成
		BGMsource		= new AudioSource[MaxSyncBgmPlay];
		SEsources		= new AudioSource[MaxSyncSePlay];
		VoiceSources	= new AudioSource[MaxSyncVoicePlay];
		volume			= new SoundVolume();

		//----- 全てのAudioSourceコンポーネントを追加する
		// BGM AudioSource
		for (int i = 0; i < BGMsource.Length; i++)
        {
			BGMsource[i] = gameObject.AddComponent<AudioSource> ();
			// BGMはループを有効にする
			BGMsource[i].loop = true;
		}

		// SE AudioSource
		for (int i = 0; i < SEsources.Length; i++) 
			SEsources[i] = gameObject.AddComponent<AudioSource>();

		// 音声 AudioSource
		for (int i = 0; i < VoiceSources.Length; i++)
			VoiceSources[i] = gameObject.AddComponent<AudioSource>();

		// 音量の初期化
		volume.Init();
        foreach (AudioSource source in BGMsource)
            source.volume = 0.25f;
        foreach (AudioSource source in SEsources)
            source.volume = 1.0f;

        this.ObserveEveryValueChanged(_ => Time.timeScale)
            .Where(_ => Time.timeScale > 0.0f)
            .Subscribe(_ =>
            {
                foreach (AudioSource source in BGMsource)
                    source.pitch = Time.timeScale;
                foreach (AudioSource source in SEsources)
                    source.pitch = Time.timeScale;
            }); 
    }

	public bool PlayBGM(eBgmValue i, float volume = 1.0f, bool isOverlap = false)
    {
		if (!bUseSound)
			return false;

		int index = (int)i; 
		if (0 > index || BGM.Length <= index)
			return false;

        if (!isOverlap)
        {
            // 同じBGMの場合は何もしない
            foreach (AudioSource source in BGMsource)
            {
                if (source.clip != BGM[index])
                    continue;

                return false;
            }
        }

		// 再生中で無いAudioSouceで鳴らす
		foreach (AudioSource source in BGMsource)
        {
			if (!source.clip)
            {
                source.clip = BGM[index];
                source.volume = volume;
                source.Play();
				return true;
			}
		}

		return false;
	}

	public void PauseBGM(bool bPause)
    {
		if (bPause)
        {
			foreach (AudioSource source in BGMsource)
				source.Pause ();
		} else
        {
			foreach (AudioSource source in BGMsource)
				source.UnPause ();
		}
	}

	// 指定したBGMがあれば再開or停止
	public bool PauseBGM(eBgmValue i, bool bPause)
    {
		int index = (int)i; 
		if (0 > index || BGM.Length <= index) 
			return false;

		if (bPause)
        {
			foreach (AudioSource source in BGMsource)
            {
				if (source.clip != BGM [index])
					continue;
				
				source.Pause ();
                return true;
			}
		} else
        {
			foreach (AudioSource source in BGMsource)
            {
				if (source.clip != BGM [index])
					continue;
				
				source.UnPause ();
                return true;
			}
        }
        return false;
	}

	public bool StopBGM(eBgmValue i)
    {
		int index = (int)i; 
		if (0 > index || BGM.Length <= index)
			return false;

		// 再生中であれば止める
		foreach (AudioSource source in BGMsource)
        {
			if (source.clip == BGM[index])
            {
				source.clip = null;
				source.Stop();
				return true;
			}
		}

		return false;
	}

	public void StopBGM()
    {
		// 全てのBGM用のAudioSouceを停止する
		foreach (AudioSource source in BGMsource)
        {
			source.Stop();
			source.clip = null;
		}
	}

	// 指定したBGMは再生中なのか 
	public bool NowOnBGM(eBgmValue i)
    {
		int index = (int)i; 
		if (0 > index || BGM.Length <= index) 
			return false;
			
		foreach (AudioSource source in BGMsource)
        {
			if (source.clip != BGM [index])
				continue;
			
			return true;
		}

		return false;
	}

	public void FadeOutVolume(float interval) {
		StartCoroutine(FadeOut(interval));
	}

	public void FadeInVolume(float interval) {
		StartCoroutine(FadeIn(interval));
	}
		
	IEnumerator FadeOut(float interval)
    {
		float time = 0.0f;
		this.volume.OldBGM = this.volume.BGM;
		while (time <= interval)
        {
			this.volume.BGM = Mathf.Lerp(this.volume.OldBGM, 0.0f, time / interval);
			time += Time.deltaTime;
			yield return 0;
		}
	}
		
	IEnumerator FadeIn(float interval)
    {
		float time = 0.0f;
		float VolumeB = this.volume.BGM;
		while (time <= interval)
        {
			this.volume.BGM = Mathf.Lerp(VolumeB, this.volume.OldBGM, time / interval);
			time += Time.deltaTime;
			yield return 0;
		}
	}

    public bool PlaySE(eSeValue i, float volume = 1.0f)
    {
        if (!bUseSound)
            return false;

        int index = (int)i;
        if (0 > index || SE.Length <= index)
            return false;

        // 再生中で無いAudioSouceで鳴らす
        foreach (AudioSource source in SEsources)
        {
            if (!source.isPlaying)
            {
                source.clip = SE[index];
                source.volume = volume;
                source.Play();
                return true;
            }
        }

        return false;
    }

	public void PauseSE(bool bPause)
    {
		if (bPause)
        {
			foreach (AudioSource source in SEsources)
                source.Pause(); 
		} else
        {
			foreach (AudioSource source in SEsources)
                source.UnPause();
		}
	}

	// 指定したSEがあれば再開or停止
	public void PauseSE(eSeValue i, bool bPause)
    {
		int index = (int)i; 
		if (0 > index || SE.Length <= index) 
            return;

		if (bPause)
        {
			foreach (AudioSource source in SEsources)
            {
				if (source.clip != SE [index])
					continue;
				
				source.Pause ();
			}
		} else
        {
			foreach (AudioSource source in SEsources)
            {
				if (source.clip != SE [index])
					continue;
				
				source.UnPause ();
			}
		}
	}

    public void StopSE()
    {
        // 全てのSE用のAudioSouceを停止する
        foreach (AudioSource source in SEsources)
        {
            source.Stop();
            source.clip = null;
        }
    }

    public void StopSE(eSeValue i)
    {
        int index = (int)i;
        if (0 > index || SE.Length <= index)
            return;

        // 全てのSE用のAudioSouceを停止する
        foreach (AudioSource source in SEsources)
        {
			if (source.clip != SE [index])
				continue;
            
            source.Stop();
            source.clip = null;
        }
    }

	public void PlayVoice(int index)
    {
		if (0 > index || Voice.Length <= index) 
			return;
	
        // 再生中で無いAudioSouceで鳴らす
		foreach (AudioSource source in VoiceSources)
        {
			if (false == source.isPlaying)
            {
				source.clip = Voice[index];
				source.Play();
				return;
			}
		}
	}
		
	public void StopVoice()
    {
		// 全ての音声用のAudioSouceを停止する
		foreach (AudioSource source in VoiceSources)
        {
			source.Stop();
			source.clip = null;
		}
	}

	public void SaveVolume()
    {
		PlayerPrefs.SetFloat("BGM", volume.BGM);
		PlayerPrefs.SetFloat("SE", volume.SE);
	}

	[System.Serializable]
	public class SoundVolume
    {
		public float BGM = 1.0f;
		public float Voice = 1.0f;
		public float SE = 1.0f;
		public bool Mute = false;

		public float OldBGM = 1.0f;
		public float OldVoice = 1.0f;
		public float OldSE = 1.0f;

		public void Init()
        {
			OldBGM		= BGM	= PlayerPrefs.GetFloat("BGM", 1.0f);
			OldSE		= SE	= PlayerPrefs.GetFloat("SE", 1.0f);
			OldVoice = Voice = 1.0f;
			Mute = false;
		}
	}
}
