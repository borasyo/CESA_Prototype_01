using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class CharaDescription : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        Text text = GetComponent<Text>();
        NowSelect nowSelect = transform.parent.GetComponentInChildren<NowSelect>();

        string[] description = new string[(int)CharacterSelect.eCharaType.MAX + 1]
        {
            "",
            "バランスの良い初心者向けキャラ\nアイテムを取ると\nポールを3つ置けるようになるぞ！",
            "ゲージが溜まるのが早いキャラ\nアイテムを取るとゲージを使わず\n何でも破壊できるぞ！",
            "移動が速い俊足キャラ\nアイテムを取ると\nさらに移動が速くなり\nポールをすり抜けられるぞ！",
            "ポールを置いてから3秒後に\n出現させる上級者向けキャラ\nアイテムを取ると目の前の\nポールを飛ばすことが出来るぞ！",
            "",
        };

        this.ObserveEveryValueChanged(_ => nowSelect.CharaType)
            .Subscribe(_ =>
            {
                text.text = description[(int)nowSelect.CharaType];
            });
    }
}
