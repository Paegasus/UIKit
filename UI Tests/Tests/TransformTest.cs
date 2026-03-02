using Xunit;

using UI.GFX.Geometry;
using System.Diagnostics;

using static UI.Tests.GeometryUtil;

namespace UI.Tests;

public static class TransformTest
{
    private static void STATIC_ROW0_EQ(double a, double b, double c, double d, Transform t)
    {
        Debug.Assert(a == t.rc(0, 0));
        Debug.Assert(b == t.rc(0, 1));
        Debug.Assert(c == t.rc(0, 2));
        Debug.Assert(d == t.rc(0, 3));
    }

    private static void STATIC_ROW1_EQ(double a, double b, double c, double d, Transform t)
    {
        Debug.Assert(a == t.rc(1, 0));
        Debug.Assert(b == t.rc(1, 1));
        Debug.Assert(c == t.rc(1, 2));
        Debug.Assert(d == t.rc(1, 3));
    }

    private static void STATIC_ROW2_EQ(double a, double b, double c, double d, Transform t)
    {
        Debug.Assert(a == t.rc(2, 0));
        Debug.Assert(b == t.rc(2, 1));
        Debug.Assert(c == t.rc(2, 2));
        Debug.Assert(d == t.rc(2, 3));
    }

    private static void STATIC_ROW3_EQ(double a, double b, double c, double d, Transform t)
    {
        Debug.Assert(a == t.rc(3, 0));
        Debug.Assert(b == t.rc(3, 1));
        Debug.Assert(c == t.rc(3, 2));
        Debug.Assert(d == t.rc(3, 3));
    }

    private static void EXPECT_ROW0_EQ(float a, float b, float c, float d, in Transform t)
    {
        Assert.True(FloatAlmostEqual(a, (float)t.rc(0, 0)));
        Assert.True(FloatAlmostEqual(b, (float)t.rc(0, 1)));
        Assert.True(FloatAlmostEqual(c, (float)t.rc(0, 2)));
        Assert.True(FloatAlmostEqual(d, (float)t.rc(0, 3)));
    }

    private static void EXPECT_ROW1_EQ(float a, float b, float c, float d, in Transform t)
    {
        Assert.True(FloatAlmostEqual(a, (float)t.rc(1, 0)));
        Assert.True(FloatAlmostEqual(b, (float)t.rc(1, 1)));
        Assert.True(FloatAlmostEqual(c, (float)t.rc(1, 2)));
        Assert.True(FloatAlmostEqual(d, (float)t.rc(1, 3)));
    }

    private static void EXPECT_ROW2_EQ(float a, float b, float c, float d, in Transform t)
    {
        Assert.True(FloatAlmostEqual(a, (float)t.rc(2, 0)));
        Assert.True(FloatAlmostEqual(b, (float)t.rc(2, 1)));
        Assert.True(FloatAlmostEqual(c, (float)t.rc(2, 2)));
        Assert.True(FloatAlmostEqual(d, (float)t.rc(2, 3)));
    }

    private static void EXPECT_ROW3_EQ(float a, float b, float c, float d, in Transform t)
    {
        Assert.True(FloatAlmostEqual(a, (float)t.rc(3, 0)));
        Assert.True(FloatAlmostEqual(b, (float)t.rc(3, 1)));
        Assert.True(FloatAlmostEqual(c, (float)t.rc(3, 2)));
        Assert.True(FloatAlmostEqual(d, (float)t.rc(3, 3)));
    }

    // Checking float values for equality close to zero is not robust using
    // EXPECT_FLOAT_EQ (see gtest documentation). So, to verify rotation matrices,
    // we must use a looser absolute error threshold in some places.
    private static void EXPECT_ROW0_NEAR(double a, double b, double c, double d, Transform transform, double errorThreshold)
    {
        Assert.Equal(a, transform.rc(0, 0), errorThreshold);
        Assert.Equal(b, transform.rc(0, 1), errorThreshold);
        Assert.Equal(c, transform.rc(0, 2), errorThreshold);
        Assert.Equal(d, transform.rc(0, 3), errorThreshold);
    }

    private static void EXPECT_ROW1_NEAR(double a, double b, double c, double d, Transform transform, double errorThreshold)
    {
        Assert.Equal(a, transform.rc(1, 0), errorThreshold);
        Assert.Equal(b, transform.rc(1, 1), errorThreshold);
        Assert.Equal(c, transform.rc(1, 2), errorThreshold);
        Assert.Equal(d, transform.rc(1, 3), errorThreshold);
    }

