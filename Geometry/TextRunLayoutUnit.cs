using System.Diagnostics;

using static UI.Numerics.ClampedMath;

namespace UI.Geometry;

public struct TextRunLayoutUnit : IEquatable<TextRunLayoutUnit>, IComparable<TextRunLayoutUnit>
{
	public const int FractionalBits = 16;
    public const int IntegralBits = sizeof(int) * 8 - FractionalBits;
    public const int FixedPointDenominator = 1 << FractionalBits;
	public const int RawValueMax = int.MaxValue;
    public const int RawValueMin = int.MinValue;
    public const int IntegerMax = RawValueMax / FixedPointDenominator;
    public const int IntegerMin = RawValueMin / FixedPointDenominator;

    // kIndefiniteSize is a special value used within layout code.
    // It is typical within layout to have sizes which are only allowed to be non-negative or "indefinite".
    // We use the value of "-1" to represent these indefinite values.
    public static readonly TextRunLayoutUnit IndefiniteSize = new(-1);

    public static readonly TextRunLayoutUnit MaxValue = FromRawValue(RawValueMax);
    public static readonly TextRunLayoutUnit MinValue = FromRawValue(RawValueMin);
    public static readonly TextRunLayoutUnit NearlyMaxValue = FromRawValue(RawValueMax - FixedPointDenominator / 2);
    public static readonly TextRunLayoutUnit NearlyMinValue = FromRawValue(RawValueMin + FixedPointDenominator / 2);
	
	private int m_Value;

	public TextRunLayoutUnit() { m_Value = 0; }

    // Constructors from signed integral types <= sizeof(int)
    public TextRunLayoutUnit(int value) => SaturatedSet(value);
    public TextRunLayoutUnit(short value) => SaturatedSet(value);
    public TextRunLayoutUnit(sbyte value) => SaturatedSet(value);

    // Constructors from unsigned integral types <= sizeof(int)
    public TextRunLayoutUnit(uint value) => SaturatedSet(value);
    public TextRunLayoutUnit(ushort value) => SaturatedSet(value);
    public TextRunLayoutUnit(byte value) => SaturatedSet(value);

    // Constructors from integral types larger than int
    public TextRunLayoutUnit(long value) { m_Value = ClampRawValue(value * FixedPointDenominator); }
    public TextRunLayoutUnit(ulong value) { m_Value = ClampRawValue(value * FixedPointDenominator); }

    // Constructors from floating-point types
    public TextRunLayoutUnit(float value) { m_Value = ClampRawValue(value * FixedPointDenominator); }
    public TextRunLayoutUnit(double value) { m_Value = ClampRawValue(value * FixedPointDenominator); }

    public readonly int RawValue() => m_Value;

	public void SetRawValue(int value) => m_Value = value;

	public void SetRawValue(long value)
	{
#if DEBUG
        Debug.WriteLineIf(value > int.MaxValue || value < int.MinValue, $"TextRunLayoutUnit overflow: {value} is out of range for a 32-bit integer.");
#endif
		m_Value = (int)value;
	}

    public void SaturatedSet(int value)
    {
        if (value > IntegerMax)
        {
            m_Value = RawValueMax;
        }
        else if (value < IntegerMin)
        {
            m_Value = RawValueMin;
        }
        else
        {
            m_Value = value << FractionalBits;
        }
    }

    public void SaturatedSet(uint value)
    {
        // Unsigned values can't be negative, so we only need to check the upper bound.
        if (value >= (uint)IntegerMax)
        {
            m_Value = RawValueMax;
        }
        else
        {
            m_Value = (int)(value << FractionalBits);
        }
    }

	public readonly int ToInteger()
	{
		return m_Value / FixedPointDenominator;
	}

	public readonly uint ToUnsignedInteger()
	{
        // unchecked is necessary here to handle the conversion of negative values correctly
		unchecked
		{
			return (uint)(m_Value / FixedPointDenominator);
		}
	}

	public readonly float ToFloat()
	{
		return (float)m_Value / FixedPointDenominator;
	}

	public readonly double ToDouble()
	{
		return (double)m_Value / FixedPointDenominator;
	}

    public static int ClampRawValue(int value)
    {
        if (value > int.MaxValue) return int.MaxValue;
        if (value < int.MinValue) return int.MinValue;
        return value;
    }

    public static int ClampRawValue(long value)
    {
        if (value > int.MaxValue) return int.MaxValue;
        if (value < int.MinValue) return int.MinValue;
        return (int)value;
    }

