using System.Runtime.CompilerServices;

namespace UI.Numerics;

public static class CheckedMath
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CheckedAdd(int x, int y, out int result)
    {
        // The addition is performed in a standard unchecked context, which wraps on overflow.
        int sum = x + y;

        // For signed integers, overflow occurs if the operands have the same sign
        // and the result has a different sign. This bitwise check is a highly
        // optimized, branch-free way to detect that condition.
        if (((sum ^ x) & (sum ^ y)) < 0)
        {
            result = 0;
            return false; // Overflow occurred.
        }

        result = sum;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CheckedAdd(uint x, uint y, out uint result)
    {
        uint sum = x + y;

        // For unsigned integers, overflow occurs if the result of the addition
        // is smaller than either of the operands.
        if (sum < x)
        {
            result = 0;
            return false; // Overflow occurred.
        }

        result = sum;
        return true;
    }
}
