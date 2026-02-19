using System.Diagnostics;
using UI.Geometry;

using static UI.Geometry.LayoutUnit;

namespace UI.Tests;

public static class LayoutUnitTestsChromium
{
    private static bool AreNear(float val1, float val2, float abs_error)
    {
        float diff = MathF.Abs(val1 - val2);
        
        return diff <= abs_error;
    }

    public static void RunAllTests()
    {
        LayoutUnitInt();
        LayoutUnitUnsigned();
        LayoutUniInt64();
        LayoutUnitFloat();
        LayoutUnitFromFloatCeil();
        LayoutUnitFromFloatFloor();
        LayoutUnitFromFloatRound();
        LayoutUnitRounding();
        LayoutUnitFromFloatEncompassRound();
        LayoutUnitSnapSizeToPixel();
        LayoutUnitMultiplication();
        LayoutUniMultiplicationByInt();
        LayoutUnitDivision();
        LayoutUnitDivisionByInt();
        LayoutUnitMulDiv();
        LayoutUnitCeil();
        LayoutUnitFloor();
        LayoutUnitFloatOverflow();
        LayoutUnitUnaryMinus();
        LayoutUnitPlusPlus();
        LayoutUnitIntMod();
        LayoutUnitFraction();
        LayoutUnitFixedConsts();
        LayoutUnitFixed();
        LayoutUnitRaw64FromInt32();
        LayoutUnitRaw64FromRaw32();
        LayoutUnitTo();
        LayoutUnitToClampSameFractional64To32();
        LayoutUnitToClampLessFractional64To32();
        LayoutUnitToClampMoreFractional();
        LayoutUnitRaw64Ceil();

        Debug.WriteLine("All LayoutUnit tests passed!");
    }

    public static void LayoutUnitInt()
    {
        Debug.Assert(IntegerMin == new LayoutUnit(int.MinValue).ToInteger());
        Debug.Assert(IntegerMin == new LayoutUnit(int.MinValue / 2).ToInteger());
        Debug.Assert(IntegerMin == new LayoutUnit(IntegerMin - 1).ToInteger());
        Debug.Assert(IntegerMin == new LayoutUnit(IntegerMin).ToInteger());
        Debug.Assert(IntegerMin + 1 == new LayoutUnit(IntegerMin + 1).ToInteger());
        Debug.Assert(IntegerMin / 2 == new LayoutUnit(IntegerMin / 2).ToInteger());
        Debug.Assert(-10000 ==  new LayoutUnit(-10000).ToInteger());
        Debug.Assert(-1000 ==  new LayoutUnit(-1000).ToInteger());
        Debug.Assert(-100 ==  new LayoutUnit(-100).ToInteger());
        Debug.Assert(-10 ==  new LayoutUnit(-10).ToInteger());
        Debug.Assert(-1 ==  new LayoutUnit(-1).ToInteger());
        Debug.Assert(0 ==  new LayoutUnit(0).ToInteger());
        Debug.Assert(1 ==  new LayoutUnit(1).ToInteger());
        Debug.Assert(100 ==  new LayoutUnit(100).ToInteger());
        Debug.Assert(1000 ==  new LayoutUnit(1000).ToInteger());
        Debug.Assert(10000 ==  new LayoutUnit(10000).ToInteger());
        Debug.Assert(IntegerMax / 2 == new LayoutUnit(IntegerMax / 2).ToInteger());
        Debug.Assert(IntegerMax - 1 == new LayoutUnit(IntegerMax - 1).ToInteger());
        Debug.Assert(IntegerMax ==  new LayoutUnit(IntegerMax).ToInteger());
        Debug.Assert(IntegerMax ==  new LayoutUnit(IntegerMax + 1).ToInteger());
        Debug.Assert(IntegerMax ==  new LayoutUnit(int.MaxValue / 2).ToInteger());
        Debug.Assert(IntegerMax ==  new LayoutUnit(int.MaxValue).ToInteger());

        // Test the raw unsaturated value
        Debug.Assert(0 == new LayoutUnit(0).RawValue());
        // Internally the max number we can represent (without saturating)
        // is all the (non-sign) bits set except for the bottom n fraction bits
        const int max_internal_representation = int.MaxValue ^ ((1 << FractionalBits) - 1);
        Debug.Assert(max_internal_representation == new LayoutUnit(IntegerMax).RawValue());
        Debug.Assert(RawValueMax == new LayoutUnit(IntegerMax + 100).RawValue());
        Debug.Assert((IntegerMax - 100) << FractionalBits == new LayoutUnit(IntegerMax - 100).RawValue());
        Debug.Assert(RawValueMin == new LayoutUnit(IntegerMin).RawValue());
        Debug.Assert(RawValueMin == new LayoutUnit(IntegerMin - 100).RawValue());
        // Shifting negative numbers left has undefined behavior, so use
        // multiplication instead of direct shifting here.
        Debug.Assert((IntegerMin + 100) * (1 << FractionalBits) == new LayoutUnit(IntegerMin + 100).RawValue());
    }

