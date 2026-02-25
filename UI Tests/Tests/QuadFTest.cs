using Xunit;

using UI.GFX.Geometry;

using static UI.GFX.Geometry.PointConversions;
using UI.Extensions;

namespace UI.Tests;

public static class QuadFTest
{
    [Fact]
    private static void TestConstruction()
    {
        // Verify constructors.
        PointF a = new(1, 1);
        PointF b = new(2, 1);
        PointF c = new(2, 2);
        PointF d = new(1, 2);
        PointF e = new();
        QuadF q1 = new();
        QuadF q2 = new(e, e, e, e);
        QuadF q3 = new(a, b, c, d);
        QuadF q4 = new(RectF.BoundingRect(a, c));
        Assert.Equal(q1, q2);
        Assert.Equal(q3, q4);

        // Verify getters.
        Assert.Equal(q3.p1, a);
        Assert.Equal(q3.p2, b);
        Assert.Equal(q3.p3, c);
        Assert.Equal(q3.p4, d);

        // Verify setters.
        q3.p1 = b;
        q3.p2 = c;
        q3.p3 = d;
        q3.p4 = a;
        Assert.Equal(q3.p1, b);
        Assert.Equal(q3.p2, c);
        Assert.Equal(q3.p3, d);
        Assert.Equal(q3.p4, a);

        // Verify operator=(Rect)
        Assert.NotEqual(q1, q4);
        q1 = RectF.BoundingRect(a, c);
        Assert.Equal(q1, q4);

        // Verify operator=(Quad)
        Assert.NotEqual(q1, q3);
        q1 = q3;
        Assert.Equal(q1, q3);
    }

    [Fact]
    private static void TestAddingVectors()
    {
        PointF a = new(1, 1);
        PointF b = new(2, 1);
        PointF c = new(2, 2);
        PointF d = new(1, 2);
        Vector2DF v = new(3.5f, -2.5f);

        QuadF q1 = new(a, b, c, d);
        QuadF added = q1 + v;
        q1 += v;
        var expected1 = new QuadF(new PointF(4.5f, -1.5f), new PointF(5.5f, -1.5f), new PointF(5.5f, -0.5f), new PointF(4.5f, -0.5f));
        Assert.Equal(expected1, added);
        Assert.Equal(expected1, q1);

        QuadF q2 = new(a, b, c, d);
        QuadF subtracted = q2 - v;
        q2 -= v;
        var expected2 = new QuadF(new PointF(-2.5f, 3.5f), new PointF(-1.5f, 3.5f), new PointF(-1.5f, 4.5f), new PointF(-2.5f, 4.5f));
        Assert.Equal(expected2, subtracted);
        Assert.Equal(expected2, q2);

        QuadF q3 = new(a, b, c, d);
        q3 += v;
        q3 -= v;
        Assert.Equal(new QuadF(a, b, c, d), q3);
        Assert.Equal(q3, (q3 + v - v));
    }

