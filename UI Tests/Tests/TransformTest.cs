using Xunit;

using UI.GFX.Geometry;
using System.Diagnostics;

using static UI.Tests.GeometryUtil;
using UI.Extensions;

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
        Assert.True(a.Is2dTransform);
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
                    Point3F transformed_p1;
                    Assert.True(xform.InverseMapPoint(p1, out transformed_p1));
                    Assert.True(PointsAreNearlyEqual(transformed_p1, p0));
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
                        Point3F transformed_p1;
                        Assert.True(xform.InverseMapPoint(p1, out transformed_p1));
                        Assert.True(PointsAreNearlyEqual(transformed_p1, p0));
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
                Point3F transformed_p1;
                Assert.True(xform.InverseMapPoint(p1, out transformed_p1));
                Assert.True(PointsAreNearlyEqual(transformed_p1, p0));
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
                        Point transformed_p1;
                        Assert.True(xform.InverseMapPoint(p1, out transformed_p1));
                        Assert.Equal(transformed_p1.X, p0.X);
                        Assert.Equal(transformed_p1.Y, p0.Y);
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
                            Point transformed_p1;
                            Assert.True(xform.InverseMapPoint(p1, out transformed_p1));
                            Assert.Equal(transformed_p1.X, p0.X);
                            Assert.Equal(transformed_p1.Y, p0.Y);
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
                    Point transformed_pt;
                    Assert.True(xform.InverseMapPoint(pt, out transformed_pt));
                    Assert.Equal(transformed_pt.X, x);
                    Assert.Equal(transformed_pt.Y, y);
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

        Assert.True(from.Is2dTransform);
        Assert.True(to.Is2dTransform);

        // OK for interpolated transform to be degenerate.
        Transform result = to;
        Assert.True(result.Blend(from, 0.5));
        var expected = Transform.Affine(0, 0, 0, 1, 250, 150);
        AssertTransformEqual(expected, result);
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
        AssertTransformEqual(expected, result);

        // Reverse from and to.
        // Expect same midpoint with counter-clockwise rotation.
        result = from;
        Assert.True(result.Blend(to, 0.5));
        AssertTransformEqual(expected, result);
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
        AssertQuaternionNear(decomp.Quaternion, new Quaternion(kSin30deg, 0, 0, kCos30deg), 1e-6f);

        m.MakeIdentity();
        m.RotateAbout(0, 1, 0, 60);
        Assert.True(m.Decompose(out decomp));
        AssertQuaternionNear(decomp.Quaternion, new Quaternion(0, kSin30deg, 0, kCos30deg), 1e-6f);

        m.MakeIdentity();
        m.RotateAbout(0, 0, 1, 60);
        Assert.True(m.Decompose(out decomp));
        AssertQuaternionNear(decomp.Quaternion, new Quaternion(0, 0, kSin30deg, kCos30deg), 1e-6f);

        // Test rotation around non-axis aligned vector.
        double sqrt2 = Math.Sqrt(2);
        m.MakeIdentity();
        m.RotateAbout(1, 1, 0, 60);
        Assert.True(m.Decompose(out decomp));
        AssertQuaternionNear(
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
        AssertQuaternionNear(decomp.Quaternion, new Quaternion(1, 0, 0, 0), 1e-6f);

        m.MakeIdentity();
        m.RotateAbout(0, 1, 0, 180);
        Assert.True(m.Decompose(out decomp));
        AssertQuaternionNear(decomp.Quaternion, new Quaternion(0, 1, 0, 0), 1e-6f);

        m.MakeIdentity();
        m.RotateAbout(0, 0, 1, 180);
        Assert.True(m.Decompose(out decomp));
        AssertQuaternionNear(decomp.Quaternion, new Quaternion(0, 0, 1, 0), 1e-6f);

        // No rotation.

        m.MakeIdentity();
        Assert.True(m.Decompose(out decomp));
        AssertQuaternionNear(decomp.Quaternion, new Quaternion(0, 0, 0, 1), 1e-6f);

        m.MakeIdentity();
        m.RotateAbout(0, 0, 1, 360);
        Assert.True(m.Decompose(out decomp));
        AssertQuaternionNear(decomp.Quaternion, new Quaternion(0, 0, 0, 1), 1e-6f);
    }

    [Fact]
    private static void TestQuaternionToRotationMatrix()
    {
        // Test rotation about each axis.
        Transform rotate_x_60deg = new();
        rotate_x_60deg.RotateAboutXAxis(60);
        AssertTransformEqual(rotate_x_60deg, Transform.Compose(GetRotationDecomp(
                                                kSin30deg, 0, 0, kCos30deg)));

        Transform rotate_y_60deg = new();
        rotate_y_60deg.RotateAboutYAxis(60);
        AssertTransformEqual(rotate_y_60deg, Transform.Compose(GetRotationDecomp(
                                                0, kSin30deg, 0, kCos30deg)));

        Transform rotate_z_60deg = new();
        rotate_z_60deg.RotateAboutZAxis(60);
        AssertTransformEqual(rotate_z_60deg, Transform.Compose(GetRotationDecomp(
                                                0, 0, kSin30deg, kCos30deg)));

        // Test non-axis aligned rotation
        Transform rotate_xy_60deg = new();
        rotate_xy_60deg.RotateAbout(1, 1, 0, 60);
        double sqrt2 = Math.Sqrt(2);
        AssertTransformEqual(rotate_xy_60deg,
                            Transform.Compose(GetRotationDecomp(
                                kSin30deg / sqrt2,
                                kSin30deg / sqrt2, 0, kCos30deg)));

        // Test 180deg rotation.
        var rotate_z_180deg = Transform.Affine(-1, 0, 0, -1, 0, 0);
        AssertTransformEqual(rotate_z_180deg, Transform.Compose(GetRotationDecomp(0, 0, 1, 0)));
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
        AssertTransformEqual(rotate_z_60, to_matrix);

        // Rotate to identity matrix.
        from_matrix.MakeIdentity();
        from_matrix.RotateAbout(0, 0, 1, 120);
        to_matrix.MakeIdentity();
        Assert.True(to_matrix.Blend(from_matrix, 0.5));
        AssertTransformEqual(rotate_z_60, to_matrix);

        // Interpolation about a common axis of rotation.
        from_matrix.MakeIdentity();
        from_matrix.RotateAbout(1, 1, 0, 45);
        to_matrix.MakeIdentity();
        from_matrix.RotateAbout(1, 1, 0, 135);
        Assert.True(to_matrix.Blend(from_matrix, 0.5));
        Transform rotate_xy_90 = new();
        rotate_xy_90.RotateAbout(1, 1, 0, 90);
        AssertTransformNear(rotate_xy_90, to_matrix, 1e-15f);

        // Interpolation without a common axis of rotation.

        from_matrix.MakeIdentity();
        from_matrix.RotateAbout(1, 0, 0, 90);
        to_matrix.MakeIdentity();
        to_matrix.RotateAbout(0, 0, 1, 90);
        Assert.True(to_matrix.Blend(from_matrix, 0.5));
        Transform expected = new();
        double sqrt2 = Math.Sqrt(2);
        expected.RotateAbout(1 / sqrt2, 0, 1 / sqrt2, 70.528778372);
        AssertTransformEqual(expected, to_matrix);
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

        Assert.True(Transform.Compose(decomp).IsIdentity);
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

        AssertDecomposedTransformEqual(expected, decomp_flip_x);

        DecomposedTransform decomp_flip_y;
        Transform.MakeScale(2, -2).Decompose(out decomp_flip_y);

        expected = new DecomposedTransform();

        expected.SetTranslate(0, 0, 0);
        expected.SetScale(2, -2, 1);
        expected.SetSkew(0, 0, 0);
        expected.SetPerspective(0, 0, 0, 1);
        expected.SetQuaternion(0, 0, 0, 1);
        
        AssertDecomposedTransformEqual(expected, decomp_flip_y);

        DecomposedTransform decomp_rotate_180;
        Transform.Make180degRotation().Decompose(out decomp_rotate_180);

        expected = new DecomposedTransform();

        expected.SetTranslate(0, 0, 0);
        expected.SetScale(1, 1, 1);
        expected.SetSkew(0, 0, 0);
        expected.SetPerspective(0, 0, 0, 1);
        expected.SetQuaternion(0, 0, 1, 0);

        AssertDecomposedTransformEqual(expected, decomp_rotate_180);

        DecomposedTransform decomp_rotate_90;
        Transform.Make90degRotation().Decompose(out decomp_rotate_90);

        expected = new DecomposedTransform();

        var sqrt2 = Math.Sqrt(2);

        expected.SetTranslate(0, 0, 0);
        expected.SetScale(1, 1, 1);
        expected.SetSkew(0, 0, 0);
        expected.SetPerspective(0, 0, 0, 1);
        expected.SetQuaternion(0, 0, 1.0 / sqrt2, 1.0 / sqrt2);

        AssertDecomposedTransformEqual(expected, decomp_rotate_90);

        var translate_rotate_90 = Transform.MakeTranslation(-1, 1) * Transform.Make90degRotation();
        DecomposedTransform decomp_translate_rotate_90;
        translate_rotate_90.Decompose(out decomp_translate_rotate_90);

        expected = new DecomposedTransform();

        expected.SetTranslate(-1, 1, 0);
        expected.SetScale(1, 1, 1);
        expected.SetSkew(0, 0, 0);
        expected.SetPerspective(0, 0, 0, 1);
        expected.SetQuaternion(0, 0, 1.0 / sqrt2, 1.0 / sqrt2);

        AssertDecomposedTransformEqual(expected, decomp_translate_rotate_90);

        DecomposedTransform decomp_skew_rotate;
        Transform.Affine(1, 1, 1, 0, 0, 0).Decompose(out decomp_skew_rotate);

        expected = new DecomposedTransform();

        expected.SetTranslate(0, 0, 0);
        expected.SetScale(sqrt2, -1.0 / sqrt2, 1);
        expected.SetSkew(-1, 0, 0);
        expected.SetPerspective(0, 0, 0, 1);
        expected.SetQuaternion(0, 0, Math.Sin(Math.PI / 8), Math.Cos(Math.PI / 8));

        AssertDecomposedTransformEqual(expected, decomp_skew_rotate);
    }

    private static double ComputeDecompRecompError(in Transform transform)
    {
        DecomposedTransform decomp;
        transform.Decompose(out decomp);
        Transform composed = Transform.Compose(decomp);

        Span<float> expected = stackalloc float[16];
        Span<float> actual = stackalloc float[16];
        transform.GetColMajorF(expected);
        composed.GetColMajorF(actual);
        double sse = 0;
        for (int i = 0; i < 16; i++)
        {
            double diff = expected[i] - actual[i];
            sse += diff * diff;
        }
        return sse;
    }

    [Fact]
    private static void TestDecomposeAndCompose()
    {
        // rotateZ(90deg)
        Assert.Equal(0, ComputeDecompRecompError(Transform.Make90degRotation()), 1e-20);

        // rotateZ(180deg)
        // Edge case where w = 0.
        Assert.Equal(0, ComputeDecompRecompError(Transform.Make180degRotation()));

        // rotateX(90deg) rotateY(90deg) rotateZ(90deg)
        // [1  0   0][ 0 0 1][0 -1 0]   [0 0 1][0 -1 0]   [0  0 1]
        // [0  0  -1][ 0 1 0][1  0 0] = [1 0 0][1  0 0] = [0 -1 0]
        // [0  1   0][-1 0 0][0  0 1]   [0 1 0][0  0 1]   [1  0 0]
        // This test case leads to Gimbal lock when using Euler angles.
        Assert.Equal(0, ComputeDecompRecompError(Transform.RowMajor(0, 0, 1, 0, 0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1)), 1e-20);

        // Quaternion matrices with 0 off-diagonal elements, and negative trace.
        // Stress tests handling of degenerate cases in computing quaternions.
        // Validates fix for https://crbug.com/647554.
        Assert.Equal(0, ComputeDecompRecompError(Transform.Affine(1, 1, 1, 0, 0, 0)));
        Assert.Equal(0, ComputeDecompRecompError(Transform.MakeScale(-1, 1)));
        Assert.Equal(0, ComputeDecompRecompError(Transform.MakeScale(1, -1)));
        Transform flip_z = new();
        flip_z.Scale3D(1, 1, -1);
        Assert.Equal(0, ComputeDecompRecompError(flip_z));

        // The following cases exercise the branches Q_xx/yy/zz for quaternion in
        // Matrix44::Decompose().
        static Transform transform(double sx, double sy, double sz, int skew_r, int skew_c)
        {
            Transform t = new();
            t.Scale3D((float)sx, (float)sy, (float)sz);
            t.set_rc(skew_r, skew_c, 1);
            t.set_rc(skew_c, skew_r, 1);
            return t;
        }

        Assert.Equal(0, ComputeDecompRecompError(transform(1, -1, -1, 0, 1)));
        Assert.Equal(0, ComputeDecompRecompError(transform(1, -1, -1, 0, 2)));
        Assert.Equal(0, ComputeDecompRecompError(transform(-1, 1, -1, 0, 1)));
        Assert.Equal(0, ComputeDecompRecompError(transform(-1, 1, -1, 1, 2)));
        Assert.Equal(0, ComputeDecompRecompError(transform(-1, -1, 1, 0, 2)));
        Assert.Equal(0, ComputeDecompRecompError(transform(-1, -1, 1, 1, 2)));
    }


    [Fact]
    private static void TestIsIdentityOr2dTranslation()
    {
        Assert.True(new Transform().IsIdentityOr2dTranslation);
        Assert.True(Transform.MakeTranslation(10, 0).IsIdentityOr2dTranslation);
        Assert.True(Transform.MakeTranslation(0, -20).IsIdentityOr2dTranslation);

        Transform transform = new();
        transform.Translate3D(0, 0, 1);
        Assert.False(transform.IsIdentityOr2dTranslation);

        transform.MakeIdentity();
        transform.Rotate(40);
        Assert.False(transform.IsIdentityOr2dTranslation);

        transform.MakeIdentity();
        transform.SkewX(30);
        Assert.False(transform.IsIdentityOr2dTranslation);
    }

    [Fact]
    private static void TestIntegerTranslation()
    {
        Transform transform = new();
        Assert.True(transform.IsIdentityOrIntegerTranslation());

        transform.Translate3D(1, 2, 3);
        Assert.True(transform.IsIdentityOrIntegerTranslation());

        transform.MakeIdentity();
        transform.Translate3D(-1, -2, -3);
        Assert.True(transform.IsIdentityOrIntegerTranslation());

        transform.MakeIdentity();
        transform.Translate3D(4.5f, 0, 0);
        Assert.False(transform.IsIdentityOrIntegerTranslation());

        transform.MakeIdentity();
        transform.Translate3D(0, -6.7f, 0);
        Assert.False(transform.IsIdentityOrIntegerTranslation());

        transform.MakeIdentity();
        transform.Translate3D(0, 0, 8.9f);
        Assert.False(transform.IsIdentityOrIntegerTranslation());

        float max_int = (float)int.MaxValue;
        transform.MakeIdentity();
        transform.Translate3D(0, 0, max_int + 1000.5f);
        Assert.False(transform.IsIdentityOrIntegerTranslation());

        float max_float = float.MaxValue;
        transform.MakeIdentity();
        transform.Translate3D(0, 0, max_float - 0.5f);
        Assert.False(transform.IsIdentityOrIntegerTranslation());
    }

    [Fact]
    private static void TestInteger2dTranslation()
    {
        Assert.True(new Transform().IsIdentityOrInteger2dTranslation());
        Assert.True(Transform.MakeTranslation(1, 2).IsIdentityOrInteger2dTranslation());
        Assert.False(Transform.MakeTranslation(1.00001f, 2f).IsIdentityOrInteger2dTranslation());
        Assert.False(Transform.MakeTranslation(1f, 2.00002f).IsIdentityOrInteger2dTranslation());
        Assert.False(Transform.Make90degRotation().IsIdentityOrInteger2dTranslation());
        Transform transform = new();
        transform.Translate3D(1, 2, 3);
        Assert.False(transform.IsIdentityOrInteger2dTranslation());
    }

    [Fact]
    private static void TestInverse()
    {
        {
            Transform identity = new();
            Transform inverse_identity;
            Assert.True(identity.GetInverse(out inverse_identity));
            Assert.Equal(identity, inverse_identity);
            Assert.Equal(identity, identity.InverseOrIdentity());
        }

        {
            // Invert a translation
            Transform translation = new();
            translation.Translate3D(2.0f, 3.0f, 4.0f);
            Assert.True(translation.IsInvertible);

            Transform inverse_translation;
            bool is_invertible = translation.GetInverse(out inverse_translation);
            Assert.True(is_invertible);
            EXPECT_ROW0_EQ(1.0f, 0.0f, 0.0f, -2.0f, inverse_translation);
            EXPECT_ROW1_EQ(0.0f, 1.0f, 0.0f, -3.0f, inverse_translation);
            EXPECT_ROW2_EQ(0.0f, 0.0f, 1.0f, -4.0f, inverse_translation);
            EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, inverse_translation);

            Assert.Equal(inverse_translation, translation.InverseOrIdentity());

            // GetInverse with the parameter pointing to itself.
            Assert.True(translation.GetInverse(out translation));
            Assert.Equal(translation, inverse_translation);
        }

        {
            // Invert a non-uniform scale
            Transform scale = new();
            scale.Scale3D(4.0f, 10.0f, 100.0f);
            Assert.True(scale.IsInvertible);

            Transform inverse_scale;
            bool is_invertible = scale.GetInverse(out inverse_scale);
            Assert.True(is_invertible);
            EXPECT_ROW0_EQ(0.25f, 0.0f, 0.0f, 0.0f, inverse_scale);
            EXPECT_ROW1_EQ(0.0f, 0.1f, 0.0f, 0.0f, inverse_scale);
            EXPECT_ROW2_EQ(0.0f, 0.0f, 0.01f, 0.0f, inverse_scale);
            EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, inverse_scale);

            Assert.Equal(inverse_scale, scale.InverseOrIdentity());
        }

        {
            Transform m1 = new();
            m1.Translate(10, 20);
            m1.Rotate(30);
            Transform m2 = new();
            m2.Rotate(-30);
            m2.Translate(-10, -20);
            Transform inverse_m1, inverse_m2;
            Assert.True(m1.GetInverse(out inverse_m1));
            Assert.True(m2.GetInverse(out inverse_m2));
            Assert.True(inverse_m1.Is2dTransform);
            Assert.True(inverse_m2.Is2dTransform);
            AssertTransformNear(m1, inverse_m2, 1e-6f);
            AssertTransformNear(m2, inverse_m1, 1e-6f);
        }

        {
            Transform m1 = new();
            m1.RotateAboutZAxis(-30);
            m1.RotateAboutYAxis(10);
            m1.RotateAboutXAxis(20);
            m1.ApplyPerspectiveDepth(100);
            Transform m2 = new();
            m2.ApplyPerspectiveDepth(-100);
            m2.RotateAboutXAxis(-20);
            m2.RotateAboutYAxis(-10);
            m2.RotateAboutZAxis(30);
            Transform inverse_m1, inverse_m2;
            Assert.True(m1.GetInverse(out inverse_m1));
            Assert.True(m2.GetInverse(out inverse_m2));
            AssertTransformNear(m1, inverse_m2, 1e-6f);
            AssertTransformNear(m2, inverse_m1, 1e-6f);
        }

        {
            // Try to invert a matrix that is not invertible.
            // The inverse() function should reset the output matrix to identity.
            Transform uninvertible = new();
            uninvertible.set_rc(0, 0, 0.0);
            uninvertible.set_rc(1, 1, 0.0);
            uninvertible.set_rc(2, 2, 0.0);
            uninvertible.set_rc(3, 3, 0.0);
            Assert.False(uninvertible.IsInvertible);

            Transform inverse_of_uninvertible = new();

            // Add a scale just to more easily ensure that inverse_of_uninvertible is reset to identity.
            inverse_of_uninvertible.Scale3D(4.0f, 10.0f, 100.0f);

            bool is_invertible = uninvertible.GetInverse(out inverse_of_uninvertible);
            Assert.False(is_invertible);
            Assert.True(inverse_of_uninvertible.IsIdentity);
            EXPECT_ROW0_EQ(1.0f, 0.0f, 0.0f, 0.0f, inverse_of_uninvertible);
            EXPECT_ROW1_EQ(0.0f, 1.0f, 0.0f, 0.0f, inverse_of_uninvertible);
            EXPECT_ROW2_EQ(0.0f, 0.0f, 1.0f, 0.0f, inverse_of_uninvertible);
            EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, inverse_of_uninvertible);

            Assert.Equal(inverse_of_uninvertible, uninvertible.InverseOrIdentity());
        }
    }

    [Fact]
    private static void TestVerifyBackfaceVisibilityBasicCases()
    {
        Transform transform = new();

        transform.MakeIdentity();
        Assert.False(transform.IsBackFaceVisible());

        transform.MakeIdentity();
        transform.RotateAboutYAxis(80.0);
        Assert.False(transform.IsBackFaceVisible());

        transform.MakeIdentity();
        transform.RotateAboutYAxis(100.0);
        Assert.True(transform.IsBackFaceVisible());

        // Edge case, 90 degree rotation should return false.
        transform.MakeIdentity();
        transform.RotateAboutYAxis(90.0);
        Assert.False(transform.IsBackFaceVisible());

        // 2d scale doesn't affect backface visibility.
        void check_scale(float scale_x, float scale_y)
        {
            transform = Transform.MakeScale(scale_x, scale_y);
            Assert.False(transform.IsBackFaceVisible());
            transform.EnsureFullMatrix();
            Assert.False(transform.IsBackFaceVisible());
        }
        
        check_scale(1, 2);
        check_scale(-1, 2);
        check_scale(1, -2);
        check_scale(-1, -2);
    }

    [Fact]
    private static void TestverifyBackfaceVisibilityForPerspective()
    {
        Transform layer_space_to_projection_plane = new();

        // This tests if IsBackFaceVisible works properly under perspective transforms.
        // Specifically, layers that may have their back face visible in orthographic projection,
        // may not actually have back face visible under perspective projection.

        // Case 1: Layer is rotated by slightly more than 90 degrees,
        //         at the center of the perspective projection.
        //         In this case, the layer's back-side is visible to the camera.
        layer_space_to_projection_plane.MakeIdentity();
        layer_space_to_projection_plane.ApplyPerspectiveDepth(1.0);
        layer_space_to_projection_plane.Translate3D(0.0f, 0.0f, 0.0f);
        layer_space_to_projection_plane.RotateAboutYAxis(100.0);
        Assert.True(layer_space_to_projection_plane.IsBackFaceVisible());

        // Case 2: Layer is rotated by slightly more than 90 degrees, 
        //         but shifted off to the side of the camera. 
        //         Because of the wide field-of-view, the layer's front side is still visible.
        //
        //                       |<-- front side of layer is visible to camera
        //                    \  |            /
        //                     \ |           /
        //                      \|          /
        //                       |         /
        //                       |\       /<-- camera field of view
        //                       | \     /
        // back side of layer -->|  \   /
        //                           \./ <-- camera origin
        //
        layer_space_to_projection_plane.MakeIdentity();
        layer_space_to_projection_plane.ApplyPerspectiveDepth(1.0);
        layer_space_to_projection_plane.Translate3D(-10.0f, 0.0f, 0.0f);
        layer_space_to_projection_plane.RotateAboutYAxis(100.0);
        Assert.False(layer_space_to_projection_plane.IsBackFaceVisible());

        // Case 3: Additionally rotating the layer by 180 degrees should of course
        //         show the opposite result of case 2.
        layer_space_to_projection_plane.RotateAboutYAxis(180.0);
        Assert.True(layer_space_to_projection_plane.IsBackFaceVisible());
    }

    [Fact]
    private static void TestverifyDefaultConstructorCreatesIdentityMatrix()
    {
        Transform A = new();
        STATIC_ROW0_EQ(1.0, 0.0, 0.0, 0.0, A);
        STATIC_ROW1_EQ(0.0, 1.0, 0.0, 0.0, A);
        STATIC_ROW2_EQ(0.0, 0.0, 1.0, 0.0, A);
        STATIC_ROW3_EQ(0.0, 0.0, 0.0, 1.0, A);
        Assert.True(A.IsIdentity);
    }

    [Fact]
    private static void TestVerifyCopyConstructor()
    {
        Transform A = GetTestMatrix1();

        // Copy constructor should produce exact same elements as matrix A.
        Transform B = A;
        Assert.Equal(A, B);
        EXPECT_ROW0_EQ(10.0f, 14.0f, 18.0f, 22.0f, B);
        EXPECT_ROW1_EQ(11.0f, 15.0f, 19.0f, 23.0f, B);
        EXPECT_ROW2_EQ(12.0f, 16.0f, 20.0f, 24.0f, B);
        EXPECT_ROW3_EQ(13.0f, 17.0f, 21.0f, 25.0f, B);
    }

    // ColMajor() and RowMajor() are tested in GetTestMatrix1() and GetTestTransform2().
    
    [Fact]
    private static void TestGetColMajor()
    {
        var transform = GetTestMatrix1();

        Span<double> data = stackalloc double[16];
        transform.GetColMajor(data);
        for (int i = 0; i < 16; i++)
        {
            Assert.Equal(i + 10.0, data[i]);
            Assert.Equal(data[i], transform.ColMajorData(i));
        }
        Assert.Equal(transform, Transform.ColMajor(data));
    }

    [Fact]
    private static void TestAffine()
    {
        var transform = Transform.Affine(2.0, 3.0, 4.0, 5.0, 6.0, 7.0);
        STATIC_ROW0_EQ(2.0, 4.0, 0.0, 6.0, transform);
        STATIC_ROW1_EQ(3.0, 5.0, 0.0, 7.0, transform);
        STATIC_ROW2_EQ(0.0, 0.0, 1.0, 0.0, transform);
        STATIC_ROW3_EQ(0.0, 0.0, 0.0, 1.0, transform);
    }

    [Fact]
    private static void TestMakeTranslation()
    {
        var t = Transform.MakeTranslation(3.5f, 4.75f);
        STATIC_ROW0_EQ(1.0, 0.0, 0.0, 3.5, t);
        STATIC_ROW1_EQ(0.0, 1.0, 0.0, 4.75, t);
        STATIC_ROW2_EQ(0.0, 0.0, 1.0, 0.0, t);
        STATIC_ROW3_EQ(0.0, 0.0, 0.0, 1.0, t);
    }

    [Fact]
    private static void TestMakeScale()
    {
        var s = Transform.MakeScale(3.5f, 4.75f);
        STATIC_ROW0_EQ(3.5, 0.0, 0.0, 0, s);
        STATIC_ROW1_EQ(0.0, 4.75, 0.0, 0, s);
        STATIC_ROW2_EQ(0.0, 0.0, 1.0, 0.0, s);
        STATIC_ROW3_EQ(0.0, 0.0, 0.0, 1.0, s);
    }

    [Fact]
    private static void TestMakeRotation()
    {
        var r1 = Transform.Make90degRotation();
        STATIC_ROW0_EQ(0.0, -1.0, 0.0, 0, r1);
        STATIC_ROW1_EQ(1.0, 0.0, 0.0, 0, r1);
        STATIC_ROW2_EQ(0.0, 0.0, 1.0, 0.0, r1);
        STATIC_ROW3_EQ(0.0, 0.0, 0.0, 1.0, r1);

        var r2 = Transform.Make180degRotation();
        STATIC_ROW0_EQ(-1.0, 0.0, 0.0, 0, r2);
        STATIC_ROW1_EQ(0.0, -1.0, 0.0, 0, r2);
        STATIC_ROW2_EQ(0.0, 0.0, 1.0, 0.0, r2);
        STATIC_ROW3_EQ(0.0, 0.0, 0.0, 1.0, r2);

        var r3 = Transform.Make270degRotation();
        STATIC_ROW0_EQ(0.0, 1.0, 0.0, 0, r3);
        STATIC_ROW1_EQ(-1.0, 0.0, 0.0, 0, r3);
        STATIC_ROW2_EQ(0.0, 0.0, 1.0, 0.0, r3);
        STATIC_ROW3_EQ(0.0, 0.0, 0.0, 1.0, r3);
    }

    [Fact]
    private static void TestColMajorF()
    {
        float[] data = [ 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17 ];
        var transform = Transform.ColMajorF(data);

        EXPECT_ROW0_EQ(2.0f, 6.0f, 10.0f, 14.0f, transform);
        EXPECT_ROW1_EQ(3.0f, 7.0f, 11.0f, 15.0f, transform);
        EXPECT_ROW2_EQ(4.0f, 8.0f, 12.0f, 16.0f, transform);
        EXPECT_ROW3_EQ(5.0f, 9.0f, 13.0f, 17.0f, transform);

        Span<float> data1 = stackalloc float[16];
        transform.GetColMajorF(data1);
        for (int i = 0; i < 16; i++)
            Assert.Equal(data1[i], data[i]);
        Assert.Equal(transform, Transform.ColMajorF(data1));
    }

    [Fact]
    private static void TestFromQuaternion()
    {
        var t = new Transform(new Quaternion(1, 2, 3, 4));
        EXPECT_ROW0_EQ(-25.0f, -20.0f, 22.0f, 0.0f, t);
        EXPECT_ROW1_EQ(28.0f, -19.0f, 4.0f, 0.0f, t);
        EXPECT_ROW2_EQ(-10.0f, 20.0f, -9.0f, 0.0f, t);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, t);
    }

    [Fact]
    private static void TestVerifyAssignmentOperator()
    {
        Transform A = GetTestMatrix1();
        Transform B = GetTestMatrix2();
        Transform C = GetTestMatrix2();
        C = B = A;

        // Both B and C should now have been re-assigned to the value of A.
        EXPECT_ROW0_EQ(10.0f, 14.0f, 18.0f, 22.0f, B);
        EXPECT_ROW1_EQ(11.0f, 15.0f, 19.0f, 23.0f, B);
        EXPECT_ROW2_EQ(12.0f, 16.0f, 20.0f, 24.0f, B);
        EXPECT_ROW3_EQ(13.0f, 17.0f, 21.0f, 25.0f, B);

        EXPECT_ROW0_EQ(10.0f, 14.0f, 18.0f, 22.0f, C);
        EXPECT_ROW1_EQ(11.0f, 15.0f, 19.0f, 23.0f, C);
        EXPECT_ROW2_EQ(12.0f, 16.0f, 20.0f, 24.0f, C);
        EXPECT_ROW3_EQ(13.0f, 17.0f, 21.0f, 25.0f, C);
    }

    [Fact]
    private static void TestVerifyEqualsBooleanOperator()
    {
        Transform A = GetTestMatrix1();
        Transform B = GetTestMatrix1();
        Assert.True(A == B);

        // Modifying multiple elements should cause equals operator to return false.
        Transform C = GetTestMatrix2();
        Assert.False(A == C);

        // Modifying any one individual element should cause equals operator to
        // return false.
        Transform D;
        D = A;
        D.set_rc(0, 0, 0.0f);
        Assert.False(A == D);

        D = A;
        D.set_rc(1, 0, 0.0f);
        Assert.False(A == D);

        D = A;
        D.set_rc(2, 0, 0.0f);
        Assert.False(A == D);

        D = A;
        D.set_rc(3, 0, 0.0f);
        Assert.False(A == D);

        D = A;
        D.set_rc(0, 1, 0.0f);
        Assert.False(A == D);

        D = A;
        D.set_rc(1, 1, 0.0f);
        Assert.False(A == D);

        D = A;
        D.set_rc(2, 1, 0.0f);
        Assert.False(A == D);

        D = A;
        D.set_rc(3, 1, 0.0f);
        Assert.False(A == D);

        D = A;
        D.set_rc(0, 2, 0.0f);
        Assert.False(A == D);

        D = A;
        D.set_rc(1, 2, 0.0f);
        Assert.False(A == D);

        D = A;
        D.set_rc(2, 2, 0.0f);
        Assert.False(A == D);

        D = A;
        D.set_rc(3, 2, 0.0f);
        Assert.False(A == D);

        D = A;
        D.set_rc(0, 3, 0.0f);
        Assert.False(A == D);

        D = A;
        D.set_rc(1, 3, 0.0f);
        Assert.False(A == D);

        D = A;
        D.set_rc(2, 3, 0.0f);
        Assert.False(A == D);

        D = A;
        D.set_rc(3, 3, 0.0f);
        Assert.False(A == D);
    }

    [Fact]
    private static void TestVerifyMultiplyOperator()
    {
        Transform A = GetTestMatrix1();
        Transform B = GetTestMatrix2();

        Transform C = A * B;
        EXPECT_ROW0_EQ(2036.0f, 2292.0f, 2548.0f, 2804.0f, C);
        EXPECT_ROW1_EQ(2162.0f, 2434.0f, 2706.0f, 2978.0f, C);
        EXPECT_ROW2_EQ(2288.0f, 2576.0f, 2864.0f, 3152.0f, C);
        EXPECT_ROW3_EQ(2414.0f, 2718.0f, 3022.0f, 3326.0f, C);

        // Just an additional sanity check; matrix multiplication is not commutative.
        Assert.False(A * B == B * A);
    }

    [Fact]
    private static void TestVerifyMultiplyAndAssignOperator()
    {
        Transform A = GetTestMatrix1();
        Transform B = GetTestMatrix2();

        A *= B;
        EXPECT_ROW0_EQ(2036.0f, 2292.0f, 2548.0f, 2804.0f, A);
        EXPECT_ROW1_EQ(2162.0f, 2434.0f, 2706.0f, 2978.0f, A);
        EXPECT_ROW2_EQ(2288.0f, 2576.0f, 2864.0f, 3152.0f, A);
        EXPECT_ROW3_EQ(2414.0f, 2718.0f, 3022.0f, 3326.0f, A);

        // Just an additional sanity check; matrix multiplication is not commutative.
        Transform C = A;
        C *= B;
        Transform D = B;
        D *= A;
        Assert.False(C == D);
    }

    [Fact]
    private static void TestPreConcat()
    {
        Transform A = GetTestMatrix1();
        Transform B = GetTestMatrix2();

        A.PreConcat(B);
        EXPECT_ROW0_EQ(2036.0f, 2292.0f, 2548.0f, 2804.0f, A);
        EXPECT_ROW1_EQ(2162.0f, 2434.0f, 2706.0f, 2978.0f, A);
        EXPECT_ROW2_EQ(2288.0f, 2576.0f, 2864.0f, 3152.0f, A);
        EXPECT_ROW3_EQ(2414.0f, 2718.0f, 3022.0f, 3326.0f, A);
    }

    [Fact]
    private static void TestVerifyMakeIdentiy()
    {
        Transform A = GetTestMatrix1();
        A.MakeIdentity();
        EXPECT_ROW0_EQ(1.0f, 0.0f, 0.0f, 0.0f, A);
        EXPECT_ROW1_EQ(0.0f, 1.0f, 0.0f, 0.0f, A);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 1.0f, 0.0f, A);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, A);
        Assert.True(A.IsIdentity);
    }

    [Fact]
    private static void TestVerifyTranslate()
    {
        Transform A = new();
        A.Translate(2.0f, 3.0f);
        EXPECT_ROW0_EQ(1.0f, 0.0f, 0.0f, 2.0f, A);
        EXPECT_ROW1_EQ(0.0f, 1.0f, 0.0f, 3.0f, A);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 1.0f, 0.0f, A);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, A);

        // Verify that Translate() post-multiplies the existing matrix.
        A.MakeIdentity();
        A.Scale(5.0f, 5.0f);
        A.Translate(2.0f, 3.0f);
        EXPECT_ROW0_EQ(5.0f, 0.0f, 0.0f, 10.0f, A);
        EXPECT_ROW1_EQ(0.0f, 5.0f, 0.0f, 15.0f, A);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 1.0f, 0.0f, A);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, A);

        Transform B = new();
        B.Scale(5.0f, 5.0f);
        B.Translate(new Vector2DF(2.0f, 3.0f));
        Assert.Equal(A, B);
    }

    [Fact]
    private static void TestVerifyPostTranslate()
    {
        Transform A = new();
        A.PostTranslate(2.0f, 3.0f);
        EXPECT_ROW0_EQ(1.0f, 0.0f, 0.0f, 2.0f, A);
        EXPECT_ROW1_EQ(0.0f, 1.0f, 0.0f, 3.0f, A);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 1.0f, 0.0f, A);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, A);

        // Verify that PostTranslate() pre-multiplies the existing matrix.
        A.MakeIdentity();
        A.Scale(5.0f, 5.0f);
        A.PostTranslate(2.0f, 3.0f);
        EXPECT_ROW0_EQ(5.0f, 0.0f, 0.0f, 2.0f, A);
        EXPECT_ROW1_EQ(0.0f, 5.0f, 0.0f, 3.0f, A);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 1.0f, 0.0f, A);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, A);

        Transform B =new();
        B.Scale(5.0f, 5.0f);
        B.PostTranslate(new Vector2DF(2.0f, 3.0f));
        Assert.Equal(A, B);
    }

    [Fact]
    private static void TestVerifyTranslate3d()
    {
        Transform A = new();
        A.Translate3D(2.0f, 3.0f, 4.0f);
        EXPECT_ROW0_EQ(1.0f, 0.0f, 0.0f, 2.0f, A);
        EXPECT_ROW1_EQ(0.0f, 1.0f, 0.0f, 3.0f, A);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 1.0f, 4.0f, A);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, A);

        // Verify that Translate3d() post-multiplies the existing matrix.
        A.MakeIdentity();
        A.Scale3D(6.0f, 7.0f, 8.0f);
        A.Translate3D(2.0f, 3.0f, 4.0f);
        EXPECT_ROW0_EQ(6.0f, 0.0f, 0.0f, 12.0f, A);
        EXPECT_ROW1_EQ(0.0f, 7.0f, 0.0f, 21.0f, A);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 8.0f, 32.0f, A);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, A);

        Transform B = new();
        B.Scale3D(6.0f, 7.0f, 8.0f);
        B.Translate3D(new Vector3DF(2.0f, 3.0f, 4.0f));
        Assert.Equal(A, B);
    }

    [Fact]
    private static void TestVerifyPostTranslate3d()
    {
        Transform A = new();
        A.PostTranslate3D(2.0f, 3.0f, 4.0f);
        EXPECT_ROW0_EQ(1.0f, 0.0f, 0.0f, 2.0f, A);
        EXPECT_ROW1_EQ(0.0f, 1.0f, 0.0f, 3.0f, A);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 1.0f, 4.0f, A);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, A);

        // Verify that PostTranslate3d() pre-multiplies the existing matrix.
        A.MakeIdentity();
        A.Scale3D(6.0f, 7.0f, 8.0f);
        A.PostTranslate3D(2.0f, 3.0f, 4.0f);
        EXPECT_ROW0_EQ(6.0f, 0.0f, 0.0f, 2.0f, A);
        EXPECT_ROW1_EQ(0.0f, 7.0f, 0.0f, 3.0f, A);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 8.0f, 4.0f, A);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, A);

        Transform B = new();
        B.Scale3D(6.0f, 7.0f, 8.0f);
        B.PostTranslate3D(new Vector3DF(2.0f, 3.0f, 4.0f));
        Assert.Equal(A, B);
    }

    [Fact]
    private static void TestVerifyScale()
    {
        Transform A = new();
        A.Scale(6.0f, 7.0f);
        EXPECT_ROW0_EQ(6.0f, 0.0f, 0.0f, 0.0f, A);
        EXPECT_ROW1_EQ(0.0f, 7.0f, 0.0f, 0.0f, A);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 1.0f, 0.0f, A);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, A);

        // Verify that Scale() post-multiplies the existing matrix.
        A.MakeIdentity();
        A.Translate3D(2.0f, 3.0f, 4.0f);
        A.Scale(6.0f, 7.0f);
        EXPECT_ROW0_EQ(6.0f, 0.0f, 0.0f, 2.0f, A);
        EXPECT_ROW1_EQ(0.0f, 7.0f, 0.0f, 3.0f, A);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 1.0f, 4.0f, A);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, A);
    }

    [Fact]
    private static void TestVerifyScale3d()
    {
        Transform A = new();
        A.Scale3D(6.0f, 7.0f, 8.0f);
        EXPECT_ROW0_EQ(6.0f, 0.0f, 0.0f, 0.0f, A);
        EXPECT_ROW1_EQ(0.0f, 7.0f, 0.0f, 0.0f, A);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 8.0f, 0.0f, A);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, A);

        // Verify that scale3d() post-multiplies the existing matrix.
        A.MakeIdentity();
        A.Translate3D(2.0f, 3.0f, 4.0f);
        A.Scale3D(6.0f, 7.0f, 8.0f);
        EXPECT_ROW0_EQ(6.0f, 0.0f, 0.0f, 2.0f, A);
        EXPECT_ROW1_EQ(0.0f, 7.0f, 0.0f, 3.0f, A);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 8.0f, 4.0f, A);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, A);
    }

    [Fact]
    private static void TestVerifyPostScale3d()
    {
        Transform A = new();
        A.PostScale3D(6.0f, 7.0f, 8.0f);
        EXPECT_ROW0_EQ(6.0f, 0.0f, 0.0f, 0.0f, A);
        EXPECT_ROW1_EQ(0.0f, 7.0f, 0.0f, 0.0f, A);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 8.0f, 0.0f, A);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, A);

        // Verify that PostScale3d() pre-multiplies the existing matrix.
        A.MakeIdentity();
        A.Translate3D(2.0f, 3.0f, 4.0f);
        A.PostScale3D(6.0f, 7.0f, 8.0f);
        EXPECT_ROW0_EQ(6.0f, 0.0f, 0.0f, 12.0f, A);
        EXPECT_ROW1_EQ(0.0f, 7.0f, 0.0f, 21.0f, A);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 8.0f, 32.0f, A);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, A);
    }

    [Fact]
    private static void TestRotate()
    {
        Transform A = new();
        A.Rotate(90.0);
        EXPECT_ROW0_EQ(0.0f, -1.0f, 0.0f, 0.0f, A);
        EXPECT_ROW1_EQ(1.0f, 0.0f, 0.0f, 0.0f, A);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 1.0f, 0.0f, A);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, A);

        // Verify that Rotate() post-multiplies the existing matrix.
        A.MakeIdentity();
        A.Scale3D(6.0f, 7.0f, 8.0f);
        A.Rotate(90.0);
        EXPECT_ROW0_EQ(0.0f, -6.0f, 0.0f, 0.0f, A);
        EXPECT_ROW1_EQ(7.0f, 0.0f, 0.0f, 0.0f, A);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 8.0f, 0.0f, A);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, A);
    }

    [Fact]
    private static void TestRotateAboutXAxis()
    {
        Transform A = new();
        double sin45 = 0.5 * Math.Sqrt(2.0);
        double cos45 = sin45;

        A.MakeIdentity();
        A.RotateAboutXAxis(90.0);
        EXPECT_ROW0_EQ(1.0f, 0.0f, 0.0f, 0.0f, A);
        EXPECT_ROW1_EQ(0.0f, 0.0f, -1.0f, 0.0f, A);
        EXPECT_ROW2_EQ(0.0f, 1.0f, 0.0f, 0.0f, A);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, A);

        A.MakeIdentity();
        A.RotateAboutXAxis(45.0);
        EXPECT_ROW0_EQ(1.0f, 0.0f, 0.0f, 0.0f, A);
        EXPECT_ROW1_NEAR(0.0, cos45, -sin45, 0.0, A, kErrorThreshold);
        EXPECT_ROW2_NEAR(0.0, sin45, cos45, 0.0, A, kErrorThreshold);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, A);

        // Verify that RotateAboutXAxis(angle) post-multiplies the existing matrix.
        A.MakeIdentity();
        A.Scale3D(6.0f, 7.0f, 8.0f);
        A.RotateAboutXAxis(90.0);
        EXPECT_ROW0_EQ(6.0f, 0.0f, 0.0f, 0.0f, A);
        EXPECT_ROW1_EQ(0.0f, 0.0f, -7.0f, 0.0f, A);
        EXPECT_ROW2_EQ(0.0f, 8.0f, 0.0f, 0.0f, A);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, A);
    }

    [Fact]
    private static void TestRotateAboutYAxis()
    {
        Transform A = new();
        double sin45 = 0.5 * Math.Sqrt(2.0);
        double cos45 = sin45;

        // Note carefully, the expected pattern is inverted compared to rotating
        // about x axis or z axis.
        A.MakeIdentity();
        A.RotateAboutYAxis(90.0);
        EXPECT_ROW0_EQ(0.0f, 0.0f, 1.0f, 0.0f, A);
        EXPECT_ROW1_EQ(0.0f, 1.0f, 0.0f, 0.0f, A);
        EXPECT_ROW2_EQ(-1.0f, 0.0f, 0.0f, 0.0f, A);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, A);

        A.MakeIdentity();
        A.RotateAboutYAxis(45.0);
        EXPECT_ROW0_NEAR(cos45, 0.0, sin45, 0.0, A, kErrorThreshold);
        EXPECT_ROW1_EQ(0.0f, 1.0f, 0.0f, 0.0f, A);
        EXPECT_ROW2_NEAR(-sin45, 0.0, cos45, 0.0, A, kErrorThreshold);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, A);

        // Verify that RotateAboutYAxis(angle) post-multiplies the existing matrix.
        A.MakeIdentity();
        A.Scale3D(6.0f, 7.0f, 8.0f);
        A.RotateAboutYAxis(90.0);
        EXPECT_ROW0_EQ(0.0f, 0.0f, 6.0f, 0.0f, A);
        EXPECT_ROW1_EQ(0.0f, 7.0f, 0.0f, 0.0f, A);
        EXPECT_ROW2_EQ(-8.0f, 0.0f, 0.0f, 0.0f, A);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, A);
    }

    [Fact]
    private static void TestRotateAboutZAxis()
    {
        Transform A = new();
        double sin45 = 0.5 * Math.Sqrt(2.0);
        double cos45 = sin45;

        A.MakeIdentity();
        A.RotateAboutZAxis(90.0);
        EXPECT_ROW0_EQ(0.0f, -1.0f, 0.0f, 0.0f, A);
        EXPECT_ROW1_EQ(1.0f, 0.0f, 0.0f, 0.0f, A);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 1.0f, 0.0f, A);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, A);

        A.MakeIdentity();
        A.RotateAboutZAxis(45.0);
        EXPECT_ROW0_NEAR(cos45, -sin45, 0.0, 0.0, A, kErrorThreshold);
        EXPECT_ROW1_NEAR(sin45, cos45, 0.0, 0.0, A, kErrorThreshold);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 1.0f, 0.0f, A);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, A);

        // Verify that RotateAboutZAxis(angle) post-multiplies the existing matrix.
        A.MakeIdentity();
        A.Scale3D(6.0f, 7.0f, 8.0f);
        A.RotateAboutZAxis(90.0);
        EXPECT_ROW0_EQ(0.0f, -6.0f, 0.0f, 0.0f, A);
        EXPECT_ROW1_EQ(7.0f, 0.0f, 0.0f, 0.0f, A);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 8.0f, 0.0f, A);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, A);
    }

    [Fact]
    private static void TestRotateAboutForAlignedAxes()
    {
        Transform A = new();

        // Check rotation about z-axis
        A.MakeIdentity();
        A.RotateAbout(new Vector3DF(0.0f, 0.0f, 1.0f), 90.0);
        EXPECT_ROW0_EQ(0.0f, -1.0f, 0.0f, 0.0f, A);
        EXPECT_ROW1_EQ(1.0f, 0.0f, 0.0f, 0.0f, A);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 1.0f, 0.0f, A);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, A);

        // Check rotation about x-axis
        A.MakeIdentity();
        A.RotateAbout(new Vector3DF(1.0f, 0.0f, 0.0f), 90.0);
        EXPECT_ROW0_EQ(1.0f, 0.0f, 0.0f, 0.0f, A);
        EXPECT_ROW1_EQ(0.0f, 0.0f, -1.0f, 0.0f, A);
        EXPECT_ROW2_EQ(0.0f, 1.0f, 0.0f, 0.0f, A);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, A);

        // Check rotation about y-axis. Note carefully, the expected pattern is
        // inverted compared to rotating about x axis or z axis.
        A.MakeIdentity();
        A.RotateAbout(new Vector3DF(0.0f, 1.0f, 0.0f), 90.0);
        EXPECT_ROW0_EQ(0.0f, 0.0f, 1.0f, 0.0f, A);
        EXPECT_ROW1_EQ(0.0f, 1.0f, 0.0f, 0.0f, A);
        EXPECT_ROW2_EQ(-1.0f, 0.0f, 0.0f, 0.0f, A);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, A);

        // Verify that rotate3d(axis, angle) post-multiplies the existing matrix.
        A.MakeIdentity();
        A.Scale3D(6.0f, 7.0f, 8.0f);
        A.RotateAboutZAxis(90.0);
        EXPECT_ROW0_EQ(0.0f, -6.0f, 0.0f, 0.0f, A);
        EXPECT_ROW1_EQ(7.0f, 0.0f, 0.0f, 0.0f, A);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 8.0f, 0.0f, A);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, A);
    }

    [Fact]
    private static void TestVerifyRotateAboutForArbitraryAxis()
    {
        // Check rotation about an arbitrary non-axis-aligned vector.
        Transform A = new();
        A.RotateAbout(new Vector3DF(1.0f, 1.0f, 1.0f), 90.0);
        EXPECT_ROW0_NEAR(0.3333333333333334258519187, -0.2440169358562924717404030,
                         0.9106836025229592124219380, 0.0, A, kErrorThreshold);
        EXPECT_ROW1_NEAR(0.9106836025229592124219380, 0.3333333333333334258519187,
                         -0.2440169358562924717404030, 0.0, A, kErrorThreshold);
        EXPECT_ROW2_NEAR(-0.2440169358562924717404030, 0.9106836025229592124219380,
                         0.3333333333333334258519187, 0.0, A, kErrorThreshold);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, A);
    }

    [Fact]
    private static void TestVerifyRotateAboutForDegenerateAxis()
    {
        // Check rotation about a degenerate zero vector.
        // It is expected to skip applying the rotation.
        Transform A = new();

        A.RotateAbout(new Vector3DF(0.0f, 0.0f, 0.0f), 45.0);
        // Verify that A remains unchanged.
        Assert.True(A.IsIdentity);

        A = GetTestMatrix1();
        A.RotateAbout(new Vector3DF(0.0f, 0.0f, 0.0f), 35.0);

        // Verify that A remains unchanged.
        EXPECT_ROW0_EQ(10.0f, 14.0f, 18.0f, 22.0f, A);
        EXPECT_ROW1_EQ(11.0f, 15.0f, 19.0f, 23.0f, A);
        EXPECT_ROW2_EQ(12.0f, 16.0f, 20.0f, 24.0f, A);
        EXPECT_ROW3_EQ(13.0f, 17.0f, 21.0f, 25.0f, A);
    }

    [Fact]
    private static void TestVerifySkew()
    {
        // Test a skew along X axis only
        Transform A = new();
        A.Skew(45.0, 0.0);
        EXPECT_ROW0_EQ(1.0f, 1.0f, 0.0f, 0.0f, A);
        EXPECT_ROW1_EQ(0.0f, 1.0f, 0.0f, 0.0f, A);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 1.0f, 0.0f, A);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, A);

        // Test a skew along Y axis only
        A.MakeIdentity();
        A.Skew(0.0, 45.0);
        EXPECT_ROW0_EQ(1.0f, 0.0f, 0.0f, 0.0f, A);
        EXPECT_ROW1_EQ(1.0f, 1.0f, 0.0f, 0.0f, A);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 1.0f, 0.0f, A);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, A);

        // Verify that skew() post-multiplies the existing matrix. Row 1, column 2,
        // would incorrectly have value "7" if the matrix is pre-multiplied instead
        // of post-multiplied.
        A.MakeIdentity();
        A.Scale3D(6.0f, 7.0f, 8.0f);
        A.Skew(45.0, 0.0);
        EXPECT_ROW0_EQ(6.0f, 6.0f, 0.0f, 0.0f, A);
        EXPECT_ROW1_EQ(0.0f, 7.0f, 0.0f, 0.0f, A);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 8.0f, 0.0f, A);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, A);

        // Test a skew along X and Y axes both
        A.MakeIdentity();
        A.Skew(45.0, 45.0);
        EXPECT_ROW0_EQ(1.0f, 1.0f, 0.0f, 0.0f, A);
        EXPECT_ROW1_EQ(1.0f, 1.0f, 0.0f, 0.0f, A);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 1.0f, 0.0f, A);
        EXPECT_ROW3_EQ(0.0f, 0.0f, 0.0f, 1.0f, A);
    }

    [Fact]
    private static void TestVerifyPerspectiveDepth()
    {
        Transform A = new();
        A.ApplyPerspectiveDepth(1.0);
        EXPECT_ROW0_EQ(1.0f, 0.0f, 0.0f, 0.0f, A);
        EXPECT_ROW1_EQ(0.0f, 1.0f, 0.0f, 0.0f, A);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 1.0f, 0.0f, A);
        EXPECT_ROW3_EQ(0.0f, 0.0f, -1.0f, 1.0f, A);

        // Verify that PerspectiveDepth() post-multiplies the existing matrix.
        A.MakeIdentity();
        A.Translate3D(2.0f, 3.0f, 4.0f);
        A.ApplyPerspectiveDepth(1.0);
        EXPECT_ROW0_EQ(1.0f, 0.0f, -2.0f, 2.0f, A);
        EXPECT_ROW1_EQ(0.0f, 1.0f, -3.0f, 3.0f, A);
        EXPECT_ROW2_EQ(0.0f, 0.0f, -3.0f, 4.0f, A);
        EXPECT_ROW3_EQ(0.0f, 0.0f, -1.0f, 1.0f, A);
    }

    [Fact]
    private static void TestVerifyHasPerspective()
    {
        Transform A = new();
        A.ApplyPerspectiveDepth(1.0);
        Assert.True(A.HasPerspective);

        A.MakeIdentity();
        A.ApplyPerspectiveDepth(0.0);
        Assert.False(A.HasPerspective);

        A.MakeIdentity();
        A.set_rc(3, 0, -1.0f);
        Assert.True(A.HasPerspective);

        A.MakeIdentity();
        A.set_rc(3, 1, -1.0f);
        Assert.True(A.HasPerspective);

        A.MakeIdentity();
        A.set_rc(3, 2, -0.3f);
        Assert.True(A.HasPerspective);

        A.MakeIdentity();
        A.set_rc(3, 3, 0.5f);
        Assert.True(A.HasPerspective);

        A.MakeIdentity();
        A.set_rc(3, 3, 0.0f);
        Assert.True(A.HasPerspective);
    }

    [Fact]
    private static void TestVerifyIsInvertible()
    {
        Transform A = new();

        // Translations, rotations, scales, skews and arbitrary combinations of them
        // are invertible.
        A.MakeIdentity();
        Assert.True(A.IsInvertible);

        A.MakeIdentity();
        A.Translate3D(2.0f, 3.0f, 4.0f);
        Assert.True(A.IsInvertible);

        A.MakeIdentity();
        A.Scale3D(6.0f, 7.0f, 8.0f);
        Assert.True(A.IsInvertible);

        A.MakeIdentity();
        A.RotateAboutXAxis(10.0);
        A.RotateAboutYAxis(20.0);
        A.RotateAboutZAxis(30.0);
        Assert.True(A.IsInvertible);

        A.MakeIdentity();
        A.Skew(45.0, 0.0);
        Assert.True(A.IsInvertible);

        // A perspective matrix (projection plane at z=0) is invertible. The
        // intuitive explanation is that perspective is equivalent to a skew of the
        // w-axis; skews are invertible.
        A.MakeIdentity();
        A.ApplyPerspectiveDepth(1.0);
        Assert.True(A.IsInvertible);

        // A "pure" perspective matrix derived by similar triangles, with rc(3, 3) set
        // to zero (i.e. camera positioned at the origin), is not invertible.
        A.MakeIdentity();
        A.ApplyPerspectiveDepth(1.0);
        A.set_rc(3, 3, 0.0f);
        Assert.False(A.IsInvertible);

        // Adding more to a non-invertible matrix will not make it invertible in the
        // general case.
        A.MakeIdentity();
        A.ApplyPerspectiveDepth(1.0);
        A.set_rc(3, 3, 0.0f);
        Assert.False(A.IsInvertible);
        A.Scale3D(6.0f, 7.0f, 8.0f);
        Assert.False(A.IsInvertible);
        A.RotateAboutXAxis(10.0);
        Assert.False(A.IsInvertible);
        A.RotateAboutYAxis(20.0);
        Assert.False(A.IsInvertible);
        A.RotateAboutZAxis(30.0);
        Assert.False(A.IsInvertible);
        A.Translate3D(6.0f, 7.0f, 8.0f);
        if (A.IsInvertible)
        {
            // Due to some computation errors, now A may become invertible with a tiny determinant.
            Assert.Equal(0.0, A.Determinant, 1e-12);
        }

        // A degenerate matrix of all zeros is not invertible.
        A.MakeIdentity();
        A.set_rc(0, 0, 0.0f);
        A.set_rc(1, 1, 0.0f);
        A.set_rc(2, 2, 0.0f);
        A.set_rc(3, 3, 0.0f);
        Assert.False(A.IsInvertible);
    }

    [Fact]
    private static void TestVerifyIsIdentity()
    {
        Transform A = GetTestMatrix1();
        Assert.False(A.IsIdentity);

        A.MakeIdentity();
        Assert.True(A.IsIdentity);

        // Modifying any one individual element should cause the matrix to no longer
        // be identity.
        A.MakeIdentity();
        A.set_rc(0, 0, 2.0f);
        Assert.False(A.IsIdentity);

        A.MakeIdentity();
        A.set_rc(1, 0, 2.0f);
        Assert.False(A.IsIdentity);

        A.MakeIdentity();
        A.set_rc(2, 0, 2.0f);
        Assert.False(A.IsIdentity);

        A.MakeIdentity();
        A.set_rc(3, 0, 2.0f);
        Assert.False(A.IsIdentity);

        A.MakeIdentity();
        A.set_rc(0, 1, 2.0f);
        Assert.False(A.IsIdentity);

        A.MakeIdentity();
        A.set_rc(1, 1, 2.0f);
        Assert.False(A.IsIdentity);

        A.MakeIdentity();
        A.set_rc(2, 1, 2.0f);
        Assert.False(A.IsIdentity);

        A.MakeIdentity();
        A.set_rc(3, 1, 2.0f);
        Assert.False(A.IsIdentity);

        A.MakeIdentity();
        A.set_rc(0, 2, 2.0f);
        Assert.False(A.IsIdentity);

        A.MakeIdentity();
        A.set_rc(1, 2, 2.0f);
        Assert.False(A.IsIdentity);

        A.MakeIdentity();
        A.set_rc(2, 2, 2.0f);
        Assert.False(A.IsIdentity);

        A.MakeIdentity();
        A.set_rc(3, 2, 2.0f);
        Assert.False(A.IsIdentity);

        A.MakeIdentity();
        A.set_rc(0, 3, 2.0f);
        Assert.False(A.IsIdentity);

        A.MakeIdentity();
        A.set_rc(1, 3, 2.0f);
        Assert.False(A.IsIdentity);

        A.MakeIdentity();
        A.set_rc(2, 3, 2.0f);
        Assert.False(A.IsIdentity);

        A.MakeIdentity();
        A.set_rc(3, 3, 2.0f);
        Assert.False(A.IsIdentity);
    }

    [Fact]
    private static void TestVerifyIsIdentityOrTranslation()
    {
        Transform A = GetTestMatrix1();
        Assert.False(A.IsIdentityOrTranslation);

        A.MakeIdentity();
        Assert.True(A.IsIdentityOrTranslation);

        // Modifying any non-translation components should cause
        // IsIdentityOrTranslation() to return false. NOTE: (0, 3), (1, 3), and
        // (2, 3) are the translation components, so modifying them should still
        // return true.
        A.MakeIdentity();
        A.set_rc(0, 0, 2.0f);
        Assert.False(A.IsIdentityOrTranslation);

        A.MakeIdentity();
        A.set_rc(1, 0, 2.0f);
        Assert.False(A.IsIdentityOrTranslation);

        A.MakeIdentity();
        A.set_rc(2, 0, 2.0f);
        Assert.False(A.IsIdentityOrTranslation);

        A.MakeIdentity();
        A.set_rc(3, 0, 2.0f);
        Assert.False(A.IsIdentityOrTranslation);

        A.MakeIdentity();
        A.set_rc(0, 1, 2.0f);
        Assert.False(A.IsIdentityOrTranslation);

        A.MakeIdentity();
        A.set_rc(1, 1, 2.0f);
        Assert.False(A.IsIdentityOrTranslation);

        A.MakeIdentity();
        A.set_rc(2, 1, 2.0f);
        Assert.False(A.IsIdentityOrTranslation);

        A.MakeIdentity();
        A.set_rc(3, 1, 2.0f);
        Assert.False(A.IsIdentityOrTranslation);

        A.MakeIdentity();
        A.set_rc(0, 2, 2.0f);
        Assert.False(A.IsIdentityOrTranslation);

        A.MakeIdentity();
        A.set_rc(1, 2, 2.0f);
        Assert.False(A.IsIdentityOrTranslation);

        A.MakeIdentity();
        A.set_rc(2, 2, 2.0f);
        Assert.False(A.IsIdentityOrTranslation);

        A.MakeIdentity();
        A.set_rc(3, 2, 2.0f);
        Assert.False(A.IsIdentityOrTranslation);

        // Note carefully - expecting true here.
        A.MakeIdentity();
        A.set_rc(0, 3, 2.0f);
        Assert.True(A.IsIdentityOrTranslation);

        // Note carefully - expecting true here.
        A.MakeIdentity();
        A.set_rc(1, 3, 2.0f);
        Assert.True(A.IsIdentityOrTranslation);

        // Note carefully - expecting true here.
        A.MakeIdentity();
        A.set_rc(2, 3, 2.0f);
        Assert.True(A.IsIdentityOrTranslation);

        A.MakeIdentity();
        A.set_rc(3, 3, 2.0f);
        Assert.False(A.IsIdentityOrTranslation);
    }

    [Fact]
    private static void TestApproximatelyIdentityOrTranslation()
    {
        double kBigError = 1e-4;
        double kSmallError = float.MachineEpsilon / 2.0;

        // Exact pure translation.
        Transform a = new();
        Assert.True(a.IsApproximatelyIdentityOrTranslation(0));
        Assert.True(a.IsApproximatelyIdentityOrTranslation(kBigError));
        Assert.True(a.IsApproximatelyIdentityOrIntegerTranslation(0));
        Assert.True(a.IsApproximatelyIdentityOrIntegerTranslation(kBigError));

        // Set translate values to integer values other than 0 or 1.
        a.set_rc(0, 3, 3);
        a.set_rc(1, 3, 4);
        a.set_rc(2, 3, 5);

        Assert.True(a.IsApproximatelyIdentityOrTranslation(0));
        Assert.True(a.IsApproximatelyIdentityOrTranslation(kBigError));
        Assert.True(a.IsApproximatelyIdentityOrIntegerTranslation(0));
        Assert.True(a.IsApproximatelyIdentityOrIntegerTranslation(kBigError));

        // Set translate values to values other than 0 or 1.
        a.set_rc(0, 3, 3.4f);
        a.set_rc(1, 3, 4.4f);
        a.set_rc(2, 3, 5.6f);

        Assert.True(a.IsApproximatelyIdentityOrTranslation(0));
        Assert.True(a.IsApproximatelyIdentityOrTranslation(kBigError));
        Assert.False(a.IsApproximatelyIdentityOrIntegerTranslation(0));
        Assert.False(a.IsApproximatelyIdentityOrIntegerTranslation(kBigError));

        // Approximately pure translation.
        a = ApproxIdentityMatrix(kBigError);

        // All these are false because the perspective error is bigger than the
        // allowed std::min(float_epsilon, tolerance);
        Assert.False(a.IsApproximatelyIdentityOrTranslation(0));
        Assert.False(a.IsApproximatelyIdentityOrTranslation(kBigError));
        Assert.False(a.IsApproximatelyIdentityOrTranslation(kSmallError));
        Assert.False(a.IsApproximatelyIdentityOrIntegerTranslation(0));
        Assert.False(a.IsApproximatelyIdentityOrTranslation(kBigError));
        Assert.False(a.IsApproximatelyIdentityOrIntegerTranslation(kSmallError));

        // Set perspective components to be exact identity.
        a.set_rc(3, 0, 0);
        a.set_rc(3, 1, 0);
        a.set_rc(3, 2, 0);
        a.set_rc(3, 3, 1);

        Assert.False(a.IsApproximatelyIdentityOrTranslation(0));
        Assert.True(a.IsApproximatelyIdentityOrTranslation(kBigError));
        Assert.False(a.IsApproximatelyIdentityOrTranslation(kSmallError));
        Assert.False(a.IsApproximatelyIdentityOrIntegerTranslation(0));
        Assert.True(a.IsApproximatelyIdentityOrIntegerTranslation(kBigError));
        Assert.False(a.IsApproximatelyIdentityOrIntegerTranslation(kSmallError));

        // Set translate values to values other than 0 or 1.
        // The error is set to kBigError / 2 instead of kBigError because the
        // arithmetic may make the error bigger.
        a.set_rc(0, 3, 3.0 + kBigError / 2);
        a.set_rc(1, 3, 4.0 + kBigError / 2);
        a.set_rc(2, 3, 5.0 + kBigError / 2);

        Assert.False(a.IsApproximatelyIdentityOrTranslation(0));
        Assert.True(a.IsApproximatelyIdentityOrTranslation(kBigError));
        Assert.False(a.IsApproximatelyIdentityOrTranslation(kSmallError));
        Assert.False(a.IsApproximatelyIdentityOrIntegerTranslation(0));
        Assert.True(a.IsApproximatelyIdentityOrIntegerTranslation(kBigError));
        Assert.False(a.IsApproximatelyIdentityOrIntegerTranslation(kSmallError));

        // Set translate values to values other than 0 or 1.
        a.set_rc(0, 3, 3.4f);
        a.set_rc(1, 3, 4.4f);
        a.set_rc(2, 3, 5.6f);

        Assert.False(a.IsApproximatelyIdentityOrTranslation(0));
        Assert.True(a.IsApproximatelyIdentityOrTranslation(kBigError));
        Assert.False(a.IsApproximatelyIdentityOrTranslation(kSmallError));
        Assert.False(a.IsApproximatelyIdentityOrIntegerTranslation(0));
        Assert.False(a.IsApproximatelyIdentityOrIntegerTranslation(kBigError));
        Assert.False(a.IsApproximatelyIdentityOrIntegerTranslation(kSmallError));

        // Test with kSmallError in the matrix.
        a = ApproxIdentityMatrix(kSmallError);

        Assert.False(a.IsApproximatelyIdentityOrTranslation(0));
        Assert.True(a.IsApproximatelyIdentityOrTranslation(kBigError));
        Assert.True(a.IsApproximatelyIdentityOrTranslation(kSmallError));
        Assert.False(a.IsApproximatelyIdentityOrIntegerTranslation(0));
        Assert.True(a.IsApproximatelyIdentityOrTranslation(kBigError));
        Assert.True(a.IsApproximatelyIdentityOrIntegerTranslation(kSmallError));

        // Set some values (not translate values) to values other than 0 or 1.
        a.set_rc(0, 1, 3.4f);
        a.set_rc(3, 2, 4.4f);
        a.set_rc(2, 0, 5.6f);

        Assert.False(a.IsApproximatelyIdentityOrTranslation(0));
        Assert.False(a.IsApproximatelyIdentityOrTranslation(kBigError));
        Assert.False(a.IsApproximatelyIdentityOrTranslation(kSmallError));
        Assert.False(a.IsApproximatelyIdentityOrIntegerTranslation(0));
        Assert.False(a.IsApproximatelyIdentityOrIntegerTranslation(kBigError));
        Assert.False(a.IsApproximatelyIdentityOrIntegerTranslation(kSmallError));
    }

    [Fact]
    private static void TestRoundToIdentityOrIntegerTranslation()
    {
        Transform a = ApproxIdentityMatrix(0.1);
        Assert.False(a.IsIdentityOrIntegerTranslation());
        a.RoundToIdentityOrIntegerTranslation();
        Assert.True(a.IsIdentity);
        Assert.True(a.IsIdentityOrIntegerTranslation());

        a.Translate3D(1.1f, 2.2f, 3.8f);
        Assert.False(a.IsIdentityOrIntegerTranslation());
        a.RoundToIdentityOrIntegerTranslation();
        Assert.True(a.IsIdentityOrIntegerTranslation());
        Assert.Equal(1.0, a.rc(0, 3));
        Assert.Equal(2.0, a.rc(1, 3));
        Assert.Equal(4.0, a.rc(2, 3));
    }

    [Fact]
    private static void TestVerifyIsScaleOrTranslation()
    {
        Assert.True(new Transform().IsScaleOrTranslation);
        Assert.True(Transform.MakeScale(2, 3).IsScaleOrTranslation);
        Assert.True(Transform.MakeTranslation(4, 5).IsScaleOrTranslation);
        Assert.True((Transform.MakeTranslation(4, 5) * Transform.MakeScale(2, 3))
                        .IsScaleOrTranslation);

        Transform A = GetTestMatrix1();
        Assert.False(A.IsScaleOrTranslation);

        // Modifying any non-scale or non-translation components should cause
        // IsScaleOrTranslation() to return false. (0, 0), (1, 1), (2, 2), (0, 3),
        // (1, 3), and (2, 3) are the scale and translation components, so
        // modifying them should still return true.

        // Note carefully - expecting true here.
        A.MakeIdentity();
        Assert.True(A.IsScaleOrTranslation);
        A.set_rc(0, 0, 2.0f);
        Assert.True(A.IsScaleOrTranslation);

        A.MakeIdentity();
        A.set_rc(1, 0, 2.0f);
        Assert.False(A.IsScaleOrTranslation);

        A.MakeIdentity();
        A.set_rc(2, 0, 2.0f);
        Assert.False(A.IsScaleOrTranslation);

        A.MakeIdentity();
        A.set_rc(3, 0, 2.0f);
        Assert.False(A.IsScaleOrTranslation);

        A.MakeIdentity();
        A.set_rc(0, 1, 2.0f);
        Assert.False(A.IsScaleOrTranslation);

        // Note carefully - expecting true here.
        A.MakeIdentity();
        A.set_rc(1, 1, 2.0f);
        Assert.True(A.IsScaleOrTranslation);

        A.MakeIdentity();
        A.set_rc(2, 1, 2.0f);
        Assert.False(A.IsScaleOrTranslation);

        A.MakeIdentity();
        A.set_rc(3, 1, 2.0f);
        Assert.False(A.IsScaleOrTranslation);

        A.MakeIdentity();
        A.set_rc(0, 2, 2.0f);
        Assert.False(A.IsScaleOrTranslation);

        A.MakeIdentity();
        A.set_rc(1, 2, 2.0f);
        Assert.False(A.IsScaleOrTranslation);

        // Note carefully - expecting true here.
        A.MakeIdentity();
        A.set_rc(2, 2, 2.0f);
        Assert.True(A.IsScaleOrTranslation);

        A.MakeIdentity();
        A.set_rc(3, 2, 2.0f);
        Assert.False(A.IsScaleOrTranslation);

        // Note carefully - expecting true here.
        A.MakeIdentity();
        A.set_rc(0, 3, 2.0f);
        Assert.True(A.IsScaleOrTranslation);

        // Note carefully - expecting true here.
        A.MakeIdentity();
        A.set_rc(1, 3, 2.0f);
        Assert.True(A.IsScaleOrTranslation);

        // Note carefully - expecting true here.
        A.MakeIdentity();
        A.set_rc(2, 3, 2.0f);
        Assert.True(A.IsScaleOrTranslation);

        A.MakeIdentity();
        A.set_rc(3, 3, 2.0f);
        Assert.False(A.IsScaleOrTranslation);
    }

    [Fact]
    private static void TestTo2DScale()
    {
        Transform t = new();
        Assert.True(t.IsScale2D);
        Assert.Equal(new Vector2DF(1, 1), t.To2DScale());

        t.Scale(2.5f, 3.75f);
        Assert.True(t.IsScale2D);
        Assert.Equal(new Vector2DF(2.5f, 3.75f), t.To2DScale());

        t.EnsureFullMatrix();
        Assert.True(t.IsScale2D);
        Assert.Equal(new Vector2DF(2.5f, 3.75f), t.To2DScale());

        t.Scale3D(3, 4, 5);
        Assert.False(t.IsScale2D);
        Assert.Equal(new Vector2DF(7.5f, 15.0f), t.To2DScale());

        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                t.MakeIdentity();
                t.set_rc(row, col, 100);
                bool is_scale_2d = row == col && (row == 0 || row == 1);
                Assert.Equal(is_scale_2d, t.IsScale2D);
            }
        }
    }

    [Fact]
    private static void TestFlatten()
    {
        Transform A = GetTestMatrix1();
        Assert.False(A.IsFlat);

        A.Flatten();
        EXPECT_ROW0_EQ(10.0f, 14.0f, 0.0f, 22.0f, A);
        EXPECT_ROW1_EQ(11.0f, 15.0f, 0.0f, 23.0f, A);
        EXPECT_ROW2_EQ(0.0f, 0.0f, 1.0f, 0.0f, A);
        EXPECT_ROW3_EQ(13.0f, 17.0f, 0.0f, 25.0f, A);

        Assert.True(A.IsFlat);
    }

    [Fact]
    private static void TestIsFlat()
    {
        Transform transform = GetTestMatrix1();

        // A transform with all entries non-zero isn't flat.
        Assert.False(transform.IsFlat);

        transform.set_rc(0, 2, 0.0f);
        transform.set_rc(1, 2, 0.0f);
        transform.set_rc(2, 2, 1.0f);
        transform.set_rc(3, 2, 0.0f);

        Assert.False(transform.IsFlat);

        transform.set_rc(2, 0, 0.0f);
        transform.set_rc(2, 1, 0.0f);
        transform.set_rc(2, 3, 0.0f);

        // Since the third column and row are both (0, 0, 1, 0), the transform is flat.
        Assert.True(transform.IsFlat);
    }

    [Fact]
    private static void TestPreserves2dAffine()
    {
        (Transform transform, bool expected)[] test_cases =
        [
            // Skew z axis in x and y direction
            (
                Transform.ColMajor(1.0, 0.0, 0.0, 0.0,
                                   0.0, 1.0, 0.0, 0.0,
                                   0.1, 0.1, 1.0, 0.0,
                                   0.0, 0.0, 0.0, 1.0),
                true
            ),
            // Scale z axis
            (
                Transform.ColMajor(1.0, 0.0, 0.0, 0.0,
                                   0.0, 1.0, 0.0, 0.0,
                                   0.0, 0.0, 2.0, 0.0,
                                   0.0, 0.0, 0.0, 1.0),
                true
            ),
            // Perspective projection along the z axis
            (
                Transform.ColMajor(1.0, 0.0, 0.0, 0.0,
                                   0.0, 1.0, 0.0, 0.0,
                                   0.0, 0.0, 1.0, 0.1,
                                   0.0, 0.0, 0.0, 1.0),
                true
            ),
            // All together, including x and y axis skew and translation
            (
                Transform.ColMajor(1.0, 0.1, 0.0, 0.0,
                                   0.1, 1.0, 0.0, 0.0,
                                   0.1, 0.1, 2.0, 0.1,
                                   0.1, 0.1, 0.0, 1.0),
                true
            ),
            // Skew x axis in the z direction.
           (
                Transform.ColMajor(1.0, 0.0, 0.1, 0.0,
                                   0.0, 1.0, 0.0, 0.0,
                                   0.0, 0.0, 1.0, 0.0,
                                   0.0, 0.0, 0.0, 1.0),
                false
           ),
           // Add y perspective
            (
                Transform.ColMajor(1.0, 0.0, 0.0, 0.0,
                                   0.0, 1.0, 0.0, 0.1,
                                   0.0, 0.0, 1.0, 0.0,
                                   0.0, 0.0, 0.0, 1.0),
                false
            ),
            // Add z translation
            (
                Transform.ColMajor(1.0, 0.0, 0.0, 0.0,
                                   0.0, 1.0, 0.0, 0.1,
                                   0.0, 0.0, 1.0, 0.0,
                                   0.0, 0.0, 0.1, 1.0),
                false
            )
        ];

        // Another implementation of Preserves2dAffine that isn't as fast, good for testing the faster implementation.
        bool EmpiricallyPreserves2dAffine(in Transform transform)
        {
            Point3F p1 = new(5.0f, 5.0f, 0.0f);
            Point3F p2 = new(10.0f, 5.0f, 0.0f);
            Point3F p3 = new(10.0f, 20.0f, 0.0f);
            Point3F p4 = new(5.0f, 20.0f, 0.0f);

            var test_quad = new QuadF(new PointF(p1.X, p1.Y), new PointF(p2.X, p2.Y),
                    new PointF(p3.X, p3.Y), new PointF(p4.X, p4.Y));
            Assert.True(test_quad.IsRectilinear());

            p1 = transform.MapPoint(p1);
            p2 = transform.MapPoint(p2);
            p3 = transform.MapPoint(p3);
            p4 = transform.MapPoint(p4);

            // We expect our quad on the x/y plane to remain so.
            if (p1.Z != 0 || p2.Z != 0 || p3.Z != 0 || p4.Z != 0)
            {
                return false;
            }

            // In an affine transform, parallel lines are preserved.
            return Vector3DF.CrossProduct(p2 - p1, p3 - p4).IsZero() &&
                   Vector3DF.CrossProduct(p4 - p1, p3 - p2).IsZero();
        }
        ;

        foreach (var (transform, expected) in test_cases)
        {
#if DEBUG
            Debug.WriteLine($"transform = {transform}, expected = {expected}");
#endif
            if (expected)
            {
                Assert.True(EmpiricallyPreserves2dAffine(transform));
                Assert.True(transform.Preserves2dAffine());
            }
            else
            {
                Assert.False(EmpiricallyPreserves2dAffine(transform));
                Assert.False(transform.Preserves2dAffine());
            }
        }
    }

    // Another implementation of Preserves2dAxisAlignment that isn't as fast,
    // good for testing the faster implementation.
    private static bool EmpiricallyPreserves2dAxisAlignment(in Transform transform)
    {
        Point3F p1 = new(5.0f, 5.0f, 0.0f);
        Point3F p2 = new(10.0f, 5.0f, 0.0f);
        Point3F p3 = new(10.0f, 20.0f, 0.0f);
        Point3F p4 = new(5.0f, 20.0f, 0.0f);

        var test_quad = new QuadF(new PointF(p1.X, p1.Y), new PointF(p2.X, p2.Y),
                  new PointF(p3.X, p3.Y), new PointF(p4.X, p4.Y));
        Assert.True(test_quad.IsRectilinear());

        p1 = transform.MapPoint(p1);
        p2 = transform.MapPoint(p2);
        p3 = transform.MapPoint(p3);
        p4 = transform.MapPoint(p4);

        QuadF transformedQuad = new(new PointF(p1.X, p1.Y), new PointF(p2.X, p2.Y),
                        new PointF(p3.X, p3.Y), new PointF(p4.X, p4.Y));
        return transformedQuad.IsRectilinear();
    }

    [Fact]
    private static void TestPreserves2dAxisAlignment()
    {
        (
        double a,  // row 1, column 1
        double b,  // row 1, column 2
        double c,  // row 2, column 1
        double d,  // row 2, column 2
        bool expected,
        bool degenerate)[] test_cases =
        [
            ( 3.0, 0.0,
            0.0, 4.0, true, false ),  // basic case
			( 0.0, 4.0,
            3.0, 0.0, true, false ),  // rotate by 90
			( 0.0, 0.0,
            0.0, 4.0, true, true ),   // degenerate x
			( 3.0, 0.0,
            0.0, 0.0, true, true ),   // degenerate y
			( 0.0, 0.0,
            3.0, 0.0, true, true ),   // degenerate x + rotate by 90
			( 0.0, 4.0,
            0.0, 0.0, true, true ),   // degenerate y + rotate by 90
			( 3.0, 4.0,
            0.0, 0.0, false, true ),
            ( 0.0, 0.0,
            3.0, 4.0, false, true ),
            ( 0.0, 3.0,
            0.0, 4.0, false, true ),
            ( 3.0, 0.0,
            4.0, 0.0, false, true ),
            ( 3.0, 4.0,
            5.0, 0.0, false, false ),
            ( 3.0, 4.0,
            0.0, 5.0, false, false ),
            ( 3.0, 0.0,
            4.0, 5.0, false, false ),
            ( 0.0, 3.0,
            4.0, 5.0, false, false ),
            ( 2.0, 3.0,
            4.0, 5.0, false, false )
        ];

        Transform transform = new();
        foreach (var (a, b, c, d, expected, degenerate) in test_cases)
        {
            transform.MakeIdentity();
            transform.set_rc(0, 0, a);
            transform.set_rc(0, 1, b);
            transform.set_rc(1, 0, c);
            transform.set_rc(1, 1, d);

            if (expected)
            {
                Assert.True(EmpiricallyPreserves2dAxisAlignment(transform));
                Assert.True(transform.Preserves2dAxisAlignment());
                if (degenerate)
                {
                    Assert.False(transform.NonDegeneratePreserves2dAxisAlignment());
                }
                else
                {
                    Assert.True(transform.NonDegeneratePreserves2dAxisAlignment());
                }
            }
            else
            {
                Assert.False(EmpiricallyPreserves2dAxisAlignment(transform));
                Assert.False(transform.Preserves2dAxisAlignment());
                Assert.False(transform.NonDegeneratePreserves2dAxisAlignment());
            }
        }

        // Try the same test cases again, but this time make sure that other matrix
        // elements (except perspective) have entries, to test that they are ignored.
        foreach (var (a, b, c, d, expected, degenerate) in test_cases)
        {
            transform.MakeIdentity();
            transform.set_rc(0, 0, a);
            transform.set_rc(0, 1, b);
            transform.set_rc(1, 0, c);
            transform.set_rc(1, 1, d);

            transform.set_rc(0, 2, 1.0f);
            transform.set_rc(0, 3, 2.0f);
            transform.set_rc(1, 2, 3.0f);
            transform.set_rc(1, 3, 4.0f);
            transform.set_rc(2, 0, 5.0f);
            transform.set_rc(2, 1, 6.0f);
            transform.set_rc(2, 2, 7.0f);
            transform.set_rc(2, 3, 8.0f);

            if (expected)
            {
                Assert.True(EmpiricallyPreserves2dAxisAlignment(transform));
                Assert.True(transform.Preserves2dAxisAlignment());
                if (degenerate)
                {
                    Assert.False(transform.NonDegeneratePreserves2dAxisAlignment());
                }
                else
                {
                    Assert.True(transform.NonDegeneratePreserves2dAxisAlignment());
                }
            }
            else
            {
                Assert.False(EmpiricallyPreserves2dAxisAlignment(transform));
                Assert.False(transform.Preserves2dAxisAlignment());
                Assert.False(transform.NonDegeneratePreserves2dAxisAlignment());
            }
        }

        // Try the same test cases again, but this time add perspective which is
        // always assumed to not-preserve axis alignment.
        foreach (var (a, b, c, d, expected, degenerate) in test_cases)
        {
            transform.MakeIdentity();
            transform.set_rc(0, 0, a);
            transform.set_rc(0, 1, b);
            transform.set_rc(1, 0, c);
            transform.set_rc(1, 1, d);

            transform.set_rc(0, 2, 1.0f);
            transform.set_rc(0, 3, 2.0f);
            transform.set_rc(1, 2, 3.0f);
            transform.set_rc(1, 3, 4.0f);
            transform.set_rc(2, 0, 5.0f);
            transform.set_rc(2, 1, 6.0f);
            transform.set_rc(2, 2, 7.0f);
            transform.set_rc(2, 3, 8.0f);
            transform.set_rc(3, 0, 9.0f);
            transform.set_rc(3, 1, 10.0f);
            transform.set_rc(3, 2, 11.0f);
            transform.set_rc(3, 3, 12.0f);

            Assert.False(EmpiricallyPreserves2dAxisAlignment(transform));
            Assert.False(transform.Preserves2dAxisAlignment());
            Assert.False(transform.NonDegeneratePreserves2dAxisAlignment());
        }

        // Try a few more practical situations to check precision
        transform.MakeIdentity();
        double kNear90Degrees = 90.0 + kErrorThreshold / 2;
        transform.RotateAboutZAxis(kNear90Degrees);
        Assert.True(EmpiricallyPreserves2dAxisAlignment(transform));
        Assert.True(transform.Preserves2dAxisAlignment());
        Assert.True(transform.NonDegeneratePreserves2dAxisAlignment());

        transform.MakeIdentity();
        transform.RotateAboutZAxis(kNear90Degrees * 2);
        Assert.True(EmpiricallyPreserves2dAxisAlignment(transform));
        Assert.True(transform.Preserves2dAxisAlignment());
        Assert.True(transform.NonDegeneratePreserves2dAxisAlignment());

        transform.MakeIdentity();
        transform.RotateAboutZAxis(kNear90Degrees * 3);
        Assert.True(EmpiricallyPreserves2dAxisAlignment(transform));
        Assert.True(transform.Preserves2dAxisAlignment());
        Assert.True(transform.NonDegeneratePreserves2dAxisAlignment());

        transform.MakeIdentity();
        transform.RotateAboutYAxis(kNear90Degrees);
        Assert.True(EmpiricallyPreserves2dAxisAlignment(transform));
        Assert.True(transform.Preserves2dAxisAlignment());
        Assert.False(transform.NonDegeneratePreserves2dAxisAlignment());

        transform.MakeIdentity();
        transform.RotateAboutXAxis(kNear90Degrees);
        Assert.True(EmpiricallyPreserves2dAxisAlignment(transform));
        Assert.True(transform.Preserves2dAxisAlignment());
        Assert.False(transform.NonDegeneratePreserves2dAxisAlignment());

        transform.MakeIdentity();
        transform.RotateAboutZAxis(kNear90Degrees);
        transform.RotateAboutYAxis(kNear90Degrees);
        Assert.True(EmpiricallyPreserves2dAxisAlignment(transform));
        Assert.True(transform.Preserves2dAxisAlignment());
        Assert.False(transform.NonDegeneratePreserves2dAxisAlignment());

        transform.MakeIdentity();
        transform.RotateAboutZAxis(kNear90Degrees);
        transform.RotateAboutXAxis(kNear90Degrees);
        Assert.True(EmpiricallyPreserves2dAxisAlignment(transform));
        Assert.True(transform.Preserves2dAxisAlignment());
        Assert.False(transform.NonDegeneratePreserves2dAxisAlignment());

        transform.MakeIdentity();
        transform.RotateAboutYAxis(kNear90Degrees);
        transform.RotateAboutZAxis(kNear90Degrees);
        Assert.True(EmpiricallyPreserves2dAxisAlignment(transform));
        Assert.True(transform.Preserves2dAxisAlignment());
        Assert.False(transform.NonDegeneratePreserves2dAxisAlignment());

        transform.MakeIdentity();
        transform.RotateAboutZAxis(45.0);
        Assert.False(EmpiricallyPreserves2dAxisAlignment(transform));
        Assert.False(transform.Preserves2dAxisAlignment());
        Assert.False(transform.NonDegeneratePreserves2dAxisAlignment());

        // 3-d case; In 2d after an orthographic projection, this case does
        // preserve 2d axis alignment. But in 3d, it does not preserve axis
        // alignment.
        transform.MakeIdentity();
        transform.RotateAboutYAxis(45.0);
        Assert.True(EmpiricallyPreserves2dAxisAlignment(transform));
        Assert.True(transform.Preserves2dAxisAlignment());
        Assert.True(transform.NonDegeneratePreserves2dAxisAlignment());

        transform.MakeIdentity();
        transform.RotateAboutXAxis(45.0);
        Assert.True(EmpiricallyPreserves2dAxisAlignment(transform));
        Assert.True(transform.Preserves2dAxisAlignment());
        Assert.True(transform.NonDegeneratePreserves2dAxisAlignment());

        // Perspective cases.
        transform.MakeIdentity();
        transform.ApplyPerspectiveDepth(10.0);
        transform.RotateAboutYAxis(45.0);
        Assert.False(EmpiricallyPreserves2dAxisAlignment(transform));
        Assert.False(transform.Preserves2dAxisAlignment());
        Assert.False(transform.NonDegeneratePreserves2dAxisAlignment());

        transform.MakeIdentity();
        transform.ApplyPerspectiveDepth(10.0);
        transform.RotateAboutZAxis(90.0);
        Assert.True(EmpiricallyPreserves2dAxisAlignment(transform));
        Assert.True(transform.Preserves2dAxisAlignment());
        Assert.True(transform.NonDegeneratePreserves2dAxisAlignment());

        transform.MakeIdentity();
        transform.ApplyPerspectiveDepth(-10.0);
        transform.RotateAboutZAxis(90.0);
        Assert.True(EmpiricallyPreserves2dAxisAlignment(transform));
        Assert.True(transform.Preserves2dAxisAlignment());
        Assert.True(transform.NonDegeneratePreserves2dAxisAlignment());

        // To be non-degenerate, the constant contribution to perspective must
        // be positive.

        transform = Transform.RowMajor(1.0, 0.0, 0.0, 0.0,
                                       0.0, 1.0, 0.0, 0.0,
                                       0.0, 0.0, 1.0, 0.0,
                                       0.0, 0.0, 0.0,-1.0);

        Assert.True(EmpiricallyPreserves2dAxisAlignment(transform));
        Assert.True(transform.Preserves2dAxisAlignment());
        Assert.False(transform.NonDegeneratePreserves2dAxisAlignment());

        transform = Transform.RowMajor(2.0, 0.0, 0.0, 0.0,
                                       0.0, 5.0, 0.0, 0.0,
                                       0.0, 0.0, 1.0, 0.0,
                                       0.0, 0.0, 0.0, 0.0);

        Assert.True(EmpiricallyPreserves2dAxisAlignment(transform));
        Assert.True(transform.Preserves2dAxisAlignment());
        Assert.False(transform.NonDegeneratePreserves2dAxisAlignment());
    }

    [Fact]
    private static void TestTo2dTranslation()
    {
        Vector2DF translation = new(3.0f, 7.0f);
        Transform transform = new();
        transform.Translate(translation.X, translation.Y + 1);
        Assert.NotEqual(translation, transform.To2dTranslation());
        transform.MakeIdentity();
        transform.Translate(translation.X, translation.Y);
        transform.set_rc(1, 1, 100);
        Assert.Equal(translation, transform.To2dTranslation());
    }

    [Fact]
    private static void TestTo3dTranslation()
    {
        Transform transform = new();
        Assert.Equal(new Vector3DF(), transform.To3dTranslation());
        transform.Translate(10, 20);
        Assert.Equal(new Vector3DF(10, 20, 0), transform.To3dTranslation());
        transform.Translate3D(20, -60, -10);
        Assert.Equal(new Vector3DF(30, -40, -10), transform.To3dTranslation());
        transform.set_rc(1, 1, 100);
        Assert.Equal(new Vector3DF(30, -40, -10), transform.To3dTranslation());
    }

    [Fact]
    private static void TestMapRect()
    {
        Transform translation = Transform.MakeTranslation(3.25f, 7.75f);
        RectF rect = new(1.25f, 2.5f, 3.75f, 4.0f);
        RectF expected = new(4.5f, 10.25f, 3.75f, 4.0f);
        Assert.Equal(expected, translation.MapRect(rect));

        Assert.Equal(rect, new Transform().MapRect(rect));

        var singular = Transform.MakeScale(0.0f);
        Assert.Equal(new RectF(0, 0, 0, 0), singular.MapRect(rect));

        var negative_scale = Transform.MakeScale(-1, -2);
        Assert.Equal(new RectF(-5.0f, -13.0f, 3.75f, 8.0f), negative_scale.MapRect(rect));

        var rotate = Transform.Make90degRotation();
        Assert.Equal(new RectF(-6.5f, 1.25f, 4.0f, 3.75f), rotate.MapRect(rect));
    }

    [Fact]
    private static void TestMapIntRect()
    {
        var translation = Transform.MakeTranslation(3.25f, 7.75f);
        Assert.Equal(new Rect(4, 9, 4, 5), translation.MapRect(new Rect(1, 2, 3, 4)));

        Assert.Equal(new Rect(1, 2, 3, 4), new Transform().MapRect(new Rect(1, 2, 3, 4)));

        var singular = Transform.MakeScale(0.0f);
        Assert.Equal(new Rect(0, 0, 0, 0), singular.MapRect(new Rect(1, 2, 3, 4)));
    }

    [Fact]
    private static void TestTransformRectReverse()
    {
        RectF result;

        var translation = Transform.MakeTranslation(3.25f, 7.75f);
        RectF rect = new(1.25f, 2.5f, 3.75f, 4.0f);
        RectF expected = new(-2.0f, -5.25f, 3.75f, 4.0f);
        translation.InverseMapRect(rect, out result);
        Assert.Equal(expected, result);

        new Transform().InverseMapRect(rect, out result);
        Assert.Equal(rect, result);

        var singular = Transform.MakeScale(0.0f);
        Assert.False(singular.InverseMapRect(rect, out result));

        var negative_scale = Transform.MakeScale(-1, -2);
        negative_scale.InverseMapRect(rect, out result);
        Assert.Equal(new RectF(-5.0f, -3.25f, 3.75f, 2.0f), result);

        var rotate = Transform.Make90degRotation();
        rotate.InverseMapRect(rect, out result);
        Assert.Equal(new RectF(2.5f, -5.0f, 4.0f, 3.75f), result);
    }

    [Fact]
    private static void TestInverseMapIntRect()
    {
        Rect result;

        var translation = Transform.MakeTranslation(3.25f, 7.75f);
        translation.InverseMapRect(new Rect(1, 2, 3, 4), out result);
        Assert.Equal(new Rect(-3, -6, 4, 5), result);

        new Transform().InverseMapRect(new Rect(1, 2, 3, 4), out result);
        Assert.Equal(new Rect(1, 2, 3, 4), result);
        
        var singular = Transform.MakeScale(0.0f);
        Assert.False(singular.InverseMapRect(new Rect(1, 2, 3, 4), out result));
    }

    [Fact]
    private static void TestMapQuad()
    {
        var translation = Transform.MakeTranslation(3.25f, 7.75f);
        QuadF q = new QuadF(new PointF(1.25f, 2.5f), new PointF(3.75f, 4.0f), new PointF(23.0f, 45.0f),
          new PointF(12.0f, 67.0f));
        Assert.Equal(new QuadF(new PointF(4.5f, 10.25f), new PointF(7.0f, 11.75f),
                        new PointF(26.25f, 52.75f), new PointF(15.25f, 74.75f)),
                  translation.MapQuad(q));

        Assert.Equal(q, new Transform().MapQuad(q));

        var singular = Transform.MakeScale(0.0f);
        Assert.Equal(new QuadF(), singular.MapQuad(q));

        var negative_scale = Transform.MakeScale(-1, -2);
        Assert.Equal(new QuadF(new PointF(-1.25f, -5.0f), new PointF(-3.75f, -8.0f),
                        new PointF(-23.0f, -90.0f), new PointF(-12.0f, -134.0f)),
                  negative_scale.MapQuad(q));

        var rotate = Transform.Make90degRotation();
        Assert.Equal(new QuadF(new PointF(-2.5f, 1.25f), new PointF(-4.0f, 3.75f),
                        new PointF(-45.0f, 23.0f), new PointF(-67.0f, 12.0f)),
                  rotate.MapQuad(q));
    }

    [Fact]
    private static void TestMapBox()
    {
        Transform translation = new();
        translation.Translate3D(3.0f, 7.0f, 6.0f);
        BoxF box = new(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f);
        BoxF expected = new(4.0f, 9.0f, 9.0f, 4.0f, 5.0f, 6.0f);
        BoxF transformed = translation.MapBox(box);
        Assert.Equal(expected, transformed);
    }

    [Fact]
    private static void TestRound2dTranslationComponents()
    {
        Transform translation = new();
        Transform expected = new();

        translation.Round2dTranslationComponents();
        Assert.Equal(expected.ToString(), translation.ToString());

        translation.Translate(1.0f, 1.0f);
        expected.Translate(1.0f, 1.0f);
        translation.Round2dTranslationComponents();
        Assert.Equal(expected.ToString(), translation.ToString());

        translation.Translate(0.5f, 0.4f);
        expected.Translate(1.0f, 0.0f);
        translation.Round2dTranslationComponents();
        Assert.Equal(expected.ToString(), translation.ToString());

        // Rounding should only affect 2d translation components.
        translation.Translate3D(0.0f, 0.0f, 0.5f);
        expected.Translate3D(0.0f, 0.0f, 0.5f);
        translation.Round2dTranslationComponents();
        Assert.Equal(expected.ToString(), translation.ToString());
    }

    [Fact]
    private static void TestFloor2dTranslationComponents()
    {
        Transform translation = new();
        Transform expected = new();

        translation.Floor2dTranslationComponents();
        Assert.Equal(expected.ToString(), translation.ToString());

        translation.Translate(1.0f, 1.0f);
        expected.Translate(1.0f, 1.0f);
        translation.Floor2dTranslationComponents();
        Assert.Equal(expected.ToString(), translation.ToString());

        translation.Translate(0.5f, 0.4f);
        expected.Translate(0.0f, 0.0f);
        translation.Floor2dTranslationComponents();
        Assert.Equal(expected.ToString(), translation.ToString());

        // Flooring should only affect 2d translation components.
        translation.Translate3D(0.0f, 0.0f, 0.5f);
        expected.Translate3D(0.0f, 0.0f, 0.5f);
        translation.Floor2dTranslationComponents();
        Assert.Equal(expected.ToString(), translation.ToString());

        translation.Translate(3.9f, 4.4f);
        expected.Translate(3.0f, 4.0f);
        translation.Floor2dTranslationComponents();
        Assert.Equal(expected.ToString(), translation.ToString());

        translation.Translate(3.9f, 4.4f);
        translation.EnsureFullMatrix();
        expected.Translate(3.0f, 4.0f);
        translation.EnsureFullMatrix();

        translation.Floor2dTranslationComponents();
        Assert.Equal(expected.ToString(), translation.ToString());
    }

    [Fact]
    private static void TestBackFaceVisiblilityTolerance()
    {
        Transform backface_invisible = new();
        backface_invisible.set_rc(0, 3, 1.0f);
        backface_invisible.set_rc(3, 0, 1.0f);
        backface_invisible.set_rc(2, 0, 1.0f);
        backface_invisible.set_rc(3, 2, 1.0f);

        // The transformation matrix has a determinant = 1 and cofactor33 = 0. So,
        // IsBackFaceVisible should return false.
        Assert.Equal(1.0f, backface_invisible.Determinant);
        Assert.False(backface_invisible.IsBackFaceVisible());

        // Adding a noise to the transformsation matrix that is within the tolerance
        // (machine epsilon) should not change the result.
        float noise = float.MachineEpsilon;
        backface_invisible.set_rc(0, 3, 1.0f + noise);
        Assert.False(backface_invisible.IsBackFaceVisible());

        // A noise that is more than the tolerance should change the result.
        backface_invisible.set_rc(0, 3, 1.0f + (2 * noise));
        Assert.True(backface_invisible.IsBackFaceVisible());
    }

    [Fact]
    private static void TestTransformVector4()
    {
        Transform transform = new();
        transform.set_rc(0, 0, 2.5f);
        transform.set_rc(1, 1, 3.5f);
        transform.set_rc(2, 2, 4.5f);
        transform.set_rc(3, 3, 5.5f);

        ReadOnlySpan<float> input = [ 11.5f, 22.5f, 33.5f, 44.5f];
        Span<float> vector = stackalloc float[4];
        input.CopyTo(vector); //vector = input

        Span<float> expected = [28.75f, 78.75f, 150.75f, 244.75f];
        transform.TransformVector4(vector);
        Assert.Equal(expected, vector);

        // With translations and perspectives.
        transform.set_rc(0, 3, 10);
        transform.set_rc(1, 3, 20);
        transform.set_rc(2, 3, 30);
        transform.set_rc(3, 0, 40);
        transform.set_rc(3, 1, 50);
        transform.set_rc(3, 2, 60);
        input.CopyTo(vector); //vector = input
        expected = [473.75f, 968.75f, 1485.75f, 3839.75f];
        transform.TransformVector4(vector);
        Assert.Equal(expected, vector);

        // TransformVector4 with simple 2d transform.
        transform = Transform.MakeTranslation(10, 20) * Transform.MakeScale(2.5f, 3.5f);
        input.CopyTo(vector); //vector = input
        expected = [473.75f, 968.75f, 33.5f, 44.5f];
        transform.TransformVector4(vector);
        Assert.Equal(expected, vector);

        input.CopyTo(vector); //vector = input
        transform.EnsureFullMatrix();
        transform.TransformVector4(vector);
        Assert.Equal(expected, vector);
    }

    [Fact]
    private static void TestMake90NRotation()
    {
        var t1 = Transform.Make90degRotation();
        Assert.Equal(new PointF(-50, 100), t1.MapPoint(new PointF(100, 50)));

        var t2 = Transform.Make180degRotation();
        Assert.Equal(Transform.MakeScale(-1), t2);
        Assert.Equal(new PointF(-100, -50), t2.MapPoint(new PointF(100, 50)));

        var t3 = Transform.Make270degRotation();
        Assert.Equal(new PointF(50, -100), t3.MapPoint(new PointF(100, 50)));

        var t4 = t1 * t1;
        Assert.Equal(t2, t4);
        t4.PreConcat(t1);
        Assert.Equal(t3, t4);
        t4.PreConcat(t1);
        Assert.True(t4.IsIdentity);
        t2.PreConcat(t2);
        Assert.True(t2.IsIdentity);
    }

    [Fact]
    private static void TestRotate90NDegrees()
    {
        Transform t1 = new();
        t1.Rotate(90);
        Assert.Equal(Transform.Make90degRotation(), t1);

        Transform t2 = new();
        t2.Rotate(180);
        Assert.Equal(Transform.Make180degRotation(), t2);

        Transform t3 = new();
        t3.Rotate(270);
        Assert.Equal(Transform.Make270degRotation(), t3);

        Transform t4 = new();
        t4.Rotate(360);
        Assert.Equal(new Transform(), t4);
        t4.Rotate(-270);
        Assert.Equal(t1, t4);
        t4.Rotate(-180);
        Assert.Equal(t3, t4);
        t4.Rotate(270);
        Assert.Equal(t2, t4);

        t1.Rotate(-90);
        t2.Rotate(180);
        t3.Rotate(-270);
        t4.Rotate(-180);
        Assert.True(t1.IsIdentity);
        Assert.True(t2.IsIdentity);
        Assert.True(t3.IsIdentity);
        Assert.True(t4.IsIdentity);

        // This should not crash. https://crbug.com/1378323.
        Transform t = new();
        t.Rotate(-1e-30);
    }

    [Fact]
    private static void TestMapPoint()
    {
        Transform transform = new();
        transform.Translate3D(1.25f, 2.75f, 3.875f);
        transform.Scale3D(3, 4, 5);
        Assert.Equal(new PointF(38.75f, 140.75f), transform.MapPoint(new PointF(12.5f, 34.5f)));
        Assert.Equal(new Point3F(38.75f, 140.75f, 286.375f), transform.MapPoint(new Point3F(12.5f, 34.5f, 56.5f)));

        transform.MakeIdentity();
        transform.set_rc(3, 0, 0.5);
        transform.set_rc(3, 1, 2);
        transform.set_rc(3, 2, 0.75);
        AssertPointFEqual(new PointF(0.2f, 0.4f), transform.MapPoint(new PointF(2, 4)));
        AssertPoint3FEqual(new Point3F(0.18181818f, 0.27272727f, 0.36363636f), transform.MapPoint(new Point3F(2, 3, 4)));

        // 0 in all perspectives should be ignored.
        transform.MakeIdentity();
        transform.Translate3D(10, 20, 30);
        transform.set_rc(3, 3, 0);
        Assert.Equal(new PointF(12, 24), transform.MapPoint(new PointF(2, 4)));
        Assert.Equal(new Point3F(12, 23, 34), transform.MapPoint(new Point3F(2, 3, 4)));

        // NaN in perspective should be ignored.
        transform.set_rc(3, 3, float.NaN);
        Assert.Equal(new PointF(12, 24), transform.MapPoint(new PointF(2, 4)));
        Assert.Equal(new Point3F(12, 23, 34), transform.MapPoint(new Point3F(2, 3, 4)));

        // MapPoint with simple 2d transform.
        transform = Transform.MakeTranslation(10, 20) * Transform.MakeScale(3, 4);
        Assert.Equal(new PointF(47.5f, 158.0f), transform.MapPoint(new PointF(12.5f, 34.5f)));
        Assert.Equal(new Point3F(47.5f, 158.0f, 56.5f), transform.MapPoint(new Point3F(12.5f, 34.5f, 56.5f)));

        transform.EnsureFullMatrix();
        Assert.Equal(new PointF(47.5f, 158.0f), transform.MapPoint(new PointF(12.5f, 34.5f)));
        Assert.Equal(new Point3F(47.5f, 158.0f, 56.5f), transform.MapPoint(new Point3F(12.5f, 34.5f, 56.5f)));
    }

    [Fact]
    private static void TestInverseMapPoint()
    {
        Point result;

        Transform transform = new();
        transform.Translate(1, 2);
        transform.Rotate(70);
        transform.Scale(3, 4);
        transform.Skew(30, 70);

        PointF point_f = new(12.34f, 56.78f);
        PointF transformed_point_f = transform.MapPoint(point_f);
        PointF reverted_point_f;
        Assert.True(transform.InverseMapPoint(transformed_point_f, out reverted_point_f));
        Assert.True(PointsAreNearlyEqual(reverted_point_f, point_f));

        Point point = new(12, 13);
        Point transformed_point = transform.MapPoint(point);
        transform.InverseMapPoint(transformed_point, out result);
        Assert.Equal(point, result);

        Transform transform3d = new();
        transform3d.Translate3D(1, 2, 3);
        transform3d.RotateAbout(new Vector3DF(4, 5, 6), 70);
        transform3d.Scale3D(7, 8, 9);
        transform3d.Skew(30, 70);

        Point3F point_3f = new(14, 15, 16);
        Point3F transformed_point_3f = transform3d.MapPoint(point_3f);
        Point3F reverted_point_3f;
        Assert.True(transform3d.InverseMapPoint(transformed_point_3f, out reverted_point_3f));
        Assert.True(PointsAreNearlyEqual(reverted_point_3f, point_3f));

        // MapPoint with simple 2d transform.
        transform = Transform.MakeTranslation(10, 20) * Transform.MakeScale(3, 4);
        Assert.Equal(new PointF(47.5f, 158.0f), transform.MapPoint(new PointF(12.5f, 34.5f)));
        Assert.Equal(new Point3F(47.5f, 158.0f, 56.5f),
                  transform.MapPoint(new Point3F(12.5f, 34.5f, 56.5f)));

        transform.EnsureFullMatrix();
        Assert.Equal(new PointF(47.5f, 158.0f), transform.MapPoint(new PointF(12.5f, 34.5f)));
        Assert.Equal(new Point3F(47.5f, 158.0f, 56.5f),
                  transform.MapPoint(new Point3F(12.5f, 34.5f, 56.5f)));
    }

    [Fact]
    private static void TestMapVector()
    {
        Transform transform = new();
        transform.Scale3D(3, 4, 5);
        Vector3DF vector = new(12.5f, 34.5f, 56.5f);
        Vector3DF expected = new(37.5f, 138.0f, 282.5f);
        Assert.Equal(expected, transform.MapVector(vector));

        // The translation components should be ignored.
        transform.Translate3D(1.25f, 2.75f, 3.875f);
        Assert.Equal(expected, transform.MapVector(vector));

        // The perspective components should be ignored.
        transform.set_rc(3, 0, 0.5f);
        transform.set_rc(3, 1, 2.5f);
        transform.set_rc(3, 2, 4.5f);
        transform.set_rc(3, 3, 8.5f);
        Assert.Equal(expected, transform.MapVector(vector));

        // MapVector with a simple 2d transform.
        transform = Transform.MakeTranslation(10, 20) * Transform.MakeScale(3, 4);
        expected.Z = vector.Z;
        Assert.Equal(expected, transform.MapVector(vector));

        transform.EnsureFullMatrix();
        Assert.Equal(expected, transform.MapVector(vector));
    }

    [Fact]
    private static void TestPreConcatAxisTransform2d()
    {
        var t = Transform.RowMajor(2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17);
        var axis = AxisTransform2D.FromScaleAndTranslation(new Vector2DF(10, 20),
                                                           new Vector2DF(100, 200));
        var axis_full = Transform.MakeTranslation(100, 200) * Transform.MakeScale(10, 20);
        var t1 = t;
        t.PreConcat(axis);
        t1.PreConcat(axis_full);
        Assert.Equal(t, t1);
    }

    [Fact]
    private static void TestPostConcatAxisTransform2d()
    {
        var t = Transform.RowMajor(2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17);
        var axis = AxisTransform2D.FromScaleAndTranslation(new Vector2DF(10, 20),
                                                           new Vector2DF(100, 200));
        var axis_full =
            Transform.MakeTranslation(100, 200) * Transform.MakeScale(10, 20);
        var t1 = t;
        t.PostConcat(axis);
        t1.PostConcat(axis_full);
        Assert.Equal(t, t1);
    }

    [Fact]
    private static void TestClampOutput()
    {
        // The first entry is used to initialize the transform.
        // The second entry is used to initialize the object to be mapped.
        (double, double)[] entries =
        [
            (float.MaxValue, float.PositiveInfinity),
            (1, float.PositiveInfinity),
            (-1, float.PositiveInfinity),
            (1, float.NegativeInfinity),
            (float.MaxValue, float.MaxValue),
            (float.MinValue, float.NegativeInfinity)
        ];
        
        foreach (var (mv, factor) in entries)
        {

            bool is_valid_point(in PointF p) => float.IsFinite(p.X) && float.IsFinite(p.Y);
            bool is_valid_point3(in Point3F p) => float.IsFinite(p.X) && float.IsFinite(p.Y) && float.IsFinite(p.Z);
            bool is_valid_vector2(in Vector2DF v) => float.IsFinite(v.X) && float.IsFinite(v.Y);
            bool is_valid_vector3(in Vector3DF v) => float.IsFinite(v.X) && float.IsFinite(v.Y) && float.IsFinite(v.Z);
            bool is_valid_rect(in RectF r) => is_valid_point(r.Origin) && float.IsFinite(r.Width) && float.IsFinite(r.Height);

            bool is_valid_array(Span<float> a)
            {
                foreach (float val in a)
                {
                    if (!float.IsFinite(val))
                        return false;
                }

                return true;
            }

            void test(in Transform m)
            {
                var p = m.MapPoint(new PointF((float)factor, (float)factor));
                Assert.True(is_valid_point(p));

                var p3 = m.MapPoint(new Point3F((float)factor, (float)factor, (float)factor));
                Assert.True(is_valid_point3(p3));

                var r = m.MapRect(new RectF((float)factor, (float)factor, (float)factor, (float)factor));
                Assert.True(is_valid_rect(r));

                var v3 = m.MapVector(new Vector3DF((float)factor, (float)factor, (float)factor));
                Assert.True(is_valid_vector3(v3));

                Span<float> v4 = [(float)factor, (float)factor, (float)factor, (float)factor];
                m.TransformVector4(v4);
                Assert.True(is_valid_array(v4));

                var v2 = m.To2dTranslation();
                Assert.True(is_valid_vector2(v2));
                v2 = m.To2dScale();
                Assert.True(is_valid_vector2(v2));

                v3 = m.To3dTranslation();
                Assert.True(is_valid_vector3(v3));
            }

            test(Transform.ColMajor(mv, mv, mv, mv, mv, mv, mv, mv, mv, mv, mv, mv, mv, mv, mv, mv));
            test(Transform.MakeTranslation((float)mv, (float)mv));
        }
    }

    private static float kProjectionClampedBigNumber = 1 << (float.Digits - 1);

    // This test also demonstrates the relationship between ProjectPoint() and MapPoint().
    [Fact]
    private static void TestProjectPoint()
    {
        Transform transform = new();
        PointF p = new(1.25f, -3.5f);
        bool clamped = true;
        Assert.Equal(p, transform.ProjectPoint(p));
        Assert.Equal(p, transform.ProjectPoint(p, ref clamped));
        Assert.False(clamped);
        // MapPoint() and ProjectPoint() are the same with a flat transform.
        Assert.Equal(p, transform.MapPoint(p));

        // ProjectPoint with simple 2d transform.
        transform = Transform.MakeTranslation(10, 20) * Transform.MakeScale(3, 4);
        clamped = true;
        PointF projected = transform.ProjectPoint(p, ref clamped);
        Assert.Equal(new PointF(13.75f, 6.0f), projected);
        Assert.False(clamped);
        // MapPoint() and ProjectPoint() are the same with a flat transform.
        Assert.Equal(projected, transform.MapPoint(p));

        clamped = true;
        transform.EnsureFullMatrix();
        Assert.Equal(projected, transform.ProjectPoint(p, ref clamped));
        Assert.False(clamped);
        Assert.Equal(projected, transform.MapPoint(p));

        // Set scale z to 0.
        transform.set_rc(2, 2, 0);
        clamped = true;
        projected = transform.ProjectPoint(p, ref clamped);
        Assert.Equal(new PointF(), projected);
        Assert.True(clamped);
        // MapPoint() still produces the original result.
        Assert.Equal(new PointF(13.75f, 6.0f), transform.MapPoint(p));

        // Normally (except the last case below),
        // t.ProjectPoint() is equivalent to
        // inverse(flatten(inverse(t))).MapPoint().
        Transform projection_transform(in Transform t)
        {
            var flat = t.GetCheckedInverse();
            flat.Flatten();
            return flat.GetCheckedInverse();
        }

        transform.MakeIdentity();
        transform.RotateAboutYAxis(60);
        clamped = true;
        projected = transform.ProjectPoint(p, ref clamped);
        Assert.Equal(new PointF(2.5f, -3.5f), projected);
        Assert.False(clamped);
        Assert.Equal(new PointF(0.625f, -3.5f), transform.MapPoint(p));

        Assert.Equal(projected, projection_transform(transform).MapPoint(p));
        Assert.Equal(projected, projection_transform(transform).ProjectPoint(p));

        transform.ApplyPerspectiveDepth(10);
        clamped = true;
        projected = transform.ProjectPoint(p, ref clamped);
        AssertPointFNear(new PointF(3.19f, -4.47f), projected, 0.01f);
        Assert.False(clamped);
        Assert.Equal(new PointF(0.625f, -3.5f), transform.MapPoint(p));

        AssertPointFNear(projected, projection_transform(transform).MapPoint(p), 1e-5f);
        AssertPointFNear(projected, projection_transform(transform).ProjectPoint(p), 1e-5f);

        // With a small perspective, the ray doesn't intersect the destination plane.
        transform.ApplyPerspectiveDepth(2);
        clamped = false;
        projected = transform.ProjectPoint(p, ref clamped);
        Assert.True(clamped);
        Assert.Equal(projected.X, kProjectionClampedBigNumber);
        Assert.Equal(projected.Y, -kProjectionClampedBigNumber);
        Assert.Equal(new PointF(0.625f, -3.5f), transform.MapPoint(p));
        // In this case, MapPoint() returns a point behind the eye.
        AssertPointFNear(new PointF(-8.36014f, 11.7042f), projection_transform(transform).MapPoint(p), 1e-5f);
        AssertPointFNear(projected, projection_transform(transform).ProjectPoint(p), 1e-5f);
    }

    [Fact]
    private static void TestProjectQuad()
    {
        var transform = Transform.MakeTranslation(3.25f, 7.75f);
        var q = new QuadF(new PointF(1.25f, 2.5f), new PointF(3.75f, 4.0f), new PointF(23.0f, 45.0f), new PointF(12.0f, 67.0f));
        Assert.Equal(new QuadF(new PointF(4.5f, 10.25f), new PointF(7.0f, 11.75f), new PointF(26.25f, 52.75f), new PointF(15.25f, 74.75f)),
            transform.ProjectQuad(q));

        transform.set_rc(2, 2, 0);
        Assert.Equal(new QuadF(), transform.ProjectQuad(q));

        transform.MakeIdentity();
        transform.RotateAboutYAxis(60);
        Assert.Equal(new QuadF(new PointF(2.5f, 2.5f), new PointF(7.5f, 4.0f), new PointF(46.0f, 45.0f), new PointF(24.0f, 67.0f)),
            transform.ProjectQuad(q));

        // With a small perspective, all points of |q| are clamped, and the
        // projected result is an empty quad.
        transform.ApplyPerspectiveDepth(2);
        Assert.Equal(new QuadF(), transform.ProjectQuad(q));

        // Change the quad so that 2 points are clamped.
        q.P1 = new PointF(-1.25f, -2.5f);
        q.P2 = new PointF(-3.75f, 4.0f);
        q.P3 = new PointF(23.0f, -45.0f);
        QuadF q1 = transform.ProjectQuad(q);
        AssertPointFNear(new PointF(-1.2f, -1.2f), q1.P1, 0.01f);
        AssertPointFNear(new PointF(-1.77f, 0.94f), q1.P2, 0.01f);
        Assert.Equal(q1.P3.X, kProjectionClampedBigNumber);
        Assert.Equal(q1.P3.Y, -kProjectionClampedBigNumber);
        Assert.Equal(q1.P4.X, kProjectionClampedBigNumber);
        Assert.Equal(q1.P4.Y, kProjectionClampedBigNumber);
    }

    [Fact]
    private static void TestToString()
    {
        var zeros = Transform.ColMajor(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        Assert.Equal("[ 0 0 0 0\n  0 0 0 0\n  0 0 0 0\n  0 0 0 0 ]\n", zeros.ToString());
        Assert.Equal("[ 0 0 0 0\n  0 0 0 0\n  0 0 0 0\n  0 0 0 0 ]\n(degenerate)", zeros.ToDecomposedString());

        Transform identity = new();
        Assert.Equal("[ 1 0 0 0\n  0 1 0 0\n  0 0 1 0\n  0 0 0 1 ]\n", identity.ToString());
        Assert.Equal("identity", identity.ToDecomposedString());

        Transform translation = new();
        translation.Translate3D(3, 5, 7);
        Assert.Equal("[ 1 0 0 3\n  0 1 0 5\n  0 0 1 7\n  0 0 0 1 ]\n", translation.ToString());
        Assert.Equal("translate: 3,5,7", translation.ToDecomposedString());

        var transform = Transform.ColMajor(1.1, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7, 8.8, 1e20, 1e-20, 1.0 / 3.0, 0, 0, 0, 0, 1);
        Assert.Equal("[ 1.1 5.5 1E+20 0\n  2.2 6.6 1E-20 0\n  3.3 7.7 0.333333 0\n  4.4 8.8 0 1 ]\n", transform.ToString());
        Assert.Equal("translate: +0 +0 +0\nscale: -4.11582 -2.88048 -4.08248e+19\nskew: +3.87836 +0.654654 +2.13809\nperspective: -6.66667e-21 -1 +2 +1\nquaternion: -0.582925 +0.603592 +0.518949 +0.162997\n", transform.ToDecomposedString());
    }

    private static void Test2()
    {
        
    }

    private static void Test3()
    {
        
    }

    private static void Test4()
    {
        
    }

    private static void Test5()
    {
        
    }

    private static void Test6()
    {
        
    }
}
