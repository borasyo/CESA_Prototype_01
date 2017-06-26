using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class SandMassData : MonoBehaviour
{
    ///<summary>
    /// 
    /// はさんでいる位置を表示するオブジェクトのデータ配列
    /// 
    /// </summary>

    #region Singleton

    private static SandMassData instance;

    public static SandMassData Instance
    {
        get
        {
            if (instance)
                return instance;

            instance = (SandMassData)FindObjectOfType(typeof(SandMassData));

            if (instance)
                return instance;

            GameObject obj = new GameObject("SandMassData");
            obj.AddComponent<SandMassData>();
            Debug.Log(typeof(SandMassData) + "が存在していないのに参照されたので生成");

            return instance;
        }
    }

    #endregion

    void Awake()
    {
        //  フィールドにオブジェクトを生成し、データを格納
        SandMassCreator creator = new SandMassCreator();
        creator.Create(GameScaler._nWidth, GameScaler._nHeight);
    }

    public void Run()
    {
        float time = 1.0f;
        GameObject obj = Resources.Load<GameObject>("Prefabs/SandItem/SandItem5");
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                time += Time.deltaTime / 1.5f;
            });

        this.ObserveEveryValueChanged(_ => time >= 1.0f)
            .Where(_ => time >= 1.0f)
            .Subscribe(_ =>
            {
                Vector3 createPos = FieldData.Instance.GetNonObjPos();
                if (createPos.x <= 0.0f)
                {
                    time = 0.0f;
                    return;
                }

                FieldObjectBase item = Instantiate(obj, createPos, Quaternion.identity).GetComponent<FieldObjectBase>();
                StartCoroutine(item.gameObject.AddComponent<DelayPut>().Init(item.GetDataNumber()));
                time = 0.0f;
            });
    }
}