    private static void EXPECT_ROW2_NEAR(double a, double b, double c, double d, Transform transform, double errorThreshold)
    {
        Assert.Equal(a, transform.rc(2, 0), errorThreshold);
        Assert.Equal(b, transform.rc(2, 1), errorThreshold);
        Assert.Equal(c, transform.rc(2, 2), errorThreshold);
        Assert.Equal(d, transform.rc(2, 3), errorThreshold);
    }

    private static bool PointsAreNearlyEqual(in PointF lhs, in PointF rhs)
    {
        return lhs.IsWithinDistance(rhs, 0.01f);
    }

    private static bool PointsAreNearlyEqual(in Point3F lhs, in Point3F rhs)
    {
        return lhs.SquaredDistanceTo(rhs) < 0.0001f;
    }

    private static bool MatricesAreNearlyEqual(in Transform lhs, in Transform rhs)
    {
        float epsilon = 0.0001f;
        for (int row = 0; row < 4; ++row)
        {
            for (int col = 0; col < 4; ++col)
            {
                if (Math.Abs(lhs.rc(row, col) - rhs.rc(row, col)) > epsilon)
                    return false;
            }
        }
        return true;
    }

    private static Transform GetTestMatrix1()
    {
        Transform transform = Transform.ColMajor(10.0, 11.0, 12.0, 13.0,
                                                 14.0, 15.0, 16.0, 17.0,
                                                 18.0, 19.0, 20.0, 21.0,
                                                 22.0, 23.0, 24.0, 25.0);

        STATIC_ROW0_EQ(10.0, 14.0, 18.0, 22.0, transform);
        STATIC_ROW1_EQ(11.0, 15.0, 19.0, 23.0, transform);
        STATIC_ROW2_EQ(12.0, 16.0, 20.0, 24.0, transform);
        STATIC_ROW3_EQ(13.0, 17.0, 21.0, 25.0, transform);

        EXPECT_ROW0_EQ(10.0f, 14.0f, 18.0f, 22.0f, transform);
        EXPECT_ROW1_EQ(11.0f, 15.0f, 19.0f, 23.0f, transform);
        EXPECT_ROW2_EQ(12.0f, 16.0f, 20.0f, 24.0f, transform);
        EXPECT_ROW3_EQ(13.0f, 17.0f, 21.0f, 25.0f, transform);
        return transform;
    }

    private static Transform GetTestMatrix2()
    {
        Transform transform =
              Transform.RowMajor(30.0, 34.0, 38.0, 42.0, 31.0, 35.0, 39.0, 43.0, 32.0,
                                  36.0, 40.0, 44.0, 33.0, 37.0, 41.0, 45.0);

        STATIC_ROW0_EQ(30.0, 34.0, 38.0, 42.0, transform);
        STATIC_ROW1_EQ(31.0, 35.0, 39.0, 43.0, transform);
        STATIC_ROW2_EQ(32.0, 36.0, 40.0, 44.0, transform);
        STATIC_ROW3_EQ(33.0, 37.0, 41.0, 45.0, transform);

        EXPECT_ROW0_EQ(30.0f, 34.0f, 38.0f, 42.0f, transform);
        EXPECT_ROW1_EQ(31.0f, 35.0f, 39.0f, 43.0f, transform);
        EXPECT_ROW2_EQ(32.0f, 36.0f, 40.0f, 44.0f, transform);
        EXPECT_ROW3_EQ(33.0f, 37.0f, 41.0f, 45.0f, transform);
        return transform;
    }

    private static Transform ApproxIdentityMatrix(double error)
    {
        return Transform.ColMajor(1.0 - error, error, error, error,  // col0
                                  error, 1.0 - error, error, error,  // col1
                                  error, error, 1.0 - error, error,  // col2
                                  error, error, error, 1.0 - error); // col3
    }

    private static double kErrorThreshold = 1e-7;

