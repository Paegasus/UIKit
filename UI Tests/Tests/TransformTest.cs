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

    [Fact]
    private static void TestCanBlend180DegreeRotation()
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
                to.RotateAbout(axis, 180.0);
                double t = i / 9.0;
                Assert.True(to.Blend(from, t));

                // A 180 degree rotation is exactly opposite on the sphere, therefore
                // either great circle arc to it is equivalent (and numerical precision
                // will determine which is closer).  Test both directions.
                Transform expected1 = new();
                expected1.RotateAbout(axis, 180.0 * t);
                Transform expected2 = new();
                expected2.RotateAbout(axis, -180.0 * t);

                Assert.True(MatricesAreNearlyEqual(expected1, to) || MatricesAreNearlyEqual(expected2, to));
            }
        }
    }

    [Fact]
    private static void TestBlendScale()
    {
        Transform from = new();
        for (int i = -5; i < 15; ++i)
        {
            Transform to = new();
            to.Scale3D(5, 4, 3);
            double s1 = i / 9.0;
            double s2 = 1 - s1;
            Assert.True(to.Blend(from, s1));
            
            FloatAlmostEqual((float)(5 * s1 + s2), (float)to.rc(0, 0));
            FloatAlmostEqual((float)(4 * s1 + s2), (float)to.rc(1, 1));
            FloatAlmostEqual((float)(3 * s1 + s2), (float)to.rc(2, 2));
        }
    }

    [Fact]
    private static void TestBlendSkew()
    {
        Transform from = new();
        for (int i = 0; i < 2; ++i)
        {
            Transform to = new();
            to.Skew(10, 5);
            double t = i;
            Transform expected = new();
            expected.Skew(t * 10, t * 5);
            Assert.True(to.Blend(from, t));
            Assert.True(MatricesAreNearlyEqual(expected, to));
        }
    }

    [Fact]
    private static void TestExtrapolateSkew()
    {
        Transform from = new();
        for (int i = -1; i < 2; ++i)
        {
            Transform to = new();
            to.Skew(20, 0);
            double t = i;
            Transform expected = new();
            expected.Skew(t * 20, t * 0);
            Assert.True(to.Blend(from, t));
            Assert.True(MatricesAreNearlyEqual(expected, to));
        }
    }

    [Fact]
    private static void TestBlendPerspective()
    {
        Transform from = new();
        from.ApplyPerspectiveDepth(200);
        for (int i = -1; i < 3; ++i)
        {
            Transform to = new();
            to.ApplyPerspectiveDepth(800);
            double t = i;
            double depth = 1.0 / ((1.0 / 200) * (1.0 - t) + (1.0 / 800) * t);
            Transform expected = new();
            expected.ApplyPerspectiveDepth(depth);
            Assert.True(to.Blend(from, t));
            Assert.True(MatricesAreNearlyEqual(expected, to));
        }
    }

    [Fact]
    private static void TestBlendIdentity()
    {
        Transform from = new();
        Transform to = new();
        Assert.True(to.Blend(from, 0.5));
        Assert.Equal(to, from);
    }

    [Fact]
    private static void TestCannotBlendSingularMatrix()
    {
        Transform from = new();
        Transform to = new();
        to.set_rc(1, 1, 0);
        Transform original_to = to;
        Assert.False(to.Blend(from, 0.25));
        Assert.Equal(original_to, to);
        Assert.False(to.Blend(from, 0.75));
        Assert.Equal(original_to, to);
    }

    [Fact]
    private static void TestVerifyBlendForTranslation()
    {
        Transform from = new();
        from.Translate3D(100.0f, 200.0f, 100.0f);

        Transform to = new();

        to.Translate3D(200.0f, 100.0f, 300.0f);
        to.Blend(from, 0.0);
        Assert.Equal(from, to);

        to = new Transform();
        to.Translate3D(200.0f, 100.0f, 300.0f);
        to.Blend(from, 0.25);
        EXPECT_ROW0_EQ(1.0f, 0.0f, 0.0f, 125.0f, to);
        EXPECT_ROW1_EQ(0.0f, 1.0f, 0.0f, 175.0f, to);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 1.0f, 150.0f, to);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, to);

        to = new Transform();
        to.Translate3D(200.0f, 100.0f, 300.0f);
        to.Blend(from, 0.5);
        EXPECT_ROW0_EQ(1.0f, 0.0f, 0.0f, 150.0f, to);
        EXPECT_ROW1_EQ(0.0f, 1.0f, 0.0f, 150.0f, to);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 1.0f, 200.0f, to);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, to);

        to = new Transform();
        to.Translate3D(200.0f, 100.0f, 300.0f);
        to.Blend(from, 1.0);
        EXPECT_ROW0_EQ(1.0f, 0.0f, 0.0f, 200.0f, to);
        EXPECT_ROW1_EQ(0.0f, 1.0f, 0.0f, 100.0f, to);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 1.0f, 300.0f, to);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, to);
    }

    [Fact]
    private static void TestVerifyBlendForScale()
    {
        Transform from = new();
        from.Scale3D(100.0f, 200.0f, 100.0f);

        Transform to = new();

        to.Scale3D(200.0f, 100.0f, 300.0f);
        to.Blend(from, 0.0);
        Assert.Equal(from, to);

        to = new Transform();
        to.Scale3D(200.0f, 100.0f, 300.0f);
        to.Blend(from, 0.25);
        EXPECT_ROW0_EQ(125.0f, 0.0f, 0.0f, 0.0f, to);
        EXPECT_ROW1_EQ(0.0f, 175.0f, 0.0f, 0.0f, to);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 150.0f, 0.0f, to);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, to);

        to = new Transform();
        to.Scale3D(200.0f, 100.0f, 300.0f);
        to.Blend(from, 0.5);
        EXPECT_ROW0_EQ(150.0f, 0.0f, 0.0f, 0.0f, to);
        EXPECT_ROW1_EQ(0.0f, 150.0f, 0.0f, 0.0f, to);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 200.0f, 0.0f, to);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, to);

        to = new Transform();
        to.Scale3D(200.0f, 100.0f, 300.0f);
        to.Blend(from, 1.0);
        EXPECT_ROW0_EQ(200.0f, 0.0f, 0.0f, 0.0f, to);
        EXPECT_ROW1_EQ(0.0f, 100.0f, 0.0f, 0.0f, to);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 300.0f, 0.0f, to);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, to);
    }

    [Fact]
    private static void TestVerifyBlendForSkew()
    {
        // Along X axis only
        Transform from = new();
        from.Skew(0.0, 0.0);

        Transform to = new();

        to.Skew(45.0, 0.0);
        to.Blend(from, 0.0);
        Assert.Equal(from, to);

        to = new Transform();
        to.Skew(45.0, 0.0);
        to.Blend(from, 0.5);
        EXPECT_ROW0_EQ(1.0f, 0.5f, 0.0f, 0.0f, to);
        EXPECT_ROW1_EQ(0.0f, 1.0f, 0.0f, 0.0f, to);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 1.0f, 0.0f, to);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, to);

        to = new Transform();
        to.Skew(45.0, 0.0);
        to.Blend(from, 0.25);
        EXPECT_ROW0_EQ(1.0f, 0.25f, 0.0f, 0.0f, to);
        EXPECT_ROW1_EQ(0.0f, 1.0f, 0.0f, 0.0f, to);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 1.0f, 0.0f, to);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, to);

        to = new Transform();
        to.Skew(45.0, 0.0);
        to.Blend(from, 1.0);
        EXPECT_ROW0_EQ(1.0f, 1.0f, 0.0f, 0.0f, to);
        EXPECT_ROW1_EQ(0.0f, 1.0f, 0.0f, 0.0f, to);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 1.0f, 0.0f, to);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, to);

        // NOTE CAREFULLY: Decomposition of skew and rotation terms of the matrix
        // is inherently underconstrained, and so it does not always compute the
        // originally intended skew parameters. The current implementation uses QR
        // decomposition, which decomposes the shear into a rotation + non-uniform scale.
        //
        // It is unlikely that the decomposition implementation will need to change
        // very often, so to get any test coverage, the compromise is to verify the
        // exact matrix that the.Blend() operation produces.
        //
        // This problem also potentially exists for skew along the X axis, but the
        // current QR decomposition implementation just happens to decompose those
        // test matrices intuitively.
        //
        // Unfortunately, this case suffers from uncomfortably large precision error.

        from = new Transform();
        from.Skew(0.0, 0.0);

        to = new Transform();

        to.Skew(0.0, 45.0);
        to.Blend(from, 0.0);
        Assert.Equal(from, to);

        to = new Transform();
        to.Skew(0.0, 45.0);
        to.Blend(from, 0.25);
        Assert.True(1.0 < to.rc(0, 0));
        Assert.True(1.5 > to.rc(0, 0));
        Assert.True(0.0 < to.rc(0, 1));
        Assert.True(0.5 > to.rc(0, 1));
        Assert.True(FloatAlmostEqual(0.0f, (float) to.rc(0, 2)));
        Assert.True(FloatAlmostEqual(0.0f, (float) to.rc(0, 3)));

        Assert.True(0.0 < to.rc(1, 0));
        Assert.True(0.5 > to.rc(1, 0));
        Assert.True(0.0 < to.rc(1, 1));
        Assert.True(1.0 > to.rc(1, 1));
        Assert.True(FloatAlmostEqual(0.0f, (float) to.rc(1, 2)));
        Assert.True(FloatAlmostEqual(0.0f, (float) to.rc(1, 3)));

        EXPECT_ROW2_EQ(0.0f, 0.0f, 1.0f, 0.0f, to);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, to);

        to = new Transform();
        to.Skew(0.0, 45.0);
        to.Blend(from, 0.5);

        Assert.True(1.0 < to.rc(0, 0));
        Assert.True(1.5 > to.rc(0, 0));
        Assert.True(0.0 < to.rc(0, 1));
        Assert.True(0.5 > to.rc(0, 1));
        Assert.True(FloatAlmostEqual(0.0f, (float) to.rc(0, 2)));
        Assert.True(FloatAlmostEqual(0.0f, (float) to.rc(0, 3)));

        Assert.True(0.0 < to.rc(1, 0));
        Assert.True(1.0 > to.rc(1, 0));
        Assert.True(0.0 < to.rc(1, 1));
        Assert.True(1.0 > to.rc(1, 1));
        Assert.True(FloatAlmostEqual(0.0f, (float) to.rc(1, 2)));
        Assert.True(FloatAlmostEqual(0.0f, (float) to.rc(1, 3)));

        EXPECT_ROW2_EQ(0.0f, 0.0f, 1.0f, 0.0f, to);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, to);

        to = new Transform();
        to.Skew(0.0, 45.0);
        to.Blend(from, 1.0);
        EXPECT_ROW0_NEAR(1.0, 0.0, 0.0, 0.0, to, kErrorThreshold);
        EXPECT_ROW1_NEAR(1.0, 1.0, 0.0, 0.0, to, kErrorThreshold);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 1.0f, 0.0f, to);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, to);
    }

    [Fact]
    private static void TestBlendForRotationAboutX()
    {
        // Even though Blending uses quaternions,
        // axis-aligned rotations should Blend the same with quaternions or Euler angles.
        // So we can test rotation Blending by comparing against manually specified matrices from Euler angles.

        Transform from = new();
        from.RotateAbout(new Vector3DF(1.0f, 0.0f, 0.0f), 0.0);

        Transform to = new();

        to.RotateAbout(new Vector3DF(1.0f, 0.0f, 0.0f), 90.0);
        to.Blend(from, 0.0);
        Assert.Equal(from, to);

        double expectedRotationAngle = double.DegreesToRadians(22.5);
        to = new Transform();
        to.RotateAbout(new Vector3DF(1.0f, 0.0f, 0.0f), 90.0);
        to.Blend(from, 0.25);
        EXPECT_ROW0_EQ(1.0f, 0.0f, 0.0f, 0.0f, to);
        EXPECT_ROW1_NEAR(0.0, Math.Cos(expectedRotationAngle),
                         -Math.Sin(expectedRotationAngle), 0.0, to, kErrorThreshold);
        EXPECT_ROW2_NEAR(0.0, Math.Sin(expectedRotationAngle),
                         Math.Cos(expectedRotationAngle), 0.0, to, kErrorThreshold);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, to);

        expectedRotationAngle = double.DegreesToRadians(45.0);
        to = new Transform();
        to.RotateAbout(new Vector3DF(1.0f, 0.0f, 0.0f), 90.0);
        to.Blend(from, 0.5);
        EXPECT_ROW0_EQ(1.0f, 0.0f, 0.0f, 0.0f, to);
        EXPECT_ROW1_NEAR(0.0, Math.Cos(expectedRotationAngle),
                         -Math.Sin(expectedRotationAngle), 0.0, to, kErrorThreshold);
        EXPECT_ROW2_NEAR(0.0, Math.Sin(expectedRotationAngle),
                         Math.Cos(expectedRotationAngle), 0.0, to, kErrorThreshold);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, to);

        to = new Transform();
        to.RotateAbout(new Vector3DF(1.0f, 0.0f, 0.0f), 90.0);
        to.Blend(from, 1.0);
        EXPECT_ROW0_EQ(1.0f, 0.0f, 0.0f, 0.0f, to);
        EXPECT_ROW1_NEAR(0.0, 0.0, -1.0, 0.0, to, kErrorThreshold);
        EXPECT_ROW2_NEAR(0.0, 1.0, 0.0, 0.0, to, kErrorThreshold);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, to);
    }

    [Fact]
    private static void TestBlendForRotationAboutY()
    {
        Transform from = new();
        from.RotateAbout(new Vector3DF(0.0f, 1.0f, 0.0f), 0.0);

        Transform to = new();

        to.RotateAbout(new Vector3DF(0.0f, 1.0f, 0.0f), 90.0);
        to.Blend(from, 0.0);
        Assert.Equal(from, to);

        double expectedRotationAngle = double.DegreesToRadians(22.5);
        to = new Transform();
        to.RotateAbout(new Vector3DF(0.0f, 1.0f, 0.0f), 90.0);
        to.Blend(from, 0.25);
        EXPECT_ROW0_NEAR(Math.Cos(expectedRotationAngle), 0.0,
                         Math.Sin(expectedRotationAngle), 0.0, to, kErrorThreshold);
        EXPECT_ROW1_EQ(0.0f, 1.0f, 0.0f, 0.0f, to);
        EXPECT_ROW2_NEAR(-Math.Sin(expectedRotationAngle), 0.0,
                         Math.Cos(expectedRotationAngle), 0.0, to, kErrorThreshold);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, to);

        expectedRotationAngle = double.DegreesToRadians(45.0);
        to = new Transform();
        to.RotateAbout(new Vector3DF(0.0f, 1.0f, 0.0f), 90.0);
        to.Blend(from, 0.5);
        EXPECT_ROW0_NEAR(Math.Cos(expectedRotationAngle), 0.0,
                         Math.Sin(expectedRotationAngle), 0.0, to, kErrorThreshold);
        EXPECT_ROW1_EQ(0.0f, 1.0f, 0.0f, 0.0f, to);
        EXPECT_ROW2_NEAR(-Math.Sin(expectedRotationAngle), 0.0,
                         Math.Cos(expectedRotationAngle), 0.0, to, kErrorThreshold);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, to);

        to = new Transform();
        to.RotateAbout(new Vector3DF(0.0f, 1.0f, 0.0f), 90.0);
        to.Blend(from, 1.0);
        EXPECT_ROW0_NEAR(0.0, 0.0, 1.0, 0.0, to, kErrorThreshold);
        EXPECT_ROW1_EQ(0.0f, 1.0f, 0.0f, 0.0f, to);
        EXPECT_ROW2_NEAR(-1.0, 0.0, 0.0, 0.0, to, kErrorThreshold);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, to);
    }

    [Fact]
    private static void TestBlendForRotationAboutZ()
    {
        Transform from = new();
        from.RotateAbout(new Vector3DF(0.0f, 0.0f, 1.0f), 0.0);

        Transform to = new();

        to.RotateAbout(new Vector3DF(0.0f, 0.0f, 1.0f), 90.0);
        to.Blend(from, 0.0);
        Assert.Equal(from, to);

        double expectedRotationAngle = double.DegreesToRadians(22.5);
        to = new Transform();
        to.RotateAbout(new Vector3DF(0.0f, 0.0f, 1.0f), 90.0);
        to.Blend(from, 0.25);
        EXPECT_ROW0_NEAR(Math.Cos(expectedRotationAngle),
                         -Math.Sin(expectedRotationAngle), 0.0, 0.0, to,
                         kErrorThreshold);
        EXPECT_ROW1_NEAR(Math.Sin(expectedRotationAngle),
                         Math.Cos(expectedRotationAngle), 0.0, 0.0, to,
                         kErrorThreshold);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 1.0f, 0.0f, to);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, to);

        expectedRotationAngle = double.DegreesToRadians(45.0);
        to = new Transform();
        to.RotateAbout(new Vector3DF(0.0f, 0.0f, 1.0f), 90.0);
        to.Blend(from, 0.5);
        EXPECT_ROW0_NEAR(Math.Cos(expectedRotationAngle),
                         -Math.Sin(expectedRotationAngle), 0.0, 0.0, to,
                         kErrorThreshold);
        EXPECT_ROW1_NEAR(Math.Sin(expectedRotationAngle),
                         Math.Cos(expectedRotationAngle), 0.0, 0.0, to,
                         kErrorThreshold);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 1.0f, 0.0f, to);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, to);

        to = new Transform();
        to.RotateAbout(new Vector3DF(0.0f, 0.0f, 1.0f), 90.0);
        to.Blend(from, 1.0);
        EXPECT_ROW0_NEAR(0.0, -1.0, 0.0, 0.0, to, kErrorThreshold);
        EXPECT_ROW1_NEAR(1.0, 0.0, 0.0, 0.0, to, kErrorThreshold);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 1.0f, 0.0f, to);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, to);
    }

    [Fact]
    private static void TestBlendForCompositeTransform()
    {
        // Verify that the.Blending was done with a decomposition in correct order
        // by blending a composite transform. Using matrix x vector notation
        // (Ax = b, where x is column vector), the ordering should be:
        // perspective * translation * rotation * skew * scale
        //
        // It is not as important (or meaningful) to check intermediate
        // interpolations; order of operations will be tested well enough by the
        // end cases that are easier to specify.

        Transform from = new();
        Transform to;

        Transform expected_end_of_animation = new();
        expected_end_of_animation.ApplyPerspectiveDepth(1.0);
        expected_end_of_animation.Translate3D(10.0f, 20.0f, 30.0f);
        expected_end_of_animation.RotateAbout(new Vector3DF(0.0f, 0.0f, 1.0f), 25.0);
        expected_end_of_animation.Skew(0.0, 45.0);
        expected_end_of_animation.Scale3D(6.0f, 7.0f, 8.0f);

        to = expected_end_of_animation;
        to.Blend(from, 0.0);
        Assert.Equal(from, to);

        to = expected_end_of_animation;
        // We short circuit if blend is >= 1, so to check the numerics, we will
        // check that we get close to what we expect when we're nearly done
        // interpolating.
        to.Blend(from, .99999f);

        // Recomposing the matrix results in a normalized matrix, so to verify we
        // need to normalize the expectedEndOfAnimation before comparing elements.
        // Normalizing means dividing everything by expectedEndOfAnimation.m44().
        Transform normalized_expected_end_of_animation = expected_end_of_animation;
        Transform normalization_matrix = new();
        double inv_w = 1.0 / expected_end_of_animation.rc(3, 3);
        normalization_matrix.set_rc(0, 0, inv_w);
        normalization_matrix.set_rc(1, 1, inv_w);
        normalization_matrix.set_rc(2, 2, inv_w);
        normalization_matrix.set_rc(3, 3, inv_w);
        normalized_expected_end_of_animation.PreConcat(normalization_matrix);

        Assert.True(MatricesAreNearlyEqual(normalized_expected_end_of_animation, to));
    }

    [Fact]
    private static void TestBlend2dXFlip()
    {
        // Test 2D x-flip (crbug.com/797472).
        var from = Transform.Affine(1, 0, 0, 1, 100, 150);
        var to = Transform.Affine(-1, 0, 0, 1, 400, 150);

        Assert.True(from.Is2dTransform());
        Assert.True(to.Is2dTransform());

        // OK for interpolated transform to be degenerate.
        Transform result = to;
        Assert.True(result.Blend(from, 0.5));
        var expected = Transform.Affine(0, 0, 0, 1, 250, 150);
        AssertTransformFloatEqual(expected, result);
    }

    [Fact]
    private static void TestBlend2dRotationDirection()
    {
        // Interpolate taking shorter rotation path.
        var from = Transform.Affine(-0.5, 0.86602575498, -0.86602575498, -0.5, 0, 0);
        var to = Transform.Affine(-0.5, -0.86602575498, 0.86602575498, -0.5, 0, 0);

        // Expect clockwise Rotation.
        Transform result = to;
        Assert.True(result.Blend(from, 0.5));
        var expected = Transform.Affine(-1, 0, 0, -1, 0, 0);
        AssertTransformFloatEqual(expected, result);

        // Reverse from and to.
        // Expect same midpoint with counter-clockwise rotation.
        result = from;
        Assert.True(result.Blend(to, 0.5));
        AssertTransformFloatEqual(expected, result);
    }

    private static DecomposedTransform GetRotationDecomp(double x, double y, double z, double w)
    {
        DecomposedTransform decomp = new()
        {
            Quaternion = new Quaternion(x, y, z, w)
        };
        return decomp;
    }

    private static readonly double kCos30deg = Math.Cos(double.DegreesToRadians(30.0));
    private static readonly double kSin30deg = 0.5;

    [Fact]
    private static void TestQuaternionFromRotationMatrix()
    {
        // Test rotation around each axis.

        Transform m = new();
        m.RotateAbout(1, 0, 0, 60);
        DecomposedTransform decomp;
        Assert.True(m.Decompose(out decomp));
        AssertQuaternionFloatNear(decomp.Quaternion, new Quaternion(kSin30deg, 0, 0, kCos30deg), 1e-6f);

        m.MakeIdentity();
        m.RotateAbout(0, 1, 0, 60);
        Assert.True(m.Decompose(out decomp));
        AssertQuaternionFloatNear(decomp.Quaternion, new Quaternion(0, kSin30deg, 0, kCos30deg), 1e-6f);

        m.MakeIdentity();
        m.RotateAbout(0, 0, 1, 60);
        Assert.True(m.Decompose(out decomp));
        AssertQuaternionFloatNear(decomp.Quaternion, new Quaternion(0, 0, kSin30deg, kCos30deg), 1e-6f);

        // Test rotation around non-axis aligned vector.
        double sqrt2 = Math.Sqrt(2);
        m.MakeIdentity();
        m.RotateAbout(1, 1, 0, 60);
        Assert.True(m.Decompose(out decomp));
        AssertQuaternionFloatNear(
        decomp.Quaternion,
        new Quaternion(kSin30deg / sqrt2,
                       kSin30deg / sqrt2, 0, kCos30deg), 1e-6f);

        // Test edge tests.

        // Cases where q_w = 0. In such cases we resort to basing the calculations on
        // the largest diagonal element in the rotation matrix to ensure numerical
        // stability.

        m.MakeIdentity();
        m.RotateAbout(1, 0, 0, 180);
        Assert.True(m.Decompose(out decomp));
        AssertQuaternionFloatNear(decomp.Quaternion, new Quaternion(1, 0, 0, 0), 1e-6f);

        m.MakeIdentity();
        m.RotateAbout(0, 1, 0, 180);
        Assert.True(m.Decompose(out decomp));
        AssertQuaternionFloatNear(decomp.Quaternion, new Quaternion(0, 1, 0, 0), 1e-6f);

        m.MakeIdentity();
        m.RotateAbout(0, 0, 1, 180);
        Assert.True(m.Decompose(out decomp));
        AssertQuaternionFloatNear(decomp.Quaternion, new Quaternion(0, 0, 1, 0), 1e-6f);

        // No rotation.

        m.MakeIdentity();
        Assert.True(m.Decompose(out decomp));
        AssertQuaternionFloatNear(decomp.Quaternion, new Quaternion(0, 0, 0, 1), 1e-6f);

        m.MakeIdentity();
        m.RotateAbout(0, 0, 1, 360);
        Assert.True(m.Decompose(out decomp));
        AssertQuaternionFloatNear(decomp.Quaternion, new Quaternion(0, 0, 0, 1), 1e-6f);
    }

    [Fact]
    private static void TestQuaternionToRotationMatrix()
    {
        // Test rotation about each axis.
        Transform rotate_x_60deg = new();
        rotate_x_60deg.RotateAboutXAxis(60);
        AssertTransformFloatEqual(rotate_x_60deg, Transform.Compose(GetRotationDecomp(
                                                kSin30deg, 0, 0, kCos30deg)));

        Transform rotate_y_60deg = new();
        rotate_y_60deg.RotateAboutYAxis(60);
        AssertTransformFloatEqual(rotate_y_60deg, Transform.Compose(GetRotationDecomp(
                                                0, kSin30deg, 0, kCos30deg)));

        Transform rotate_z_60deg = new();
        rotate_z_60deg.RotateAboutZAxis(60);
        AssertTransformFloatEqual(rotate_z_60deg, Transform.Compose(GetRotationDecomp(
                                                0, 0, kSin30deg, kCos30deg)));

        // Test non-axis aligned rotation
        Transform rotate_xy_60deg = new();
        rotate_xy_60deg.RotateAbout(1, 1, 0, 60);
        double sqrt2 = Math.Sqrt(2);
        AssertTransformFloatEqual(rotate_xy_60deg,
                            Transform.Compose(GetRotationDecomp(
                                kSin30deg / sqrt2,
                                kSin30deg / sqrt2, 0, kCos30deg)));

        // Test 180deg rotation.
        var rotate_z_180deg = Transform.Affine(-1, 0, 0, -1, 0, 0);
        AssertTransformFloatEqual(rotate_z_180deg, Transform.Compose(GetRotationDecomp(0, 0, 1, 0)));
    }

    [Fact]
    private static void TestQuaternionInterpolation()
    {
        // Rotate from identity matrix.
        Transform from_matrix = new();
        Transform to_matrix = new();
        to_matrix.RotateAbout(0, 0, 1, 120);
        to_matrix.Blend(from_matrix, 0.5);
        Transform rotate_z_60 = new();
        rotate_z_60.Rotate(60);
        AssertTransformFloatEqual(rotate_z_60, to_matrix);

        // Rotate to identity matrix.
        from_matrix.MakeIdentity();
        from_matrix.RotateAbout(0, 0, 1, 120);
        to_matrix.MakeIdentity();
        Assert.True(to_matrix.Blend(from_matrix, 0.5));
        AssertTransformFloatEqual(rotate_z_60, to_matrix);

        // Interpolation about a common axis of rotation.
        from_matrix.MakeIdentity();
        from_matrix.RotateAbout(1, 1, 0, 45);
        to_matrix.MakeIdentity();
        from_matrix.RotateAbout(1, 1, 0, 135);
        Assert.True(to_matrix.Blend(from_matrix, 0.5));
        Transform rotate_xy_90 = new();
        rotate_xy_90.RotateAbout(1, 1, 0, 90);
        AssertTransformFloatNear(rotate_xy_90, to_matrix, 1e-15f);

        // Interpolation without a common axis of rotation.

        from_matrix.MakeIdentity();
        from_matrix.RotateAbout(1, 0, 0, 90);
        to_matrix.MakeIdentity();
        to_matrix.RotateAbout(0, 0, 1, 90);
        Assert.True(to_matrix.Blend(from_matrix, 0.5));
        Transform expected = new();
        double sqrt2 = Math.Sqrt(2);
        expected.RotateAbout(1 / sqrt2, 0, 1 / sqrt2, 70.528778372);
        AssertTransformFloatEqual(expected, to_matrix);
    }

    [Fact]
    private static void TestComposeIdentity()
    {
        DecomposedTransform decomp = new();

        Assert.Equal(0.0, decomp.Translate.X);
        Assert.Equal(1.0, decomp.Scale.X);
        Assert.Equal(0.0, decomp.Skew.X);
        Assert.Equal(0.0, decomp.Perspective.X);

        Assert.Equal(0.0, decomp.Translate.Y);
        Assert.Equal(1.0, decomp.Scale.Y);
        Assert.Equal(0.0, decomp.Skew.Y);
        Assert.Equal(0.0, decomp.Perspective.Y);

        Assert.Equal(0.0, decomp.Translate.Z);
        Assert.Equal(1.0, decomp.Scale.Z);
        Assert.Equal(0.0, decomp.Skew.Z);
        Assert.Equal(0.0, decomp.Perspective.Z);

        Assert.Equal(1.0, decomp.Perspective.W);

        Assert.Equal(0.0, decomp.Quaternion.X);
        Assert.Equal(0.0, decomp.Quaternion.Y);
        Assert.Equal(0.0, decomp.Quaternion.Z);
        Assert.Equal(1.0, decomp.Quaternion.W);

        Assert.True(Transform.Compose(decomp).IsIdentity());
    }

    [Fact]
    private static void TestDecomposeTranslateRotateScale()
    {
        for (int degrees = 0; degrees < 180; ++degrees)
        {
            // build a transformation matrix.
            Transform transform = new();
            transform.Translate(degrees * 2, -degrees * 3);
            transform.Rotate(degrees);
            transform.Scale(degrees + 1, 2 * degrees + 1);

            // factor the matrix
            DecomposedTransform decomp;
            Assert.True(transform.Decompose(out decomp));
            FloatAlmostEqual((float)decomp.Translate.X, degrees * 2);
            FloatAlmostEqual((float)decomp.Translate.Y, -degrees * 3);
            double rotation =
                double.RadiansToDegrees(Math.Acos(decomp.Quaternion.W) * 2);
            while (rotation < 0.0)
            rotation += 360.0;
            while (rotation > 360.0)
            rotation -= 360.0;

            const float epsilon = 0.00015f;
            Assert.Equal(rotation, degrees, epsilon);
            Assert.Equal(decomp.Scale.X, degrees + 1, epsilon);
            Assert.Equal(decomp.Scale.Y, 2 * degrees + 1, epsilon);
        }
    }

    [Fact]
    private static void TestDecomposeScaleTransform()
    {
        for (float scale = 0.001f; scale < 2.0f; scale += 0.001f)
        {
            Transform transform = Transform.MakeScale(scale);

            DecomposedTransform decomp;
            Assert.True(transform.Decompose(out decomp));

            Transform compose_transform = Transform.Compose(decomp);
            Assert.True(compose_transform.Preserves2dAxisAlignment());
            Assert.Equal(transform, compose_transform);
        }
    }

    [Fact] 
    private static void TestDecompose2d()
    {
        DecomposedTransform decomp_flip_x;
        Transform.MakeScale(-2, 2).Decompose(out decomp_flip_x);

        var expected = new DecomposedTransform();

        expected.SetTranslate(0, 0, 0);
        expected.SetScale(-2, 2, 1);
        expected.SetSkew(0, 0, 0);
        expected.SetPerspective(0, 0, 0, 1);
        expected.SetQuaternion(0, 0, 0, 1);

        AssertDecomposedTransformFloatEqual(expected, decomp_flip_x);

        DecomposedTransform decomp_flip_y;
        Transform.MakeScale(2, -2).Decompose(out decomp_flip_y);

        expected = new DecomposedTransform();

        expected.SetTranslate(0, 0, 0);
        expected.SetScale(2, -2, 1);
        expected.SetSkew(0, 0, 0);
        expected.SetPerspective(0, 0, 0, 1);
        expected.SetQuaternion(0, 0, 0, 1);
        
        AssertDecomposedTransformFloatEqual(expected, decomp_flip_y);

        DecomposedTransform decomp_rotate_180;
        Transform.Make180degRotation().Decompose(out decomp_rotate_180);

        expected = new DecomposedTransform();

        expected.SetTranslate(0, 0, 0);
        expected.SetScale(1, 1, 1);
        expected.SetSkew(0, 0, 0);
        expected.SetPerspective(0, 0, 0, 1);
        expected.SetQuaternion(0, 0, 1, 0);

        AssertDecomposedTransformFloatEqual(expected, decomp_rotate_180);

        DecomposedTransform decomp_rotate_90;
        Transform.Make90degRotation().Decompose(out decomp_rotate_90);

        expected = new DecomposedTransform();

        var sqrt2 = Math.Sqrt(2);

        expected.SetTranslate(0, 0, 0);
        expected.SetScale(1, 1, 1);
        expected.SetSkew(0, 0, 0);
        expected.SetPerspective(0, 0, 0, 1);
        expected.SetQuaternion(0, 0, 1.0 / sqrt2, 1.0 / sqrt2);

        AssertDecomposedTransformFloatEqual(expected, decomp_rotate_90);
/*
        auto translate_rotate_90 =
            Transform::MakeTranslation(-1, 1) * Transform::Make90degRotation();
        DecomposedTransform decomp_translate_rotate_90 =
            *translate_rotate_90.Decompose();
        EXPECT_DECOMPOSED_TRANSFORM_EQ(
            (DecomposedTransform{
            { -1, 1, 0},
          { 1, 1, 1},
          { 0, 0, 0},
          { 0, 0, 0, 1},
          { 0, 0, 1.0 / std::numbers::sqrt2, 1.0 / std::numbers::sqrt2}
        }),
      decomp_translate_rotate_90);

        DecomposedTransform decomp_skew_rotate =
            *Transform::Affine(1, 1, 1, 0, 0, 0).Decompose();
        EXPECT_DECOMPOSED_TRANSFORM_EQ(
            (DecomposedTransform{
            { 0, 0, 0},
                           { std::numbers::sqrt2, -1.0 / std::numbers::sqrt2, 1},
                           { -1, 0, 0},
                           { 0, 0, 0, 1},
                           {
                0, 0, std::sin(std::numbers::pi / 8),
                            std::cos(std::numbers::pi / 8)}
        }),
      decomp_skew_rotate);
      */
    }
/*
    private static double ComputeDecompRecompError(in Transform transform)
    {
        DecomposedTransform decomp = *transform.Decompose();
        Transform composed = Transform::Compose(decomp);

        float expected[16];
        float actual[16];
        transform.GetColMajorF(expected);
        composed.GetColMajorF(actual);
        double sse = 0;
        for (int i = 0; i < 16; i++)
        {
            double diff = UNSAFE_TODO(expected[i]) - UNSAFE_TODO(actual[i]);
            sse += diff * diff;
        }
        return sse;
    }

    [Fact]
    private static void TestDecomposeAndCompose()
    {
        
    }

    [Fact]
    private static void TestIsIdentityOr2dTranslation()
    {
        
    }

    [Fact]
    private static void TestIntegerTranslation()
    {
        
    }

    [Fact]
    private static void TestInteger2dTranslation()
    {
        
    }

    [Fact]
    private static void TestInverse()
    {
        
    }

    [Fact]
    private static void TestVerifyBackfaceVisibilityBasicCases()
    {
        
    }
*/
}
