using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UI.Extensions;
using UI.Numerics;

namespace UI.GFX.Geometry;

[StructLayout(LayoutKind.Sequential)]
public struct Matrix3F
{
    // Row-major storage: data_[i * 3 + j] = element at row i, col j.
    // Matches C++ MatrixToArrayCoords(i, j) = i * 3 + j.
    [InlineArray(9)]
    private struct Matrix3FData { private float _element0; }

    private Matrix3FData _data;

    public static Matrix3F Zeros() => new();  // InlineArray zero-initializes

    public static Matrix3F Ones()
    {
        Matrix3F m = new();
        m.Set(1, 1, 1, 1, 1, 1, 1, 1, 1);
        return m;
    }

    public static Matrix3F Identity()
    {
        Matrix3F m = new();
        m.Set(1, 0, 0, 0, 1, 0, 0, 0, 1);
        return m;
    }

    public static Matrix3F FromOuterProduct(in Vector3DF a, in Vector3DF bt)
    {
        Matrix3F m = new();
        m.Set(a.X * bt.X, a.X * bt.Y, a.X * bt.Z,
              a.Y * bt.X, a.Y * bt.Y, a.Y * bt.Z,
              a.Z * bt.X, a.Z * bt.Y, a.Z * bt.Z);
        return m;
    }

    public readonly float Get(int i, int j)
    {
        Debug.Assert(i >= 0 && i < 3 && j >= 0 && j < 3);
        return ((ReadOnlySpan<float>)_data)[i * 3 + j];
    }

    public void Set(int i, int j, float v)
    {
        Debug.Assert(i >= 0 && i < 3 && j >= 0 && j < 3);
        ((Span<float>)_data)[i * 3 + j] = v;
    }

    public void Set(float m00, float m01, float m02,
                    float m10, float m11, float m12,
                    float m20, float m21, float m22)
    {
        Span<float> d = _data;
        d[0] = m00; d[1] = m01; d[2] = m02;
        d[3] = m10; d[4] = m11; d[5] = m12;
        d[6] = m20; d[7] = m21; d[8] = m22;
    }

    public readonly Vector3DF GetRow(int i)
    {
        Debug.Assert(i >= 0 && i < 3);
        return new Vector3DF(Get(i, 0), Get(i, 1), Get(i, 2));
    }

    public readonly Vector3DF GetColumn(int j)
    {
        Debug.Assert(j >= 0 && j < 3);
        return new Vector3DF(Get(0, j), Get(1, j), Get(2, j));
    }

    public void SetColumn(int j, in Vector3DF c)
    {
        Set(0, j, c.X);
        Set(1, j, c.Y);
        Set(2, j, c.Z);
    }

    public readonly Matrix3F Add(in Matrix3F rhs)
    {
        Matrix3F result = new();
        Span<float> r = result._data;
        ReadOnlySpan<float> a = _data;
        ReadOnlySpan<float> b = rhs._data;
        for (int i = 0; i < 9; i++) r[i] = a[i] + b[i];
        return result;
    }

    public readonly Matrix3F Subtract(in Matrix3F rhs)
    {
        Matrix3F result = new();
        Span<float> r = result._data;
        ReadOnlySpan<float> a = _data;
        ReadOnlySpan<float> b = rhs._data;
        for (int i = 0; i < 9; i++) r[i] = a[i] - b[i];
        return result;
    }

    public readonly Matrix3F Inverse()
    {
        ReadOnlySpan<float> d = _data;
        double det = Determinant3x3(d);
        if (float.MachineEpsilon > Math.Abs(det))
            return Zeros();

        Matrix3F inv = new();
        inv.Set(
            (float)((d[4] * d[8] - d[5] * d[7]) / det),
            (float)((d[2] * d[7] - d[1] * d[8]) / det),
            (float)((d[1] * d[5] - d[2] * d[4]) / det),
            (float)((d[5] * d[6] - d[3] * d[8]) / det),
            (float)((d[0] * d[8] - d[2] * d[6]) / det),
            (float)((d[2] * d[3] - d[0] * d[5]) / det),
            (float)((d[3] * d[7] - d[4] * d[6]) / det),
            (float)((d[1] * d[6] - d[0] * d[7]) / det),
            (float)((d[0] * d[4] - d[1] * d[3]) / det));
        return inv;
    }