    public static int ClampRawValue(float value)
    {
        if (float.IsNaN(value)) return 0;
        if (value > int.MaxValue) return int.MaxValue;
        if (value < int.MinValue) return int.MinValue;

        return (int)value;
    }

    public static int ClampRawValue(double value)
    {
        if (double.IsNaN(value)) return 0;
        if (value > int.MaxValue) return int.MaxValue;
        if (value < int.MinValue) return int.MinValue;

        return (int)value;
    }

	public static TextRunLayoutUnit FromRawValue(int value)
	{
		TextRunLayoutUnit unit = new()
		{
			m_Value = value
		};
		
		return unit;
	}

    public static TextRunLayoutUnit FromRawValueWithClamp(int raw_value)
	{
		// Note: Might be better to just call Conversion.SaturatedCast() directly and avoid the indirect call to ClampRawValue()
		// return FromRawValue(Conversion.SaturatedCast<int, T>(raw_value));
		return FromRawValue(ClampRawValue(raw_value));
	}

    public static TextRunLayoutUnit FromRawValueWithClamp(long raw_value)
	{
		// Note: Might be better to just call Conversion.SaturatedCast() directly and avoid the indirect call to ClampRawValue()
		// return FromRawValue(Conversion.SaturatedCast<int, T>(raw_value));
		return FromRawValue(ClampRawValue(raw_value));
	}
	
	public static TextRunLayoutUnit FromRawValueWithClamp(float raw_value)
	{
		// Note: Might be better to just call Conversion.SaturatedCast() directly and avoid the indirect call to ClampRawValue()
		// return FromRawValue(Conversion.SaturatedCast<int, T>(raw_value));
		return FromRawValue(ClampRawValue(raw_value));
	}

    public static TextRunLayoutUnit FromRawValueWithClamp(double raw_value)
	{
		// Note: Might be better to just call Conversion.SaturatedCast() directly and avoid the indirect call to ClampRawValue()
		// return FromRawValue(Conversion.SaturatedCast<int, T>(raw_value));
		return FromRawValue(ClampRawValue(raw_value));
	}

    // Example usage: var (start, end) = TextRunLayoutUnit.FromFloatEncompassRound(a, b);
    public static (TextRunLayoutUnit Start, TextRunLayoutUnit End) FromFloatEncompassRound(float startValue, float endValue)
    {
        TextRunLayoutUnit startPosition;
        TextRunLayoutUnit endPosition;

        if (startValue < endValue)
        {
            startPosition = FromFloatFloor(startValue);
            endPosition = FromFloatCeil(endValue);
        }
        else if (startValue > endValue)
        {
            startPosition = FromFloatCeil(startValue);
            endPosition = FromFloatFloor(endValue);
        }
        else
        {
            startPosition = FromFloatFloor(startValue);
            endPosition = startPosition;
        }

        return (startPosition, endPosition);
    }

    // The specified `value` is rounded up to a multiple of `Epsilon()`, and is
    // clamped by `Min()` and `Max()`. A NaN `value` produces `FixedPoint(0)`.
    public static TextRunLayoutUnit FromFloatCeil(float value)
    {
        return FromRawValue(ClampRawValue(MathF.Ceiling(value * FixedPointDenominator)));
    }

    // The specified `value` is truncated to a multiple of `Epsilon()`, and is
    // clamped by `Min()` and `Max()`. A NaN `value` produces `FixedPoint(0)`.
    public static TextRunLayoutUnit FromFloatFloor(float value)
    {
        return FromRawValue(ClampRawValue(MathF.Floor(value * FixedPointDenominator)));
    }

    public static TextRunLayoutUnit Clamp(double value)
    {
        return FromFloatFloor((float)value);
    }

    // The specified `value` is rounded to a multiple of `Epsilon()`, and is
    // clamped by `Min()` and `Max()`. A NaN `value` produces `FixedPoint(0)`.
    public static TextRunLayoutUnit FromFloatRound(float value)
    {
        return FromRawValue(ClampRawValue(MathF.Round(value * FixedPointDenominator)));
    }

    // The specified `value` is rounded to a multiple of `Epsilon()`, and is
    // clamped by `Min()` and `Max()`. A NaN `value` produces `FixedPoint(0)`.
    public static TextRunLayoutUnit FromDoubleRound(double value)
    {
        return FromRawValue(ClampRawValue(Math.Round(value * FixedPointDenominator)));
    }

    public readonly bool HasFraction => (m_Value % FixedPointDenominator) != 0;

    public readonly bool IsInteger => (m_Value % FixedPointDenominator) == 0;

    public readonly TextRunLayoutUnit Fraction()
    {
        // Compute fraction using the mod operator to preserve the sign of the value as it may affect rounding.
        return FromRawValue(m_Value % FixedPointDenominator);
    }