    // This test is to make it easier to understand the order of operations.
    [Fact]
    private static void TestPrePostOperations()
    {
        var m1 = Transform.Affine(1, 2, 3, 4, 5, 6);
        var m2 = m1;
        m1.Translate(10, 20);
        m2.PreConcat(Transform.MakeTranslation(10, 20));
        Assert.Equal(m1, m2);

        m1.PostTranslate(11, 22);
        m2.PostConcat(Transform.MakeTranslation(11, 22));
        Assert.Equal(m1, m2);

        m1.Scale(3, 4);
        m2.PreConcat(Transform.MakeScale(3, 4));
        Assert.Equal(m1, m2);

        m1.PostScale(5, 6);
        m2.PostConcat(Transform.MakeScale(5, 6));
        Assert.Equal(m1, m2);
    }

    // This test mostly overlaps with other tests, but similar to the above test,
    // this test may help understand how accumulated transforms are equivalent to
    // multiple mapping operations e.g. MapPoint().
    [Fact]
    private static void TestBasicOperations()
    {
        // Just some arbitrary matrix that introduces no rounding, and is unlikely
        // to commute with other operations.
        var m = Transform.ColMajor(2.0f, 3.0f, 5.0f, 0.0f, 7.0f, 11.0f, 13.0f, 0.0f, 17.0f,
                               19.0f, 23.0f, 0.0f, 29.0f, 31.0f, 37.0f, 1.0f);

        Point3F p = new(41.0f, 43.0f, 47.0f);

        Assert.Equal(new Point3F(1211.0f, 1520.0f, 1882.0f), m.MapPoint(p));

        {
            Transform n = new();
            n.Scale(2.0f);
            Assert.Equal(new Point3F(82.0f, 86.0f, 47.0f), n.MapPoint(p));

            Transform mn = m;
            mn.Scale(2.0f);
            Assert.Equal(mn.MapPoint(p), m.MapPoint(n.MapPoint(p)));
        }

        {
            Transform n = new();
            n.Scale(2.0f, 3.0f);
            Assert.Equal(new Point3F(82.0f, 129.0f, 47.0f), n.MapPoint(p));

            Transform mn = m;
            mn.Scale(2.0f, 3.0f);
            Assert.Equal(mn.MapPoint(p), m.MapPoint(n.MapPoint(p)));
        }

        {
            Transform n = new();
            n.Scale3D(2.0f, 3.0f, 4.0f);
            Assert.Equal(new Point3F(82.0f, 129.0f, 188.0f), n.MapPoint(p));

            Transform mn = m;
            mn.Scale3D(2.0f, 3.0f, 4.0f);
            Assert.Equal(mn.MapPoint(p), m.MapPoint(n.MapPoint(p)));
        }

        {
            Transform n = new();
            n.Rotate(90.0f);
            Assert.True(FloatAlmostEqual(0.0f, (new Point3F(-43.0f, 41.0f, 47.0f) - n.MapPoint(p)).Length()));

            Transform mn = m;
            mn.Rotate(90.0f);
            Assert.True(FloatAlmostEqual(0.0f, (mn.MapPoint(p) - m.MapPoint(n.MapPoint(p))).Length()));
        }

        {
            Transform n = new();
            n.RotateAbout(10.0f, 10.0f, 10.0f, 120.0f);
            Assert.True(FloatAlmostEqual(0.0f, (new Point3F(47.0f, 41.0f, 43.0f) - n.MapPoint(p)).Length()));

            Transform mn = m;
            mn.RotateAbout(10.0f, 10.0f, 10.0f, 120.0f);
            Assert.True(FloatAlmostEqual(0.0f, (mn.MapPoint(p) - m.MapPoint(n.MapPoint(p))).Length()));
        }

        {
            Transform n = new();
            n.Translate(5.0f, 6.0f);
            Assert.Equal(new Point3F(46.0f, 49.0f, 47.0f), n.MapPoint(p));

            Transform mn = m;
            mn.Translate(5.0f, 6.0f);
            Assert.Equal(mn.MapPoint(p), m.MapPoint(n.MapPoint(p)));
        }

        {
            Transform n = new();
            n.Translate3D(5.0f, 6.0f, 7.0f);
            Assert.Equal(new Point3F(46.0f, 49.0f, 54.0f), n.MapPoint(p));

            Transform mn = m;
            mn.Translate3D(5.0f, 6.0f, 7.0f);
            Assert.Equal(mn.MapPoint(p), m.MapPoint(n.MapPoint(p)));
        }

        {
            Transform nm = m;
            nm.PostTranslate(5.0f, 6.0f);
            Assert.Equal(nm.MapPoint(p), m.MapPoint(p) + new Vector3DF(5.0f, 6.0f, 0.0f));
        }

        {
            Transform nm = m;
            nm.PostTranslate3D(5.0f, 6.0f, 7.0f);
            Assert.Equal(nm.MapPoint(p), m.MapPoint(p) + new Vector3DF(5.0f, 6.0f, 7.0f));
        }

        {
            Transform n = new();
            n.Skew(45.0f, -45.0f);
            Assert.True(FloatAlmostEqual(0.0f, (new Point3F(84.0f, 2.0f, 47.0f) - n.MapPoint(p)).Length()));

            Transform mn = m;
            mn.Skew(45.0f, -45.0f);
            Assert.True(FloatAlmostEqual(0.0f, (mn.MapPoint(p) - m.MapPoint(n.MapPoint(p))).Length()));
        }

        {
            Transform n = new();
            n.SkewX(45.0f);
            Assert.True(FloatAlmostEqual(0.0f, (new Point3F(84.0f, 43.0f, 47.0f) - n.MapPoint(p)).Length()));

            Transform mn = m;
            mn.SkewX(45.0f);
            Assert.True(FloatAlmostEqual(0.0f, (mn.MapPoint(p) - m.MapPoint(n.MapPoint(p))).Length()));
        }

        {
            Transform n = new();
            n.SkewY(45.0f);
            Assert.True(FloatAlmostEqual(0.0f, (new Point3F(41.0f, 84.0f, 47.0f) - n.MapPoint(p)).Length()));

            Transform mn = m;
            mn.SkewY(45.0f);
            Assert.True(FloatAlmostEqual(0.0f, (mn.MapPoint(p) - m.MapPoint(n.MapPoint(p))).Length()));
        }

        {
            Transform n = new();
            n.ApplyPerspectiveDepth(94.0f);
            Assert.True(FloatAlmostEqual(0.0f, (new Point3F(82.0f, 86.0f, 94.0f) - n.MapPoint(p)).Length()));

            Transform mn = m;
            mn.ApplyPerspectiveDepth(94.0f);
            Assert.True(FloatAlmostEqual(0.0f, (mn.MapPoint(p) - m.MapPoint(n.MapPoint(p))).Length()));
        }

        {
            Transform n = m;
            n.Zoom(2.0f);
            Point3F expectation = p;
            expectation.Scale(0.5f, 0.5f, 0.5f);
            expectation = m.MapPoint(expectation);
            expectation.Scale(2.0f, 2.0f, 2.0f);
            Assert.Equal(expectation, n.MapPoint(p));
        }
    }

