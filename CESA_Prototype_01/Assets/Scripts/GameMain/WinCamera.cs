using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class WinCamera : MonoBehaviour
{
    public void Run(Character chara)
    {
        float time = 0.0f;
        Vector3 minPos = transform.position;
        Vector3 maxPos = chara.transform.position + new Vector3(0.0f, 0.5f, -1.5f);

        Vector3 minAngle = transform.eulerAngles;
        Vector3 maxAngle = new Vector3(0.0f, 0.0f, 0.0f);

        Time.timeScale = 0.0f;
        this.UpdateAsObservable()
            .Where(_ => time < 1.0f)
            .Subscribe(_ =>
            {
                time += Time.unscaledDeltaTime;
                if (time > 1.0f)
                {
                    time = 1.0f;
                    Time.timeScale = 1.0f;
                    //CreateWinEffect(chara);
                    SoundManager.Instance.PlaySE(SoundManager.eSeValue.GAMEEND);
                }

                transform.position = Vector3.Lerp(minPos, maxPos, time);
                transform.eulerAngles = Vector3.Lerp(minAngle, maxAngle, time);
            });

        this.ObserveEveryValueChanged(_ => time >= 0.8f)
            .Where(_ => time >= 0.8f)
            .Subscribe(_ =>
            {
                int charaNumber = chara.GetDataNumber();
                FieldObjectBase obj = FieldData.Instance.GetObjData(charaNumber);
                if (obj && obj.tag != "Character")
                {
                    obj.gameObject.SetActive(false);
                    FieldData.Instance.SetObjData(null, charaNumber);
                }

                obj = FieldData.Instance.GetObjData(charaNumber - GameScaler._nWidth);
                if (obj && obj.tag != "Character")
                {
                    obj.gameObject.SetActive(false);
                    FieldData.Instance.SetObjData(null, charaNumber - GameScaler._nWidth);
                }
                obj = FieldData.Instance.GetObjData(charaNumber - GameScaler._nWidth + 1);
                if (obj && obj.tag != "Character")
                {
                    obj.gameObject.SetActive(false);
                    FieldData.Instance.SetObjData(null, charaNumber - GameScaler._nWidth + 1);
                }
                obj = FieldData.Instance.GetObjData(charaNumber - GameScaler._nWidth - 1);
                if (obj && obj.tag != "Character")
                {
                    obj.gameObject.SetActive(false);
                    FieldData.Instance.SetObjData(null, charaNumber - GameScaler._nWidth - 1);
                }
                obj = FieldData.Instance.GetObjData(charaNumber + 1);
                if (obj && obj.tag != "Character")
                {
                    obj.gameObject.SetActive(false);
                    FieldData.Instance.SetObjData(null, charaNumber + 1);
                }
                obj = FieldData.Instance.GetObjData(charaNumber - 1);
                if (obj && obj.tag != "Character")
                {
                    obj.gameObject.SetActive(false);
                    FieldData.Instance.SetObjData(null, charaNumber - 1);
                }
            });
    }

    void CreateWinEffect(Character chara)
    {
        GameObject effectPrefabs = Resources.Load<GameObject>("Prefabs/Effect/IntervalEffect");
        int number = chara.GetDataNumber();

        Instantiate(effectPrefabs).transform.position = chara.GetPosForNumber(number + 1);
        Instantiate(effectPrefabs).transform.position = chara.GetPosForNumber(number - 1);
    }
}
