using Xunit;

using UI.GFX.Geometry;

using static UI.GFX.Geometry.TransformUtil;
using static UI.Tests.GeometryUtil;

namespace UI.Tests;

public static class TransformUtilTest
{
    [Fact]
    private static void TestGetScaleTransform()
    {
        Point kAnchor = new(20, 40);
        const float kScale = 0.5f;

        Transform scale = GetScaleTransform(kAnchor, kScale);

        const int kOffset = 10;

        for (int sign_x = -1; sign_x <= 1; ++sign_x)
        {
            for (int sign_y = -1; sign_y <= 1; ++sign_y)
            {
                Point test = scale.MapPoint(new Point(kAnchor.X + sign_x * kOffset, kAnchor.Y + sign_y * kOffset));

                Assert.Equal(new Point((int)(kAnchor.X + sign_x * kOffset * kScale), (int)(kAnchor.Y + sign_y * kOffset * kScale)), test);
            }
        }
    }

    [Fact]
    private static void TestTransformAboutPivot()
    {
        Transform transform = new();
        transform.Scale(3, 4);
        transform = TransformAboutPivot(new PointF(7, 8), transform);

        Point point = transform.MapPoint(new Point(0, 0));
        Assert.Equal(new Point(-14, -24).ToString(), point.ToString());

        point = transform.MapPoint(new Point(1, 1));
        Assert.Equal(new Point(-11, -20).ToString(), point.ToString());
    }

    [Fact]
    private static void TestBlendOppositeQuaternions()
    {
        DecomposedTransform first = new();
        DecomposedTransform second = new();
        second.Quaternion.W = -second.Quaternion.W;

        DecomposedTransform result = BlendDecomposedTransforms(first, second, 0.25);

        Assert.True(double.IsFinite(result.Quaternion.X));
        Assert.True(double.IsFinite(result.Quaternion.Y));
        Assert.True(double.IsFinite(result.Quaternion.Z));
        Assert.True(double.IsFinite(result.Quaternion.W));

        Assert.False(double.IsNaN(result.Quaternion.X));
        Assert.False(double.IsNaN(result.Quaternion.Y));
        Assert.False(double.IsNaN(result.Quaternion.Z));
        Assert.False(double.IsNaN(result.Quaternion.W));
    }

    [Fact]
    private static void TestAccumulateDecomposedTransforms()
    {
        DecomposedTransform a = new()
        {
            Translate = new(2.5, -3.25, 4.75),
            Scale = new(4.5, -5.25, 6.75),
            Skew = new(1.25, -2.5, 3.75),
            Perspective = new(5, -4, 3, -2),
            Quaternion = new(-5, 6, -7, 8)
        };

        DecomposedTransform b = new()
        {
            Translate = new( -2, 3, 4),
            Scale = new( -4, 5, 6),
            Skew = new( -1, 2, 3),
            Perspective = new( 6, 7, -8, -9),
            Quaternion = new( 5, 4, -3, -2)
        };

        DecomposedTransform expected = new()
        {
            Translate = new( 0.5, -0.25, 8.75),
            Scale = new( -0.5, -1.25, 11.75),
            Skew = new( 0.25, -0.5, 6.75),
            Perspective = new( 11, 3, -5, -12),
            Quaternion = new( +60, -30, -60, -36)
        };

        AssertDecomposedTransformEqual(expected, AccumulateDecomposedTransforms(a, b));
    }
}
