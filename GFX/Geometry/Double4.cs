using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UI.GFX.Geometry;

[StructLayout(LayoutKind.Sequential)]
// Mirrors the C++ Double4 SIMD type.
public struct Double4(double v0, double v1, double v2, double v3)
{
    public double V0 = v0, V1 = v1, V2 = v2, V3 = v3;

    public static Double4 operator +(Double4 a, Double4 b) => new(a.V0 + b.V0, a.V1 + b.V1, a.V2 + b.V2, a.V3 + b.V3);
    public static Double4 operator -(Double4 a, Double4 b) => new(a.V0 - b.V0, a.V1 - b.V1, a.V2 - b.V2, a.V3 - b.V3);
    public static Double4 operator *(Double4 a, Double4 b) => new(a.V0 * b.V0, a.V1 * b.V1, a.V2 * b.V2, a.V3 * b.V3);
    public static Double4 operator *(Double4 a, double s) => new(a.V0 * s, a.V1 * s, a.V2 * s, a.V3 * s);

    // {v[2], v[3], v[0], v[1]}
    public static Double4 SwapHighLow(Double4 v) => new(v.V2, v.V3, v.V0, v.V1);
    // {v[1], v[0], v[3], v[2]}
    public static Double4 SwapInPairs(Double4 v) => new(v.V1, v.V0, v.V3, v.V2);

    public static double Sum(Double4 v) => v.V0 + v.V1 + v.V2 + v.V3;
}
