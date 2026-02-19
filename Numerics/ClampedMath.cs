using System.Runtime.CompilerServices;

namespace UI.Numerics;

public static class ClampedMath
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ClampAdd(int x, int y)
    {
        // Use 'long' to perform the addition, as it can hold any result
        // from adding two 'int's without overflowing.
        long result = (long)x + y;

        // Now, check if the long result fits back into an int.
        if (result > int.MaxValue) return int.MaxValue;
        if (result < int.MinValue) return int.MinValue;

        // The result is within the valid range for an int.
        return (int)result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ClampAdd(long x, long y)
    {
        long result = x + y;

        if (result > long.MaxValue) return long.MaxValue;
        if (result < long.MinValue) return long.MinValue;

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ClampSub(int x, int y)
    {
        // Use 'long' to perform the subtraction, as it can hold any result
        // from subtracting two 'int's without overflowing.
        long result = (long)x - y;

        // Now, check if the long result fits back into an int.
        if (result > int.MaxValue) return int.MaxValue;
        if (result < int.MinValue) return int.MinValue;

        // The result is within the valid range for an int.
        return (int)result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ClampSub(long x, long y)
    {
        long result = x - y;

        if (result > long.MaxValue) return long.MaxValue;
        if (result < long.MinValue) return long.MinValue;

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ClampMul(int x, int y)
    {
        long result = (long)x * y;

        if (result > int.MaxValue) return int.MaxValue;
        if (result < int.MinValue) return int.MinValue;

        return (int)result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ClampMul(long x, long y)
    {
        long result = x * y;

        if (result > long.MaxValue) return long.MaxValue;
        if (result < long.MinValue) return long.MinValue;

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ClampDiv(int x, int y)
    {
        if (y == 0)
            return x >= 0 ? int.MaxValue : int.MinValue;

        // Only overflow case in signed division.
        if (x == int.MinValue && y == -1)
            return int.MaxValue;

        return x / y;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ClampDiv(long x, long y)
    {
        if (y == 0)
            return x >= 0 ? long.MaxValue : long.MinValue;

        // Only overflow case in signed division.
        if (x == long.MinValue && y == -1)
            return long.MaxValue;

        return x / y;
    }
}
