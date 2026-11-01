using System.Diagnostics;
using System.Runtime.CompilerServices;

using SkiaSharp;

using UI.GFX.Geometry;

using static UI.Numerics.SafeConversions;

namespace UI.GFX.Animation;

// Ported from ui/gfx/animation/tween.h and tween.cc.
public static class Tween
{
    public enum TweenType
    {
        Linear,       // Linear.
        EaseOut,      // Fast in, slow out (default).
        EaseOut2,     // Variant of EaseOut that ends slower than EaseOut.
        EaseOut3,     // Variant of EaseOut that ends slower than EaseOut2.
        EaseOut4,     // Variant of EaseOut that starts slower than EaseOut3,
                      // and ends faster. Best used to lead into a bounce
                      // animation.
        EaseIn,       // Slow in, fast out.
        EaseIn2,      // Variant of EaseIn that starts out slower than EaseIn.
        EaseInOut,    // Slow in and out, fast in the middle.
        EaseInOutEmphasized,  // Variant of EaseInOut that starts slowly,
                              // speeds up shortly thereafter, and then ends
                              // slowly. This curve can be imagined as a
                              // steeper version of EaseInOut.
        EaseInOut2,       // Variant of EaseInOut that starts and ends slower
                          // than EaseInOut.
        SmoothInOut,      // Smooth, consistent speeds in and out (sine wave).
        FastOutSlowIn,    // Variant of EaseInOut which should be used in most
                          // cases.
        FastOutSlowIn2,   // Variant of FastOutSlowIn that starts out quicker.
        FastOutSlowIn3,   // Variant of FastOutSlowIn that starts out quicker
                          // than FastOutSlowIn2. Best used for rebound in
                          // bounce animation.
        LinearOutSlowIn,  // Variant of EaseOut which should be used for fading
                          // in from 0% or motion when entering a scene.
        SlowOutLinearIn,  // Reverse of LinearOutSlowIn which should be used in
                          // reverse animation to create a rubberband effect.
        FastOutLinearIn,  // Variant of EaseIn which should be used for fading
                          // out to 0% or motion when exiting a scene.
        Zero,             // Returns a value of 0 always.

        // TODO(zxdan): New animation curve name convention will be used to
        // resolve the confusion caused by "IN" and "OUT".

        // The new name convention is below:
        // ACCEL_<1>_DECEL_<2> where <1> and <2> are used to express the
        // acceleration and deceleration speeds. The corresponding cubic bezier
        // curve parameters would be ( 0.01 * <1>, 0, 1 - 0.01 * <2>, 1 ).
        // Note that LIN means the speed is 0. For example,
        // ACCEL_20_DECEL_20 = (0.2, 0, 0.8, 1)
        // ACCEL_100_DECEL_100 = (1, 0, 0, 1)
        // ACCEL_LIN_DECEL_LIN = (0, 0, 1, 1)
        // ACCEL_40_DECEL_20 = (0.4, 0, 0.8, 1)
        // ACCEL_<1>_<2>_DECEL_<3>_<4> correspond to cubic bezier with curve
        // parameters (0.01 * <1>, 0.01 * <2>, 1 - 0.01 * <3>, 1 - 0.01 * <4>).
        AccelLinDecel60,   // Pulling a small to medium element into a place.
        AccelLinDecel100,  // Pulling a small to medium element into a place
                           // that has very fast deceleration.
        // Starts with linear speed and soft deceleration. Use for elements
        // that are not visible at the beginning of a transition, but are
        // visible at the end.
        AccelLinDecel100_3,
        Accel20Decel60,  // Moving a small, low emphasis or responsive elements.
        Accel20Decel100,
        Accel30Decel20_85,
        Accel30DecelLin,  // Used for browser controls snap animation.
        Accel40Decel20,
        // Moderate acceleration and soft deceleration. Used for elements that
        // are visible at the beginning and end of a transition.
        Accel40Decel100_3,
        Accel45Decel88,     // A more dramatic version of EaseInOut that starts
                            // very slowly, accelerates very rapidly, and then
                            // decelerates very rapidly. This creates a snappy,
                            // high-energy transition.
        Accel80Decel20,     // Slow in and fast out with ease.
        Accel0_40Decel100,  // Specialized curve with an emphasized deceleration
                            // drift.
        Accel0_80Decel80,   // Variant of Accel0_40Decel100 which drops in
                            // value faster, but flattens out into the drift
                            // sooner.

