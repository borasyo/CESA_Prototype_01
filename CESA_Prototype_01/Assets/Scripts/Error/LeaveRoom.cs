using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class LeaveRoom : MonoBehaviour
{
    void Awake()
    {
        StartCoroutine(FadeManager.Instance.StopFade());
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            this.UpdateAsObservable()
               .Subscribe(_ =>
               {
                   if (Input.touchCount <= 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                       return;

                   Return();
               });
        }

        else
        {
            this.UpdateAsObservable()
               .Subscribe(_ =>
               {
                   if (!Input.GetMouseButtonDown(0))
                       return;

                   Return();
               });
        }
    }

    void Return()
    {
        PhotonNetwork.LeaveRoom();      // 退室
        CharacterSelectOnline._nMyNumber = 0;
        SceneChanger.Instance.ChangeScene("ModeSelect", true);
        SoundManager.Instance.PlaySE(SoundManager.eSeValue.DECISION);
    }

    void OnDestroy()
    {
        Time.timeScale = 1.0f;
    }
}
