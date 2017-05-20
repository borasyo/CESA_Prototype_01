using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// AIでFieldDataをチェックする
public class FieldDataChecker : MonoBehaviour
{ 
    #region Singleton

    private static FieldDataChecker instance;

    public static FieldDataChecker Instance
    {
        get
        {
            if (instance)
                return instance;

            instance = (FieldDataChecker)FindObjectOfType(typeof(FieldDataChecker));

            if (instance)
                return instance;

            GameObject obj = new GameObject("FieldDataChecker");
            obj.AddComponent<FieldDataChecker>();
            //Debug.Log(typeof(FieldDataChecker) + "が存在していないのに参照されたので生成");

            return instance;
        }
    }

    #endregion

    int _width; // 幅
	int _height; // 高さ
	int _outOfRange = -1; // 領域外を指定した時の値

	public int Width { get { return _width; } }
	public int Height { get { return _height; } }

	/// 作成
    void Start ()
    {
        _width = GameScaler._nWidth;
		_height = GameScaler._nHeight;
	}

	/// 座標をインデックスに変換する
	public int ToIdx(int x, int z)
    {
		return x + (z * Width);
	}

    /// 領域外かどうかチェックする
    public bool IsOutOfRange(int x, int z)
    {
        return IsOutOfRange(ToIdx(x,z));
    }

    public bool IsOutOfRange(int idx)
    {
        if (idx < 0 || idx >= Width * Height)
            return true;

        // 領域内
        return false;
    }

    public FieldObjectBase CheckObstacleObj(int x, int z, GameObject me)
    {
        return CheckObstacleObj(ToIdx(x,z), me);
    }

    // 障害物をチェック
    public FieldObjectBase CheckObstacleObj(int idx, GameObject me)
    {
        if (IsOutOfRange(idx)) {
			return null;
		}

        FieldObjectBase obj = FieldData.Instance.GetObjData(idx);

        if (obj && obj.gameObject == me)
            return null;

        return obj;
	}

    public bool SandCheck(int x, int z, string p)
    {
        return SandCheck(ToIdx(x,z), p);
    }
    public bool SandCheck(int idx, string name)
    {
        //  変換
        string player = name[name.IndexOf("Player") - 1].ToString();

        List<SandData.tSandData> dataList = SandData.Instance.GetSandDataList.FindAll(_ => _._number == idx);
        if (dataList.Count <= 0)
            return false;

        foreach (SandData.tSandData data in dataList)
        {
            switch (player)
            {
                case "1":
                    if (data._Type == SandItem.eType.ONE_P)
                        return false;
                    break;
                case "2":
                    if (data._Type == SandItem.eType.TWO_P)
                        return false;
                    break;
                case "3":
                    if (data._Type == SandItem.eType.THREE_P)
                        return false;
                    break;
                case "4":
                    if (data._Type == SandItem.eType.FOUR_P)
                        return false;
                    break;
            }
        }
        return true;
    }
}
