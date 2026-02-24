using System.Diagnostics;
using System.Text;

namespace UI.GFX.Geometry;

// A class that defines a linear gradient mask.
// Up to 6 steps are supported.
//
// ex. Horizontal linear gradient that starts in the middle.
// LinearGradient gradient(0);
// gradient.AddStep(20, 0);
// gradient.AddStep(30, 255);
// gradient.AddStep(70, 255);
// gradient.AddStep(80, 0);
public class LinearGradient
{
    public struct Step
    {
        // Fraction that defines a position in the gradient, from 0 to 1.
        public float Fraction;
        // Alpha, from 0 to 255.
        public byte Alpha;

        public readonly bool Equals(in Step other) => Fraction == other.Fraction && Alpha == other.Alpha;
        public override readonly bool Equals(object? obj) => obj is Step other && Equals(other);

        public static bool operator ==(Step left, Step right) => left.Equals(right);
        public static bool operator !=(Step left, Step right) => !left.Equals(right);

        public override readonly int GetHashCode() => HashCode.Combine(Fraction, Alpha);
    }

    public const int MaxStepSize = 8;

    private short _angle;
    private int _stepCount;
    private readonly Step[] _steps = new Step[MaxStepSize];

    private static readonly LinearGradient _empty = new();
    public static LinearGradient GetEmpty() => _empty;

    public LinearGradient() { }

    public LinearGradient(short angle)
    {
        _angle = angle;
    }

    public LinearGradient(LinearGradient copy)
    {
        _angle = copy._angle;
        _stepCount = copy._stepCount;
        copy._steps.CopyTo(_steps, 0);
    }

    public bool IsEmpty => _stepCount == 0;

    // Add a new step. Fraction must be in [0, 1] and monotonically increasing.
    public void AddStep(float fraction, byte alpha)
    {
#if DEBUG
        Debug.Assert(_stepCount < MaxStepSize);
        Debug.Assert(fraction >= 0);
        Debug.Assert(fraction <= 1);

        // make sure the step's fraction is monotonically increasing.
        Debug.Assert(_stepCount != 0 ? _steps[_stepCount - 1].Fraction < fraction : true);
        //Debug.WriteLine($"prev[{_stepCount - 1}]={_steps[_stepCount - 1].Fraction}, next[{_stepCount}]={fraction}"
#endif
        _steps[_stepCount].Fraction = fraction;
        _steps[_stepCount].Alpha = alpha;
        _stepCount++;
    }

    // Gets a read-only view of the active steps.
    public ReadOnlySpan<Step> Steps => _steps.AsSpan(0, _stepCount);

    public int StepCount => _stepCount;

    // Gets/Sets the angle in degrees.
    public short Angle { get => _angle; set => _angle = value; }

    // Reverses the order of steps and flips their fractions around 1.
    public void ReverseSteps()
    {
        // Reverse the active steps in-place within the array.
        _steps.AsSpan(0, _stepCount).Reverse();
        for (int i = 0; i < _stepCount; i++)
            _steps[i].Fraction = 1f - _steps[i].Fraction;
    }

    // Transforms the gradient angle by a full Transform.
    public void ApplyTransform(Transform transform)
    {
        if (transform.IsIdentityOrTranslation)
            return;

        float radian = float.DegreesToRadians(_angle);
        float y = -MathF.Sin(radian);
        float x =  MathF.Cos(radian);
        PointF origin = transform.MapPoint(new PointF());
        PointF end    = transform.MapPoint(new PointF(x, y));
        Vector2DF diff = end - origin;
        float newAngle = float.RadiansToDegrees(MathF.Atan2(diff.Y, diff.X));
        //_angle = (short)-(short)MathF.Round(newAngle, MidpointRounding.AwayFromZero);
        _angle = (short)-MathF.Round(newAngle, MidpointRounding.AwayFromZero);
    }

    // Transforms the gradient angle by an AxisTransform2D (scale only — translation
    // and uniform scale have no effect on the angle).
    public void ApplyTransform(AxisTransform2D transform)
    {
        if (transform.Scale.X == transform.Scale.Y)
            return;

        float radian = float.DegreesToRadians(_angle);
        float y = -MathF.Sin(radian) * transform.Scale.Y;
        float x =  MathF.Cos(radian) * transform.Scale.X;
        float newAngle = float.RadiansToDegrees(MathF.Atan2(y, x));
        //_angle = (short)-(short)MathF.Round(newAngle, MidpointRounding.AwayFromZero);
        _angle = (short)-MathF.Round(newAngle, MidpointRounding.AwayFromZero);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"LinearGradient{{angle={_angle}, step_count={_stepCount} [");
        for (int i = 0; i < _stepCount; i++)
        {
            if (i > 0) sb.Append(" - ");
            sb.Append($"{_steps[i].Fraction:F6}:{_steps[i].Alpha}");
        }
        sb.Append("]}");
        return sb.ToString();
    }

    public bool Equals(LinearGradient other)
    {
        if (_angle != other._angle || _stepCount != other._stepCount)
            return false;
        for (int i = 0; i < _stepCount; i++)
            if (_steps[i] != other._steps[i]) return false;
        return true;
    }

    public override bool Equals(object? obj) => obj is LinearGradient other && Equals(other);

    public static bool operator ==(LinearGradient? left, LinearGradient? right)
    {
        if (left is null) return right is null;
        return left.Equals(right!);
    }

    public static bool operator !=(LinearGradient? left, LinearGradient? right) => !(left == right);

    public override int GetHashCode()
    {
        var hc = new HashCode();
        hc.Add(_angle);
        hc.Add(_stepCount);
        for (int i = 0; i < _stepCount; i++)
            hc.Add(_steps[i]);
        return hc.ToHashCode();
    }
}
