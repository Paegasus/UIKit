using static UI.Numerics.SafeConversions;

namespace UI.GFX.Geometry;

public static class SizeConversions
{
    // Returns a Size with each component from the input SizeF floored.
    public static Size ToFlooredSize(in SizeF size)
    {
        return new Size(ClampFloor(size.Width), ClampFloor(size.Height));
    }

    // Returns a Size with each component from the input SizeF ceiled.
    public static Size ToCeiledSize(in SizeF size)
    {
        return new Size(ClampCeil(size.Width), ClampCeil(size.Height));
    }

    // Returns a Size with each component from the input SizeF rounded.
    public static Size ToRoundedSize(in SizeF size)
    {
        return new Size(ClampRound(size.Width), ClampRound(size.Height));
    }
}