    public static void LayoutUnitUnsigned()
    {
        // Test the raw unsaturated value
        Debug.Assert(0 == new LayoutUnit((uint)0).RawValue());
        Debug.Assert(RawValueMax == new LayoutUnit((uint)IntegerMax).RawValue());
        const uint kOverflowed = IntegerMax + 100;
        Debug.Assert(RawValueMax == new LayoutUnit(kOverflowed).RawValue());
        const uint kNotOverflowed = IntegerMax - 100;
        Debug.Assert((IntegerMax - 100) << FractionalBits == new LayoutUnit(kNotOverflowed).RawValue());
    }

    public static void LayoutUniInt64()
    {
        const int raw_min = int.MinValue;
        const int raw_max = int.MaxValue;

        Debug.Assert(new LayoutUnit((long)raw_min - 100) == MinValue);
        Debug.Assert(new LayoutUnit((long)raw_max + 100) == MaxValue);
        Debug.Assert(new LayoutUnit((long)raw_max + 100) == MaxValue);
    }

    public static void LayoutUnitFloat()
    {
        const float Tolerance = 1.0f / FixedPointDenominator;

        Debug.Assert(1.0f == new LayoutUnit(1.0f).ToFloat());
        Debug.Assert(1.25f == new LayoutUnit(1.25f).ToFloat());
        Debug.Assert(new LayoutUnit(1.25f) == new LayoutUnit(1.25f + Tolerance / 2));
        Debug.Assert(new LayoutUnit(-2.0f) == new LayoutUnit(-2.0f - Tolerance / 2));
        Debug.Assert(AreNear(new LayoutUnit(1.1f).ToFloat(), 1.1f, Tolerance));
        Debug.Assert(AreNear(new LayoutUnit(1.33f).ToFloat(), 1.33f, Tolerance));
        Debug.Assert(AreNear(new LayoutUnit(1.3333f).ToFloat(), 1.3333f, Tolerance));
        Debug.Assert(AreNear(new LayoutUnit(1.53434f).ToFloat(), 1.53434f, Tolerance));
        Debug.Assert(AreNear(new LayoutUnit(345634).ToFloat(), 345634.0f, Tolerance));
        Debug.Assert(AreNear(new LayoutUnit(345634.12335f).ToFloat(), 345634.12335f, Tolerance));
        Debug.Assert(AreNear(new LayoutUnit(-345634.12335f).ToFloat(), -345634.12335f, Tolerance));
        Debug.Assert(AreNear(new LayoutUnit(-345634).ToFloat(), -345634.0f, Tolerance));

        // Larger than Max()
        Debug.Assert(MaxValue == new LayoutUnit(float.MaxValue));
        Debug.Assert(MaxValue == new LayoutUnit(float.PositiveInfinity));
        // Smaller than Min()
        Debug.Assert(MinValue == new LayoutUnit(float.MinValue));
        Debug.Assert(MinValue == new LayoutUnit(float.NegativeInfinity));

        Debug.Assert(new LayoutUnit() == Clamp(float.NaN));
    }

    public static void LayoutUnitFromFloatCeil()
    {
        const float Tolerance = 1.0f / FixedPointDenominator;

        Debug.Assert(new LayoutUnit(1.25f) == FromFloatCeil(1.25f));
        Debug.Assert(new LayoutUnit(1.25f + Tolerance) == FromFloatCeil(1.25f + Tolerance / 2));
        Debug.Assert(new LayoutUnit() == FromFloatCeil(-Tolerance / 2));

        // Larger than Max()
        Debug.Assert(MaxValue == FromFloatCeil(float.MaxValue));
        Debug.Assert(MaxValue == FromFloatCeil(float.PositiveInfinity));
        // Smaller than Min()
        Debug.Assert(MinValue == FromFloatCeil(float.MinValue));
        Debug.Assert(MinValue == FromFloatCeil(float.NegativeInfinity));

        Debug.Assert(new LayoutUnit() == FromFloatCeil(float.NaN));
    }

