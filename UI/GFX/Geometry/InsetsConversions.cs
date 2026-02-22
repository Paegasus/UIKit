using static UI.Numerics.SafeConversions;

namespace UI.GFX.Geometry;

public static class InsetsConversions
{
    public static Insets ToFlooredInsets(in InsetsF insets)
    {
        return Insets.TLBR(
            ClampFloor(insets.Top), ClampFloor(insets.Left),
            ClampFloor(insets.Bottom), ClampFloor(insets.Right));
    }

    public static Insets ToCeiledInsets(in InsetsF insets)
    {
        return Insets.TLBR(
            ClampCeil(insets.Top), ClampCeil(insets.Left),
            ClampCeil(insets.Bottom), ClampCeil(insets.Right));
    }

    public static Insets ToRoundedInsets(in InsetsF insets)
    {
        return Insets.TLBR(
            ClampRound(insets.Top), ClampRound(insets.Left),
            ClampRound(insets.Bottom), ClampRound(insets.Right));
    }
}
