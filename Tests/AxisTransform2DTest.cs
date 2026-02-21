using System.Diagnostics;
using UI.GFX.Geometry;

namespace UI.Tests;

public static class AxisTransform2DTest
{
    public static void Run()
    {
        TestMapping();

        Debug.WriteLine("All AxisTransform2D tests passed!");
    }

    private static bool FloatEqual(float a, float b) => MathF.Abs(a - b) <= 1e-6f;

    private static bool FloatNear(float val1, float val2, float abs_error) => MathF.Abs(val1 - val2) <= abs_error;


    public static void TestMapping()
    {
        AxisTransform2D t = new(1.25f, new Vector2DF(3.75f, 55.0f));

        PointF p = new(150.0f, 100.0f);

        Debug.Assert(new PointF(191.25f, 180.0f) == t.MapPoint(p));
        //EXPECT_POINTF_EQ(PointF(117.f, 36.f), t.InverseMapPoint(p));

        //RectF r(150.f, 100.f, 22.5f, 37.5f);
        //EXPECT_EQ(RectF(191.25f, 180.f, 28.125f, 46.875f), t.MapRect(r));
        //EXPECT_RECTF_EQ(RectF(117.f, 36.f, 18.f, 30.f), t.InverseMapRect(r));
    }
}