    public static void LayoutUnitFromFloatFloor()
    {
        const float Tolerance = 1.0f / FixedPointDenominator;

        Debug.Assert(new LayoutUnit(1.25f) == FromFloatFloor(1.25f));
        Debug.Assert(new LayoutUnit(1.25f) == FromFloatFloor(1.25f + Tolerance / 2));
        Debug.Assert(new LayoutUnit(-Tolerance) == FromFloatFloor(-Tolerance / 2));

        // Larger than Max()
        Debug.Assert(MaxValue == FromFloatFloor(float.MaxValue));
        Debug.Assert(MaxValue == FromFloatFloor(float.PositiveInfinity));
        // Smaller than Min()
        Debug.Assert(MinValue == FromFloatFloor(float.MinValue));
        Debug.Assert(MinValue == FromFloatFloor(float.NegativeInfinity));

        Debug.Assert(new LayoutUnit() == FromFloatFloor(float.NaN));
    }

    public static void LayoutUnitFromFloatRound()
    {
        const float Tolerance = 1.0f / FixedPointDenominator;

        Debug.Assert(new LayoutUnit(1.25f) == FromFloatRound(1.25f));
        Debug.Assert(new LayoutUnit(1.25f) == FromFloatRound(1.25f + Tolerance / 4));
        Debug.Assert(new LayoutUnit(1.25f + Tolerance) == FromFloatRound(1.25f + Tolerance * 3 / 4));
        Debug.Assert(new LayoutUnit(-Tolerance) == FromFloatRound(-Tolerance * 3 / 4));

        // Larger than Max()
        Debug.Assert(MaxValue == FromFloatRound(float.MaxValue));
        Debug.Assert(MaxValue == FromFloatRound(float.PositiveInfinity));
        // Smaller than Min()
        Debug.Assert(MinValue == FromFloatRound(float.MinValue));
        Debug.Assert(MinValue == FromFloatRound(float.NegativeInfinity));

        Debug.Assert(new LayoutUnit() == FromFloatRound(float.NaN));
    }

    private static void LayoutUnitRounding()
    {
        Debug.Assert(-2 == new LayoutUnit(-1.9f).Round());
        Debug.Assert(-2 == new LayoutUnit(-1.6f).Round());
        Debug.Assert(-2 == FromFloatRound(-1.51f).Round());
        Debug.Assert(-1 == FromFloatRound(-1.5f).Round());
        Debug.Assert(-1 == FromFloatRound(-1.49f).Round());
        Debug.Assert(-1 == new LayoutUnit(-1.0f).Round());
        Debug.Assert(-1 == FromFloatRound(-0.99f).Round());
        Debug.Assert(-1 == FromFloatRound(-0.51f).Round());
        Debug.Assert(0 == FromFloatRound(-0.50f).Round());
        Debug.Assert(0 == FromFloatRound(-0.49f).Round());
        Debug.Assert(0 == new LayoutUnit(-0.1f).Round());
        Debug.Assert(0 == new LayoutUnit(0.0f).Round());
        Debug.Assert(0 == new LayoutUnit(0.1f).Round());
        Debug.Assert(0 == FromFloatRound(0.49f).Round());
        Debug.Assert(1 == FromFloatRound(0.50f).Round());
        Debug.Assert(1 == FromFloatRound(0.51f).Round());
        Debug.Assert(1 == new LayoutUnit(0.99f).Round());
        Debug.Assert(1 == new LayoutUnit(1.0f).Round());
        Debug.Assert(1 == FromFloatRound(1.49f).Round());
        Debug.Assert(2 == FromFloatRound(1.5f).Round());
        Debug.Assert(2 == FromFloatRound(1.51f).Round());
        // The fractional part of LayoutUnit::Max() is 0x3f, so it should round up.
        Debug.Assert(((int.MaxValue / FixedPointDenominator) + 1) == MaxValue.Round());
        // The fractional part of LayoutUnit::Min() is 0, so the next bigger possible
        // value should round down.
        LayoutUnit epsilon = new();
        epsilon.SetRawValue(1);
        Debug.Assert((int.MinValue / FixedPointDenominator) == (MinValue + epsilon).Round());
    }

