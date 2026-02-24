using Xunit;

using UI.GFX.Geometry;

namespace UI.Tests;

public static class SizeTest
{
    [Fact]
    private static void TestSetToMinMax()
    {
        Size a;

        a = new Size(3, 5);
        Assert.Equal(new Size(3, 5).ToString(), a.ToString());
        a.SetToMax(new Size(2, 4));
        Assert.Equal(new Size(3, 5).ToString(), a.ToString());
        a.SetToMax(new Size(3, 5));
        Assert.Equal(new Size(3, 5).ToString(), a.ToString());
        a.SetToMax(new Size(4, 2));
        Assert.Equal(new Size(4, 5).ToString(), a.ToString());
        a.SetToMax(new Size(8, 10));
        Assert.Equal(new Size(8, 10).ToString(), a.ToString());

        a.SetToMin(new Size(9, 11));
        Assert.Equal(new Size(8, 10).ToString(), a.ToString());
        a.SetToMin(new Size(8, 10));
        Assert.Equal(new Size(8, 10).ToString(), a.ToString());
        a.SetToMin(new Size(11, 9));
        Assert.Equal(new Size(8, 9).ToString(), a.ToString());
        a.SetToMin(new Size(7, 11));
        Assert.Equal(new Size(7, 9).ToString(), a.ToString());
        a.SetToMin(new Size(3, 5));
        Assert.Equal(new Size(3, 5).ToString(), a.ToString());
    }

    [Fact]
    private static void TestEnlarge()
    {
        Size test = new(3, 4);
        test.Enlarge(5, -8);
        Assert.Equal(test, new Size(8, -4));
    }

    [Fact]
    private static void TestIntegerOverflow()
    {
        int int_max = int.MaxValue;
        int int_min = int.MinValue;

        Size max_size = new(int_max, int_max);
        Size min_size = new(int_min, int_min);
        Size test;

        test = new Size();
        test.Enlarge(int_max, int_max);
        Assert.Equal(test, max_size);

        test = new Size();
        test.Enlarge(int_min, int_min);
        Assert.Equal(test, min_size);

        test = new Size(10, 20);
        test.Enlarge(int_max, int_max);
        Assert.Equal(test, max_size);

        test = new Size(-10, -20);
        test.Enlarge(int_min, int_min);
        Assert.Equal(test, min_size);
    }

    [Fact]
    private static void TestOperatorAddSub()
    {
        Size lhs = new(100, 20);
        Size rhs = new(50, 10);

        lhs += rhs;
        Assert.Equal(new Size(150, 30), lhs);

        lhs = new Size(100, 20);
        Assert.Equal(new Size(150, 30), lhs + rhs);

        lhs = new Size(100, 20);
        lhs -= rhs;
        Assert.Equal(new Size(50, 10), lhs);

        lhs = new Size(100, 20);
        Assert.Equal(new Size(50, 10), lhs - rhs);
    }

    [Fact]
    private static void TestOperatorAddOverflow()
    {
        int int_max = int.MaxValue;

        Size lhs = new(int_max, int_max);
        Size rhs = new(int_max, int_max);
        Assert.Equal(new Size(int_max, int_max), lhs + rhs);
    }

    [Fact]
    private static void TestOperatorSubClampAtZero()
    {
        Size lhs = new(10, 10);
        Size rhs = new(100, 100);
        Assert.Equal(new Size(0, 0), lhs - rhs);

        lhs = new Size(10, 10);
        rhs = new Size(100, 100);
        lhs -= rhs;
        Assert.Equal(new Size(0, 0), lhs);
    }

    [Fact]
    private static void TestOperatorCompare()
    {
        Size lhs = new(100, 20);
        Size rhs = new(50, 10);

        Assert.True(lhs != rhs);
        Assert.False(lhs == rhs);

        rhs = new Size(100, 20);
        Assert.True(lhs == rhs);
        Assert.False(lhs != rhs);
    }

    [Fact]
    private static void TestTranspose()
    {
        Size s = new(1, 2);
        Assert.Equal(new Size(2, 1), Size.TransposeSize(s));
        s.Transpose();
        Assert.Equal(new Size(2, 1), s);
    }
}
