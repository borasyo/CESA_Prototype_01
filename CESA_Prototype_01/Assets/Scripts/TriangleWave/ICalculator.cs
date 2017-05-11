using UnityEngine;
using System.Collections;

// 演算を行うオブジェクトを考える
public interface ICalculator<T>
{
    T Add(T l, T r);
    T Sub(T l, T r);
    T Mul(T l, float r);
}

// 型ごとに演算を用意する
public class FloatCalculator : ICalculator<float>
{
    public float Add(float l, float r) { return l + r; }
    public float Sub(float l, float r) { return l - r; }
    public float Mul(float l, float r) { return l * r; }
}

public class Vector3Calculator : ICalculator<Vector3>
{
    public Vector3 Add(Vector3 l, Vector3 r) { return l + r; }
    public Vector3 Sub(Vector3 l, Vector3 r) { return l - r; }
    public Vector3 Mul(Vector3 l, float   r) { return l * r; }
}

public class ColorCalculator : ICalculator<Color>
{
    public Color Add(Color l, Color r) { return l + r; }
    public Color Sub(Color l, Color r) { return l - r; }
    public Color Mul(Color l, float r) { return l * r; }
}