    public readonly bool MightBeSaturated()
    {
        return m_Value == RawValueMax || m_Value == RawValueMin;
    }

    public static float Epsilon() => 1.0f / FixedPointDenominator;

    public readonly TextRunLayoutUnit AddEpsilon()
    {
        return FromRawValue(m_Value < RawValueMax ? m_Value + 1 : m_Value);
    }

    public readonly int Ceil()
    {
        if (m_Value >= RawValueMax - FixedPointDenominator + 1)
            return IntegerMax;

        if (m_Value >= 0)
            return (m_Value + FixedPointDenominator - 1) / FixedPointDenominator;

        return ToInteger();
    }

    public readonly int Round()
    {
        return ToInteger() + ((Fraction().RawValue() + (FixedPointDenominator / 2)) >> FractionalBits);
    }

    public readonly int Floor() 
    {
        if (m_Value <= RawValueMin + FixedPointDenominator - 1)
            return IntegerMin;
        
        return m_Value >> FractionalBits;
    }

    public readonly TextRunLayoutUnit Abs()
    {
        // Make the operation explicitly unchecked (Even with "checked" compiler flag on)
        unchecked
        {
            // Not using Math.Abs() here because it throws a overflow exception if value < 0
            return FromRawValue(m_Value < 0 ? -m_Value : m_Value);
        }
    }

    public readonly TextRunLayoutUnit ClampNegativeToZero()
    {
        return m_Value < 0 ? new TextRunLayoutUnit() : this;
    }

    public readonly TextRunLayoutUnit ClampPositiveToZero()
    {
        return m_Value > 0 ? new TextRunLayoutUnit() : this;
    }

    public readonly TextRunLayoutUnit ClampIndefiniteToZero()
    {
        if (m_Value == -FixedPointDenominator)
            return new TextRunLayoutUnit();

#if DEBUG
        Debug.Assert(m_Value >= 0, "ClampIndefiniteToZero called on a negative value that is not the 'indefinite' sentinel.");
#endif

        return this;
    }

    public override readonly string ToString()
    {
        string formatted = ToDouble().ToString("G14");

        if (m_Value == MaxValue.RawValue()) return $"Max({formatted})";
        if (m_Value == MinValue.RawValue()) return $"Min({formatted})";
        if (m_Value == NearlyMaxValue.RawValue()) return $"NearlyMax({formatted})";
        if (m_Value == NearlyMinValue.RawValue()) return $"NearlyMin({formatted})";

        return formatted;
    }

    public override readonly bool Equals(object? obj) => obj is TextRunLayoutUnit other && Equals(other);

    public readonly bool Equals(TextRunLayoutUnit other) => m_Value == other.m_Value;

    public readonly int CompareTo(TextRunLayoutUnit other) => m_Value.CompareTo(other.m_Value);

    public override readonly int GetHashCode() => m_Value.GetHashCode();

    public static implicit operator double(in TextRunLayoutUnit a) => a.ToDouble();
    public static implicit operator float(in TextRunLayoutUnit a) => a.ToFloat();
    public static implicit operator bool(in TextRunLayoutUnit a) => a.m_Value != 0;

    public static bool operator ==(in TextRunLayoutUnit left, in TextRunLayoutUnit right) => left.Equals(right);
    public static bool operator !=(in TextRunLayoutUnit left, in TextRunLayoutUnit right) => !left.Equals(right);
    public static bool operator <(in TextRunLayoutUnit left, in TextRunLayoutUnit right) => left.m_Value < right.m_Value;
    public static bool operator <=(in TextRunLayoutUnit left, in TextRunLayoutUnit right) => left.m_Value <= right.m_Value;
    public static bool operator >(in TextRunLayoutUnit left, in TextRunLayoutUnit right) => left.m_Value > right.m_Value;
    public static bool operator >=(in TextRunLayoutUnit left, in TextRunLayoutUnit right) => left.m_Value >= right.m_Value;

