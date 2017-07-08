using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using UniRx;
using UniRx.Triggers;

public class FieldDataOnline : FieldData
{
    GameObject _BlockObj = null;

    private bool _isInitialized = false;
    private readonly string ReadyStateKey = "SceneReady";
    /// <param name="data"></param>
    private void OnPhotonPlayerPropertiesChanged(object[] data)
    {
        //誰かのカスタムプロパティが書き換わるたびに確認
        CheckAllPlayerState();
    }
    
    private void CheckAllPlayerState()
    {
        if (_isInitialized) return;

        //全員のフラグが設定されているか？
        var isAllPlayerLoaded = PhotonNetwork.playerList
            .Select(x => x.customProperties)
            .All(x => x.ContainsKey(ReadyStateKey) && (bool)x[ReadyStateKey]);

        if (isAllPlayerLoaded)
        {
            //全員のフラグが設定されていたら初期化開始
            _isInitialized = true;
            ClearReadyStatus();
            Initialize();
        }
    }
    
    private void Ready()
    {
        var cp = PhotonNetwork.player.customProperties;
        cp[ReadyStateKey] = true;
        PhotonNetwork.player.SetCustomProperties(cp);
    }
    
    private void ClearReadyStatus()
    {
        var cp = PhotonNetwork.player.customProperties;
        cp[ReadyStateKey] = null;
        PhotonNetwork.player.SetCustomProperties(cp);
    }    

    void Start()
    {
        Ready();
        CheckAllPlayerState();
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    void Initialize()
    {
        //  データ配列生成
        _ObjectDataArray = new FieldObjectBase[GameScaler._nWidth * GameScaler._nHeight];
        _ChangeDataList = new tChangeData[GameScaler._nWidth * GameScaler._nHeight];

        //  フィールドにオブジェクトを生成し、データを格納
        FieldCreatorOnline creator = new FieldCreatorOnline();
        creator.Create();

        UpdateStart();
        StartCoroutine(CharaSet());

        GameObject readyGo = Resources.Load<GameObject>("Prefabs/GameMain/ReadyGoOnline");
        Instantiate(readyGo, readyGo.transform.position, Quaternion.identity);

        Destroy(GameObject.Find("LoadCircle").gameObject);
    }

    protected override void Init()
    {
        return; // 無効化
    }

    protected override IEnumerator CharaSet()
    {
        GameObject[] charas = null;
        yield return new WaitWhile(() =>
        {
            charas = GameObject.FindGameObjectsWithTag("Character");

            //   プレイヤーの数と、生成されたUserキャラが等しくなるまで待つ
            if (charas.Where(x => !x.name.Contains("CPU")).ToArray().Length != PhotonNetwork.playerList.Length)
                return true;

            return false;
        });

        foreach(GameObject obj in charas)
        {
            _CharaList.Add(obj.GetComponent<Character>());
        }
    }

    [PunRPC]
    public void CreateBlock(Vector3 pos)
    {
        if(!_BlockObj)
            _BlockObj = Resources.Load<GameObject>("Prefabs/Field/Block");

        GameObject obj = (GameObject)Instantiate(_BlockObj, pos, _BlockObj.transform.rotation);
    }
}