    [Fact]
    private static void TestEquality()
    {
        Transform lhs = new();
        Transform interpolated;
        var rhs = GetTestMatrix1();
        interpolated = lhs;
        for (int i = 0; i <= 100; ++i)
        {
            for (int row = 0; row < 4; ++row)
            {
                for (int col = 0; col < 4; ++col)
                {
                    float a = (float)lhs.rc(row, col);
                    float b = (float)rhs.rc(row, col);
                    float t = i / 100.0f;
                    interpolated.set_rc(row, col, a + (b - a) * t);
                }
            }
            if (i == 100)
            {
                Assert.True(rhs == interpolated);
            }
            else
            {
                Assert.True(rhs != interpolated);
            }
        }
        lhs = new Transform();
        rhs = new Transform();
        for (int i = 1; i < 100; ++i)
        {
            lhs.MakeIdentity();
            rhs.MakeIdentity();
            lhs.Translate(i, i);
            rhs.Translate(-i, -i);
            Assert.True(lhs != rhs);
            rhs.Translate(2 * i, 2 * i);
            Assert.True(lhs == rhs);
        }
    }

    [Fact]
    private static void TestConcatTranslate()
    {
        (int x1, int y1, float tx, float ty, int x2, int y2)[] test_cases =
        [
            (0, 0, 10.0f, 20.0f, 10, 20),
            (0, 0, -10.0f, -20.0f, 0, 0),
            (0, 0, -10.0f, -20.0f, -10, -20),
            (0, 0, float.NaN, float.NaN, 10, 20),
        ];

        Transform xform = new();

        foreach (var (x1, y1, tx, ty, x2, y2) in test_cases)
        {
            Transform translation = new();
            translation.Translate(tx, ty);
            xform = translation * xform;
            Point3F p1 = xform.MapPoint(new Point3F(x1, y1, 0));
            Point3F p2 = new(x2, y2, 0);
            //if (tx == tx && ty == ty)
            if (!float.IsNaN(tx) && !float.IsNaN(ty))
            {
                Assert.True(PointsAreNearlyEqual(p1, p2));
            }
        }
    }

