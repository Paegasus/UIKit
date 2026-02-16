
using System;
using System.Numerics;

namespace UI.Numerics;

/// <summary>
/// Defines the boundaries for a saturation operation. This allows for custom
/// clamping behavior, similar to the SaturationHandler template in C++.
/// </summary>
public interface ISaturationHandler<T> where T : INumber<T>
{
    /// <summary>The value to return for a NaN input.</summary>
    static abstract T NaN();
    /// <summary>The upper boundary to clamp to on overflow.</summary>
    static abstract T Overflow();
    /// <summary>The lower boundary to clamp to on underflow.</summary>
    static abstract T Underflow();
}

/// <summary>
/// The default saturation handler for floating-point types, clamping to infinity and NaN.
/// </summary>
public struct SaturationDefaultLimitsFloat<T> : ISaturationHandler<T>
    where T : IFloatingPointIeee754<T>
{
    public static T NaN() => T.NaN;
    public static T Overflow() => T.PositiveInfinity;
    public static T Underflow() => T.NegativeInfinity;
}

/// <summary>
/// The default saturation handler for integer types, clamping to MaxValue and MinValue.
/// </summary>
public struct SaturationDefaultLimitsInt<T> : ISaturationHandler<T>
    where T : IBinaryInteger<T>
{
    public static T NaN() => T.Zero;
    public static T Overflow() => T.MaxValue;
    public static T Underflow() => T.MinValue;
}


/// <summary>
/// Provides safe and explicit numeric conversion methods.
/// </summary>
public static class SafeConversions
{
    /// <summary>
    /// The core implementation of saturated_cast. It uses a generic saturation handler to define its clamping boundaries.
    /// </summary>
    public static TDest SaturatedCast<TDest, TSrc, THandler>(TSrc value)
        where TDest : INumber<TDest>
        where TSrc : IFloatingPointIeee754<TSrc> // Source is always float/double for these conversions
        where THandler : ISaturationHandler<TDest>
    {
        if (TSrc.IsNaN(value))
            return THandler.NaN();
        
        if (TSrc.IsPositiveInfinity(value))
            return THandler.Overflow();

        if (TSrc.IsNegativeInfinity(value))
            return THandler.Underflow();

        if (TSrc.CreateSaturating(value) > TSrc.CreateSaturating(THandler.Overflow()))
            return THandler.Overflow();

        if (TSrc.CreateSaturating(value) < TSrc.CreateSaturating(THandler.Underflow()))
            return THandler.Underflow();

        return TDest.CreateTruncating(value);
    }

    /// <summary>
    /// Public overload of SaturatedCast for floating-point destinations. Uses the float-specific default handler.
    /// </summary>
    public static TDest SaturatedCast<TDest, TSrc>(TSrc value)
        where TDest : IFloatingPointIeee754<TDest>
        where TSrc : IFloatingPointIeee754<TSrc>
    {
        return SaturatedCast<TDest, TSrc, SaturationDefaultLimitsFloat<TDest>>(value);
    }

    /// <summary>
    /// Public overload of SaturatedCast for integer destinations. Uses the int-specific default handler.
    /// </summary>
    public static TDest SaturatedCast<TDest, TSrc>(TSrc value)
        where TDest : IBinaryInteger<TDest>
        where TSrc : IFloatingPointIeee754<TSrc>
    {
        return SaturatedCast<TDest, TSrc, SaturationDefaultLimitsInt<TDest>>(value);
    }

    public static TDest CheckedCast<TDest, TSrc>(TSrc value)
        where TDest : INumber<TDest>
        where TSrc : INumber<TSrc>
    {
        return TDest.CreateChecked(value);
    }

    public static TDest StrictCast<TDest, TSrc>(TSrc value)
        where TDest : INumber<TDest>
        where TSrc : INumber<TSrc>
    {
        return TDest.CreateTruncating(value);
    }

    // floating -> integral conversions
    public static int ClampFloor(float value) => SaturatedCast<int, float>(MathF.Floor(value));
    public static int ClampFloor(double value) => SaturatedCast<int, double>(Math.Floor(value));
    public static int ClampCeil(float value) => SaturatedCast<int, float>(MathF.Ceiling(value));
    public static int ClampCeil(double value) => SaturatedCast<int, double>(Math.Ceiling(value));
    public static int ClampRound(float value) => SaturatedCast<int, float>(MathF.Round(value));
    public static int ClampRound(double value) => SaturatedCast<int, double>(Math.Round(value));

    public static uint SafeUnsignedAbs(int value) => (uint)(value < 0 ? -value : value);
}
