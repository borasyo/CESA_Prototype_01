using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;

public class OnlineStateChecker : Photon.MonoBehaviour
{
    void Awake()
    {
        bool isInit = false;
        this.ObserveEveryValueChanged(_ => PhotonNetwork.playerList.Length)
            .Subscribe(_ =>
            {
                if (!isInit)
                {
                    isInit = true;
                    return;
                }

                Instantiate(Resources.Load<GameObject>("Prefabs/Error/LeaveRoomCanvas"));

                //  時間と全てのボタンを停止
                Time.timeScale = 0.0f;
                foreach (Button button in FindObjectsOfType<Button>())
                    button.enabled = false;
            });
    }
}
