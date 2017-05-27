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

    public FieldObjectBase CheckObstacleObj(int x, int z, Character me)
    {
        return CheckObstacleObj(ToIdx(x,z), me);
    }

    // 障害物をチェック
    public FieldObjectBase CheckObstacleObj(int idx, Character me)
    {
        if (IsOutOfRange(idx))
            return null;

        FieldObjectBase obj = FieldData.Instance.GetObjData(idx);

        if (!obj)
            return null;

        if (obj.gameObject.tag == "Character")
            return null;

        // TODO : SPEEDの特殊モードのためだけにある判定なので、どうにかして分けたい....
        if (obj.tag != "Block" && me.GetSpecialModeFlg &&
           (me._charaType == Character.eCharaType.SPEED || me._charaType == Character.eCharaType.TECHNICAL))
            return null;

        return obj;
    }
    public FieldObjectBase CheckObstacleObj(int idx, GameObject me)
    {
        if (IsOutOfRange(idx))
            return null;

        FieldObjectBase obj = FieldData.Instance.GetObjData(idx);

        if (!obj)
            return null;

        if (obj.gameObject.tag == "Character")
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

        SandItem.eType type = SandData.Instance.GetSandDataList[idx];
        if (type == SandItem.eType.MAX)
            return false;
        
        switch (player)
        {
            case "1":
                if (type == SandItem.eType.ONE_P)
                    return false;
                break;
            case "2":
                if (type == SandItem.eType.TWO_P)
                    return false;
                break;
            case "3":
                if (type == SandItem.eType.THREE_P)
                    return false;
                break;
            case "4":
                if (type == SandItem.eType.FOUR_P)
                    return false;
                break;
        }
           
        return true;
    }
    public bool TypeCheck(string name, SandItem.eType type)
    {
        switch (type)
        {
            case SandItem.eType.ONE_P:
                if (name.Contains("1P"))
                    return true;
                break;
            case SandItem.eType.TWO_P:
                if (name.Contains("2P"))
                    return true;
                break;
            case SandItem.eType.THREE_P:
                if (name.Contains("3P"))
                    return true;
                break;
            case SandItem.eType.FOUR_P:
                if (name.Contains("4P"))
                    return true;
                break;
        }

        return false;
    }
}
