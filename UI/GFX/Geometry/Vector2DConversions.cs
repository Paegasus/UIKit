using static UI.Numerics.SafeConversions;

namespace UI.GFX.Geometry;

public static class Vector2DConversions
{
    // Returns a Vector2D with each component from the input Vector2DF floored.
    public static Vector2D ToFlooredVector2D(in Vector2DF vector)
    {
        return new Vector2D(ClampFloor(vector.X), ClampFloor(vector.Y));
    }

    // Returns a Vector2D with each component from the input Vector2DF ceiled.
    public static Vector2D ToCeiledVector2D(in Vector2DF vector)
    {
        return new Vector2D(ClampCeil(vector.X), ClampCeil(vector.Y));
    }

    // Returns a Vector2D with each component from the input Vector2DF rounded.
    public static Vector2D ToRoundedVector2Dd(in Vector2DF vector)
    {
        return new Vector2D(ClampRound(vector.X), ClampRound(vector.Y));
    }
}