    public readonly Matrix3F Transpose()
    {
        Matrix3F t = new();
        ReadOnlySpan<float> d = _data;
        t.Set(d[0], d[3], d[6],
              d[1], d[4], d[7],
              d[2], d[5], d[8]);
        return t;
    }

    public readonly float Determinant() => (float)Determinant3x3(_data);

    public readonly float Trace() => Get(0, 0) + Get(1, 1) + Get(2, 2);

    // Higher precision determinant used internally for Inverse().
    private static double Determinant3x3(ReadOnlySpan<float> d) =>
        (double)d[0] * ((double)d[4] * d[8] - (double)d[5] * d[7]) +
        (double)d[1] * ((double)d[5] * d[6] - (double)d[3] * d[8]) +
        (double)d[2] * ((double)d[3] * d[7] - (double)d[4] * d[6]);

    public static Matrix3F MatrixProduct(in Matrix3F lhs, in Matrix3F rhs)
    {
        Matrix3F result = Zeros();
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                result.Set(i, j, Vector3DF.DotProduct(lhs.GetRow(i), rhs.GetColumn(j)));
        return result;
    }

    public static Vector3DF MatrixProduct(in Matrix3F lhs, in Vector3DF rhs) =>
        new(Vector3DF.DotProduct(lhs.GetRow(0), rhs),
            Vector3DF.DotProduct(lhs.GetRow(1), rhs),
            Vector3DF.DotProduct(lhs.GetRow(2), rhs));

    public readonly bool IsEqual(in Matrix3F rhs)
    {
        ReadOnlySpan<float> a = _data;
        ReadOnlySpan<float> b = rhs._data;
        return a.SequenceEqual(b);
    }

    public readonly bool IsNear(in Matrix3F rhs, float precision)
    {
        Debug.Assert(precision >= 0);
        ReadOnlySpan<float> a = _data;
        ReadOnlySpan<float> b = rhs._data;
        for (int i = 0; i < 9; i++)
            if (MathF.Abs(a[i] - b[i]) > precision) return false;
        return true;
    }

    public override readonly string ToString() =>
        $"[[{Get(0,0):+0.0000;-0.0000}, {Get(0,1):+0.0000;-0.0000}, {Get(0,2):+0.0000;-0.0000}]," +
        $" [{Get(1,0):+0.0000;-0.0000}, {Get(1,1):+0.0000;-0.0000}, {Get(1,2):+0.0000;-0.0000}]," +
        $" [{Get(2,0):+0.0000;-0.0000}, {Get(2,1):+0.0000;-0.0000}, {Get(2,2):+0.0000;-0.0000}]]";

    public readonly bool Equals(in Matrix3F other) => IsEqual(other);
    public override readonly bool Equals(object? obj) => obj is Matrix3F other && IsEqual(other);
    public static bool operator ==(in Matrix3F lhs, in Matrix3F rhs) => lhs.IsEqual(rhs);
    public static bool operator !=(in Matrix3F lhs, in Matrix3F rhs) => !lhs.IsEqual(rhs);
    public static Matrix3F operator +(in Matrix3F lhs, in Matrix3F rhs) => lhs.Add(rhs);
    public static Matrix3F operator -(in Matrix3F lhs, in Matrix3F rhs) => lhs.Subtract(rhs);

    public override readonly int GetHashCode()
    {
        var hc = new HashCode();
        ReadOnlySpan<float> d = _data;
        for (int i = 0; i < 9; i++) hc.Add(d[i]);
        return hc.ToHashCode();
    }
}