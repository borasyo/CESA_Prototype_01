using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// 2次元レイヤー
public class Layer2D {

    GameObject _me;
	int _width; // 幅
	int _height; // 高さ
	int _outOfRange = -1; // 領域外を指定した時の値

	/// 幅
	public int Width {
		get { return _width; }
	}
	/// 高さ
	public int Height {
		get { return _height; }
	}

	/// 作成
	public void Create(GameObject me) {
        _me = me;
        _width = GameScaler._nWidth;
		_height = GameScaler._nHeight;
	}

	/// 座標をインデックスに変換する
	public int ToIdx(int x, int z) {
		return x + (z * Width);
	}

	/// 領域外かどうかチェックする
	public bool IsOutOfRange(int x, int z) {
		if(x < 0 || x >= Width) { return true; }
		if(z < 0 || z >= Height) { return true; }

		// 領域内
		return false;
	}
	/// 値の取得
	// @param x X座標
	// @param y Y座標
	// @return 指定の座標の値（領域外を指定したら_outOfRangeを返す）
	public FieldObjectBase Get(int x, int z) {
		if(IsOutOfRange(x, z)) {
			return null;
		}

        FieldObjectBase obj = FieldData.Instance.GetObjData(ToIdx(x, z));

        if (obj && obj.gameObject == _me)
            return null;

        return obj;
	}

    public bool SandCheck(int x, int z, string p) {
        List<SandData.tSandData> dataList = SandData.Instance.GetSandDataList.FindAll(_ => _._number == ToIdx(x,z));
        if (dataList.Count <= 0)
            return false;

        foreach (SandData.tSandData data in dataList)
        {
            switch (p)
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