    [Fact]
    private static void TestConcatScale()
    {

        (int before, float scale, int after)[] test_cases =
        [
            (1, 10.0f, 10),
            (1, .1f, 1),
            (1, 100.0f, 100),
            (1, -1.0f, -100),
            (1, float.NaN, 1)
        ];

        Transform xform = new();

        foreach (var (before, scale, after) in test_cases)
        {
            Transform scale_transform = new();
            scale_transform.Scale(scale, scale);
            xform = scale_transform * xform;
            Point3F p1 = xform.MapPoint(new Point3F(before, before, 0));
            Point3F p2 = new(after, after, 0);
            //if (scale == scale)
            if (!float.IsNaN(scale))
            {
                Assert.True(PointsAreNearlyEqual(p1, p2));
            }
        }
    }

    [Fact]
    private static void TestConcatRotate()
    {
        (int x1, int y1, float degrees, int x2, int y2)[] test_cases =
        [
            (1, 0, 90.0f, 0, 1),
            (1, 0, -90.0f, 1, 0),
            (1, 0, 90.0f, 0, 1),
            (1, 0, 360.0f, 0, 1),
            (1, 0, 0.0f, 0, 1),
            (1, 0, float.NaN, 1, 0)
        ];

        Transform xform = new();
        foreach (var (x1, y1, degrees, x2, y2) in test_cases)
        {
            Transform rotation = new();
            rotation.Rotate(degrees);
            xform = rotation * xform;
            Point3F p1 = xform.MapPoint(new Point3F(x1, y1, 0));
            Point3F p2 = new(x2, y2, 0);
            //if (degrees == degrees)
            if (!float.IsNaN(degrees))
            {
                AssertPoint3FNear(p1, p2, 0.0001f);
            }
        }
    }

    [Fact]
    private static void TestConcatSelf()
    {
        var a = Transform.ColMajor(2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15,
                               16, 17);
        var expected_a_times_a =
            Transform.ColMajor(132, 146, 160, 174, 260, 290, 320, 350, 388, 434, 480,
                                526, 516, 578, 640, 702);
        a.PreConcat(a);
        Assert.Equal(expected_a_times_a, a);

        a = Transform.Affine(2, 3, 4, 5, 6, 7);
        expected_a_times_a = Transform.Affine(16, 21, 28, 37, 46, 60);
        a.PreConcat(a);
        Assert.True(a.Is2dTransform());
        Assert.Equal(expected_a_times_a, a);
    }

    [Fact]
    private static void TestTranslate()
    {
        (int x1, int y1, float tx, float ty, int x2, int y2)[] test_cases =
        [
            (0, 0, 10.0f, 20.0f, 10, 20),
            (10, 20, 10.0f, 20.0f, 20, 40),
            (10, 20, 0.0f, 0.0f, 10, 20),
            (0, 0, float.NaN,
            float.NaN, 0, 0)
        ];

        foreach (var (x1, y1, tx, ty, x2, y2) in test_cases)
        {
            for (int k = 0; k < 3; ++k)
            {
                Point3F p0;
                Point3F p1 = new();
                Point3F p2 = new();
                Transform xform = new();
                switch (k)
                {
                    case 0:
                        p1.SetPoint(x1, 0, 0);
                        p2.SetPoint(x2, 0, 0);
                        xform.Translate(tx, 0.0f);
                        break;
                    case 1:
                        p1.SetPoint(0, y1, 0);
                        p2.SetPoint(0, y2, 0);
                        xform.Translate(0.0f, ty);
                        break;
                    case 2:
                        p1.SetPoint(x1, y1, 0);
                        p2.SetPoint(x2, y2, 0);
                        xform.Translate(tx, ty);
                        break;
                }
                p0 = p1;
                p1 = xform.MapPoint(p1);
                //if (tx == tx && ty == ty)
                if (!float.IsNaN(tx) && !float.IsNaN(ty))
                {
                    Assert.True(PointsAreNearlyEqual(p1, p2));
                    Point3F? transformed_p1 = xform.InverseMapPoint(p1);
                    Assert.NotNull(transformed_p1);
                    Assert.True(PointsAreNearlyEqual(transformed_p1.Value, p0));
                }
            }
        }
    }

