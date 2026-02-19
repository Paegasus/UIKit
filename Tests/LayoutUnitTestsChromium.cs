using System.Diagnostics;
using UI.Geometry;

using static UI.Geometry.LayoutUnit;

namespace UI.Tests;

public static class LayoutUnitTestsChromium
{
    public static void RunAllTests()
    {
        LayoutUnitCeil();
        LayoutUnitFloor();
        LayoutUnitRounding();
        LayoutUnitMultiplication();
        LayoutUnitSnapSizeToPixel();

        Debug.WriteLine("All LayoutUnit tests passed!");
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

    private static void LayoutUnitMultiplication()
    {
        Debug.Assert(333 == (new LayoutUnit(100) * new LayoutUnit(3.33)).Round());
        Debug.Assert(-333 == (new LayoutUnit(-100) * new LayoutUnit(3.33)).Round());
        Debug.Assert(333 == (new LayoutUnit(-100) * new LayoutUnit(-3.33)).Round());
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
}
