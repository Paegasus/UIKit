using Xunit;

using UI.GFX.Geometry;

using static UI.GFX.Geometry.SizeConversions;

namespace UI.Tests;

public static class SizeFTest
{
    [Fact]
    private static void TestSizeToSizeF()
    {
        // Check that explicit conversion from integer to float compiles.
        Size a = new(10, 20);
        Assert.Equal(10, new SizeF(a).Width);
        Assert.Equal(20, new SizeF(a).Height);

        SizeF b = new(10, 20);
        Assert.Equal(b, new SizeF(a));
    }

    [Fact]
    private static void TestToFlooredSize()
    {
        Assert.Equal(new Size(0, 0), ToFlooredSize(new SizeF(0, 0)));
        Assert.Equal(new Size(0, 0), ToFlooredSize(new SizeF(0.0001f, 0.0001f)));
        Assert.Equal(new Size(0, 0), ToFlooredSize(new SizeF(0.4999f, 0.4999f)));
        Assert.Equal(new Size(0, 0), ToFlooredSize(new SizeF(0.5f, 0.5f)));
        Assert.Equal(new Size(0, 0), ToFlooredSize(new SizeF(0.9999f, 0.9999f)));

        Assert.Equal(new Size(10, 10), ToFlooredSize(new SizeF(10, 10)));
        Assert.Equal(new Size(10, 10), ToFlooredSize(new SizeF(10.0001f, 10.0001f)));
        Assert.Equal(new Size(10, 10), ToFlooredSize(new SizeF(10.4999f, 10.4999f)));
        Assert.Equal(new Size(10, 10), ToFlooredSize(new SizeF(10.5f, 10.5f)));
        Assert.Equal(new Size(10, 10), ToFlooredSize(new SizeF(10.9999f, 10.9999f)));
    }

    [Fact]
    private static void TestToCeiledSize()
    {
        Assert.Equal(new Size(0, 0), ToCeiledSize(new SizeF(0, 0)));
        Assert.Equal(new Size(1, 1), ToCeiledSize(new SizeF(0.0001f, 0.0001f)));
        Assert.Equal(new Size(1, 1), ToCeiledSize(new SizeF(0.4999f, 0.4999f)));
        Assert.Equal(new Size(1, 1), ToCeiledSize(new SizeF(0.5f, 0.5f)));
        Assert.Equal(new Size(1, 1), ToCeiledSize(new SizeF(0.9999f, 0.9999f)));

        Assert.Equal(new Size(10, 10), ToCeiledSize(new SizeF(10, 10)));
        Assert.Equal(new Size(11, 11), ToCeiledSize(new SizeF(10.0001f, 10.0001f)));
        Assert.Equal(new Size(11, 11), ToCeiledSize(new SizeF(10.4999f, 10.4999f)));
        Assert.Equal(new Size(11, 11), ToCeiledSize(new SizeF(10.5f, 10.5f)));
        Assert.Equal(new Size(11, 11), ToCeiledSize(new SizeF(10.9999f, 10.9999f)));
    }

    [Fact]
    private static void TestToRoundedSize()
    {
        Assert.Equal(new Size(0, 0), ToRoundedSize(new SizeF(0, 0)));
        Assert.Equal(new Size(0, 0), ToRoundedSize(new SizeF(0.0001f, 0.0001f)));
        Assert.Equal(new Size(0, 0), ToRoundedSize(new SizeF(0.4999f, 0.4999f)));
        Assert.Equal(new Size(1, 1), ToRoundedSize(new SizeF(0.5f, 0.5f)));
        Assert.Equal(new Size(1, 1), ToRoundedSize(new SizeF(0.9999f, 0.9999f)));

        Assert.Equal(new Size(10, 10), ToRoundedSize(new SizeF(10, 10)));
        Assert.Equal(new Size(10, 10), ToRoundedSize(new SizeF(10.0001f, 10.0001f)));
        Assert.Equal(new Size(10, 10), ToRoundedSize(new SizeF(10.4999f, 10.4999f)));
        Assert.Equal(new Size(11, 11), ToRoundedSize(new SizeF(10.5f, 10.5f)));
        Assert.Equal(new Size(11, 11), ToRoundedSize(new SizeF(10.9999f, 10.9999f)));
    }

