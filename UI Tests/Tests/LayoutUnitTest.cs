using Xunit;

using UI.Geometry;

using static UI.Geometry.LayoutUnit;

namespace UI.Tests;

public static class LayoutUnitTest
{
    [Fact]
    private static void TestInt()
    {
        Assert.Equal(IntegerMin, new LayoutUnit(int.MinValue).ToInteger());
        Assert.Equal(IntegerMin, new LayoutUnit(int.MinValue / 2).ToInteger());
        Assert.Equal(IntegerMin, new LayoutUnit(IntegerMin - 1).ToInteger());
        Assert.Equal(IntegerMin, new LayoutUnit(IntegerMin).ToInteger());
        Assert.Equal(IntegerMin + 1, new LayoutUnit(IntegerMin + 1).ToInteger());
        Assert.Equal(IntegerMin / 2, new LayoutUnit(IntegerMin / 2).ToInteger());
        Assert.Equal(-10000,  new LayoutUnit(-10000).ToInteger());
        Assert.Equal(-1000,  new LayoutUnit(-1000).ToInteger());
        Assert.Equal(-100,  new LayoutUnit(-100).ToInteger());
        Assert.Equal(-10,  new LayoutUnit(-10).ToInteger());
        Assert.Equal(-1,  new LayoutUnit(-1).ToInteger());
        Assert.Equal(0,  new LayoutUnit(0).ToInteger());
        Assert.Equal(1,  new LayoutUnit(1).ToInteger());
        Assert.Equal(100,  new LayoutUnit(100).ToInteger());
        Assert.Equal(1000,  new LayoutUnit(1000).ToInteger());
        Assert.Equal(10000,  new LayoutUnit(10000).ToInteger());
        Assert.Equal(IntegerMax / 2, new LayoutUnit(IntegerMax / 2).ToInteger());
        Assert.Equal(IntegerMax - 1, new LayoutUnit(IntegerMax - 1).ToInteger());
        Assert.Equal(IntegerMax,  new LayoutUnit(IntegerMax).ToInteger());
        Assert.Equal(IntegerMax,  new LayoutUnit(IntegerMax + 1).ToInteger());
        Assert.Equal(IntegerMax,  new LayoutUnit(int.MaxValue / 2).ToInteger());
        Assert.Equal(IntegerMax,  new LayoutUnit(int.MaxValue).ToInteger());

        // Test the raw unsaturated value
        Assert.Equal(0, new LayoutUnit(0).RawValue());
        // Internally the max number we can represent (without saturating)
        // is all the (non-sign) bits set except for the bottom n fraction bits
        const int max_internal_representation = int.MaxValue ^ ((1 << FractionalBits) - 1);
        Assert.Equal(max_internal_representation, new LayoutUnit(IntegerMax).RawValue());
        Assert.Equal(RawValueMax, new LayoutUnit(IntegerMax + 100).RawValue());
        Assert.Equal((IntegerMax - 100) << FractionalBits, new LayoutUnit(IntegerMax - 100).RawValue());
        Assert.Equal(RawValueMin, new LayoutUnit(IntegerMin).RawValue());
        Assert.Equal(RawValueMin, new LayoutUnit(IntegerMin - 100).RawValue());
        // Shifting negative numbers left has undefined behavior, so use
        // multiplication instead of direct shifting here.
        Assert.Equal((IntegerMin + 100) * (1 << FractionalBits), new LayoutUnit(IntegerMin + 100).RawValue());
    }

    [Fact]
    private static void TestUnsigned()
    {
        // Test the raw unsaturated value
        Assert.Equal(0, new LayoutUnit((uint)0).RawValue());
        Assert.Equal(RawValueMax, new LayoutUnit((uint)IntegerMax).RawValue());
        const uint kOverflowed = IntegerMax + 100;
        Assert.Equal(RawValueMax,new LayoutUnit(kOverflowed).RawValue());
        const uint kNotOverflowed = IntegerMax - 100;
        Assert.Equal((IntegerMax - 100) << FractionalBits, new LayoutUnit(kNotOverflowed).RawValue());
    }

    [Fact]
    private static void TestInt64()
    {
        const int raw_min = int.MinValue;
        const int raw_max = int.MaxValue;

        Assert.Equal(new LayoutUnit((long)raw_min - 100), MinValue);
        Assert.Equal(new LayoutUnit((long)raw_max + 100), MaxValue);
        Assert.Equal(new LayoutUnit((long)raw_max + 100), MaxValue);
    }