    public static bool operator <=(in TextRunLayoutUnit a, int b) => a <= new TextRunLayoutUnit(b);
    public static bool operator <=(int a, in TextRunLayoutUnit b) => new TextRunLayoutUnit(a) <= b;
    public static bool operator >=(in TextRunLayoutUnit a, int b) => a >= new TextRunLayoutUnit(b);
    public static bool operator >=(int a, in TextRunLayoutUnit b) => new TextRunLayoutUnit(a) >= b;
    public static bool operator <(in TextRunLayoutUnit a, int b) => a < new TextRunLayoutUnit(b);
    public static bool operator <(int a, in TextRunLayoutUnit b) => new TextRunLayoutUnit(a) < b;
    public static bool operator >(in TextRunLayoutUnit a, int b) => a > new TextRunLayoutUnit(b);
    public static bool operator >(int a, in TextRunLayoutUnit b) => new TextRunLayoutUnit(a) > b;
    public static bool operator ==(in TextRunLayoutUnit a, int b) => a == new TextRunLayoutUnit(b);
    public static bool operator ==(int a, in TextRunLayoutUnit b) => new TextRunLayoutUnit(a) == b;
    public static bool operator !=(in TextRunLayoutUnit a, int b) => a != new TextRunLayoutUnit(b);
    public static bool operator !=(int a, in TextRunLayoutUnit b) => new TextRunLayoutUnit(a) != b;

    // --- Floating-Point Operators ---

    // Operators with TextRunLayoutUnit as the left operand return a new TextRunLayoutUnit.
    // This allows for compound assignment (e.g., myTextRunLayoutUnit += 5.0f).
    public static TextRunLayoutUnit operator +(in TextRunLayoutUnit a, float b) => new(a.ToFloat() + b);
    public static TextRunLayoutUnit operator +(in TextRunLayoutUnit a, double b) => new(a.ToDouble() + b);
    public static TextRunLayoutUnit operator -(in TextRunLayoutUnit a, float b) => new(a.ToFloat() - b);
    public static TextRunLayoutUnit operator -(in TextRunLayoutUnit a, double b) => new(a.ToDouble() - b);
    public static TextRunLayoutUnit operator *(in TextRunLayoutUnit a, float b) => new(a.ToFloat() * b);
    public static TextRunLayoutUnit operator *(in TextRunLayoutUnit a, double b) => new(a.ToDouble() * b);
    public static TextRunLayoutUnit operator /(in TextRunLayoutUnit a, float b) => new(a.ToFloat() / b);
    public static TextRunLayoutUnit operator /(in TextRunLayoutUnit a, double b) => new(a.ToDouble() / b);

    // Operators with a float/double as the left operand return a float/double.
    // This preserves standard arithmetic behavior.
    public static float operator +(float a, TextRunLayoutUnit b) => a + b.ToFloat();
    public static double operator +(double a, TextRunLayoutUnit b) => a + b.ToDouble();
    public static float operator -(float a, TextRunLayoutUnit b) => a - b.ToFloat();
    public static double operator -(double a, TextRunLayoutUnit b) => a - b.ToDouble();
    public static float operator *(float a, TextRunLayoutUnit b) => a * b.ToFloat();
    public static double operator *(double a, TextRunLayoutUnit b) => a * b.ToDouble();
    public static float operator /(float a, TextRunLayoutUnit b) => a / b.ToFloat();
    public static double operator /(double a, TextRunLayoutUnit b) => a / b.ToDouble();

    // Saturating negation: -MinValue clamps to MaxValue to preserve domain invariants.
    public static TextRunLayoutUnit operator -(in TextRunLayoutUnit a)
    {
        int raw = a.RawValue();

        if (raw == int.MinValue)
            return FromRawValue(int.MaxValue);

        return FromRawValue(-raw);
    }

    public static TextRunLayoutUnit operator ++(in TextRunLayoutUnit a)
    {
        return a + new TextRunLayoutUnit(1);
    }

    public static TextRunLayoutUnit operator --(in TextRunLayoutUnit a)
    {
        return a - new TextRunLayoutUnit(1);
    }

    public static TextRunLayoutUnit operator +(in TextRunLayoutUnit a, in TextRunLayoutUnit b)
    {
        return FromRawValue(ClampAdd(a.RawValue(), b.RawValue()));
    }

    public static TextRunLayoutUnit operator -(in TextRunLayoutUnit a, in TextRunLayoutUnit b)
    {
        return FromRawValue(ClampSub(a.RawValue(), b.RawValue()));
    }

    public static TextRunLayoutUnit operator *(in TextRunLayoutUnit a, int b)
    {
        return FromRawValue(ClampMul(a.RawValue(), b));
    }

    public static TextRunLayoutUnit operator /(in TextRunLayoutUnit a, int b)
    {
        return FromRawValue(ClampDiv(a.RawValue(), b));
    }

    public static TextRunLayoutUnit operator *(int a, in TextRunLayoutUnit b) => b * a;
    

    public static TextRunLayoutUnit operator /(int a, in TextRunLayoutUnit b) => new TextRunLayoutUnit(a) / b;

