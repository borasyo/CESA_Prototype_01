using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;

public class CPUArriveCheck : MonoBehaviour
{
    void Start()
    {
        NowSelect nowSelect = GetComponentInChildren<NowSelect>();

        // 取得したくないボタンもあるので明示的に取得
        List<Button> buttonList = new List<Button>();
        //buttonList.Add(GetComponent<Button>());
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).tag != "Level")
                continue;

            buttonList.Add(transform.GetChild(i).GetComponent<Button>());
        }

        Button myButton = GetComponent<Button>();
        //GameObject chara = transform.Find("Chara").gameObject;
        this.ObserveEveryValueChanged(_ => nowSelect.CharaType)
            .Subscribe(_ =>
            {
                bool enabled = nowSelect.CharaType != CharacterSelect.eCharaType.NONE;

                myButton.enabled = enabled;
                foreach (Button button in buttonList)
                {
                    button.enabled = enabled;
                    button.GetComponent<Image>().color = enabled ? Color.white : Color.gray;
                }
            });
    }
}