    [Fact]
    private static void TestFloat()
    {
        const float Tolerance = 1.0f / FixedPointDenominator;

        Assert.Equal(1.0f, new LayoutUnit(1.0f).ToFloat());
        Assert.Equal(1.25f, new LayoutUnit(1.25f).ToFloat());
        Assert.Equal(new LayoutUnit(1.25f), new LayoutUnit(1.25f + Tolerance / 2));
        Assert.Equal(new LayoutUnit(-2.0f), new LayoutUnit(-2.0f - Tolerance / 2));
        Assert.Equal(1.1f, new LayoutUnit(1.1f).ToFloat(), Tolerance);
        Assert.Equal(1.33f, new LayoutUnit(1.33f).ToFloat(), Tolerance);
        Assert.Equal(1.3333f, new LayoutUnit(1.3333f).ToFloat(), Tolerance);
        Assert.Equal(1.53434f, new LayoutUnit(1.53434f).ToFloat(), Tolerance);
        Assert.Equal(345634.0f, new LayoutUnit(345634).ToFloat(), Tolerance);
        Assert.Equal(345634.12335f, new LayoutUnit(345634.12335f).ToFloat(), Tolerance);
        Assert.Equal(-345634.12335f, new LayoutUnit(-345634.12335f).ToFloat(), Tolerance);
        Assert.Equal(-345634.0f, new LayoutUnit(-345634).ToFloat(), Tolerance);
        // Larger than Max()
        Assert.Equal(MaxValue, new LayoutUnit(float.MaxValue));
        Assert.Equal(MaxValue, new LayoutUnit(float.PositiveInfinity));
        // Smaller than Min()
        Assert.Equal(MinValue, new LayoutUnit(float.MinValue));
        Assert.Equal(MinValue, new LayoutUnit(float.NegativeInfinity));

        Assert.Equal(new LayoutUnit(), Clamp(float.NaN));
    }

    [Fact]
    private static void TestFromFloatCeil()
    {
        const float Tolerance = 1.0f / FixedPointDenominator;

        Assert.Equal(new LayoutUnit(1.25f), FromFloatCeil(1.25f));
        Assert.Equal(new LayoutUnit(1.25f + Tolerance), FromFloatCeil(1.25f + Tolerance / 2));
        Assert.Equal(new LayoutUnit(), FromFloatCeil(-Tolerance / 2));

        // Larger than Max()
        Assert.Equal(MaxValue, FromFloatCeil(float.MaxValue));
        Assert.Equal(MaxValue, FromFloatCeil(float.PositiveInfinity));
        // Smaller than Min()
        Assert.Equal(MinValue, FromFloatCeil(float.MinValue));
        Assert.Equal(MinValue, FromFloatCeil(float.NegativeInfinity));

        Assert.Equal(new LayoutUnit(), FromFloatCeil(float.NaN));
    }

    [Fact]
    private static void TestFromFloatFloor()
    {
        const float Tolerance = 1.0f / FixedPointDenominator;

        Assert.Equal(new LayoutUnit(1.25f), FromFloatFloor(1.25f));
        Assert.Equal(new LayoutUnit(1.25f), FromFloatFloor(1.25f + Tolerance / 2));
        Assert.Equal(new LayoutUnit(-Tolerance), FromFloatFloor(-Tolerance / 2));

        // Larger than Max()
        Assert.Equal(MaxValue, FromFloatFloor(float.MaxValue));
        Assert.Equal(MaxValue, FromFloatFloor(float.PositiveInfinity));
        // Smaller than Min()
        Assert.Equal(MinValue, FromFloatFloor(float.MinValue));
        Assert.Equal(MinValue, FromFloatFloor(float.NegativeInfinity));

        Assert.Equal(new LayoutUnit(), FromFloatFloor(float.NaN));
    }

    [Fact]
    private static void TestFromFloatRound()
    {
        const float Tolerance = 1.0f / FixedPointDenominator;

        Assert.Equal(new LayoutUnit(1.25f), FromFloatRound(1.25f));
        Assert.Equal(new LayoutUnit(1.25f), FromFloatRound(1.25f + Tolerance / 4));
        Assert.Equal(new LayoutUnit(1.25f + Tolerance), FromFloatRound(1.25f + Tolerance * 3 / 4));
        Assert.Equal(new LayoutUnit(-Tolerance), FromFloatRound(-Tolerance * 3 / 4));

        // Larger than Max()
        Assert.Equal(MaxValue, FromFloatRound(float.MaxValue));
        Assert.Equal(MaxValue, FromFloatRound(float.PositiveInfinity));
        // Smaller than Min()
        Assert.Equal(MinValue, FromFloatRound(float.MinValue));
        Assert.Equal(MinValue, FromFloatRound(float.NegativeInfinity));

        Assert.Equal(new LayoutUnit(), FromFloatRound(float.NaN));
    }