    public static TextRunLayoutUnit operator +(in TextRunLayoutUnit a, int b) => a + new TextRunLayoutUnit(b);
    public static TextRunLayoutUnit operator +(int a, in TextRunLayoutUnit b) => new TextRunLayoutUnit(a) + b;
    public static TextRunLayoutUnit operator -(in TextRunLayoutUnit a, int b) => a - new TextRunLayoutUnit(b);
    public static TextRunLayoutUnit operator -(int a, in TextRunLayoutUnit b) => new TextRunLayoutUnit(a) - b;

    public static TextRunLayoutUnit operator *(in TextRunLayoutUnit a, in TextRunLayoutUnit b)
    {
        long result = ((long)a.RawValue() * b.RawValue()) >> FractionalBits;
        return FromRawValue(ClampRawValue(result));
    }

    public static TextRunLayoutUnit operator /(in TextRunLayoutUnit a, in TextRunLayoutUnit b)
    {
        if (b.RawValue() == 0)
            return FromRawValue(a.RawValue() >= 0 ? RawValueMax : RawValueMin);
        
        long result = ((long)a.RawValue() << FractionalBits) / b.RawValue();
        return FromRawValue(ClampRawValue(result));
    }

    /// <summary>
    /// Returns the remainder after a division with integer results.
    /// This calculates the modulo so that:
    /// a = (int)(a / b) * b + IntMod(a, b).
    /// </summary>
    public static TextRunLayoutUnit IntMod(in TextRunLayoutUnit a, in TextRunLayoutUnit b)
    {
        return FromRawValue(a.RawValue() % b.RawValue());
    }

    public static int SnapSizeToPixel(in TextRunLayoutUnit size, in TextRunLayoutUnit location)
    {
        TextRunLayoutUnit fraction = location.Fraction();
        
        int result = (fraction + size).Round() - fraction.Round();

        // This check handles cases where a TextRunLayoutUnit is small but non-zero.
        // The C++ version uses [[unlikely]] which is a hint for branch prediction.
        // We just implement the logic directly. The threshold of 4 is 4/64 pixels.
        if (result == 0 && (size.RawValue() > 4 || size.RawValue() < -4))
        {
            return size > 0 ? 1 : -1;
        }

        return result;
    }

    public static int SnapSizeToPixelAllowingZero(in TextRunLayoutUnit size, in TextRunLayoutUnit location)
    {
        TextRunLayoutUnit fraction = location.Fraction();
        return (fraction + size).Round() - fraction.Round();
    }


    public readonly TextRunLayoutUnit MulDiv(in TextRunLayoutUnit m, in TextRunLayoutUnit d)
    {
        long n = (long)RawValue() * m.RawValue();
        long q = n / d.RawValue();
        return FromRawValueWithClamp(q);
    }

    /// <summary>
    /// Returns (a * b) / c.
    /// </summary>
    public static TextRunLayoutUnit MulDiv(in TextRunLayoutUnit a, in TextRunLayoutUnit b, in TextRunLayoutUnit c)
    {
        if (c.RawValue() == 0) return FromRawValue(a.RawValue() >= 0 ? RawValueMax : RawValueMin);

        long result = (long)a.RawValue() * b.RawValue() / c.RawValue();
        return FromRawValue(ClampRawValue(result));
    }

    /// <summary>
    /// Returns (a * b) / c.
    /// </summary>
    public static TextRunLayoutUnit MulDiv(in TextRunLayoutUnit a, int b, int c)
    {
        if (c == 0) return FromRawValue(a.RawValue() >= 0 ? RawValueMax : RawValueMin);

        long result = (long)a.RawValue() * b / c;
        return FromRawValue(ClampRawValue(result));
    }

    /// <summary>
    /// Rounds |a| down to the nearest multiple of |b|.
    /// The rounding is towards zero.
    /// </summary>
    public static TextRunLayoutUnit RoundDownToMultiple(in TextRunLayoutUnit a, in TextRunLayoutUnit b)
    {
        if (b.RawValue() <= 0) return new TextRunLayoutUnit(); // Return 0

        if (a.RawValue() >= 0) return a - IntMod(a, b);

        // For negative a, the logic is a bit different to round towards zero.
        TextRunLayoutUnit r = a + IntMod(-a, b);
        
        // If the result crossed over to be positive, it should be clamped to zero.
        return r.RawValue() > 0 ? new TextRunLayoutUnit() : r;
    }

    public static TextRunLayoutUnit Min(in TextRunLayoutUnit a, in TextRunLayoutUnit b) => a < b ? a : b;
    public static TextRunLayoutUnit Max(in TextRunLayoutUnit a, in TextRunLayoutUnit b) => a > b ? a : b;
}
