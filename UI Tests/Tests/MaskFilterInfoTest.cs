using Xunit;

using UI.GFX.Geometry;

namespace UI.Tests;

public static class MaskFilterInfoTest
{

    private static LinearGradient CreateGradient(short angle)
    {
        LinearGradient gradient = new(angle);
        gradient.AddStep(0.5f, 50);
        return gradient;
    }

    [Fact]
    private static void TestApplyTransform()
    {
        var info = new MaskFilterInfo(new RRectF(1.0f, 2.0f, 20.0f, 25.0f, 5.0f));
        MaskFilterInfo expected = info;
        info.ApplyTransform(new Transform());
        Assert.Equal(expected, info);
    }
}