    [Fact]
    private static void TestRounding()
    {
        Assert.Equal(-2, new LayoutUnit(-1.9f).Round());
        Assert.Equal(-2, new LayoutUnit(-1.6f).Round());
        Assert.Equal(-2, FromFloatRound(-1.51f).Round());
        Assert.Equal(-1, FromFloatRound(-1.5f).Round());
        Assert.Equal(-1, FromFloatRound(-1.49f).Round());
        Assert.Equal(-1, new LayoutUnit(-1.0f).Round());
        Assert.Equal(-1, FromFloatRound(-0.99f).Round());
        Assert.Equal(-1, FromFloatRound(-0.51f).Round());
        Assert.Equal(0, FromFloatRound(-0.50f).Round());
        Assert.Equal(0, FromFloatRound(-0.49f).Round());
        Assert.Equal(0, new LayoutUnit(-0.1f).Round());
        Assert.Equal(0, new LayoutUnit(0.0f).Round());
        Assert.Equal(0, new LayoutUnit(0.1f).Round());
        Assert.Equal(0, FromFloatRound(0.49f).Round());
        Assert.Equal(1, FromFloatRound(0.50f).Round());
        Assert.Equal(1, FromFloatRound(0.51f).Round());
        Assert.Equal(1, new LayoutUnit(0.99f).Round());
        Assert.Equal(1, new LayoutUnit(1.0f).Round());
        Assert.Equal(1, FromFloatRound(1.49f).Round());
        Assert.Equal(2, FromFloatRound(1.5f).Round());
        Assert.Equal(2, FromFloatRound(1.51f).Round());
        // The fractional part of LayoutUnit::Max() is 0x3f, so it should round up.
        Assert.Equal(((int.MaxValue / FixedPointDenominator) + 1), MaxValue.Round());
        // The fractional part of LayoutUnit::Min() is 0, so the next bigger possible
        // value should round down.
        LayoutUnit epsilon = new();
        epsilon.SetRawValue(1);
        Assert.Equal((int.MinValue / FixedPointDenominator), (MinValue + epsilon).Round());
    }

    [Fact]
    private static void TestFromFloatEncompassRound()
    {
        var (first, second) = FromFloatEncompassRound(55.152481f, 55.152481f);

        Assert.Equal(new LayoutUnit(55.140625f), first);
        Assert.Equal(new LayoutUnit(55.140625f), second);

        (first, second) = FromFloatEncompassRound(55.152481f, 56.25f);
        Assert.Equal(new LayoutUnit(55.140625f), first);
        Assert.Equal(new LayoutUnit(56.25f), second);

        (first, second) = FromFloatEncompassRound(54.25f, 55.152481f);
        Assert.Equal(new LayoutUnit(54.25f), first);
        Assert.Equal(new LayoutUnit(55.156250f), second);
    }

    [Fact]
    private static void TestSnapSizeToPixel()
    {
        Assert.Equal(1, SnapSizeToPixel(new(1), new(0)));
        Assert.Equal(1, SnapSizeToPixel(new(1), new(0.5)));
        Assert.Equal(2, SnapSizeToPixel(new(1.5), new(0)));
        Assert.Equal(2, SnapSizeToPixel(new(1.5), new(0.49)));
        Assert.Equal(1, SnapSizeToPixel(new(1.5), new(0.5)));
        Assert.Equal(1, SnapSizeToPixel(new(1.5), new(0.75)));
        Assert.Equal(1, SnapSizeToPixel(new(1.5), new(0.99)));
        Assert.Equal(2, SnapSizeToPixel(new(1.5), new(1)));

        // 0.046875 is 3/64, lower than 4 * LayoutUnit::Epsilon()
        Assert.Equal(0, SnapSizeToPixel(new(0.046875), new(0)));
        // 0.078125 is 5/64, higher than 4 * LayoutUnit::Epsilon()
        Assert.Equal(1, SnapSizeToPixel(new(0.078125), new(0)));

        // Negative versions
        Assert.Equal(0, SnapSizeToPixel(new(-0.046875), new(0)));
        Assert.Equal(-1, SnapSizeToPixel(new(-0.078125), new(0)));

        // The next 2 would snap to zero but for the requirement that we not snap
        // sizes greater than 4 * LayoutUnit::Epsilon() to 0.
        Assert.Equal(1, SnapSizeToPixel(new(0.5), new(1.5)));
        Assert.Equal(1, SnapSizeToPixel(new(0.99), new(1.5)));

        Assert.Equal(1, SnapSizeToPixel(new(1.0), new(1.5)));
        Assert.Equal(1, SnapSizeToPixel(new(1.49), new(1.5)));
        Assert.Equal(1, SnapSizeToPixel(new(1.5), new(1.5)));

        Assert.Equal(101, SnapSizeToPixel(new(100.5), new(100)));
        Assert.Equal(IntegerMax, SnapSizeToPixel(new(IntegerMax), new(0.3)));
        Assert.Equal(IntegerMin, SnapSizeToPixel(new(IntegerMin), new(-0.3)));
    }

