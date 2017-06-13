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

        if (PhotonNetwork.insideLobby)
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
            colBlock.pressedColor = Color.gray / 2.0f;
            button.colors = colBlock;
        }

        this.ObserveEveryValueChanged(_ => PhotonNetwork.insideLobby)
            .Subscribe(_ =>
            {
                if (PhotonNetwork.insideLobby)
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
                    colBlock.pressedColor = Color.gray / 2.0f;
                    button.colors = colBlock;
                }
            });
	}
}