    [Fact]
    private static void TestIsRectilinear()
    {
        PointF a = new(1, 1);
        PointF b = new(2, 1);
        PointF c = new(2, 2);
        PointF d = new(1, 2);
        Vector2DF v = new(3.5f, -2.5f);

        Assert.True(new QuadF().IsRectilinear());
        Assert.True(new QuadF(a, b, c, d).IsRectilinear());
        Assert.True((new QuadF(a, b, c, d) + v).IsRectilinear());

        float epsilon = float.MachineEpsilon;
        PointF a2 = new(1 + epsilon / 2, 1 + epsilon / 2);
        PointF b2 = new(2 + epsilon / 2, 1 + epsilon / 2);
        PointF c2 = new(2 + epsilon / 2, 2 + epsilon / 2);
        PointF d2 = new(1 + epsilon / 2, 2 + epsilon / 2);
        Assert.True(new QuadF(a2, b, c, d).IsRectilinear());
        Assert.True((new QuadF(a2, b, c, d) + v).IsRectilinear());
        Assert.True(new QuadF(a, b2, c, d).IsRectilinear());
        Assert.True((new QuadF(a, b2, c, d) + v).IsRectilinear());
        Assert.True(new QuadF(a, b, c2, d).IsRectilinear());
        Assert.True((new QuadF(a, b, c2, d) + v).IsRectilinear());
        Assert.True(new QuadF(a, b, c, d2).IsRectilinear());
        Assert.True((new QuadF(a, b, c, d2) + v).IsRectilinear());

        (PointF a_off, PointF b_off, PointF c_off, PointF d_off)[] tests =
        [
            (new PointF(1, 1.00001f), new PointF(2, 1.00001f), new PointF(2, 2.00001f), new PointF(1, 2.00001f)),
            (new PointF(1.00001f, 1), new PointF(2.00001f, 1), new PointF(2.00001f, 2), new PointF(1.00001f, 2)),
            (new PointF(1.00001f, 1.00001f), new PointF(2.00001f, 1.00001f), new PointF(2.00001f, 2.00001f), new PointF(1.00001f, 2.00001f)),
            (new PointF(1, 0.99999f), new PointF(2, 0.99999f), new PointF(2, 1.99999f), new PointF(1, 1.99999f)),
            (new PointF(0.99999f, 1), new PointF(1.99999f, 1), new PointF(1.99999f, 2), new PointF(0.99999f, 2)),
            (new PointF(0.99999f, 0.99999f), new PointF(1.99999f, 0.99999f), new PointF(1.99999f, 1.99999f), new PointF(0.99999f, 1.99999f))
        ];

        foreach (var test in tests)
        {
            PointF a_off = test.a_off;
            PointF b_off = test.b_off;
            PointF c_off = test.c_off;
            PointF d_off = test.d_off;

            Assert.False(new QuadF(a_off, b, c, d).IsRectilinear());
            Assert.False((new QuadF(a_off, b, c, d) + v).IsRectilinear());
            Assert.False(new QuadF(a, b_off, c, d).IsRectilinear());
            Assert.False((new QuadF(a, b_off, c, d) + v).IsRectilinear());
            Assert.False(new QuadF(a, b, c_off, d).IsRectilinear());
            Assert.False((new QuadF(a, b, c_off, d) + v).IsRectilinear());
            Assert.False(new QuadF(a, b, c, d_off).IsRectilinear());
            Assert.False((new QuadF(a, b, c, d_off) + v).IsRectilinear());
            Assert.False(new QuadF(a_off, b, c_off, d).IsRectilinear());
            Assert.False((new QuadF(a_off, b, c_off, d) + v).IsRectilinear());
            Assert.False(new QuadF(a, b_off, c, d_off).IsRectilinear());
            Assert.False((new QuadF(a, b_off, c, d_off) + v).IsRectilinear());
            Assert.False(new QuadF(a, b_off, c_off, d_off).IsRectilinear());
            Assert.False((new QuadF(a, b_off, c_off, d_off) + v).IsRectilinear());
            Assert.False(new QuadF(a_off, b, c_off, d_off).IsRectilinear());
            Assert.False((new QuadF(a_off, b, c_off, d_off) + v).IsRectilinear());
            Assert.False(new QuadF(a_off, b_off, c, d_off).IsRectilinear());
            Assert.False((new QuadF(a_off, b_off, c, d_off) + v).IsRectilinear());
            Assert.False(new QuadF(a_off, b_off, c_off, d).IsRectilinear());
            Assert.False((new QuadF(a_off, b_off, c_off, d) + v).IsRectilinear());
            Assert.True(new QuadF(a_off, b_off, c_off, d_off).IsRectilinear());
            Assert.True((new QuadF(a_off, b_off, c_off, d_off) + v).IsRectilinear());
        }
    }