    private static void LayoutUnitFromFloatEncompassRound()
    {
        var (first, second) = FromFloatEncompassRound(55.152481f, 55.152481f);

        Debug.Assert(new LayoutUnit(55.140625f) == first);
        Debug.Assert(new LayoutUnit(55.140625f) == second);

        (first, second) = FromFloatEncompassRound(55.152481f, 56.25f);
        Debug.Assert(new LayoutUnit(55.140625f) == first);
        Debug.Assert(new LayoutUnit(56.25f) == second);

        (first, second) = FromFloatEncompassRound(54.25f, 55.152481f);
        Debug.Assert(new LayoutUnit(54.25f) == first);
        Debug.Assert(new LayoutUnit(55.156250f) == second);
    }

    private static void LayoutUnitSnapSizeToPixel()
    {
        Debug.Assert(1 == SnapSizeToPixel(new(1), new(0)));
        Debug.Assert(1 == SnapSizeToPixel(new(1), new(0.5)));
        Debug.Assert(2 == SnapSizeToPixel(new(1.5), new(0)));
        Debug.Assert(2 == SnapSizeToPixel(new(1.5), new(0.49)));
        Debug.Assert(1 == SnapSizeToPixel(new(1.5), new(0.5)));
        Debug.Assert(1 == SnapSizeToPixel(new(1.5), new(0.75)));
        Debug.Assert(1 == SnapSizeToPixel(new(1.5), new(0.99)));
        Debug.Assert(2 == SnapSizeToPixel(new(1.5), new(1)));

        // 0.046875 is 3/64, lower than 4 * LayoutUnit::Epsilon()
        Debug.Assert(0 == SnapSizeToPixel(new(0.046875), new(0)));
        // 0.078125 is 5/64, higher than 4 * LayoutUnit::Epsilon()
        Debug.Assert(1 == SnapSizeToPixel(new(0.078125), new(0)));

        // Negative versions
        Debug.Assert(0 == SnapSizeToPixel(new(-0.046875), new(0)));
        Debug.Assert(-1 == SnapSizeToPixel(new(-0.078125), new(0)));

        // The next 2 would snap to zero but for the requirement that we not snap
        // sizes greater than 4 * LayoutUnit::Epsilon() to 0.
        Debug.Assert(1 == SnapSizeToPixel(new(0.5), new(1.5)));
        Debug.Assert(1 == SnapSizeToPixel(new(0.99), new(1.5)));

        Debug.Assert(1 == SnapSizeToPixel(new(1.0), new(1.5)));
        Debug.Assert(1 == SnapSizeToPixel(new(1.49), new(1.5)));
        Debug.Assert(1 == SnapSizeToPixel(new(1.5), new(1.5)));

        Debug.Assert(101 == SnapSizeToPixel(new(100.5), new(100)));
        Debug.Assert(IntegerMax == SnapSizeToPixel(new(IntegerMax), new(0.3)));
        Debug.Assert(IntegerMin == SnapSizeToPixel(new(IntegerMin), new(-0.3)));
    }

