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

    [Fact]
    private static void TestTransformBetweenRects()
    {
        void verify(in RectF src_rect, in RectF dst_rect)
        {
            Transform transform = TransformBetweenRects(src_rect, dst_rect);

            // Applies |transform| to calculate the target rectangle from |src_rect|.
            // Notes that |transform| is in |src_rect|'s local coordinates.
            RectF dst_in_parent_coordinates = transform.MapRect(new RectF(src_rect.Size));
            dst_in_parent_coordinates.Offset(src_rect.OffsetFromOrigin());

            // Verifies that the target rectangle is expected.
            Assert.Equal(dst_rect, dst_in_parent_coordinates);
        }

        (RectF first, RectF second)[] test_cases =
        [
            (new RectF(0.0f, 0.0f, 2.0f, 3.0f), new RectF(3.0f, 5.0f, 4.0f, 9.0f)),
            (new RectF(10.0f, 7.0f, 2.0f, 6.0f), new RectF(4.0f, 2.0f, 1.0f, 12.0f)),
            (new RectF(0.0f, 0.0f, 3.0f, 5.0f), new RectF(0.0f, 0.0f, 6.0f, 2.5f))
        ];

        foreach (var (first, second) in test_cases)
        {
            verify(first, second);
            verify(second, first);
        }

        // Tests the case where the destination is an empty rectangle.
        verify(new RectF(0.0f, 0.0f, 3.0f, 5.0f), new RectF());
    }
}
