using System.Runtime.CompilerServices;

namespace UI.GFX.Animation;

public static class Tween
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float FloatValueBetween(double value, float start, float target)
    {
        return (float) (start + (target - start) * value);
    }
}
  