    private static void LayoutUnitMultiplication()
    {
        Debug.Assert(1 == (new LayoutUnit(1) * new LayoutUnit(1)).ToInteger());
        Debug.Assert(2 == (new LayoutUnit(1) * new LayoutUnit(2)).ToInteger());
        Debug.Assert(2 == (new LayoutUnit(2) * new LayoutUnit(1)).ToInteger());
        Debug.Assert(1 == (new LayoutUnit(2) * new LayoutUnit(0.5)).ToInteger());
        Debug.Assert(1 == (new LayoutUnit(0.5) * new LayoutUnit(2)).ToInteger());
        Debug.Assert(100 == (new LayoutUnit(100) * new LayoutUnit(1)).ToInteger());

        Debug.Assert(-1 == (new LayoutUnit(-1) * new LayoutUnit(1)).ToInteger());
        Debug.Assert(-2 == (new LayoutUnit(-1) * new LayoutUnit(2)).ToInteger());
        Debug.Assert(-2 == (new LayoutUnit(-2) * new LayoutUnit(1)).ToInteger());
        Debug.Assert(-1 == (new LayoutUnit(-2) * new LayoutUnit(0.5)).ToInteger());
        Debug.Assert(-1 == (new LayoutUnit(-0.5) * new LayoutUnit(2)).ToInteger());
        Debug.Assert(-100 == (new LayoutUnit(-100) * new LayoutUnit(1)).ToInteger());

        Debug.Assert(1 == (new LayoutUnit(-1) * new LayoutUnit(-1)).ToInteger());
        Debug.Assert(2 == (new LayoutUnit(-1) * new LayoutUnit(-2)).ToInteger());
        Debug.Assert(2 == (new LayoutUnit(-2) * new LayoutUnit(-1)).ToInteger());
        Debug.Assert(1 == (new LayoutUnit(-2) * new LayoutUnit(-0.5)).ToInteger());
        Debug.Assert(1 == (new LayoutUnit(-0.5) * new LayoutUnit(-2)).ToInteger());
        Debug.Assert(100 == (new LayoutUnit(-100) * new LayoutUnit(-1)).ToInteger());

        Debug.Assert(333 == (new LayoutUnit(100) * new LayoutUnit(3.33)).Round());
        Debug.Assert(-333 == (new LayoutUnit(-100) * new LayoutUnit(3.33)).Round());
        Debug.Assert(333 == (new LayoutUnit(-100) * new LayoutUnit(-3.33)).Round());

        int a_hundred_size_t = 100;
        Debug.Assert(100 == (new LayoutUnit(a_hundred_size_t) * new LayoutUnit(1)).ToInteger());
        Debug.Assert(400 == (a_hundred_size_t * new LayoutUnit(4)).ToInteger());
        Debug.Assert(400 == (new LayoutUnit(4) * a_hundred_size_t).ToInteger());

        int quarter_max = IntegerMax / 4;
        Debug.Assert(quarter_max * 2 == (new LayoutUnit(quarter_max) * new LayoutUnit(2)).ToInteger());
        Debug.Assert(quarter_max * 3 == (new LayoutUnit(quarter_max) * new LayoutUnit(3)).ToInteger());
        Debug.Assert(quarter_max * 4 == (new LayoutUnit(quarter_max) * new LayoutUnit(4)).ToInteger());
        Debug.Assert(IntegerMax == (new LayoutUnit(quarter_max) * new LayoutUnit(5)).ToInteger());

        int overflow_int_size_t = IntegerMax * 4;
        Debug.Assert(IntegerMax == (new LayoutUnit(overflow_int_size_t) * new LayoutUnit(2)).ToInteger());
        Debug.Assert(IntegerMax == (overflow_int_size_t * new LayoutUnit(4)).ToInteger());
        Debug.Assert(IntegerMax == (new LayoutUnit(4) * overflow_int_size_t).ToInteger());

        {
            // Multiple by float 1.0 can produce a different value.
            LayoutUnit source = FromRawValue(2147483009);
            Debug.Assert(source != new LayoutUnit(source * 1.0f));
            LayoutUnit updated = source;
            updated *= 1.0f;
            Debug.Assert(source != updated);
        }
    }

    private static void LayoutUniMultiplicationByInt()
    {
        var quarter_max = IntegerMax / 4;
        Debug.Assert(new LayoutUnit(quarter_max * 2) == new LayoutUnit(quarter_max) * 2);
        Debug.Assert(new LayoutUnit(quarter_max * 3) ==  new LayoutUnit(quarter_max) * 3);
        Debug.Assert(new LayoutUnit(quarter_max * 4) ==  new LayoutUnit(quarter_max) * 4);
        Debug.Assert(MaxValue == new LayoutUnit(quarter_max) * 5);
    }