    [Fact]
    private static void TestMultiplication()
    {
        Assert.Equal(1, (new LayoutUnit(1) * new LayoutUnit(1)).ToInteger());
        Assert.Equal(2, (new LayoutUnit(1) * new LayoutUnit(2)).ToInteger());
        Assert.Equal(2, (new LayoutUnit(2) * new LayoutUnit(1)).ToInteger());
        Assert.Equal(1, (new LayoutUnit(2) * new LayoutUnit(0.5)).ToInteger());
        Assert.Equal(1, (new LayoutUnit(0.5) * new LayoutUnit(2)).ToInteger());
        Assert.Equal(100, (new LayoutUnit(100) * new LayoutUnit(1)).ToInteger());

        Assert.Equal(-1, (new LayoutUnit(-1) * new LayoutUnit(1)).ToInteger());
        Assert.Equal(-2, (new LayoutUnit(-1) * new LayoutUnit(2)).ToInteger());
        Assert.Equal(-2, (new LayoutUnit(-2) * new LayoutUnit(1)).ToInteger());
        Assert.Equal(-1, (new LayoutUnit(-2) * new LayoutUnit(0.5)).ToInteger());
        Assert.Equal(-1, (new LayoutUnit(-0.5) * new LayoutUnit(2)).ToInteger());
        Assert.Equal(-100, (new LayoutUnit(-100) * new LayoutUnit(1)).ToInteger());

        Assert.Equal(1, (new LayoutUnit(-1) * new LayoutUnit(-1)).ToInteger());
        Assert.Equal(2, (new LayoutUnit(-1) * new LayoutUnit(-2)).ToInteger());
        Assert.Equal(2, (new LayoutUnit(-2) * new LayoutUnit(-1)).ToInteger());
        Assert.Equal(1, (new LayoutUnit(-2) * new LayoutUnit(-0.5)).ToInteger());
        Assert.Equal(1, (new LayoutUnit(-0.5) * new LayoutUnit(-2)).ToInteger());
        Assert.Equal(100, (new LayoutUnit(-100) * new LayoutUnit(-1)).ToInteger());

        Assert.Equal(333, (new LayoutUnit(100) * new LayoutUnit(3.33)).Round());
        Assert.Equal(-333, (new LayoutUnit(-100) * new LayoutUnit(3.33)).Round());
        Assert.Equal(333, (new LayoutUnit(-100) * new LayoutUnit(-3.33)).Round());

        int a_hundred_size_t = 100;
        Assert.Equal(100, (new LayoutUnit(a_hundred_size_t) * new LayoutUnit(1)).ToInteger());
        Assert.Equal(400, (a_hundred_size_t * new LayoutUnit(4)).ToInteger());
        Assert.Equal(400, (new LayoutUnit(4) * a_hundred_size_t).ToInteger());

        int quarter_max = IntegerMax / 4;
        Assert.Equal(quarter_max * 2, (new LayoutUnit(quarter_max) * new LayoutUnit(2)).ToInteger());
        Assert.Equal(quarter_max * 3, (new LayoutUnit(quarter_max) * new LayoutUnit(3)).ToInteger());
        Assert.Equal(quarter_max * 4, (new LayoutUnit(quarter_max) * new LayoutUnit(4)).ToInteger());
        Assert.Equal(IntegerMax, (new LayoutUnit(quarter_max) * new LayoutUnit(5)).ToInteger());

        int overflow_int_size_t = IntegerMax * 4;
        Assert.Equal(IntegerMax, (new LayoutUnit(overflow_int_size_t) * new LayoutUnit(2)).ToInteger());
        Assert.Equal(IntegerMax, (overflow_int_size_t * new LayoutUnit(4)).ToInteger());
        Assert.Equal(IntegerMax, (new LayoutUnit(4) * overflow_int_size_t).ToInteger());

        {
            // Multiple by float 1.0 can produce a different value.
            LayoutUnit source = FromRawValue(2147483009);
            Assert.NotEqual(source, new LayoutUnit(source * 1.0f));
            LayoutUnit updated = source;
            updated *= 1.0f;
            Assert.NotEqual(source, updated);
        }
    }

    [Fact]
    private static void TestMultiplicationByInt()
    {
        var quarter_max = IntegerMax / 4;
        Assert.Equal(new LayoutUnit(quarter_max * 2), new LayoutUnit(quarter_max) * 2);
        Assert.Equal(new LayoutUnit(quarter_max * 3),  new LayoutUnit(quarter_max) * 3);
        Assert.Equal(new LayoutUnit(quarter_max * 4),  new LayoutUnit(quarter_max) * 4);
        Assert.Equal(MaxValue, new LayoutUnit(quarter_max) * 5);
    }

