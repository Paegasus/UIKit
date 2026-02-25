using Xunit;

using UI.GFX.Geometry;
using System.Diagnostics;

namespace UI.Tests;

public static class RRectFTest
{
    [Fact]
    private static void TestIsEmpty()
    {
        Assert.True(new RRectF().IsEmpty());
        Assert.True(new RRectF(0, 0, 0, 0, 0).IsEmpty());
        Assert.True(new RRectF(0, 0, 10, 0, 0).IsEmpty());
        Assert.True(new RRectF(0, 0, 0, 10, 0).IsEmpty());
        Assert.True(new RRectF(0, 0, 0, 10, 10).IsEmpty());
        Assert.False(new RRectF(0, 0, 10, 10, 0).IsEmpty());
    }

    [Fact]
    private static void TestEquals()
    {
        Assert.Equal(new RRectF(0, 0, 0, 0, 0, 0), new RRectF(0, 0, 0, 0, 0, 0));
        Assert.Equal(new RRectF(1, 2, 3, 4, 5, 6), new RRectF(1, 2, 3, 4, 5, 6));
        Assert.Equal(new RRectF(1, 2, 3, 4, 5, 5), new RRectF(1, 2, 3, 4, 5));
        Assert.Equal(new RRectF(0, 0, 2, 3, 0, 0), new RRectF(0, 0, 2, 3, 0, 1));
        Assert.Equal(new RRectF(0, 0, 2, 3, 0, 0), new RRectF(0, 0, 2, 3, 1, 0));
        Assert.Equal(new RRectF(1, 2, 3, 0, 5, 6), new RRectF(0, 0, 0, 0, 0, 0));
        Assert.Equal(new RRectF(0, 0, 0, 0, 5, 6), new RRectF(0, 0, 0, 0, 0, 0));

        Assert.NotEqual(new RRectF(10, 20, 30, 40, 7, 8), new RRectF(1, 20, 30, 40, 7, 8));
        Assert.NotEqual(new RRectF(10, 20, 30, 40, 7, 8), new RRectF(10, 2, 30, 40, 7, 8));
        Assert.NotEqual(new RRectF(10, 20, 30, 40, 7, 8), new RRectF(10, 20, 3, 40, 7, 8));
        Assert.NotEqual(new RRectF(10, 20, 30, 40, 7, 8), new RRectF(10, 20, 30, 4, 7, 8));
        Assert.NotEqual(new RRectF(10, 20, 30, 40, 7, 8), new RRectF(10, 20, 30, 40, 5, 8));
        Assert.NotEqual(new RRectF(10, 20, 30, 40, 7, 8), new RRectF(10, 20, 30, 40, 7, 6));
    }

    [Fact]
    private static void TestPlusMinusOffset()
    {
        RRectF a = new(40, 50, 60, 70, 5);
        Vector2D offset = new(23, 34);
        RRectF correct = new(63, 84, 60, 70, 5);
        RRectF b = a + offset;
        Assert.Equal(b, correct);
        b = a;
        b.Offset(offset);
        Assert.Equal(b, correct);

        correct = new RRectF(17, 16, 60, 70, 5);
        b = a - offset;
        Assert.Equal(b, correct); // Fails
        b = a;
        b.Offset(-offset);
        Assert.Equal(b, correct);
    }
}
