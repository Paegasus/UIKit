namespace UI.GFX.Animation;

public static class Tween
{
    public static float FloatValueBetween(double value, float start, float target)
    {
        return (float) (start + (target - start) * value);
    }
}
  