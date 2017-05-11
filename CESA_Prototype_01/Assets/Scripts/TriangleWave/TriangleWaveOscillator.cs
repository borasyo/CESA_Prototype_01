using UnityEngine;
using System.Collections;

public class TriangleWaveOscillator
{
    float m_fTime;
    public float m_fValue = 0f; // 0-1 の範囲

    float m_fHalfPeriod_Sec;    // 間隔(半往復)

    public TriangleWaveOscillator (float halfPeriod_Sec)
    {
        m_fHalfPeriod_Sec = halfPeriod_Sec;
    }
        
    // TODO : 今のところ必要ない
    //    public void Set (float halfPeriod_sec) { m_fHalfPeriod_Sec = halfPeriod_sec; }

    public int GetHalfLapCnt { get { return (int)(m_fTime / m_fHalfPeriod_Sec); } }

    public int GetLapCnt { get { return (int)(m_fTime / (m_fHalfPeriod_Sec * 2.0f)); } }

    public virtual void Progress ()
    {
        // 指定した間隔で補正したdeltaTimeを加算
        m_fTime += Time.deltaTime;

        float period = m_fHalfPeriod_Sec * 2.0f; // 半周期を1周期に変換
        m_fValue = Mathf.Acos (Mathf.Cos (2.0f * Mathf.PI * m_fTime / period)) / Mathf.PI;
    }
}
