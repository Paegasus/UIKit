namespace UI.Numerics;

public static class AngleConversions
{
    public static float DegToRad(float deg)
    {
        return deg * MathF.PI / 180.0f;
    }

    public static float RadToDeg(float rad)
    {
        return rad * 180.0f / MathF.PI;
    }

    public static double DegToRad(double deg)
    {
        return deg * Math.PI / 180.0d;
    }

    public static double RadToDeg(double rad)
    {
        return rad * 180.0d / Math.PI;
    }
}
