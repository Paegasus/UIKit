using System.Diagnostics;

using static UI.Numerics.ClampedMath;

namespace UI.Geometry;

// `InlineLayoutUnit` stores the same precision as `TextRunLayoutUnit` using an
// `int64_t` storage. It can provide the text precision, and represent the whole
// layout space (and more, 48 bits vs 26 bits), but it's double-sized.
public struct InlineLayoutUnit : IEquatable<InlineLayoutUnit>, IComparable<InlineLayoutUnit>
{
	public const int FractionalBits = 16;
    public const int IntegralBits = sizeof(long) * 8 - FractionalBits;
    public const int FixedPointDenominator = 1 << FractionalBits;
	public const long RawValueMax = long.MaxValue;
    public const long RawValueMin = long.MinValue;
    public const long IntegerMax = RawValueMax / FixedPointDenominator;
    public const long IntegerMin = RawValueMin / FixedPointDenominator;

    // kIndefiniteSize is a special value used within layout code.
    // It is typical within layout to have sizes which are only allowed to be non-negative or "indefinite".
    // We use the value of "-1" to represent these indefinite values.
    public static readonly InlineLayoutUnit IndefiniteSize = new(-1);

    public static readonly InlineLayoutUnit MaxValue = FromRawValue(RawValueMax);
    public static readonly InlineLayoutUnit MinValue = FromRawValue(RawValueMin);
    public static readonly InlineLayoutUnit NearlyMaxValue = FromRawValue(RawValueMax - FixedPointDenominator / 2);
    public static readonly InlineLayoutUnit NearlyMinValue = FromRawValue(RawValueMin + FixedPointDenominator / 2);
	
	private long m_Value;

	public InlineLayoutUnit() { m_Value = 0; }

    // Constructors from signed integral types <= sizeof(int)
    public InlineLayoutUnit(int value) => SaturatedSet(value);
    public InlineLayoutUnit(short value) => SaturatedSet(value);
    public InlineLayoutUnit(sbyte value) => SaturatedSet(value);

    // Constructors from unsigned integral types <= sizeof(int)
    public InlineLayoutUnit(uint value) => SaturatedSet(value);
    public InlineLayoutUnit(ushort value) => SaturatedSet(value);
    public InlineLayoutUnit(byte value) => SaturatedSet(value);

    // Constructors from integral types larger than int
    public InlineLayoutUnit(long value) { m_Value = ClampRawValue(value * FixedPointDenominator); }
    public InlineLayoutUnit(ulong value) { m_Value = ClampRawValue(value * FixedPointDenominator); }

    // Constructors from floating-point types
    public InlineLayoutUnit(float value) { m_Value = ClampRawValue(value * FixedPointDenominator); }
    public InlineLayoutUnit(double value) { m_Value = ClampRawValue(value * FixedPointDenominator); }

    public InlineLayoutUnit(LayoutUnit source)
    {
        // Convert 6-bit fractional LayoutUnit to 16-bit fractional InlineLayoutUnit
        m_Value = source.RawValue() << 10;
    }

    public readonly long RawValue() => m_Value;

	public void SetRawValue(long value) => m_Value = value;

    public void SaturatedSet(long value)
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

    public void SaturatedSet(ulong value)
    {
        // Unsigned values can't be negative, so we only need to check the upper bound.
        if (value >= (ulong)IntegerMax)
        {
            m_Value = RawValueMax;
        }
        else
        {
            m_Value = (int)(value << FractionalBits);
        }
    }

	public readonly long ToInteger()
	{
		return m_Value / FixedPointDenominator;
	}