    private static void LayoutUnitDivision()
    {
        Debug.Assert(1 == (new LayoutUnit(1) / new LayoutUnit(1)).ToInteger());
        Debug.Assert(0 == (new LayoutUnit(1) / new LayoutUnit(2)).ToInteger());
        Debug.Assert(2 == (new LayoutUnit(2) / new LayoutUnit(1)).ToInteger());
        Debug.Assert(4 == (new LayoutUnit(2) / new LayoutUnit(0.5)).ToInteger());
        Debug.Assert(0 == (new LayoutUnit(0.5) / new LayoutUnit(2)).ToInteger());
        Debug.Assert(10 == (new LayoutUnit(100) / new LayoutUnit(10)).ToInteger());
        Debug.Assert(0.5f == (new LayoutUnit(1) / new LayoutUnit(2)).ToFloat());
        Debug.Assert(0.25f == (new LayoutUnit(0.5) / new LayoutUnit(2)).ToFloat());

        Debug.Assert(-1 == (new LayoutUnit(-1) / new LayoutUnit(1)).ToInteger());
        Debug.Assert(0 == (new LayoutUnit(-1) / new LayoutUnit(2)).ToInteger());
        Debug.Assert(-2 == (new LayoutUnit(-2) / new LayoutUnit(1)).ToInteger());
        Debug.Assert(-4 == (new LayoutUnit(-2) / new LayoutUnit(0.5)).ToInteger());
        Debug.Assert(0 == (new LayoutUnit(-0.5) / new LayoutUnit(2)).ToInteger());
        Debug.Assert(-10 == (new LayoutUnit(-100) / new LayoutUnit(10)).ToInteger());
        Debug.Assert(-0.5f == (new LayoutUnit(-1) / new LayoutUnit(2)).ToFloat());
        Debug.Assert(-0.25f == (new LayoutUnit(-0.5) / new LayoutUnit(2)).ToFloat());

        Debug.Assert(1 == (new LayoutUnit(-1) / new LayoutUnit(-1)).ToInteger());
        Debug.Assert(0 == (new LayoutUnit(-1) / new LayoutUnit(-2)).ToInteger());
        Debug.Assert(2 == (new LayoutUnit(-2) / new LayoutUnit(-1)).ToInteger());
        Debug.Assert(4 == (new LayoutUnit(-2) / new LayoutUnit(-0.5)).ToInteger());
        Debug.Assert(0 == (new LayoutUnit(-0.5) / new LayoutUnit(-2)).ToInteger());
        Debug.Assert(10 == (new LayoutUnit(-100) / new LayoutUnit(-10)).ToInteger());
        Debug.Assert(0.5f == (new LayoutUnit(-1) / new LayoutUnit(-2)).ToFloat());
        Debug.Assert(0.25f == (new LayoutUnit(-0.5) / new LayoutUnit(-2)).ToFloat());

        int a_hundred_size_t = 100;
        Debug.Assert(50 == (new LayoutUnit(a_hundred_size_t) / new LayoutUnit(2)).ToInteger());
        Debug.Assert(25 == (a_hundred_size_t / new LayoutUnit(4)).ToInteger());
        Debug.Assert(4 == (new LayoutUnit(400) / a_hundred_size_t).ToInteger());

        Debug.Assert(IntegerMax / 2 == (new LayoutUnit(IntegerMax) / new LayoutUnit(2)).ToInteger());
        Debug.Assert(IntegerMax == (new LayoutUnit(IntegerMax) / new LayoutUnit(0.5)).ToInteger());
    }

    private static void LayoutUnitDivisionByInt()
    {
        Debug.Assert(new LayoutUnit(1) == new LayoutUnit(1) / 1);
        Debug.Assert(new LayoutUnit(0.5) == new LayoutUnit(1) / 2);
        Debug.Assert(new LayoutUnit(-0.5) == new LayoutUnit(1) / -2);
        Debug.Assert(new LayoutUnit(-0.5) == new LayoutUnit(-1) / 2);
        Debug.Assert(new LayoutUnit(0.5) == new LayoutUnit(-1) / -2);

        Debug.Assert(IntegerMax / 2.0 == (new LayoutUnit(IntegerMax) / 2).ToDouble());

        Debug.Assert(InlineLayoutUnit.IntegerMax / 2.0 == (new InlineLayoutUnit(InlineLayoutUnit.IntegerMax) / 2).ToDouble());
    }

    private static void LayoutUnitMulDiv()
    {
        LayoutUnit kMaxValue = MaxValue;
        LayoutUnit kMinValue = MinValue;
        LayoutUnit kEpsilon = new LayoutUnit().AddEpsilon();

        Debug.Assert(kMaxValue == kMaxValue.MulDiv(kMaxValue, kMaxValue));
        Debug.Assert(kMinValue == kMinValue.MulDiv(kMinValue, kMinValue));
        Debug.Assert(kMinValue == kMaxValue.MulDiv(kMinValue, kMaxValue));
        Debug.Assert(kMaxValue == kMinValue.MulDiv(kMinValue, kMaxValue));
        Debug.Assert(kMinValue + kEpsilon * 2 == kMaxValue.MulDiv(kMaxValue, kMinValue));

        Debug.Assert(kMaxValue == kMaxValue.MulDiv(new LayoutUnit(2), kEpsilon));
        Debug.Assert(kMinValue == kMinValue.MulDiv(new LayoutUnit(2), kEpsilon));

        LayoutUnit kLargerInt = new(16384);
        LayoutUnit kLargerInt2 = new(32768);

        Debug.Assert(new LayoutUnit(8192)== kLargerInt.MulDiv(kLargerInt, kLargerInt2));
    }