    [Fact]
    private static void TestIsRectilinearForMappedQuad()
    {
        const int kNumRectilinear = 8;
        Transform[] rectilinear_trans =
        [
            new Transform(),
            new Transform(),
            new Transform(),
            new Transform(),
            new Transform(),
            new Transform(),
            new Transform(),
            new Transform()
        ];

        rectilinear_trans[1].Rotate(90.0f);
        rectilinear_trans[2].Rotate(180.0f);
        rectilinear_trans[3].Rotate(270.0f);
        rectilinear_trans[4].Skew(0.00000000001f, 0.0f);
        rectilinear_trans[5].Skew(0.0f, 0.00000000001f);
        rectilinear_trans[6].Scale(0.00001f, 0.00001f);
        rectilinear_trans[6].Rotate(180.0f);
        rectilinear_trans[7].Scale(100000.0f, 100000.0f);
        rectilinear_trans[7].Rotate(180.0f);

        QuadF original = new QuadF(new RectF(0.01010101f, 0.01010101f, 100.01010101f, 100.01010101f));

        for (int i = 0; i < kNumRectilinear; ++i)
        {
            QuadF quad = rectilinear_trans[i].MapQuad(original);
            Assert.True(quad.IsRectilinear()); // << "case " << i;
        }

        const int kNumNonRectilinear = 10;
        Transform[] non_rectilinear_trans =
        [
            new Transform(),
            new Transform(),
            new Transform(),
            new Transform(),
            new Transform(),
            new Transform(),
            new Transform(),
            new Transform(),
            new Transform(),
            new Transform()
        ];
        
        non_rectilinear_trans[0].Rotate(359.9999f);
        non_rectilinear_trans[1].Rotate(0.0000001f);
        non_rectilinear_trans[2].Rotate(89.9999f);
        non_rectilinear_trans[3].Rotate(90.00001f);
        non_rectilinear_trans[4].Rotate(179.9999f);
        non_rectilinear_trans[5].Rotate(180.00001f);
        non_rectilinear_trans[6].Rotate(269.9999f);
        non_rectilinear_trans[7].Rotate(270.0001f);
        non_rectilinear_trans[8].Skew(0.00001f, 0.0f);
        non_rectilinear_trans[9].Skew(0.0f, 0.00001f);

        for (int i = 0; i < kNumNonRectilinear; ++i)
        {
            QuadF quad = non_rectilinear_trans[i].MapQuad(original);
            Assert.False(quad.IsRectilinear()); // << "case " << i;
        }
    }

    [Fact]
    private static void TestIsCounterClockwise()
    {
        PointF a1 = new(1, 1);
        PointF b1 = new(2, 1);
        PointF c1 = new(2, 2);
        PointF d1 = new(1, 2);
        Assert.False(new QuadF(a1, b1, c1, d1).IsCounterClockwise());
        Assert.False(new QuadF(b1, c1, d1, a1).IsCounterClockwise());
        Assert.True(new QuadF(a1, d1, c1, b1).IsCounterClockwise());
        Assert.True(new QuadF(c1, b1, a1, d1).IsCounterClockwise());

        // Slightly more complicated quads should work just as easily.
        PointF a2 = new(1.3f, 1.4f);
        PointF b2 = new(-0.7f, 4.9f);
        PointF c2 = new(1.8f, 6.2f);
        PointF d2 = new(2.1f, 1.6f);
        Assert.True(new QuadF(a2, b2, c2, d2).IsCounterClockwise());
        Assert.True(new QuadF(b2, c2, d2, a2).IsCounterClockwise());
        Assert.False(new QuadF(a2, d2, c2, b2).IsCounterClockwise());
        Assert.False(new QuadF(c2, b2, a2, d2).IsCounterClockwise());

        // Quads with 3 collinear points should work correctly, too.
        PointF a3 = new(0, 0);
        PointF b3 = new(1, 0);
        PointF c3 = new(2, 0);
        PointF d3 = new(1, 1);
        Assert.False(new QuadF(a3, b3, c3, d3).IsCounterClockwise());
        Assert.False(new QuadF(b3, c3, d3, a3).IsCounterClockwise());
        Assert.True(new QuadF(a3, d3, c3, b3).IsCounterClockwise());
        // The next expectation in particular would fail for an implementation
        // that incorrectly uses only a cross product of the first 3 vertices.
        Assert.True(new QuadF(c3, b3, a3, d3).IsCounterClockwise());

        // Non-convex quads should work correctly, too.
        PointF a4 = new(0, 0);
        PointF b4 = new(1, 1);
        PointF c4 = new(2, 0);
        PointF d4 = new(1, 3);
        Assert.False(new QuadF(a4, b4, c4, d4).IsCounterClockwise());
        Assert.False(new QuadF(b4, c4, d4, a4).IsCounterClockwise());
        Assert.True(new QuadF(a4, d4, c4, b4).IsCounterClockwise());
        Assert.True(new QuadF(c4, b4, a4, d4).IsCounterClockwise());

        // A quad with huge coordinates should not fail this check due to
        // single-precision overflow.
        PointF a5 = new(1e30f, 1e30f);
        PointF b5 = new(1e35f, 1e30f);
        PointF c5 = new(1e35f, 1e35f);
        PointF d5 = new(1e30f, 1e35f);
        Assert.False(new QuadF(a5, b5, c5, d5).IsCounterClockwise());
        Assert.False(new QuadF(b5, c5, d5, a5).IsCounterClockwise());
        Assert.True(new QuadF(a5, d5, c5, b5).IsCounterClockwise());
        Assert.True(new QuadF(c5, b5, a5, d5).IsCounterClockwise());
    }