    [Fact]
    private static void TestSetToMinMax()
    {
        SizeF a;

        a = new SizeF(3.5f, 5.5f);
        Assert.Equal(new SizeF(3.5f, 5.5f).ToString(), a.ToString());
        a.SetToMax(new SizeF(2.5f, 4.5f));
        Assert.Equal(new SizeF(3.5f, 5.5f).ToString(), a.ToString());
        a.SetToMax(new SizeF(3.5f, 5.5f));
        Assert.Equal(new SizeF(3.5f, 5.5f).ToString(), a.ToString());
        a.SetToMax(new SizeF(4.5f, 2.5f));
        Assert.Equal(new SizeF(4.5f, 5.5f).ToString(), a.ToString());
        a.SetToMax(new SizeF(8.5f, 10.5f));
        Assert.Equal(new SizeF(8.5f, 10.5f).ToString(), a.ToString());

        a.SetToMin(new SizeF(9.5f, 11.5f));
        Assert.Equal(new SizeF(8.5f, 10.5f).ToString(), a.ToString());
        a.SetToMin(new SizeF(8.5f, 10.5f));
        Assert.Equal(new SizeF(8.5f, 10.5f).ToString(), a.ToString());
        a.SetToMin(new SizeF(11.5f, 9.5f));
        Assert.Equal(new SizeF(8.5f, 9.5f).ToString(), a.ToString());
        a.SetToMin(new SizeF(7.5f, 11.5f));
        Assert.Equal(new SizeF(7.5f, 9.5f).ToString(), a.ToString());
        a.SetToMin(new SizeF(3.5f, 5.5f));
        Assert.Equal(new SizeF(3.5f, 5.5f).ToString(), a.ToString());
    }

    [Fact]
    private static void TestOperatorAddSub()
    {
        SizeF lhs = new(100.5f, 20);
        SizeF rhs = new(50, 10.25f);

        lhs += rhs;
        Assert.Equal(new SizeF(150.5f, 30.25f), lhs);

        lhs = new SizeF(100, 20.25f);
        Assert.Equal(new SizeF(150, 30.5f), lhs + rhs);

        lhs = new SizeF(100.5f, 20);
        lhs -= rhs;
        Assert.Equal(new SizeF(50.5f, 9.75f), lhs);

        lhs = new SizeF(100, 20.75f);
        Assert.Equal(new SizeF(50, 10.5f), lhs - rhs);

        Assert.Equal(new SizeF(0, 0), rhs - lhs);
        rhs -= lhs;
        Assert.Equal(new SizeF(0, 0), rhs);
    }

    [Fact]
    private static void TestIsEmpty()
    {
        float clearly_trivial = SizeF.Trivial / 2.0f;
        float massize_dimension = 4e13f;

        // First, using the constructor.
        Assert.True(new SizeF(clearly_trivial, 1.0f).
        // First, using the constructor.
        IsEmpty);
        Assert.True(new SizeF(.01f, clearly_trivial).IsEmpty);
        Assert.True(new SizeF(0.0f, 0.0f).IsEmpty);
        Assert.False(new SizeF(.01f, .01f).IsEmpty);

        // Then use the setter.
        SizeF test = new(2.0f, 1.0f);
        Assert.False(test.IsEmpty);

        test.SetSize(clearly_trivial, 1.0f);
        Assert.True(test.IsEmpty);

        test.SetSize(.01f, clearly_trivial);
        Assert.True(test.IsEmpty);

        test.SetSize(0.0f, 0.0f);
        Assert.True(test.IsEmpty);

        test.SetSize(.01f, .01f);
        Assert.False(test.IsEmpty);

        // Now just one dimension at a time.
        test.Width = clearly_trivial;
        Assert.True(test.IsEmpty);

        test.Width = massize_dimension;
        test.Height = clearly_trivial;
        Assert.True(test.IsEmpty);

        test.Width = clearly_trivial;
        test.Height = massize_dimension;
        Assert.True(test.IsEmpty);

        test.Width = 2.0f;
        Assert.False(test.IsEmpty);
    }

    // These are the ramifications of the decision to keep the recorded size at zero for trivial sizes.
    [Fact]
    private static void TestClampsToZero()
    {
        float clearly_trivial = SizeF.Trivial / 2.0f;
        float nearly_trivial = SizeF.Trivial * 1.5f;

        SizeF test = new(clearly_trivial, 1.0f);

        Assert.Equal(0.0f, test.Width);
        Assert.Equal(1.0f, test.Height);

        test.SetSize(.01f, clearly_trivial);

        Assert.Equal(0.01f, test.Width);
        Assert.Equal(0.0f, test.Height);

        test.SetSize(nearly_trivial, nearly_trivial);

        Assert.Equal(nearly_trivial, test.Width);
        Assert.Equal(nearly_trivial, test.Height);

        test.Scale(0.5f);

        Assert.Equal(0.0f, test.Width);
        Assert.Equal(0.0f, test.Height);

        test.SetSize(0.0f, 0.0f);
        test.Enlarge(clearly_trivial, clearly_trivial);
        test.Enlarge(clearly_trivial, clearly_trivial);
        test.Enlarge(clearly_trivial, clearly_trivial);

        Assert.Equal(new SizeF(0.0f, 0.0f), test);
    }
}