    private static void LayoutUnitCeil()
    {
        Debug.Assert(0 == new LayoutUnit(0).Ceil());
        Debug.Assert(1 == new LayoutUnit(0.1).Ceil());
        Debug.Assert(1 == new LayoutUnit(0.5).Ceil());
        Debug.Assert(1 == new LayoutUnit(0.9).Ceil());
        Debug.Assert(1 == new LayoutUnit(1.0).Ceil());
        Debug.Assert(2 == new LayoutUnit(1.1).Ceil());

        Debug.Assert(0 == new LayoutUnit(-0.1).Ceil());
        Debug.Assert(0 == new LayoutUnit(-0.5).Ceil());
        Debug.Assert(0 == new LayoutUnit(-0.9).Ceil());
        Debug.Assert(-1 == new LayoutUnit(-1.0).Ceil());

        Debug.Assert(IntegerMax == new LayoutUnit(IntegerMax).Ceil());
        Debug.Assert(IntegerMax == (new LayoutUnit(IntegerMax) - new LayoutUnit(0.5)).Ceil());
        Debug.Assert(IntegerMax - 1 == (new LayoutUnit(IntegerMax) - new LayoutUnit(1)).Ceil());

        Debug.Assert(IntegerMin == new LayoutUnit(IntegerMin).Ceil());
    }

    private static void LayoutUnitFloor()
    {
        Debug.Assert(0 == new LayoutUnit(0).Floor());
        Debug.Assert(0 == new LayoutUnit(0.1).Floor());
        Debug.Assert(0 == new LayoutUnit(0.5).Floor());
        Debug.Assert(0 == new LayoutUnit(0.9).Floor());
        Debug.Assert(1 == new LayoutUnit(1.0).Floor());
        Debug.Assert(1 == new LayoutUnit(1.1).Floor());

        Debug.Assert(-1 == new LayoutUnit(-0.1).Floor());
        Debug.Assert(-1 == new LayoutUnit(-0.5).Floor());
        Debug.Assert(-1 == new LayoutUnit(-0.9).Floor());
        Debug.Assert(-1 == new LayoutUnit(-1.0).Floor());

        Debug.Assert(IntegerMax == new LayoutUnit(IntegerMax).Floor());

        Debug.Assert(IntegerMin == new LayoutUnit(IntegerMin).Floor());
        Debug.Assert(IntegerMin == (new LayoutUnit(IntegerMin) + new LayoutUnit(0.5)).Floor());
        Debug.Assert(IntegerMin + 1 == (new LayoutUnit(IntegerMin) + new LayoutUnit(1)).Floor());
    }

    private static void LayoutUnitFloatOverflow()
    {
        // These should overflow to the max/min according to their sign.
        Debug.Assert(IntegerMax == new LayoutUnit(176972000.0f).ToInteger());
        Debug.Assert(IntegerMin == new LayoutUnit(-176972000.0f).ToInteger());
        Debug.Assert(IntegerMax == new LayoutUnit(176972000.0).ToInteger());
        Debug.Assert(IntegerMin == new LayoutUnit(-176972000.0).ToInteger());
    }

    private static void LayoutUnitUnaryMinus()
    {
        Debug.Assert(new LayoutUnit() == -new LayoutUnit());
        Debug.Assert(new LayoutUnit(999) == -new LayoutUnit(-999));
        Debug.Assert(new LayoutUnit(-999) == -new LayoutUnit(999));

        LayoutUnit negative_max = new();
        negative_max.SetRawValue(MinValue.RawValue() + 1);
        Debug.Assert(negative_max == -MaxValue);
        Debug.Assert(MaxValue == -negative_max);

        // -LayoutUnit::min() is saturated to LayoutUnit::max()
        Debug.Assert(LayoutUnit.MaxValue == -LayoutUnit.MinValue); // 2147483647 == -2147483648
    }