    [Fact]
    private static void TestBoundingBox()
    {
        RectF r = new(3.2f, 5.4f, 7.007f, 12.01f);
        Assert.Equal(r, new QuadF(r).BoundingBox());

        PointF a = new(1.3f, 1.4f);
        PointF b = new(-0.7f, 4.9f);
        PointF c = new(1.8f, 6.2f);
        PointF d = new(2.1f, 1.6f);
        float left = -0.7f;
        float top = 1.4f;
        float right = 2.1f;
        float bottom = 6.2f;
        Assert.Equal(new RectF(left, top, right - left, bottom - top), new QuadF(a, b, c, d).BoundingBox());
    }

    [Fact]
    private static void TestContainsPoint()
    {
        PointF a = new(1.3f, 1.4f);
        PointF b = new(-0.8f, 4.4f);
        PointF c = new(1.8f, 6.1f);
        PointF d = new(2.1f, 1.6f);

        Vector2DF epsilon_x = new(2 * float.MachineEpsilon, 0);
        Vector2DF epsilon_y = new(0, 2 * float.MachineEpsilon);

        Vector2DF ac_center = c - a;
        ac_center.Scale(0.5f);
        Vector2DF bd_center = d - b;
        bd_center.Scale(0.5f);

        Assert.True(new QuadF(a, b, c, d).Contains(a + ac_center));
        Assert.True(new QuadF(a, b, c, d).Contains(b + bd_center));
        Assert.True(new QuadF(a, b, c, d).Contains(c - ac_center));
        Assert.True(new QuadF(a, b, c, d).Contains(d - bd_center));
        Assert.False(new QuadF(a, b, c, d).Contains(a - ac_center));
        Assert.False(new QuadF(a, b, c, d).Contains(b - bd_center));
        Assert.False(new QuadF(a, b, c, d).Contains(c + ac_center));
        Assert.False(new QuadF(a, b, c, d).Contains(d + bd_center));

        Assert.True(new QuadF(a, b, c, d).Contains(a));
        Assert.False(new QuadF(a, b, c, d).Contains(a - epsilon_x));
        Assert.False(new QuadF(a, b, c, d).Contains(a - epsilon_y));
        Assert.False(new QuadF(a, b, c, d).Contains(a + epsilon_x));
        Assert.True(new QuadF(a, b, c, d).Contains(a + epsilon_y));

        Assert.True(new QuadF(a, b, c, d).Contains(b));
        Assert.False(new QuadF(a, b, c, d).Contains(b - epsilon_x));
        Assert.False(new QuadF(a, b, c, d).Contains(b - epsilon_y));
        Assert.True(new QuadF(a, b, c, d).Contains(b + epsilon_x));
        Assert.False(new QuadF(a, b, c, d).Contains(b + epsilon_y));

        Assert.True(new QuadF(a, b, c, d).Contains(c));
        Assert.False(new QuadF(a, b, c, d).Contains(c - epsilon_x));
        Assert.True(new QuadF(a, b, c, d).Contains(c - epsilon_y));
        Assert.False(new QuadF(a, b, c, d).Contains(c + epsilon_x));
        Assert.False(new QuadF(a, b, c, d).Contains(c + epsilon_y));

        Assert.True(new QuadF(a, b, c, d).Contains(d));
        Assert.True(new QuadF(a, b, c, d).Contains(d - epsilon_x));
        Assert.False(new QuadF(a, b, c, d).Contains(d - epsilon_y));
        Assert.False(new QuadF(a, b, c, d).Contains(d + epsilon_x));
        Assert.False(new QuadF(a, b, c, d).Contains(d + epsilon_y));

        // Test a simple square.
        PointF s1 = new(-1, -1);
        PointF s2 = new(1, -1);
        PointF s3 = new(1, 1);
        PointF s4 = new(-1, 1);
        // Top edge.
        Assert.False(new QuadF(s1, s2, s3, s4).Contains(new PointF(-1.1f, -1.0f)));
        Assert.True(new QuadF(s1, s2, s3, s4).Contains(new PointF(-1.0f, -1.0f)));
        Assert.True(new QuadF(s1, s2, s3, s4).Contains(new PointF(0.0f, -1.0f)));
        Assert.True(new QuadF(s1, s2, s3, s4).Contains(new PointF(1.0f, -1.0f)));
        Assert.False(new QuadF(s1, s2, s3, s4).Contains(new PointF(1.1f, -1.0f)));
        // Bottom edge.
        Assert.False(new QuadF(s1, s2, s3, s4).Contains(new PointF(-1.1f, 1.0f)));
        Assert.True(new QuadF(s1, s2, s3, s4).Contains(new PointF(-1.0f, 1.0f)));
        Assert.True(new QuadF(s1, s2, s3, s4).Contains(new PointF(0.0f, 1.0f)));
        Assert.True(new QuadF(s1, s2, s3, s4).Contains(new PointF(1.0f, 1.0f)));
        Assert.False(new QuadF(s1, s2, s3, s4).Contains(new PointF(1.1f, 1.0f)));
        // Left edge.
        Assert.False(new QuadF(s1, s2, s3, s4).Contains(new PointF(-1.0f, -1.1f)));
        Assert.True(new QuadF(s1, s2, s3, s4).Contains(new PointF(-1.0f, -1.0f)));
        Assert.True(new QuadF(s1, s2, s3, s4).Contains(new PointF(-1.0f, 0.0f)));
        Assert.True(new QuadF(s1, s2, s3, s4).Contains(new PointF(-1.0f, 1.0f)));
        Assert.False(new QuadF(s1, s2, s3, s4).Contains(new PointF(-1.0f, 1.1f)));
        // Right edge.
        Assert.False(new QuadF(s1, s2, s3, s4).Contains(new PointF(1.0f, -1.1f)));
        Assert.True(new QuadF(s1, s2, s3, s4).Contains(new PointF(1.0f, -1.0f)));
        Assert.True(new QuadF(s1, s2, s3, s4).Contains(new PointF(1.0f, 0.0f)));
        Assert.True(new QuadF(s1, s2, s3, s4).Contains(new PointF(1.0f, 1.0f)));
        Assert.False(new QuadF(s1, s2, s3, s4).Contains(new PointF(1.0f, 1.1f)));
        // Centered inside.
        Assert.True(new QuadF(s1, s2, s3, s4).Contains(new PointF(0, 0)));
        // Centered outside.
        Assert.False(new QuadF(s1, s2, s3, s4).Contains(new PointF(-1.1f, 0)));
        Assert.False(new QuadF(s1, s2, s3, s4).Contains(new PointF(1.1f, 0)));
        Assert.False(new QuadF(s1, s2, s3, s4).Contains(new PointF(0, -1.1f)));
        Assert.False(new QuadF(s1, s2, s3, s4).Contains(new PointF(0, 1.1f)));
    }
}