    [Fact]
    private static void TestDivision()
    {
        Assert.Equal(1, (new LayoutUnit(1) / new LayoutUnit(1)).ToInteger());
        Assert.Equal(0, (new LayoutUnit(1) / new LayoutUnit(2)).ToInteger());
        Assert.Equal(2, (new LayoutUnit(2) / new LayoutUnit(1)).ToInteger());
        Assert.Equal(4, (new LayoutUnit(2) / new LayoutUnit(0.5)).ToInteger());
        Assert.Equal(0, (new LayoutUnit(0.5) / new LayoutUnit(2)).ToInteger());
        Assert.Equal(10, (new LayoutUnit(100) / new LayoutUnit(10)).ToInteger());
        Assert.Equal(0.5f, (new LayoutUnit(1) / new LayoutUnit(2)).ToFloat());
        Assert.Equal(0.25f, (new LayoutUnit(0.5) / new LayoutUnit(2)).ToFloat());

        Assert.Equal(-1, (new LayoutUnit(-1) / new LayoutUnit(1)).ToInteger());
        Assert.Equal(0, (new LayoutUnit(-1) / new LayoutUnit(2)).ToInteger());
        Assert.Equal(-2, (new LayoutUnit(-2) / new LayoutUnit(1)).ToInteger());
        Assert.Equal(-4, (new LayoutUnit(-2) / new LayoutUnit(0.5)).ToInteger());
        Assert.Equal(0, (new LayoutUnit(-0.5) / new LayoutUnit(2)).ToInteger());
        Assert.Equal(-10, (new LayoutUnit(-100) / new LayoutUnit(10)).ToInteger());
        Assert.Equal(-0.5f, (new LayoutUnit(-1) / new LayoutUnit(2)).ToFloat());
        Assert.Equal(-0.25f, (new LayoutUnit(-0.5) / new LayoutUnit(2)).ToFloat());

        Assert.Equal(1, (new LayoutUnit(-1) / new LayoutUnit(-1)).ToInteger());
        Assert.Equal(0, (new LayoutUnit(-1) / new LayoutUnit(-2)).ToInteger());
        Assert.Equal(2, (new LayoutUnit(-2) / new LayoutUnit(-1)).ToInteger());
        Assert.Equal(4, (new LayoutUnit(-2) / new LayoutUnit(-0.5)).ToInteger());
        Assert.Equal(0, (new LayoutUnit(-0.5) / new LayoutUnit(-2)).ToInteger());
        Assert.Equal(10, (new LayoutUnit(-100) / new LayoutUnit(-10)).ToInteger());
        Assert.Equal(0.5f, (new LayoutUnit(-1) / new LayoutUnit(-2)).ToFloat());
        Assert.Equal(0.25f, (new LayoutUnit(-0.5) / new LayoutUnit(-2)).ToFloat());

        int a_hundred_size_t = 100;
        Assert.Equal(50, (new LayoutUnit(a_hundred_size_t) / new LayoutUnit(2)).ToInteger());
        Assert.Equal(25, (a_hundred_size_t / new LayoutUnit(4)).ToInteger());
        Assert.Equal(4, (new LayoutUnit(400) / a_hundred_size_t).ToInteger());

        Assert.Equal(IntegerMax / 2, (new LayoutUnit(IntegerMax) / new LayoutUnit(2)).ToInteger());
        Assert.Equal(IntegerMax, (new LayoutUnit(IntegerMax) / new LayoutUnit(0.5)).ToInteger());
    }

    [Fact]
    private static void TestDivisionByInt()
    {
        Assert.Equal(new LayoutUnit(1), new LayoutUnit(1) / 1);
        Assert.Equal(new LayoutUnit(0.5), new LayoutUnit(1) / 2);
        Assert.Equal(new LayoutUnit(-0.5), new LayoutUnit(1) / -2);
        Assert.Equal(new LayoutUnit(-0.5), new LayoutUnit(-1) / 2);
        Assert.Equal(new LayoutUnit(0.5), new LayoutUnit(-1) / -2);

        Assert.Equal(IntegerMax / 2.0, (new LayoutUnit(IntegerMax) / 2).ToDouble());

        Assert.Equal(InlineLayoutUnit.IntegerMax / 2.0, (new InlineLayoutUnit(InlineLayoutUnit.IntegerMax) / 2).ToDouble());
    }

    [Fact]
    private static void TestMulDiv()
    {
        LayoutUnit kMaxValue = MaxValue;
        LayoutUnit kMinValue = MinValue;
        LayoutUnit kEpsilon = new LayoutUnit().AddEpsilon();

        Assert.Equal(kMaxValue, kMaxValue.MulDiv(kMaxValue, kMaxValue));
        Assert.Equal(kMinValue, kMinValue.MulDiv(kMinValue, kMinValue));
        Assert.Equal(kMinValue, kMaxValue.MulDiv(kMinValue, kMaxValue));
        Assert.Equal(kMaxValue, kMinValue.MulDiv(kMinValue, kMaxValue));
        Assert.Equal(kMinValue + kEpsilon * 2, kMaxValue.MulDiv(kMaxValue, kMinValue));

        Assert.Equal(kMaxValue, kMaxValue.MulDiv(new LayoutUnit(2), kEpsilon));
        Assert.Equal(kMinValue, kMinValue.MulDiv(new LayoutUnit(2), kEpsilon));

        LayoutUnit kLargerInt = new(16384);
        LayoutUnit kLargerInt2 = new(32768);

        Assert.Equal(new LayoutUnit(8192), kLargerInt.MulDiv(kLargerInt, kLargerInt2));
    }