    [Fact]
    private static void TestScale()
    {
        (int before, float s, int after)[] test_cases =
        [
            (1, 10.0f, 10),
            (1, 1.0f, 1),
            (1, 0.0f, 0),
            (0, 10.0f, 0),
            (1, float.NaN, 0)
        ];

        foreach (var (before, s, after) in test_cases)
        {
            for (int k = 0; k < 3; ++k)
            {
                Point3F p0;
                Point3F p1 = new();
                Point3F p2 = new();
                Transform xform = new();

                switch (k)
                {
                    case 0:
                        p1.SetPoint(before, 0, 0);
                        p2.SetPoint(after, 0, 0);
                        xform.Scale(s, 1.0f);
                        break;
                    case 1:
                        p1.SetPoint(0, before, 0);
                        p2.SetPoint(0, after, 0);
                        xform.Scale(1.0f, s);
                        break;
                    case 2:
                        p1.SetPoint(before, before, 0);
                        p2.SetPoint(after, after, 0);
                        xform.Scale(s, s);
                        break;
                }
                p0 = p1;
                p1 = xform.MapPoint(p1);
                if (!float.IsNaN(s))
                {
                    Assert.True(PointsAreNearlyEqual(p1, p2));
                    if (s != 0.0f)
                    {
                        Point3F? transformed_p1 = xform.InverseMapPoint(p1);
                        Assert.NotNull(transformed_p1);
                        Assert.True(PointsAreNearlyEqual(transformed_p1.Value, p0));
                    }
                }
            }
        }
    }

    [Fact]
    private static void TestSetRotate()
    {
        (int x, int y, float degree, int xprime, int yprime)[] set_rotate_cases =
        [
            (100, 0, 90.0f, 0, 100),
            (0, 0, 90.0f, 0, 0),
            (0, 100, 90.0f, -100, 0),
            (0, 1, -90.0f, 1, 0),
            (100, 0, 0.0f, 100, 0),
            (0, 0, 0.0f, 0, 0),
            (0, 0, float.NaN, 0, 0),
            (100, 0, 360.0f, 100, 0)
        ];

        foreach (var (x, y, degree, xprime, yprime) in set_rotate_cases)
        {
            Point3F p0;
            Point3F p1 = new(x, y, 0);
            Point3F p2 = new(xprime, yprime, 0);
            p0 = p1;
            Transform xform = new();
            xform.Rotate(degree);
            // just want to make sure that we don't crash in the case of NaN.
            //if (degree == degree)
            if (!float.IsNaN(degree))
            {
                p1 = xform.MapPoint(p1);
                Assert.True(PointsAreNearlyEqual(p1, p2));
                Point3F? transformed_p1 = xform.InverseMapPoint(p1);
                Assert.NotNull(transformed_p1);
                Assert.True(PointsAreNearlyEqual(transformed_p1.Value, p0));
            }
        }
    }

    [Fact]
    private static void TestConcatTranslate2D()
    {
        (int x1, int y1, float tx, float ty, int x2, int y2)[] test_cases =
        [
            (0, 0, 10.0f, 20.0f, 10, 20),
            (0, 0, -10.0f, -20.0f, 0, 0),
            (0, 0, -10.0f, -20.0f, -10, -20)
        ];

        Transform xform = new();
        foreach (var (x1, y1, tx, ty, x2, y2) in test_cases)
        {
            Transform translation = new();
            translation.Translate(tx, ty);
            xform = translation * xform;
            Point p1 = xform.MapPoint(new Point(x1, y1));
            Point p2 = new(x2, y2);
            //if (tx == tx && ty == ty)
            if (!float.IsNaN(tx) && !float.IsNaN(ty))
            {
                Assert.Equal(p1.X, p2.X);
                Assert.Equal(p1.Y, p2.Y);
            }
        }
    }

