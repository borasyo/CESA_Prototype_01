using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class WinerPlayer : MonoBehaviour
{
    void Start()
    {
        for (int number = 0; number < 4; number++)
        {
            if (RoundCounter.nRoundCounter[number] < RoundAmount.GetRound())
                continue;

            GetComponent<Image>().sprite = Resources.Load<Sprite>("Texture/Result/winner_" + (number + 1).ToString() + "P");
            break;
        }

        Vector3 min = transform.localScale;
        Vector3 max = transform.localScale * 0.75f;
        TriangleWave<Vector3> triangleScaler = TriangleWaveFactory.Vector3(min, max, 0.25f);
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                triangleScaler.Progress();
                transform.localScale = triangleScaler.CurrentValue;
            });

        if(PhotonNetwork.inRoom)
        {
            SoundManager.Instance.PlaySE(SoundManager.eSeValue.RESULT);
        }
        else
        {
            if(RoundCounter.nRoundCounter[0] >= RoundAmount.GetRound())
            {
                SoundManager.Instance.PlaySE(SoundManager.eSeValue.RESULT_WIN);
            }
            else
            {
                SoundManager.Instance.PlaySE(SoundManager.eSeValue.RESULT_LOSE);
            }
        }
    }
}
