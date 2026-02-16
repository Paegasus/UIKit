
using System;
using System.Numerics;

namespace UI.Numerics;

/// <summary>
/// Defines the boundaries for a saturation operation. This allows for custom
/// clamping behavior, similar to the SaturationHandler template in C++.
/// </summary>
public interface ISaturationHandler<T> where T : INumber<T>
{
    /// <summary>The value to return for a NaN input when converting to an integral type.</summary>
    static abstract T NaN();
    /// <summary>The upper boundary to clamp to on overflow.</summary>
    static abstract T Overflow();
    /// <summary>The lower boundary to clamp to on underflow.</summary>
    static abstract T Underflow();
}

/// <summary>
/// The default saturation handler, which clamps to the destination type's
/// maximum, minimum, or infinity values.
/// </summary>
public struct SaturationDefaultLimits<T> : ISaturationHandler<T> where T : INumber<T>
{
    public static T NaN()
    {
        // Replicates C++: NaN source to integer is 0.
        if (T.IsNaN(T.Zero))
            return T.CreateSaturating(double.NaN); // Returns NaN for float/double
        return T.Zero;
    }

    public static T Overflow()
    {
        if (T.IsPositiveInfinity(T.Zero))
            return T.PositiveInfinity;
        return T.MaxValue;
    }

    public static T Underflow()
    {
        if (T.IsNegativeInfinity(T.Zero))
            return T.NegativeInfinity;
        return T.MinValue;
    }
}


/// <summary>
/// Provides safe and explicit numeric conversion methods.
/// </summary>
public static class SafeConversions
{
    /// <summary>
    /// saturated_cast<> is analogous to static_cast<> for numeric types, except
    // that the specified numeric conversion will saturate by default rather than
    // overflow or underflow, and NaN assignment to an integral will return 0.
    // All boundary condition behaviors can be overridden with a custom handler.
    /// </summary>
    public static TDest SaturatedCast<TDest, TSrc, THandler>(TSrc value)
        where TDest : INumber<TDest>, IMinMaxValue<TDest>
        where TSrc : INumber<TSrc>
        where THandler : ISaturationHandler<TDest>
    {
        if (TSrc.IsNaN(value))
            return THandler.NaN();
        
        if (TSrc.IsPositiveInfinity(value))
            return THandler.Overflow();

        if (TSrc.IsNegativeInfinity(value))
            return THandler.Underflow();

        // Compare against the handler's custom boundaries.
        if (TSrc.CreateSaturating(value) > TSrc.CreateSaturating(THandler.Overflow()))
            return THandler.Overflow();

        if (TSrc.CreateSaturating(value) < TSrc.CreateSaturating(THandler.Underflow()))
            return THandler.Underflow();

        // The value is within the custom bounds, so we can safely cast it.
        // CreateTruncating is used because we've already done the safety checks.
        return TDest.CreateTruncating(value);
    }
    
    /// <summary>
    /// A version of SaturatedCast that uses the default saturation limits for the destination type.
    /// </summary>
    public static TDest SaturatedCast<TDest, TSrc>(TSrc value)
        where TDest : INumber<TDest>, IMinMaxValue<TDest>
        where TSrc : INumber<TSrc>
    {
        // Use the default handler. This more closely matches the behavior of the old implementation.
        return SaturatedCast<TDest, TSrc, SaturationDefaultLimits<TDest>>(value);
    }

    /// <summary>
    /// checked_cast<> is analogous to static_cast<> for numeric types,
    /// except that it CHECKs that the specified numeric conversion will not
    /// overflow or underflow. NaN source will always trigger a CHECK.
    /// </summary>
    public static TDest CheckedCast<TDest, TSrc>(TSrc value)
        where TDest : INumber<TDest>
        where TSrc : INumber<TSrc>
    {
        // .NET also provides an optimized checked version.
        return TDest.CreateChecked(value);
    }

    /// <summary>
    /// strict_cast<> is analogous to static_cast<> for numeric types, except that
    /// it will cause a compile failure if the destination type is not large enough
    /// to contain any value in the source type. It performs no runtime checking.
    /// (This must be enforced by a separate static analyzer).
    /// </summary>
    public static TDest StrictCast<TDest, TSrc>(TSrc value)
        where TDest : INumber<TDest>
        where TSrc : INumber<TSrc>
    {
        // This operation truncates, like a standard C# cast (e.g., (short)myInt).
        // The "strictness" must be enforced at compile time by an analyzer.
        return TDest.CreateTruncating(value);
    }

    // floating -> integral conversions that saturate and thus can actually return
    // an integral type.

    // Rounds towards negative infinity (i.e., down).
    public static int ClampFloor(float value) => SaturatedCast<int, float>((float)Math.Floor(value));
    public static int ClampFloor(double value) => SaturatedCast<int, double>(Math.Floor(value));

    // Rounds towards positive infinity (i.e., up).
    public static int ClampCeil(float value) => SaturatedCast<int, float>((float)Math.Ceiling(value));
    public static int ClampCeil(double value) => SaturatedCast<int, double>(Math.Ceiling(value));

    // Rounds towards nearest integer, with ties away from zero.
    public static int ClampRound(float value) => SaturatedCast<int, float>((float)Math.Round(value));
    public static int ClampRound(double value) => SaturatedCast<int, double>(Math.Round(value));

    // This performs a safe, absolute value via unsigned overflow.
    public static uint SafeUnsignedAbs(int value)
    {
        return value < 0 ? 0u - (uint)value : (uint)value; // This works even for int.MinValue, because it uses unsigned wraparound.
    }

    public static bool IsValueInRangeForInt(long value) => value == (int)value;

    public static bool IsValueInRangeForInt(double value) => value >= int.MinValue && value <= int.MaxValue && value == (int)value;

    public static bool IsValueInRangeForInt(float value) => value >= int.MinValue && value <= int.MaxValue && value == (int)value;

    public static bool IsValueInRangeForUInt(int value) => (uint)value <= uint.MaxValue; // always true for int >= 0, false otherwise

    public static bool IsValueInRangeForUInt(long value) => (ulong)value <= uint.MaxValue;
}