    private static void LayoutUnitPlusPlus()
    {
        var val1 = new LayoutUnit(-2);
        var val2 = new LayoutUnit(-1);
        var val3 = new LayoutUnit(0);
        var val4 = new LayoutUnit(1);
        var val5 = new LayoutUnit(MaxValue);
        

        Debug.Assert(new LayoutUnit(-1) == ++val1);
        Debug.Assert(new LayoutUnit(0) == ++val2);
        Debug.Assert(new LayoutUnit(1) == ++val3);
        Debug.Assert(new LayoutUnit(2) == ++val4);
        
        Debug.Assert(MaxValue == ++val5);
    }

    private static void LayoutUnitIntMod()
    {
        Debug.Assert(new LayoutUnit(5) == IntMod(new LayoutUnit(55), new LayoutUnit(10)));
        Debug.Assert(new LayoutUnit(5) == IntMod(new LayoutUnit(55), new LayoutUnit(-10)));
        Debug.Assert(new LayoutUnit(-5) == IntMod(new LayoutUnit(-55), new LayoutUnit(10)));
        Debug.Assert(new LayoutUnit(-5) == IntMod(new LayoutUnit(-55), new LayoutUnit(-10)));
        Debug.Assert(new LayoutUnit(1.5) == IntMod(new LayoutUnit(7.5), new LayoutUnit(3)));
        Debug.Assert(new LayoutUnit(1.25) == IntMod(new LayoutUnit(7.5), new LayoutUnit(3.125)));
        Debug.Assert(new LayoutUnit() == IntMod(new LayoutUnit(7.5), new LayoutUnit(2.5)));
        Debug.Assert(new LayoutUnit() == IntMod(new LayoutUnit(), new LayoutUnit(123)));
    }

    private static void LayoutUnitFraction()
    {
        Debug.Assert(new LayoutUnit(-1.9f).HasFraction);
        Debug.Assert(new LayoutUnit(-1.6f).HasFraction);
        Debug.Assert(FromFloatRound(-1.51f).HasFraction);
        Debug.Assert(FromFloatRound(-1.5f).HasFraction);
        Debug.Assert(FromFloatRound(-1.49f).HasFraction);
        Debug.Assert(!new LayoutUnit(-1.0f).HasFraction);
        Debug.Assert(FromFloatRound(-0.95f).HasFraction);
        Debug.Assert(FromFloatRound(-0.51f).HasFraction);
        Debug.Assert(FromFloatRound(-0.50f).HasFraction);
        Debug.Assert(FromFloatRound(-0.49f).HasFraction);
        Debug.Assert(new LayoutUnit(-0.1f).HasFraction);
        Debug.Assert(!new LayoutUnit(-1.0f).HasFraction);
        Debug.Assert(!new LayoutUnit(0.0f).HasFraction);
        Debug.Assert(new LayoutUnit(0.1f).HasFraction);
        Debug.Assert(FromFloatRound(0.49f).HasFraction);
        Debug.Assert(FromFloatRound(0.50f).HasFraction);
        Debug.Assert(FromFloatRound(0.51f).HasFraction);
        Debug.Assert(new LayoutUnit(0.95f).HasFraction);
        Debug.Assert(!new LayoutUnit(1.0f).HasFraction);
    }

    private static void LayoutUnitFixedConsts()
    {
        Debug.Assert(LayoutUnit.FractionalBits == 6u);
        Debug.Assert(LayoutUnit.IntegralBits == 26u);
        Debug.Assert(TextRunLayoutUnit.FractionalBits == 16u);
        Debug.Assert(TextRunLayoutUnit.IntegralBits == 16u);
        Debug.Assert(InlineLayoutUnit.FractionalBits == 16u);
        Debug.Assert(InlineLayoutUnit.IntegralBits == 48u);
    }

    private static void LayoutUnitFixed()
    {
        int raw_value16 = 0x12345678;
        int raw_value6 = raw_value16 >> 10;
        var value16 = TextRunLayoutUnit.FromRawValue(raw_value16);
        var value6 = LayoutUnit.FromRawValue(raw_value6);
        //Debug.Assert(value16.To<LayoutUnit>(), value6);
    }

    private static void LayoutUnitRaw64FromInt32()
    {
        
    }

    private static void LayoutUnitRaw64FromRaw32()
    {
        
    }

    private static void LayoutUnitTo()
    {
        
    }

    private static void LayoutUnitToClampSameFractional64To32()
    {
        
    }

    private static void LayoutUnitToClampLessFractional64To32()
    {
        
    }

    private static void LayoutUnitToClampMoreFractional()
    {
        
    }

    private static void LayoutUnitRaw64Ceil()
    {
        
    }
}