    [Fact]
    private static void TestConcatScale2D()
    {
        (int before, float scale, int after)[] test_cases =
        [
            (1, 10.0f, 10),
            (1, .1f, 1),
            (1, 100.0f, 100),
            (1, -1.0f, -100)
        ];

        Transform xform = new();
        foreach (var (before, scale, after) in test_cases)
        {
            Transform scale_transform = new();
            scale_transform.Scale(scale, scale);
            xform = scale_transform * xform;
            Point p1 = xform.MapPoint(new Point(before, before));
            Point p2 = new(after, after);
            //if (scale == scale)
            if (!float.IsNaN(scale))
            {
                Assert.Equal(p1.X, p2.X);
                Assert.Equal(p1.Y, p2.Y);
            }
        }
    }

    [Fact]
    private static void TestConcatRotate2D()
    {
        (int x1, int y1, float degrees, int x2, int y2)[] test_cases =
        [
            (1, 0, 90.0f, 0, 1),
            (1, 0, -90.0f, 1, 0),
            (1, 0, 90.0f, 0, 1),
            (1, 0, 360.0f, 0, 1),
            (1, 0, 0.0f, 0, 1)
        ];

        Transform xform = new();
        foreach (var (x1, y1, degrees, x2, y2) in test_cases)
        {
            Transform rotation = new();
            rotation.Rotate(degrees);
            xform = rotation * xform;
            Point p1 = xform.MapPoint(new Point(x1, y1));
            Point p2 = new(x2, y2);
            //if (degrees == degrees)
            if (!float.IsNaN(degrees))
            {
                Assert.Equal(p1.X, p2.X);
                Assert.Equal(p1.Y, p2.Y);
            }
        }
    }

    [Fact]
    private static void TestSetTranslate2D()
    {
        (int x1, int y1, float tx, float ty, int x2, int y2)[] test_cases =
        [
            (0, 0, 10.0f, 20.0f, 10, 20),
            (10, 20, 10.0f, 20.0f, 20, 40),
            (10, 20, 0.0f, 0.0f, 10, 20)
        ];

        foreach (var (x1, y1, tx, ty, x2, y2) in test_cases)
        {
            for (int j = -1; j < 2; ++j)
            {
                for (int k = 0; k < 3; ++k)
                {
                    float epsilon = 0.0001f;
                    Point p0;
                    Point p1 = new();
                    Point p2 = new();
                    Transform xform = new();
                    switch (k)
                    {
                        case 0:
                            p1.SetPoint(x1, 0);
                            p2.SetPoint(x2, 0);
                            xform.Translate(tx + j * epsilon, 0.0f);
                            break;
                        case 1:
                            p1.SetPoint(0, y1);
                            p2.SetPoint(0, y2);
                            xform.Translate(0.0f, ty + j * epsilon);
                            break;
                        case 2:
                            p1.SetPoint(x1, y1);
                            p2.SetPoint(x2, y2);
                            xform.Translate(tx + j * epsilon, ty + j * epsilon);
                            break;
                    }
                    p0 = p1;
                    p1 = xform.MapPoint(p1);
                    //if (tx == tx && ty == ty)
                    if (!float.IsNaN(tx) && !float.IsNaN(tx))
                    {
                        Assert.Equal(p1.X, p2.X);
                        Assert.Equal(p1.Y, p2.Y);
                        Point? transformed_p1 = xform.InverseMapPoint(p1);
                        Assert.NotNull(transformed_p1);
                        Assert.Equal(transformed_p1.Value.X, p0.X);
                        Assert.Equal(transformed_p1.Value.Y, p0.Y);
                    }
                }
            }
        }
    }

    [Fact]
    private static void TestSetScale2D()
    {
        (int before, float s, int after)[] test_cases =
        [
            (1, 10.0f, 10),
            (1, 1.0f, 1),
            (1, 0.0f, 0),
            (0, 10.0f, 0)
        ];

        foreach (var (before, s, after) in test_cases)
        {
            for (int j = -1; j < 2; ++j)
            {
                for (int k = 0; k < 3; ++k)
                {
                    float epsilon = 0.0001f;
                    Point p0;
                    Point p1 = new();
                    Point p2 = new();
                    Transform xform = new();
                    switch (k)
                    {
                        case 0:
                            p1.SetPoint(before, 0);
                            p2.SetPoint(after, 0);
                            xform.Scale(s + j * epsilon, 1.0f);
                            break;
                        case 1:
                            p1.SetPoint(0, before);
                            p2.SetPoint(0, after);
                            xform.Scale(1.0f, s + j * epsilon);
                            break;
                        case 2:
                            p1.SetPoint(before, before);
                            p2.SetPoint(after, after);
                            xform.Scale(s + j * epsilon, s + j * epsilon);
                            break;
                    }
                    p0 = p1;
                    p1 = xform.MapPoint(p1);
                    //if (s == s)
                    if (!float.IsNaN(s))
                    {
                        Assert.Equal(p1.X, p2.X);
                        Assert.Equal(p1.Y, p2.Y);
                        if (s != 0.0f)
                        {
                            Point? transformed_p1 = xform.InverseMapPoint(p1);
                            Assert.NotNull(transformed_p1);
                            Assert.Equal(transformed_p1.Value.X, p0.X);
                            Assert.Equal(transformed_p1.Value.Y, p0.Y);
                        }
                    }
                }
            }
        }
    }