        Accel0_100Decel80,  // Variant of Accel0_80Decel80 which drops in value
                            // even faster.

        Accel5_70Decel90,  // Start at peak velocity and very soft
                           // deceleration.
    }

    private static readonly ThreePointCubicBezier kEaseInOutEmphasized =
        new(0.05, 0, 0.133333, 0.06, 0.166666, 0.4, 0.208333, 0.82, 0.25, 1);

    private static readonly CubicBezier kEaseInOut2 = new(0.33, 0, 0.67, 1);
    private static readonly CubicBezier kEaseOut2 = new(0.4, 0, 0, 1);
    private static readonly CubicBezier kEaseOut3 = new(0.6, 0, 0, 1);
    private static readonly CubicBezier kEaseOut4 = new(1, 0, 0.8, 1);
    private static readonly CubicBezier kFastOutSlowIn = new(0.4, 0, 0.2, 1);
    private static readonly CubicBezier kFastOutSlowIn2 = new(0.2, 0, 0.2, 1);
    private static readonly CubicBezier kFastOutSlowIn3 = new(0.2, 0, 0, 1);
    private static readonly CubicBezier kLinearOutSlowIn = new(0, 0, .2, 1);
    private static readonly CubicBezier kSlowOutLinearIn = new(0, 0, 1, .2);
    private static readonly CubicBezier kFastOutLinearIn = new(0.4, 0, 1, 1);
    private static readonly CubicBezier kAccelLinDecel60 = new(0, 0, 0.4, 1);
    private static readonly CubicBezier kAccelLinDecel100 = new(0, 0, 0, 1);
    private static readonly CubicBezier kAccelLinDecel100_3 = new(0, 0, 0, 0.97);
    private static readonly CubicBezier kAccel20Decel60 = new(0.2, 0, 0.4, 1);
    private static readonly CubicBezier kAccel20Decel100 = new(0.2, 0, 0, 1);
    private static readonly CubicBezier kAccel30Decel20_85 = new(0.3, 0, 0.8, 0.15);
    private static readonly CubicBezier kAccel30DecelLin = new(0.3, 0, 1, 1);
    private static readonly CubicBezier kAccel40Decel20 = new(0.4, 0, 0.8, 1);
    private static readonly CubicBezier kAccel45Decel88 = new(0.45, 0.00, 0.12, 1.00);
    private static readonly CubicBezier kAccel80Decel20 = new(0.8, 0, 0.8, 1);
    private static readonly CubicBezier kAccel0_40Decel100 = new(0, 0.4, 0, 1);
    private static readonly CubicBezier kAccel40Decel100_3 = new(0.40, 0, 0, 0.97);
    private static readonly CubicBezier kAccel0_80Decel80 = new(0, 0.8, 0.2, 1);
    private static readonly CubicBezier kAccel0_100Decel80 = new(0, 1, 0.2, 1);
    private static readonly CubicBezier kAccel5_70Decel90 = new(0.05, 0.7, 0.1, 1);

