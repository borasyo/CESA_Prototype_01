using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldObjectBase : MonoBehaviour
{
    //  データ配列の自分の情報を手動で更新する
    protected void DataUpdate()
    {
        if (_IsDestroy || FieldData.Instance.GetObjData(GetDataNumber()))
            return;

        FieldData.Instance.SetObjData(this, GetDataNumber());
    }

    //  1座標1マスの値に変換
    public Vector3 NormalizePosition()
    {
        return transform.position * (1.0f / GameScaler._fScale);
    }

    public Vector3 NormalizePosition(Vector3 pos)
    {
        return pos * (1.0f / GameScaler._fScale);
    }

    //  1座標1マスの値に変換し、整数化
    public Vector3 NormalizePositionToInt()
    {
        Vector3 npos = NormalizePosition();
        npos.x = Mathf.FloorToInt(npos.x + 0.5f);
        npos.z = Mathf.FloorToInt(npos.z + 0.5f);

        return npos;
    }

    public Vector3 NormalizePositionToInt(Vector3 pos)
    {
        Vector3 npos = NormalizePosition(pos);
        npos.x = Mathf.FloorToInt(npos.x + 0.5f);
        npos.z = Mathf.FloorToInt(npos.z + 0.5f);

        return npos;
    }

    //  現在の自分の位置をデータ上の番号で返す
    public int GetDataNumber()
    {
        Vector3 npos = NormalizePositionToInt();

        return (int)(npos.x + (npos.z * GameScaler._nWidth));
    }

    public int GetDataNumber(Vector3 pos)
    {
        Vector3 npos = NormalizePositionToInt(pos);

        return (int)(npos.x + (npos.z * GameScaler._nWidth));
    }

    public Vector3 GetPosForNumber()
    {
        float x, z;
        x = (float)((GetDataNumber() % GameScaler._nWidth) * GameScaler._fScale);
        z = (float)((GetDataNumber() / GameScaler._nWidth) * GameScaler._fScale);

        return new Vector3(x,0,z);
    }

    public Vector3 GetPosForNumber(int number)
    {
        float x, z;
        x = (float)((number % GameScaler._nWidth) * GameScaler._fScale);
        z = (float)((number / GameScaler._nWidth) * GameScaler._fScale);

        return new Vector3(x,0,z);
    }

    private bool _IsDestroy = false;

    public IEnumerator DestroyObj()
    {
        _IsDestroy = true;

        yield return null;

        Destroy(this.gameObject);
    }
}
