using Xunit;

using UI.GFX.Geometry;

using static UI.GFX.Geometry.TransformUtil;
using static UI.Tests.GeometryUtil;
using System.Diagnostics;

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
        static void verify(in RectF src_rect, in RectF dst_rect)
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

    [Fact]
    private static void TestOrthoProjectionTransform()
    {
        static void verify(float left, float right, float bottom, float top)
        {
            AxisTransform2D t = OrthoProjectionTransform(left, right, bottom, top);
            
            if (right == left || top == bottom)
            {
                Assert.Equal(new AxisTransform2D(), t);
            }
            else
            {
                Assert.Equal(new PointF(-1, -1), t.MapPoint(new PointF(left, bottom)));
                Assert.Equal(new PointF(1, 1), t.MapPoint(new PointF(right, top)));
            }
        }

        verify(0, 0, 0, 0);
        verify(10, 20, 10, 30);
        verify(10, 30, 20, 30);
        verify(0, 0, 10, 20);
        verify(-100, 400, 200, -200);
        verify(-1.5f, 4.25f, 2.75f, -3.75f);
    }

    [Fact]
    private static void TestWindowTransform()
    {
        static void verify(int x, int y, int width, int height)
        {
            AxisTransform2D t = WindowTransform(x, y, width, height);
            Assert.Equal(new PointF(x, y), t.MapPoint(new PointF(-1, -1)));
            Assert.Equal(new PointF(x + width, y + height), t.MapPoint(new PointF(1, 1)));
        }

        verify(0, 0, 0, 0);
        verify(10, 20, 0, 30);
        verify(10, 30, 20, 0);
        verify(0, 0, 10, 20);
        verify(-100, -400, 200, 300);
    }

    [Fact]
	private static void TestTransform2dScaleComponents()
	{
		// Values to test quiet NaN, infinity, and a denormal float if they're
		// present; zero otherwise.
		float quiet_NaN_or_zero = float.IsNaN(float.NaN) ? float.NaN : 0;
        float infinity_or_zero  = float.IsInfinity(float.PositiveInfinity) ? float.PositiveInfinity : 0;
        // float denorm_min = float.Epsilon fails
        // double.IsNormal(double.Epsilon) returns false,
        // so TryComputeTransform2dScaleComponents correctly returns null
        double denorm_min = double.Epsilon;

		(Transform transform, Vector2DF? expected_scale)[] tests =
		[
			// A matrix with only scale and translation.
            (Transform.RowMajor(3, 0, 0, -23, 0, 7, 0, 31, 0, 0, 11, 47, 0, 0, 0, 1), new Vector2DF(3, 7)),
            // Matrices like the first, but also with various
            // perspective-altering components.
            (Transform.RowMajor(3, 0, 0, -23, 0, 7, 0, 31, 0, 0, 11, 47, 0, 0, -0.5, 1), new Vector2DF(3, 7)),
            // The result is always non-negative.
            (Transform.RowMajor(3, 0, 0, -23, 0, -7, 0, 31, 0, 0, 11, 47, 0, 0, -0.5, 1), new Vector2DF(3, 7)),
            // Values are clamped.
            (Transform.RowMajor(double.MaxValue, 0, 0, -23, 0, double.MinValue, 0, 31, 0, 0, 11, 47, 0, 0, -0.5f, 1), new Vector2DF(ClampFloatGeometryHelper.Max, ClampFloatGeometryHelper.Max)),
            (Transform.RowMajor(3, 0, 0, -23, 0, 7, 0, 31, 0, 0, 11, 47, 0.2f, 0, -0.5f, 1), null),
            (Transform.RowMajor(3, 0, 0, -23, 0, 7, 0, 31, 0, 0, 11, 47, 0.2f, -0.2f, -0.5f, 1), null),
            (Transform.RowMajor(3, 0, 0, -23, 0, 7, 0, 31, 0, 0, 11, 47, 0.2f, -0.2f, -0.5f, 1), null),
            (Transform.RowMajor(3, 0, 0, -23, 0, 7, 0, 31, 0, 0, 11, 47, 0, -0.2f, -0.5f, 1), null),
            (Transform.RowMajor(3, 0, 0, -23, 0, 7, 0, 31, 0, 0, 11, 47, 0, 0, -0.5f, 0.25f), new Vector2DF(12, 28)),
            
            // Matrices like the first, but with some types of rotation.
            (Transform.RowMajor(0, 3, 0, -23, 7, 0, 0, 31, 0, 0, 11, 47, 0, 0, 0, 1), new Vector2DF(7, 3)),
            (Transform.RowMajor(3, 8, 0, -23, 4, 6, 0, 31, 0, 0, 11, 47, 0, 0, 0, 1), new Vector2DF(5, 10)),
            // Combination of rotation and perspective
            (Transform.RowMajor(3, 8, 0, -23, 4, 6, 0, 31, 0, 0, 11, 47, 0, 0, 0, 0.25f), new Vector2DF(20, 40)),
            // Error handling cases for final perspective component.
            (Transform.RowMajor(3, 0, 0, -23, 0, 7, 0, 31, 0, 0, 11, 47, 0, 0, 0, 0), null),
            (Transform.RowMajor(3, 0, 0, -23, 0, 7, 0, 31, 0, 0, 11, 47, 0, 0, 0, quiet_NaN_or_zero), null),
            (Transform.RowMajor(3, 0, 0, -23, 0, 7, 0, 31, 0, 0, 11, 47, 0, 0, 0, infinity_or_zero), null),
            (Transform.RowMajor(3, 0, 0, -23, 0, 7, 0, 31, 0, 0, 11, 47, 0, 0, 0, denorm_min), null)
		];

		const float fallback = 1.409718f; // randomly generated in [1, 2)

		foreach (var (transform, expected_scale) in tests)
		{
			Vector2DF? try_result = TryComputeTransform2dScaleComponents(transform);

			Assert.Equal(expected_scale, try_result);

			Vector2DF result = ComputeTransform2dScaleComponents(transform, fallback);
            
			if (expected_scale.HasValue)
  
				Assert.Equal(expected_scale, result);

			else

				Assert.Equal(new Vector2DF(fallback, fallback), result);
		}
	}

