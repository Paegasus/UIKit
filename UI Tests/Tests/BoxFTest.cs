using Xunit;

using UI.GFX.Geometry;

namespace UI.Tests;

public static class BoxFTest
{
    [Fact]
    private static void TestConstructors()
    {
        Assert.Equal(new BoxF(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f).ToString(), new BoxF().ToString());
        Assert.Equal(new BoxF(0.0f, 0.0f, 0.0f, -3.0f, -5.0f, -7.0f).ToString(), new BoxF().ToString());

        Assert.Equal(new BoxF(0.0f, 0.0f, 0.0f, 3.0f, 5.0f, 7.0f).ToString(), new BoxF(3.0f, 5.0f, 7.0f).ToString());
        Assert.Equal(new BoxF(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f).ToString(), new BoxF(-3.0f, -5.0f, -7.0f).ToString());

        Assert.Equal(new BoxF(2.0f, 4.0f, 6.0f, 3.0f, 5.0f, 7.0f).ToString(), new BoxF(new Point3F(2.0f, 4.0f, 6.0f), 3.0f, 5.0f, 7.0f).ToString());
        Assert.Equal(new BoxF(2.0f, 4.0f, 6.0f, 0.0f, 0.0f, 0.0f).ToString(), new BoxF(new Point3F(2.0f, 4.0f, 6.0f), -3.0f, -5.0f, -7.0f).ToString());
    }
}