	public readonly ulong ToUnsignedInteger()
	{
        // unchecked is necessary here to handle the conversion of negative values correctly
		unchecked
		{
			return (ulong)(m_Value / FixedPointDenominator);
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
    
    public static long ClampRawValue(long value)
    {
        if (value > long.MaxValue) return long.MaxValue;
        if (value < long.MinValue) return long.MinValue;
        return value;
    }

    public static long ClampRawValue(float value)
    {
        if (float.IsNaN(value)) return 0;
        if (value > long.MaxValue) return long.MaxValue;
        if (value < long.MinValue) return long.MinValue;

        return (long)value;
    }

    public static long ClampRawValue(double value)
    {
        if (double.IsNaN(value)) return 0;
        if (value > long.MaxValue) return long.MaxValue;
        if (value < long.MinValue) return long.MinValue;

        return (long)value;
    }

	public static InlineLayoutUnit FromRawValue(long value)
	{
		InlineLayoutUnit unit = new()
		{
			m_Value = value
		};
		
		return unit;
	}
	
	public static InlineLayoutUnit FromRawValueWithClamp(float raw_value)
	{
		// Note: Might be better to just call Conversion.SaturatedCast() directly and avoid the indirect call to ClampRawValue()
		// return FromRawValue(Conversion.SaturatedCast<int, T>(raw_value));
		return FromRawValue(ClampRawValue(raw_value));
	}

    public static InlineLayoutUnit FromRawValueWithClamp(double raw_value)
	{
		// Note: Might be better to just call Conversion.SaturatedCast() directly and avoid the indirect call to ClampRawValue()
		// return FromRawValue(Conversion.SaturatedCast<int, T>(raw_value));
		return FromRawValue(ClampRawValue(raw_value));
	}

    // Example usage: var (start, end) = InlineLayoutUnit.FromFloatEncompassRound(a, b);
    public static (InlineLayoutUnit Start, InlineLayoutUnit End) FromFloatEncompassRound(float startValue, float endValue)
    {
        InlineLayoutUnit startPosition;
        InlineLayoutUnit endPosition;

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
    public static InlineLayoutUnit FromFloatCeil(float value)
    {
        return FromRawValue(ClampRawValue(MathF.Ceiling(value * FixedPointDenominator)));
    }

    // The specified `value` is truncated to a multiple of `Epsilon()`, and is
    // clamped by `Min()` and `Max()`. A NaN `value` produces `FixedPoint(0)`.
    public static InlineLayoutUnit FromFloatFloor(float value)
    {
        return FromRawValue(ClampRawValue(MathF.Floor(value * FixedPointDenominator)));
    }

    public static InlineLayoutUnit Clamp(double value)
    {
        return FromFloatFloor((float)value);
    }

    // The specified `value` is rounded to a multiple of `Epsilon()`, and is
    // clamped by `Min()` and `Max()`. A NaN `value` produces `FixedPoint(0)`.
    public static InlineLayoutUnit FromFloatRound(float value)
    {
        return FromRawValue(ClampRawValue(MathF.Round(value * FixedPointDenominator)));
    }

    // The specified `value` is rounded to a multiple of `Epsilon()`, and is
    // clamped by `Min()` and `Max()`. A NaN `value` produces `FixedPoint(0)`.
    public static InlineLayoutUnit FromDoubleRound(double value)
    {
        return FromRawValue(ClampRawValue(Math.Round(value * FixedPointDenominator)));
    }

    public readonly bool HasFraction => (m_Value % FixedPointDenominator) != 0;

    public readonly bool IsInteger => (m_Value % FixedPointDenominator) == 0;

    public readonly InlineLayoutUnit Fraction()
    {
        // Compute fraction using the mod operator to preserve the sign of the value as it may affect rounding.
        return FromRawValue(m_Value % FixedPointDenominator);
    }

    public readonly bool MightBeSaturated()
    {
        return m_Value == RawValueMax || m_Value == RawValueMin;
    }

    public static float Epsilon() => 1.0f / FixedPointDenominator;

    public readonly InlineLayoutUnit AddEpsilon()
    {
        return FromRawValue(m_Value < RawValueMax ? m_Value + 1 : m_Value);
    }

    public readonly long Ceil()
    {
        if (m_Value >= RawValueMax - FixedPointDenominator + 1)
            return IntegerMax;

        if (m_Value >= 0)
            return (m_Value + FixedPointDenominator - 1) / FixedPointDenominator;

        return ToInteger();
    }

    public readonly long Round()
    {
        return ToInteger() + ((Fraction().RawValue() + (FixedPointDenominator / 2)) >> FractionalBits);
    }

    public readonly long Floor() 
    {
        if (m_Value <= RawValueMin + FixedPointDenominator - 1)
            return IntegerMin;
        
        return m_Value >> FractionalBits;
    }

    public readonly InlineLayoutUnit Abs()
    {
        // Make the operation explicitly unchecked (Even with "checked" compiler flag on)
        unchecked
        {
            // Not using Math.Abs() here because it throws a overflow exception if value < 0
            return FromRawValue(m_Value < 0 ? -m_Value : m_Value);
        }
    }

    public readonly InlineLayoutUnit ClampNegativeToZero()
    {
        return m_Value < 0 ? new InlineLayoutUnit() : this;
    }

    public readonly InlineLayoutUnit ClampPositiveToZero()
    {
        return m_Value > 0 ? new InlineLayoutUnit() : this;
    }

    public readonly InlineLayoutUnit ClampIndefiniteToZero()
    {
        if (m_Value == -FixedPointDenominator)
            return new InlineLayoutUnit();

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

    public override readonly bool Equals(object? obj) => obj is InlineLayoutUnit other && Equals(other);

    public readonly bool Equals(InlineLayoutUnit other) => m_Value == other.m_Value;

    public readonly int CompareTo(InlineLayoutUnit other) => m_Value.CompareTo(other.m_Value);

    public override readonly int GetHashCode() => m_Value.GetHashCode();

    public static implicit operator double(in InlineLayoutUnit a) => a.ToDouble();
    public static implicit operator float(in InlineLayoutUnit a) => a.ToFloat();
    public static implicit operator bool(in InlineLayoutUnit a) => a.m_Value != 0;

    public static bool operator ==(in InlineLayoutUnit left, in InlineLayoutUnit right) => left.Equals(right);
    public static bool operator !=(in InlineLayoutUnit left, in InlineLayoutUnit right) => !left.Equals(right);
    public static bool operator <(in InlineLayoutUnit left, in InlineLayoutUnit right) => left.m_Value < right.m_Value;
    public static bool operator <=(in InlineLayoutUnit left, in InlineLayoutUnit right) => left.m_Value <= right.m_Value;
    public static bool operator >(in InlineLayoutUnit left, in InlineLayoutUnit right) => left.m_Value > right.m_Value;
    public static bool operator >=(in InlineLayoutUnit left, in InlineLayoutUnit right) => left.m_Value >= right.m_Value;

    public static bool operator <=(in InlineLayoutUnit a, int b) => a <= new InlineLayoutUnit(b);
    public static bool operator <=(int a, in InlineLayoutUnit b) => new InlineLayoutUnit(a) <= b;
    public static bool operator >=(in InlineLayoutUnit a, int b) => a >= new InlineLayoutUnit(b);
    public static bool operator >=(int a, in InlineLayoutUnit b) => new InlineLayoutUnit(a) >= b;
    public static bool operator <(in InlineLayoutUnit a, int b) => a < new InlineLayoutUnit(b);
    public static bool operator <(int a, in InlineLayoutUnit b) => new InlineLayoutUnit(a) < b;
    public static bool operator >(in InlineLayoutUnit a, int b) => a > new InlineLayoutUnit(b);
    public static bool operator >(int a, in InlineLayoutUnit b) => new InlineLayoutUnit(a) > b;
    public static bool operator ==(in InlineLayoutUnit a, int b) => a == new InlineLayoutUnit(b);
    public static bool operator ==(int a, in InlineLayoutUnit b) => new InlineLayoutUnit(a) == b;
    public static bool operator !=(in InlineLayoutUnit a, int b) => a != new InlineLayoutUnit(b);
    public static bool operator !=(int a, in InlineLayoutUnit b) => new InlineLayoutUnit(a) != b;

    // --- Floating-Point Operators ---

    // Operators with InlineLayoutUnit as the left operand return a new InlineLayoutUnit.
    // This allows for compound assignment (e.g., myInlineLayoutUnit += 5.0f).
    public static InlineLayoutUnit operator +(in InlineLayoutUnit a, float b) => new(a.ToFloat() + b);
    public static InlineLayoutUnit operator +(in InlineLayoutUnit a, double b) => new(a.ToDouble() + b);
    public static InlineLayoutUnit operator -(in InlineLayoutUnit a, float b) => new(a.ToFloat() - b);
    public static InlineLayoutUnit operator -(in InlineLayoutUnit a, double b) => new(a.ToDouble() - b);
    public static InlineLayoutUnit operator *(in InlineLayoutUnit a, float b) => new(a.ToFloat() * b);
    public static InlineLayoutUnit operator *(in InlineLayoutUnit a, double b) => new(a.ToDouble() * b);
    public static InlineLayoutUnit operator /(in InlineLayoutUnit a, float b) => new(a.ToFloat() / b);
    public static InlineLayoutUnit operator /(in InlineLayoutUnit a, double b) => new(a.ToDouble() / b);

    // Operators with a float/double as the left operand return a float/double.
    // This preserves standard arithmetic behavior.
    public static float operator +(float a, InlineLayoutUnit b) => a + b.ToFloat();
    public static double operator +(double a, InlineLayoutUnit b) => a + b.ToDouble();
    public static float operator -(float a, InlineLayoutUnit b) => a - b.ToFloat();
    public static double operator -(double a, InlineLayoutUnit b) => a - b.ToDouble();
    public static float operator *(float a, InlineLayoutUnit b) => a * b.ToFloat();
    public static double operator *(double a, InlineLayoutUnit b) => a * b.ToDouble();
    public static float operator /(float a, InlineLayoutUnit b) => a / b.ToFloat();
    public static double operator /(double a, InlineLayoutUnit b) => a / b.ToDouble();

    public static InlineLayoutUnit operator -(in InlineLayoutUnit a)
    {
        // This is implemented to match the C++ behavior of two's complement
        // integer negation, where -int.MinValue is int.MinValue. The unchecked
        // block prevents an OverflowException.
        unchecked
        {
            return FromRawValue(-a.RawValue());
        }
    }

    public static InlineLayoutUnit operator ++(in InlineLayoutUnit a)
    {
        return a + new InlineLayoutUnit(1);
    }

    public static InlineLayoutUnit operator --(in InlineLayoutUnit a)
    {
        return a - new InlineLayoutUnit(1);
    }

    public static InlineLayoutUnit operator +(in InlineLayoutUnit a, in InlineLayoutUnit b)
    {
        return FromRawValue(ClampAdd(a.RawValue(), b.RawValue()));
    }

    public static InlineLayoutUnit operator -(in InlineLayoutUnit a, in InlineLayoutUnit b)
    {
        return FromRawValue(ClampSub(a.RawValue(), b.RawValue()));
    }

    public static InlineLayoutUnit operator *(in InlineLayoutUnit a, int b)
    {
        return FromRawValue(ClampMul(a.RawValue(), b));
    }

    public static InlineLayoutUnit operator /(in InlineLayoutUnit a, int b)
    {
        return FromRawValue(ClampDiv(a.RawValue(), b));
    }

    public static InlineLayoutUnit operator *(int a, in InlineLayoutUnit b) => b * a;
    

    public static InlineLayoutUnit operator /(int a, in InlineLayoutUnit b) => new InlineLayoutUnit(a) / b;

    public static InlineLayoutUnit operator +(in InlineLayoutUnit a, int b) => a + new InlineLayoutUnit(b);
    public static InlineLayoutUnit operator +(int a, in InlineLayoutUnit b) => new InlineLayoutUnit(a) + b;
    public static InlineLayoutUnit operator -(in InlineLayoutUnit a, int b) => a - new InlineLayoutUnit(b);
    public static InlineLayoutUnit operator -(int a, in InlineLayoutUnit b) => new InlineLayoutUnit(a) - b;

    public static InlineLayoutUnit operator *(in InlineLayoutUnit a, in InlineLayoutUnit b)
    {
        long result = a.RawValue() * b.RawValue() >> FractionalBits;
        return FromRawValue(ClampRawValue(result));
    }

    public static InlineLayoutUnit operator /(in InlineLayoutUnit a, in InlineLayoutUnit b)
    {
        if (b.RawValue() == 0)
            return FromRawValue(a.RawValue() >= 0 ? RawValueMax : RawValueMin);
        
        long result = (a.RawValue() << FractionalBits) / b.RawValue();
        return FromRawValue(ClampRawValue(result));
    }

    /// <summary>
    /// Returns the remainder after a division with integer results.
    /// This calculates the modulo so that:
    /// a = (int)(a / b) * b + IntMod(a, b).
    /// </summary>
    public static InlineLayoutUnit IntMod(in InlineLayoutUnit a, in InlineLayoutUnit b)
    {
        return FromRawValue(a.RawValue() % b.RawValue());
    }

    public static long SnapSizeToPixel(in InlineLayoutUnit size, in InlineLayoutUnit location)
    {
        InlineLayoutUnit fraction = location.Fraction();
        
        long result = (fraction + size).Round() - fraction.Round();

        // This check handles cases where a InlineLayoutUnit is small but non-zero.
        // The C++ version uses [[unlikely]] which is a hint for branch prediction.
        // We just implement the logic directly. The threshold of 4 is 4/64 pixels.
        if (result == 0 && (size.RawValue() > 4 || size.RawValue() < -4))
        {
            return size > 0 ? 1 : -1;
        }

        return result;
    }

    public static long SnapSizeToPixelAllowingZero(in InlineLayoutUnit size, in InlineLayoutUnit location)
    {
        InlineLayoutUnit fraction = location.Fraction();
        return (fraction + size).Round() - fraction.Round();
    }

    /// <summary>
    /// Returns (a * b) / c.
    /// </summary>
    public static InlineLayoutUnit MulDiv(in InlineLayoutUnit a, in InlineLayoutUnit b, in InlineLayoutUnit c)
    {
        if (c.RawValue() == 0) return FromRawValue(a.RawValue() >= 0 ? RawValueMax : RawValueMin);

        long result = a.RawValue() * b.RawValue() / c.RawValue();
        return FromRawValue(ClampRawValue(result));
    }

    /// <summary>
    /// Returns (a * b) / c.
    /// </summary>
    public static InlineLayoutUnit MulDiv(in InlineLayoutUnit a, int b, int c)
    {
        if (c == 0) return FromRawValue(a.RawValue() >= 0 ? RawValueMax : RawValueMin);

        long result = a.RawValue() * b / c;
        return FromRawValue(ClampRawValue(result));
    }

    /// <summary>
    /// Rounds |a| down to the nearest multiple of |b|.
    /// The rounding is towards zero.
    /// </summary>
    public static InlineLayoutUnit RoundDownToMultiple(in InlineLayoutUnit a, in InlineLayoutUnit b)
    {
        if (b.RawValue() <= 0) return new InlineLayoutUnit(); // Return 0

        if (a.RawValue() >= 0) return a - IntMod(a, b);

        // For negative a, the logic is a bit different to round towards zero.
        InlineLayoutUnit r = a + IntMod(-a, b);
        
        // If the result crossed over to be positive, it should be clamped to zero.
        return r.RawValue() > 0 ? new InlineLayoutUnit() : r;
    }

    public static InlineLayoutUnit Min(in InlineLayoutUnit a, in InlineLayoutUnit b) => a < b ? a : b;
    public static InlineLayoutUnit Max(in InlineLayoutUnit a, in InlineLayoutUnit b) => a > b ? a : b;
}