    // Returns the value based on the tween type. |state| is from 0-1.
    public static double CalculateValue(TweenType type, double state)
    {
        Debug.Assert(state >= 0);
        Debug.Assert(state <= 1);

        switch (type)
        {
            case TweenType.EaseIn:
                return Math.Pow(state, 2);

            case TweenType.EaseIn2:
                return Math.Pow(state, 4);

            case TweenType.EaseInOut:
                if (state < 0.5)
                    return Math.Pow(state * 2, 2) / 2.0;
                return 1.0 - (Math.Pow((state - 1.0) * 2, 2) / 2.0);

            case TweenType.EaseInOutEmphasized:
                return kEaseInOutEmphasized.Solve(state);

            case TweenType.EaseInOut2:
                return kEaseInOut2.Solve(state);

            case TweenType.EaseOut3:
                return kEaseOut3.Solve(state);

            case TweenType.EaseOut4:
                return kEaseOut4.Solve(state);

            case TweenType.Linear:
                return state;

            case TweenType.EaseOut:
                return 1.0 - Math.Pow(1.0 - state, 2);

            case TweenType.EaseOut2:
                return kEaseOut2.Solve(state);

            case TweenType.SmoothInOut:
                return Math.Sin(state);

            case TweenType.FastOutSlowIn:
                return kFastOutSlowIn.Solve(state);

            case TweenType.FastOutSlowIn2:
                return kFastOutSlowIn2.Solve(state);

            case TweenType.FastOutSlowIn3:
                return kFastOutSlowIn3.Solve(state);

            case TweenType.LinearOutSlowIn:
                return kLinearOutSlowIn.Solve(state);

            case TweenType.SlowOutLinearIn:
                return kSlowOutLinearIn.Solve(state);

            case TweenType.FastOutLinearIn:
                return kFastOutLinearIn.Solve(state);

            case TweenType.Zero:
                return 0;

            case TweenType.AccelLinDecel60:
                return kAccelLinDecel60.Solve(state);

            case TweenType.AccelLinDecel100:
                return kAccelLinDecel100.Solve(state);

            case TweenType.AccelLinDecel100_3:
                return kAccelLinDecel100_3.Solve(state);

            case TweenType.Accel20Decel60:
                return kAccel20Decel60.Solve(state);

            case TweenType.Accel20Decel100:
                return kAccel20Decel100.Solve(state);

            case TweenType.Accel30Decel20_85:
                return kAccel30Decel20_85.Solve(state);

            case TweenType.Accel30DecelLin:
                return kAccel30DecelLin.Solve(state);

            case TweenType.Accel40Decel20:
                return kAccel40Decel20.Solve(state);

            case TweenType.Accel45Decel88:
                return kAccel45Decel88.Solve(state);

            case TweenType.Accel80Decel20:
                return kAccel80Decel20.Solve(state);

            case TweenType.Accel0_40Decel100:
                return kAccel0_40Decel100.Solve(state);

            case TweenType.Accel40Decel100_3:
                return kAccel40Decel100_3.Solve(state);

            case TweenType.Accel0_80Decel80:
                return kAccel0_80Decel80.Solve(state);

            case TweenType.Accel0_100Decel80:
                return kAccel0_100Decel80.Solve(state);

            case TweenType.Accel5_70Decel90:
                return kAccel5_70Decel90.Solve(state);
        }

        throw new ArgumentOutOfRangeException(nameof(type));
    }

    private static byte FloatToColorByte(float f)
        => (byte)Math.Clamp(ClampRound(f * 255.0f), 0, 255);

    private static byte BlendColorComponents(byte start,
                                             byte target,
                                             float startAlpha,
                                             float targetAlpha,
                                             float blendedAlpha,
                                             double progress)
    {
        // Since progress can be outside [0, 1], blending can produce a value
        // outside [0, 255].
        float blendedPremultiplied = FloatValueBetween(
            progress, start / 255f * startAlpha, target / 255f * targetAlpha);
        return FloatToColorByte(blendedPremultiplied / blendedAlpha);
    }

    private static float BlendColorComponentsFloat(float start,
                                                   float target,
                                                   float startAlpha,
                                                   float targetAlpha,
                                                   float blendedAlpha,
                                                   double progress)
    {
        // Since progress can be outside [0, 1], blending can produce a value
        // outside [0, 1].
        float blendedPremultiplied = FloatValueBetween(
            progress, start * startAlpha, target * targetAlpha);
        return blendedPremultiplied / blendedAlpha;
    }

    public static SKColorF ColorValueBetween(double value, SKColorF start, SKColorF target)
    {
        float startA = start.Alpha;
        float targetA = target.Alpha;
        float blendedA = FloatValueBetween(value, startA, targetA);
        if (blendedA <= 0f)
            return SKColorF.Empty;
        blendedA = Math.Min(blendedA, 1f);

        float blendedR = BlendColorComponentsFloat(start.Red, target.Red, startA,
                                                   targetA, blendedA, value);
        float blendedG = BlendColorComponentsFloat(start.Green, target.Green, startA,
                                                   targetA, blendedA, value);
        float blendedB = BlendColorComponentsFloat(start.Blue, target.Blue, startA,
                                                   targetA, blendedA, value);

        return new SKColorF(blendedR, blendedG, blendedB, blendedA);
    }

