using System.Runtime.CompilerServices;

namespace UI.Numerics;

// Note: Verify that the behavior of the clamping functions is the same as the C++ versions in wtf/MathExtras.h
public static class MathExtras
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ClampToInt(float value)
    {
        if (float.IsNaN(value)) return 0;
        if (value >= int.MaxValue) return int.MaxValue;
        if (value <= int.MinValue) return int.MinValue;

        return (int)value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ClampToInt(double value)
    {
        if (double.IsNaN(value)) return 0;
        if (value >= int.MaxValue) return int.MaxValue;
        if (value <= int.MinValue) return int.MinValue;

        return (int)value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWithinIntRange(float x) => x > (float)int.MinValue && x < (float)int.MaxValue;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWithinIntRange(double x) => x > (double)int.MinValue && x < (double)int.MaxValue;
}
