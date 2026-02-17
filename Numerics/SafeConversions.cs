using System.Numerics;

namespace UI.Numerics;

/// <summary>
/// Provides safe and explicit numeric conversion methods.
/// </summary>
public static class SafeConversions
{
    public static TDest CheckedCast<TDest, TSrc>(TSrc value) where TDest : INumber<TDest> where TSrc : INumber<TSrc>
    {
        return TDest.CreateChecked(value);
    }

    public static TDest StrictCast<TDest, TSrc>(TSrc value) where TDest : INumber<TDest> where TSrc : INumber<TSrc>
    {
        return TDest.CreateTruncating(value);
    }

    // floating point -> integral conversions

    public static int ClampFloor(float value)
    {
        value = MathF.Floor(value);

        if (float.IsNaN(value)) return 0;
        if (value > int.MaxValue) return int.MaxValue;
        if (value < int.MinValue) return int.MinValue;

        return (int)value;
    }

    public static int ClampFloor(double value)
    {
        value = Math.Floor(value);

        if (double.IsNaN(value)) return 0;
        if (value > int.MaxValue) return int.MaxValue;
        if (value < int.MinValue) return int.MinValue;

        return (int)value;
    }
    

    public static int ClampCeil(float value)
    {
        value = MathF.Ceiling(value);

        if (float.IsNaN(value)) return 0;
        if (value > int.MaxValue) return int.MaxValue;
        if (value < int.MinValue) return int.MinValue;

        return (int)value;
    }

    public static int ClampCeil(double value)
    {
        value = Math.Ceiling(value);

        if (double.IsNaN(value)) return 0;
        if (value > int.MaxValue) return int.MaxValue;
        if (value < int.MinValue) return int.MinValue;

        return (int)value;
    }

    public static int ClampRound(float value)
    {
        value = MathF.Round(value);

        if (float.IsNaN(value)) return 0;
        if (value > int.MaxValue) return int.MaxValue;
        if (value < int.MinValue) return int.MinValue;

        return (int)value;
    }

    public static int ClampRound(double value)
    {
        value = Math.Round(value);

        if (double.IsNaN(value)) return 0;
        if (value > int.MaxValue) return int.MaxValue;
        if (value < int.MinValue) return int.MinValue;

        return (int)value;
    }

    public static uint SafeUnsignedAbs(int value) => (uint)(value < 0 ? -value : value);
}