    public static SKColor ColorValueBetween(double value, SKColor start, SKColor target)
    {
        float startA = start.Alpha / 255f;
        float targetA = target.Alpha / 255f;
        float blendedA = FloatValueBetween(value, startA, targetA);
        if (blendedA <= 0f)
            return SKColors.Transparent;
        blendedA = Math.Min(blendedA, 1f);

        byte blendedR = BlendColorComponents(start.Red, target.Red, startA,
                                             targetA, blendedA, value);
        byte blendedG = BlendColorComponents(start.Green, target.Green, startA,
                                             targetA, blendedA, value);
        byte blendedB = BlendColorComponents(start.Blue, target.Blue, startA,
                                             targetA, blendedA, value);

        return new SKColor(blendedR, blendedG, blendedB, FloatToColorByte(blendedA));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double DoubleValueBetween(double value, double start, double target)
        => start + (target - start) * value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float FloatValueBetween(double value, float start, float target)
        => (float)(start + (target - start) * value);

    public static float ClampedFloatValueBetween(TimeSpan time,
                                                 TimeSpan startTime,
                                                 float start,
                                                 TimeSpan targetTime,
                                                 float target,
                                                 TweenType type = TweenType.Linear)
    {
        if (time <= startTime)
            return start;
        if (time >= targetTime)
            return target;

        double progress = (double)(time - startTime).Ticks / (targetTime - startTime).Ticks;
        return FloatValueBetween(CalculateValue(type, progress), start, target);
    }

    // Steps one representable double towards zero, matching nextafter(delta, 0).
    private static double NextAfterTowardZero(double value)
        => value > 0 ? Math.BitDecrement(value)
         : value < 0 ? Math.BitIncrement(value)
         : value;

    // Interpolated between start and target, with every integer in this range
    // given equal weight.
    public static int IntValueBetween(double value, int start, int target)
    {
        if (start == target)
            return start;
        double delta = target - start;
        if (delta < 0)
            delta--;
        else
            delta++;
        return start + (int)(value * NextAfterTowardZero(delta));
    }

    // Interpolates between start and target as real numbers, and rounds the
    // result to the nearest integer, with ties broken by rounding towards
    // positive infinity. This gives start and target half the weight of the
    // other integers in the range. This is the integer interpolation approach
    // specified by www.w3.org/TR/css3-transitions.
    public static int LinearIntValueBetween(double value, int start, int target)
    {
        // NOTE: Do not use ClampRound()! See comments on function declaration.
        return ClampFloor(0.5 + DoubleValueBetween(value, start, target));
    }

    // Interpolates between |start| and |target| rects, animating the rect
    // corners (as opposed to animating the rect origin and size) to minimize
    // rounding error accumulation at intermediate stages.
    public static Rect RectValueBetween(double value, in Rect start, in Rect target)
    {
        int x = LinearIntValueBetween(value, start.X, target.X);
        int y = LinearIntValueBetween(value, start.Y, target.Y);
        int right = LinearIntValueBetween(value, start.Right, target.Right);
        int bottom = LinearIntValueBetween(value, start.Bottom, target.Bottom);
        return new Rect(x, y, right - x, bottom - y);
    }

    public static RectF RectFValueBetween(double value, in RectF start, in RectF target)
    {
        float x = FloatValueBetween(value, start.X, target.X);
        float y = FloatValueBetween(value, start.Y, target.Y);
        float right = FloatValueBetween(value, start.Right, target.Right);
        float bottom = FloatValueBetween(value, start.Bottom, target.Bottom);
        return new RectF(x, y, right - x, bottom - y);
    }

    public static Transform TransformValueBetween(double value, in Transform start, in Transform target)
    {
        if (value >= 1.0)
            return target;
        if (value <= 0.0)
            return start;

        Transform toReturn = target;
        toReturn.Blend(in start, value);
        return toReturn;
    }

    public static TransformOperations TransformOperationsValueBetween(double value,
                                                                      TransformOperations start,
                                                                      TransformOperations target)
        => target.Blend(start, (float)value);

    public static Size SizeValueBetween(double value, in Size start, in Size target)
        => new(LinearIntValueBetween(value, start.Width, target.Width),
               LinearIntValueBetween(value, start.Height, target.Height));

    public static SizeF SizeFValueBetween(double value, in SizeF start, in SizeF target)
        => new(FloatValueBetween(value, start.Width, target.Width),
               FloatValueBetween(value, start.Height, target.Height));
}