    [Fact]
    private static void TestCeil()
    {
        Assert.Equal(0, new LayoutUnit(0).Ceil());
        Assert.Equal(1, new LayoutUnit(0.1).Ceil());
        Assert.Equal(1, new LayoutUnit(0.5).Ceil());
        Assert.Equal(1, new LayoutUnit(0.9).Ceil());
        Assert.Equal(1, new LayoutUnit(1.0).Ceil());
        Assert.Equal(2, new LayoutUnit(1.1).Ceil());

        Assert.Equal(0, new LayoutUnit(-0.1).Ceil());
        Assert.Equal(0, new LayoutUnit(-0.5).Ceil());
        Assert.Equal(0, new LayoutUnit(-0.9).Ceil());
        Assert.Equal(-1, new LayoutUnit(-1.0).Ceil());

        Assert.Equal(IntegerMax, new LayoutUnit(IntegerMax).Ceil());
        Assert.Equal(IntegerMax, (new LayoutUnit(IntegerMax) - new LayoutUnit(0.5)).Ceil());
        Assert.Equal(IntegerMax - 1, (new LayoutUnit(IntegerMax) - new LayoutUnit(1)).Ceil());

        Assert.Equal(IntegerMin, new LayoutUnit(IntegerMin).Ceil());
    }

    [Fact]
    private static void TestFloor()
    {
        Assert.Equal(0, new LayoutUnit(0).Floor());
        Assert.Equal(0, new LayoutUnit(0.1).Floor());
        Assert.Equal(0, new LayoutUnit(0.5).Floor());
        Assert.Equal(0, new LayoutUnit(0.9).Floor());
        Assert.Equal(1, new LayoutUnit(1.0).Floor());
        Assert.Equal(1, new LayoutUnit(1.1).Floor());

        Assert.Equal(-1, new LayoutUnit(-0.1).Floor());
        Assert.Equal(-1, new LayoutUnit(-0.5).Floor());
        Assert.Equal(-1, new LayoutUnit(-0.9).Floor());
        Assert.Equal(-1, new LayoutUnit(-1.0).Floor());

        Assert.Equal(IntegerMax, new LayoutUnit(IntegerMax).Floor());

        Assert.Equal(IntegerMin, new LayoutUnit(IntegerMin).Floor());
        Assert.Equal(IntegerMin, (new LayoutUnit(IntegerMin) + new LayoutUnit(0.5)).Floor());
        Assert.Equal(IntegerMin + 1, (new LayoutUnit(IntegerMin) + new LayoutUnit(1)).Floor());
    }

    [Fact]
    private static void TestFloatOverflow()
    {
        // These should overflow to the max/min according to their sign.
        Assert.Equal(IntegerMax, new LayoutUnit(176972000.0f).ToInteger());
        Assert.Equal(IntegerMin, new LayoutUnit(-176972000.0f).ToInteger());
        Assert.Equal(IntegerMax, new LayoutUnit(176972000.0).ToInteger());
        Assert.Equal(IntegerMin, new LayoutUnit(-176972000.0).ToInteger());
    }

    [Fact]
    private static void TestUnaryMinus()
    {
        Assert.Equal(new LayoutUnit(), -new LayoutUnit());
        Assert.Equal(new LayoutUnit(999), -new LayoutUnit(-999));
        Assert.Equal(new LayoutUnit(-999), -new LayoutUnit(999));

        LayoutUnit negative_max = new();
        negative_max.SetRawValue(MinValue.RawValue() + 1);
        Assert.Equal(negative_max, -MaxValue);
        Assert.Equal(MaxValue, -negative_max);

        // -LayoutUnit::min() is saturated to LayoutUnit::max()
        Assert.Equal(LayoutUnit.MaxValue, -LayoutUnit.MinValue);
    }

    [Fact]
    private static void TestPlusPlus()
    {
        var val1 = new LayoutUnit(-2);
        var val2 = new LayoutUnit(-1);
        var val3 = new LayoutUnit(0);
        var val4 = new LayoutUnit(1);
        var val5 = new LayoutUnit(MaxValue);
        

        Assert.Equal(new LayoutUnit(-1), ++val1);
        Assert.Equal(new LayoutUnit(0), ++val2);
        Assert.Equal(new LayoutUnit(1), ++val3);
        Assert.Equal(new LayoutUnit(2), ++val4);
        
        Assert.Equal(MaxValue, ++val5);
    }