    [Fact]
    private static void TestSetRotate2D()
    {
        (int x, int y, float degree, int xprime, int yprime)[] set_rotate_cases =
        [
            (100, 0, 90.0f, 0, 100),
            (0, 0, 90.0f, 0, 0),
            (0, 100, 90.0f, -100, 0),
            (0, 1, -90.0f, 1, 0),
            (100, 0, 0.0f, 100, 0),
            (0, 0, 0.0f, 0, 0),
            (0, 0, float.NaN, 0, 0),
            (100, 0, 360.0f, 100, 0)
        ];

        foreach (var (x, y, degree, xprime, yprime) in set_rotate_cases)
        {
            for (int j = 1; j >= -1; --j)
            {
                float epsilon = 0.1f;
                Point pt = new(x, y);
                Transform xform = new();
                // should be invariant to small floating point errors.
                xform.Rotate(degree + j * epsilon);
                // just want to make sure that we don't crash in the case of NaN.
                //if (degree == degree)
                if(!float.IsNaN(degree))
                {
                    pt = xform.MapPoint(pt);
                    Assert.Equal(xprime, pt.X);
                    Assert.Equal(yprime, pt.Y);
                    Point? transformed_pt = xform.InverseMapPoint(pt);
                    Assert.NotNull(transformed_pt);
                    Assert.Equal(transformed_pt.Value.X, x);
                    Assert.Equal(transformed_pt.Value.Y, y);
                }
            }
        }
    }

    [Fact]
    private static void TestMapPointWithExtremePerspective()
    {
        Point3F point = new(1.0f, 1.0f, 1.0f);
        Transform perspective = new();
        perspective.ApplyPerspectiveDepth(1.0f);
        Point3F transformed = perspective.MapPoint(point);
        Assert.Equal(point.ToString(), transformed.ToString());

        perspective.MakeIdentity();
        perspective.ApplyPerspectiveDepth(1.1f);
        transformed = perspective.MapPoint(point);
        
        Assert.True(FloatAlmostEqual(11.0f, transformed.X));
        Assert.True(FloatAlmostEqual(11.0f, transformed.Y));
        Assert.True(FloatAlmostEqual(11.0f, transformed.Z));
    }

    [Fact]
    private static void TestBlendTranslate()
    {
        Transform from = new();
        for (int i = -5; i < 15; ++i)
        {
            Transform to = new();
            to.Translate3D(1, 1, 1);
            double t = i / 9.0;
            Assert.True(to.Blend(from, t));
            Assert.True(FloatAlmostEqual((float)t, (float)to.rc(0, 3)));
            Assert.True(FloatAlmostEqual((float)t, (float)to.rc(1, 3)));
            Assert.True(FloatAlmostEqual((float)t, (float)to.rc(2, 3)));
        }
    }

    [Fact]
    private static void TestBlendRotate()
    {
        Vector3DF[] axes =
        [
            new(1, 0, 0),
            new(0, 1, 0),
            new(0, 0, 1),
            new(1, 1, 1)
        ];
        
        Transform from = new();
        
        foreach (var axis in axes)
        {
            for (int i = -5; i < 15; ++i)
            {
                Transform to = new();
                to.RotateAbout(axis, 90);
                double t = i / 9.0;
                Assert.True(to.Blend(from, t));

                Transform expected = new();
                expected.RotateAbout(axis, 90 * t);

                Assert.True(MatricesAreNearlyEqual(expected, to));
            }
        }
    }

    private static void Test7()
    {
    }

    private static void Test8()
    {
    }

    private static void Test9()
    {
    }

    private static void Test10()
    {
    }
}
