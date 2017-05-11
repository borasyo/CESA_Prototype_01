using UnityEngine;
using System.Collections;

// ファクトリ
public class TriangleWaveFactory
{
    public static TriangleWave<float> Float (float min, float max, float halfPeriod_sec)
    {
        return new TriangleWave<float> (min, max, halfPeriod_sec, new FloatCalculator ());
    }

    public static TriangleWave<Vector3> Vector3 (Vector3 min, Vector3 max, float halfPeriod_sec)
    {
        return new TriangleWave<Vector3> (min, max, halfPeriod_sec, new Vector3Calculator ());
    }

    public static TriangleWave<Color> Color (Color min, Color max, float halfPeriod_sec)
    {
        return new TriangleWave<Color> (min, max, halfPeriod_sec, new ColorCalculator ());
    }
}