    [Fact]
    private static void TestIntMod()
    {
        Assert.Equal(new LayoutUnit(5), IntMod(new LayoutUnit(55), new LayoutUnit(10)));
        Assert.Equal(new LayoutUnit(5), IntMod(new LayoutUnit(55), new LayoutUnit(-10)));
        Assert.Equal(new LayoutUnit(-5), IntMod(new LayoutUnit(-55), new LayoutUnit(10)));
        Assert.Equal(new LayoutUnit(-5), IntMod(new LayoutUnit(-55), new LayoutUnit(-10)));
        Assert.Equal(new LayoutUnit(1.5), IntMod(new LayoutUnit(7.5), new LayoutUnit(3)));
        Assert.Equal(new LayoutUnit(1.25), IntMod(new LayoutUnit(7.5), new LayoutUnit(3.125)));
        Assert.Equal(new LayoutUnit(), IntMod(new LayoutUnit(7.5), new LayoutUnit(2.5)));
        Assert.Equal(new LayoutUnit(), IntMod(new LayoutUnit(), new LayoutUnit(123)));
    }

    [Fact]
    private static void TestFraction()
    {
        Assert.True(new LayoutUnit(-1.9f).HasFraction);
        Assert.True(new LayoutUnit(-1.6f).HasFraction);
        Assert.True(FromFloatRound(-1.51f).HasFraction);
        Assert.True(FromFloatRound(-1.5f).HasFraction);
        Assert.True(FromFloatRound(-1.49f).HasFraction);
        Assert.False(new LayoutUnit(-1.0f).HasFraction);
        Assert.True(FromFloatRound(-0.95f).HasFraction);
        Assert.True(FromFloatRound(-0.51f).HasFraction);
        Assert.True(FromFloatRound(-0.50f).HasFraction);
        Assert.True(FromFloatRound(-0.49f).HasFraction);
        Assert.True(new LayoutUnit(-0.1f).HasFraction);
        Assert.False(new LayoutUnit(-1.0f).HasFraction);
        Assert.False(new LayoutUnit(0.0f).HasFraction);
        Assert.True(new LayoutUnit(0.1f).HasFraction);
        Assert.True(FromFloatRound(0.49f).HasFraction);
        Assert.True(FromFloatRound(0.50f).HasFraction);
        Assert.True(FromFloatRound(0.51f).HasFraction);
        Assert.True(new LayoutUnit(0.95f).HasFraction);
        Assert.False(new LayoutUnit(1.0f).HasFraction);
    }

    [Fact]
    private static void TestFixedConsts()
    {
        Assert.Equal(LayoutUnit.FractionalBits, 6);
        Assert.Equal(LayoutUnit.IntegralBits, 26);
        Assert.Equal(TextRunLayoutUnit.FractionalBits, 16);
        Assert.Equal(TextRunLayoutUnit.IntegralBits, 16);
        Assert.Equal(InlineLayoutUnit.FractionalBits, 16);
        Assert.Equal(InlineLayoutUnit.IntegralBits, 48);
    }

    [Fact]
    private static void TestFixed()
    {
        int raw_value16 = 0x12345678;
        int raw_value6 = raw_value16 >> 10;
        var value16 = TextRunLayoutUnit.FromRawValue(raw_value16);
        var value6 = LayoutUnit.FromRawValue(raw_value6);
        Assert.Equal(value16.ToLayoutUnit(), value6);
    }

    [Fact]
    private static void TestRaw64FromInt32()
    {
        int int32_max_plus = LayoutUnit.IntegerMax + 10;
        LayoutUnit int32_max_plus_32 = new(int32_max_plus);
        Assert.NotEqual(int32_max_plus_32.ToInteger(), int32_max_plus);
        InlineLayoutUnit int32_max_plus_64 = new(int32_max_plus);
        Assert.Equal(int32_max_plus_64.ToInteger(), int32_max_plus);

        int int32_min_minus = LayoutUnit.IntegerMin - 10;
        LayoutUnit int32_min_minus_32 = new(int32_min_minus);
        Assert.NotEqual(int32_min_minus_32.ToInteger(), int32_min_minus);
        InlineLayoutUnit int32_min_minus_64 = new(int32_min_minus);
        Assert.Equal(int32_min_minus_64.ToInteger(), int32_min_minus);

        long raw32_max_plus = (long) LayoutUnit.RawValueMax + 10;
        LayoutUnit raw32_max_plus_32 = new(raw32_max_plus);
        Assert.NotEqual(raw32_max_plus_32.ToInteger(), raw32_max_plus);
        InlineLayoutUnit raw32_max_plus_64 = new(raw32_max_plus);
        Assert.Equal(raw32_max_plus_64.ToInteger(), raw32_max_plus);

        long raw32_min_minus = (long)LayoutUnit.RawValueMin - 10;
        LayoutUnit raw32_min_minus_32 = new(raw32_min_minus);
        Assert.NotEqual(raw32_min_minus_32.ToInteger(), raw32_min_minus);
        InlineLayoutUnit raw32_min_minus_64 = new(raw32_min_minus);
        Assert.Equal(raw32_min_minus_64.ToInteger(), raw32_min_minus);
    }

