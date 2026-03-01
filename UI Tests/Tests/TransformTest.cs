using Xunit;

using UI.GFX.Geometry;
using System.Diagnostics;

using static UI.Tests.GeometryUtil;

namespace UI.Tests;

public static class TransformTest
{
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
}
