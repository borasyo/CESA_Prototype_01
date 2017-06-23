using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UniRx;
using UniRx.Triggers;

public class NoneJoinLobby : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        Button button = GetComponent<Button>();
        ColorBlock colBlock = new ColorBlock();
        colBlock = button.colors;

        if (PhotonNetwork.insideLobby && PhotonNetwork.GetRoomList().Length < RoomManager.LimitRoomCount)
        {
            colBlock.normalColor = Color.white;
            colBlock.highlightedColor = Color.white;
            colBlock.pressedColor = new Color(0.79f, 0.79f, 0.79f, 1.0f);
            button.colors = colBlock;
        }
        else
        {
            colBlock.normalColor = Color.gray;
            colBlock.highlightedColor = Color.gray;
            colBlock.pressedColor = new Color(Color.gray.r / 2.0f, Color.gray.g / 2.0f, Color.gray.b / 2.0f, 1.0f);
            button.colors = colBlock;
        }

        this.ObserveEveryValueChanged(_ => PhotonNetwork.insideLobby && PhotonNetwork.GetRoomList().Length < RoomManager.LimitRoomCount)
            .Subscribe(_ =>
            {
                if (PhotonNetwork.insideLobby && PhotonNetwork.GetRoomList().Length < RoomManager.LimitRoomCount)
                {
                    colBlock.normalColor = Color.white;
                    colBlock.highlightedColor = Color.white;
                    colBlock.pressedColor = new Color(0.79f, 0.79f, 0.79f, 1.0f);
                    button.colors = colBlock;
                }
                else
                {
                    colBlock.normalColor = Color.gray;
                    colBlock.highlightedColor = Color.gray;
                    colBlock.pressedColor = new Color(Color.gray.r / 2.0f, Color.gray.g / 2.0f, Color.gray.b / 2.0f, 1.0f);
                    button.colors = colBlock;
                }
            });
	}
}