    [Fact]
    private static void TestRaw64FromRaw32()
    {
        float value = 1.0f + LayoutUnit.Epsilon() * 234;
        LayoutUnit value32_6 = new(value);
        Assert.Equal(new InlineLayoutUnit(value32_6), new InlineLayoutUnit(value));
        TextRunLayoutUnit value32_16 = new(value);
        Assert.Equal(new InlineLayoutUnit(value32_16), new InlineLayoutUnit(value));

        // The following code should fail to compile.
        // TextRunLayoutUnit back_to_32{InlineLayoutUnit(value)};
    }

    [Fact]
    private static void TestTo()
    {
        // LayoutUnit <-> TextRunLayoutUnit

        Assert.Equal(new LayoutUnit(1.0f), new TextRunLayoutUnit(1.0f).ToLayoutUnit());
        Assert.Equal(new TextRunLayoutUnit(1.0f), new LayoutUnit(1.0f).ToTextRunLayoutUnit());

        Assert.Equal(new LayoutUnit(1.5f), new TextRunLayoutUnit(1.5f).ToLayoutUnit());
        Assert.Equal(new TextRunLayoutUnit(1.5f), new LayoutUnit(1.5f).ToTextRunLayoutUnit());

        Assert.Equal(new LayoutUnit(-1.0f), new TextRunLayoutUnit(-1.0f).ToLayoutUnit());
        Assert.Equal(new TextRunLayoutUnit(-1.0f), new LayoutUnit(-1.0f).ToTextRunLayoutUnit());

        // LayoutUnit <-> InlineLayoutUnit

        Assert.Equal(new LayoutUnit(1.0f), new InlineLayoutUnit(1.0f).ToLayoutUnit());
        Assert.Equal(new InlineLayoutUnit(1.0f), new LayoutUnit(1.0f).ToInlineLayoutUnit());

        Assert.Equal(new LayoutUnit(1.5f), new InlineLayoutUnit(1.5f).ToLayoutUnit());
        Assert.Equal(new InlineLayoutUnit(1.5f), new LayoutUnit(1.5f).ToInlineLayoutUnit());

        Assert.Equal(new LayoutUnit(-1.0f), new InlineLayoutUnit(-1.0f).ToLayoutUnit());
        Assert.Equal(new InlineLayoutUnit(-1.0f), new LayoutUnit(-1.0f).ToInlineLayoutUnit());

        // TextRunLayoutUnit <-> InlineLayoutUnit

        Assert.Equal(new TextRunLayoutUnit(1.0f), new InlineLayoutUnit(1.0f).ToTextRunLayoutUnit());
        Assert.Equal(new InlineLayoutUnit(1.0f), new TextRunLayoutUnit(1.0f).ToInlineLayoutUnit());

        Assert.Equal(new TextRunLayoutUnit(1.5f), new InlineLayoutUnit(1.5f).ToTextRunLayoutUnit());
        Assert.Equal(new InlineLayoutUnit(1.5f), new TextRunLayoutUnit(1.5f).ToInlineLayoutUnit());

        Assert.Equal(new TextRunLayoutUnit(-1.0f), new InlineLayoutUnit(-1.0f).ToTextRunLayoutUnit());
        Assert.Equal(new InlineLayoutUnit(-1.0f), new TextRunLayoutUnit(-1.0f).ToInlineLayoutUnit());
    }

    [Fact]
    private static void TestToClampSameFractional64To32()
    {
        Assert.Equal(TextRunLayoutUnit.MaxValue, new InlineLayoutUnit(TextRunLayoutUnit.IntegerMax + 1).ToTextRunLayoutUnit());
        Assert.Equal(TextRunLayoutUnit.MinValue, new InlineLayoutUnit(TextRunLayoutUnit.IntegerMin - 1).ToTextRunLayoutUnit());
    }

    [Fact]
    private static void TestToClampLessFractional64To32()
    {
        Assert.Equal(LayoutUnit.MaxValue, new InlineLayoutUnit(LayoutUnit.IntegerMax + 1).ToLayoutUnit());
        Assert.Equal(LayoutUnit.MinValue, new InlineLayoutUnit(LayoutUnit.IntegerMin - 1).ToLayoutUnit());
    }

    [Fact]
    private static void TestToClampMoreFractional()
    {
        Assert.Equal(TextRunLayoutUnit.MaxValue, new LayoutUnit(TextRunLayoutUnit.IntegerMax + 1).ToTextRunLayoutUnit());
        Assert.Equal(TextRunLayoutUnit.MinValue, new LayoutUnit(TextRunLayoutUnit.IntegerMin - 1).ToTextRunLayoutUnit());
    }

    [Fact]
    private static void TestRaw64Ceil()
    {
        LayoutUnit layout = new(1.234);
        InlineLayoutUnit inline_value = new(layout);
        Assert.Equal(layout, inline_value.ToCeil());

        inline_value = inline_value.AddEpsilon();
        Assert.NotEqual(layout, inline_value.ToCeil());
        Assert.Equal(layout.AddEpsilon(), inline_value.ToCeil());
    }
}