/*
    [Fact]
    private static void TestTransform2dScaleComponents()
    {
        // Values to test quiet NaN, infinity, and a denormal float if they're present;
        // zero otherwise (since for the case this is used for, it should produce the same result).

        const float quiet_NaN_or_zero = float.NaN;
        const float infinity_or_zero = float.PositiveInfinity;  // Or float.NegativeInfinity if needed
        const float denorm_min = float.Epsilon;

        (Transform transform, Vector2DF? expected_scale)[] tests =
        [
            // A matrix with only scale and translation.
            (Transform.RowMajor(3, 0, 0, -23, 0, 7, 0, 31, 0, 0, 11, 47, 0, 0, 0, 1), new Vector2DF(3, 7)),
            // Matrices like the first, but also with various
            // perspective-altering components.
            (Transform.RowMajor(3, 0, 0, -23, 0, 7, 0, 31, 0, 0, 11, 47, 0, 0, -0.5, 1), new Vector2DF(3, 7)),
            // The result is always non-negative.
            (Transform.RowMajor(3, 0, 0, -23, 0, -7, 0, 31, 0, 0, 11, 47, 0, 0, -0.5, 1), new Vector2DF(3, 7)),
            // Values are clamped.
            (Transform.RowMajor(double.MaxValue, 0, 0, -23, 0, double.MinValue, 0, 31, 0, 0, 11, 47, 0, 0, -0.5f, 1), new Vector2DF(ClampFloatGeometryHelper.Max, ClampFloatGeometryHelper.Max)),
            (Transform.RowMajor(3, 0, 0, -23, 0, 7, 0, 31, 0, 0, 11, 47, 0.2f, 0, -0.5f, 1), null),
            (Transform.RowMajor(3, 0, 0, -23, 0, 7, 0, 31, 0, 0, 11, 47, 0.2f, -0.2f, -0.5f, 1), null),
            (Transform.RowMajor(3, 0, 0, -23, 0, 7, 0, 31, 0, 0, 11, 47, 0.2f, -0.2f, -0.5f, 1), null),
            (Transform.RowMajor(3, 0, 0, -23, 0, 7, 0, 31, 0, 0, 11, 47, 0, -0.2f, -0.5f, 1), null),
            (Transform.RowMajor(3, 0, 0, -23, 0, 7, 0, 31, 0, 0, 11, 47, 0, 0, -0.5f, 0.25f), new Vector2DF(12, 28)),
            // Matrices like the first, but with some types of rotation.
            (Transform.RowMajor(0, 3, 0, -23, 7, 0, 0, 31, 0, 0, 11, 47, 0, 0, 0, 1), new Vector2DF(7, 3)),
            (Transform.RowMajor(3, 8, 0, -23, 4, 6, 0, 31, 0, 0, 11, 47, 0, 0, 0, 1), new Vector2DF(5, 10)),
            // Combination of rotation and perspective
            (Transform.RowMajor(3, 8, 0, -23, 4, 6, 0, 31, 0, 0, 11, 47, 0, 0, 0, 0.25f), new Vector2DF(20, 40)),
            // Error handling cases for final perspective component.
            (Transform.RowMajor(3, 0, 0, -23, 0, 7, 0, 31, 0, 0, 11, 47, 0, 0, 0, 0), null),
            (Transform.RowMajor(3, 0, 0, -23, 0, 7, 0, 31, 0, 0, 11, 47, 0, 0, 0, quiet_NaN_or_zero), null),
            (Transform.RowMajor(3, 0, 0, -23, 0, 7, 0, 31, 0, 0, 11, 47, 0, 0, 0, infinity_or_zero), null),
            (Transform.RowMajor(3, 0, 0, -23, 0, 7, 0, 31, 0, 0, 11, 47, 0, 0, 0, denorm_min), null)
        ];

        const float fallback = 1.409718f;  // randomly generated in [1,2)

        foreach (var (transform, expected_scale) in tests)
        {
            Vector2DF? try_result = TryComputeTransform2dScaleComponents(transform);
            Assert.Equal(try_result, expected_scale);
            Vector2DF result = ComputeTransform2dScaleComponents(transform, fallback);
            if (expected_scale.HasValue)
            {
                Assert.Equal(result, expected_scale);
            }
            else
            {
                Assert.Equal(result, new Vector2DF(fallback, fallback));
            }
        }
    }
*/
}
