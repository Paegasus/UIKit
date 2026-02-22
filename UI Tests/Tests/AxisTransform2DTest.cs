using System.Diagnostics;
using UI.GFX.Geometry;

namespace UI.Tests;

public static class AxisTransform2DTest
{
    public static void Run()
    {
        TestMapping();
        TestScaling();
        TestTranslating();
        TestConcat();
        TestInverse();
        TestClampOutput();
        TestDecompose();

        Debug.WriteLine("All AxisTransform2D tests passed!");
    }

    private static bool FloatEqual(float a, float b) => MathF.Abs(a - b) <= 1e-6f;

    private static bool FloatNear(float val1, float val2, float abs_error) => MathF.Abs(val1 - val2) <= abs_error;


    private static void TestMapping()
    {
        AxisTransform2D t = new(1.25f, new Vector2DF(3.75f, 55.0f));

        PointF p = new(150.0f, 100.0f);
        Debug.Assert(new PointF(191.25f, 180.0f) == t.MapPoint(p));
        Debug.Assert(new PointF(117.0f, 36.0f) == t.InverseMapPoint(p));

        RectF r = new(150.0f, 100.0f, 22.5f, 37.5f);
        Debug.Assert(new RectF(191.25f, 180.0f, 28.125f, 46.875f) == t.MapRect(r));
        Debug.Assert(new RectF(117.0f, 36.0f, 18.0f, 30.0f) == t.InverseMapRect(r));
    }

    private static void TestScaling()
    {
        {
            AxisTransform2D t = new(1.25f, new Vector2DF(3.75f, 55.0f));
            Debug.Assert(new AxisTransform2D(1.5625f, new Vector2DF(3.75f, 55.0f)) == AxisTransform2D.PreScaleAxisTransform2D(t, 1.25f));
            t.PreScale(new Vector2DF(1.25f, 1.25f));
            Debug.Assert(new AxisTransform2D(1.5625f, new Vector2DF(3.75f, 55.0f)) == t);
        }

        {
            AxisTransform2D t = new(1.25f, new Vector2DF(3.75f, 55.0f));
            Debug.Assert(new AxisTransform2D(1.5625f, new Vector2DF(4.6875f, 68.75f)) == AxisTransform2D.PostScaleAxisTransform2D(t, 1.25f));
            t.PostScale(new Vector2DF(1.25f, 1.25f));
            Debug.Assert(new AxisTransform2D(1.5625f, new Vector2DF(4.6875f, 68.75f)) == t);
        }
    }

    private static void TestTranslating()
    {
        Vector2DF tr = new(3.0f, -5.0f);
        {
            AxisTransform2D t = new(1.25f, new Vector2DF(3.75f, 55.0f));
            Debug.Assert(new AxisTransform2D(1.25f, new Vector2DF(7.5f, 48.75f)) == AxisTransform2D.PreTranslateAxisTransform2D(t, tr));
            t.PreTranslate(tr);
            Debug.Assert(new AxisTransform2D(1.25f, new Vector2DF(7.5f, 48.75f)) == t);
        }

        {
            AxisTransform2D t = new(1.25f, new Vector2DF(3.75f, 55.0f));
            Debug.Assert(new AxisTransform2D(1.25f, new Vector2DF(6.75f, 50.0f)) == AxisTransform2D.PostTranslateAxisTransform2D(t, tr));
            t.PostTranslate(tr);
            Debug.Assert(new AxisTransform2D(1.25f, new Vector2DF(6.75f, 50.0f)) == t);
        }
    }

    private static void TestConcat()
    {
        AxisTransform2D pre = new(1.25f, new Vector2DF(3.75f, 55.0f));
        AxisTransform2D post = new(0.5f, new Vector2DF(10.0f, 5.0f));
        AxisTransform2D expectation = new(0.625f, new Vector2DF(11.875f, 32.5f));
        Debug.Assert(expectation == AxisTransform2D.ConcatAxisTransform2D(post, pre));

        AxisTransform2D post_concat = pre;
        post_concat.PostConcat(post);
        Debug.Assert(expectation == post_concat);

        AxisTransform2D pre_concat = post;
        pre_concat.PreConcat(pre);
        Debug.Assert(expectation == pre_concat);
    }

    private static void TestInverse()
    {
        AxisTransform2D t = new(1.25f, new Vector2DF(3.75f, 55.0f));
        AxisTransform2D inv_inplace = t;
        inv_inplace.Invert();
        AxisTransform2D inv_out_of_place = AxisTransform2D.InvertAxisTransform2D(t);

        Debug.Assert(inv_inplace == inv_out_of_place);
        Debug.Assert(new AxisTransform2D() == AxisTransform2D.ConcatAxisTransform2D(t, inv_inplace));
        Debug.Assert(new AxisTransform2D() == AxisTransform2D.ConcatAxisTransform2D(inv_inplace, t));
    }

    private static void TestClampOutput()
    {
        (float, float)[] entries =
        [
            // The first entry is used to initialize the transform.
            // The second entry is used to initialize the object to be mapped.
            (float.MaxValue, float.PositiveInfinity),
            (1, float.PositiveInfinity),
            (-1, float.PositiveInfinity),
            (1, float.NegativeInfinity),
            (float.MaxValue, float.MaxValue),
            (float.MinValue, float.NegativeInfinity)
        ];

        foreach(var (mv, factor) in entries)
        {
            bool is_valid_point (in PointF p) => float.IsFinite(p.X) && float.IsFinite(p.Y);

            bool is_valid_rect (in RectF r) => is_valid_point(r.Origin) && float.IsFinite(r.Width) && float.IsFinite(r.Height);

            void test(in AxisTransform2D m)
            {
                //Debug.WriteLine($"m: {m} factor: {factor}");
                PointF p = m.MapPoint(new PointF(factor, factor));
                Debug.Assert(is_valid_point(p));

                // AxisTransform2d::MapRect() requires non-negative scales.
                if (m.Scale.X >= 0 && m.Scale.Y >= 0)
                {
                    RectF r = m.MapRect(new RectF(factor, factor, factor, factor));
                    Debug.Assert(is_valid_rect(r));
                }
            }

            test(AxisTransform2D.FromScaleAndTranslation(new Vector2DF(mv, mv), new Vector2DF(mv, mv)));
            test(AxisTransform2D.FromScaleAndTranslation(new Vector2DF(mv, mv), new Vector2DF(0, 0)));
            test(AxisTransform2D.FromScaleAndTranslation(new Vector2DF(1, 1), new Vector2DF(mv, mv)));
        }
    }

    private static void TestDecompose()
    {
        {
            var transform = AxisTransform2D.FromScaleAndTranslation(new Vector2DF(2.5f, -3.75f), new Vector2DF(4.25f, -5.5f));
            DecomposedTransform decomp = transform.Decompose();
            DecomposedTransform expected = new();

            expected.SetTranslate(4.25f, -5.5f, 0f);
            expected.SetScale(2.5f, -3.75f, 1f);
            expected.SetSkew(0f, 0f, 0f);
            expected.SetPerspective(0f, 0f, 0f, 1f);
            expected.SetQuaternion(0f, 0f, 0f, 1f);

            Debug.Assert(expected == decomp);
            //Debug.Assert(new Transform(transform), Transform.Compose(decomp));
        }
        {
            var transform = AxisTransform2D.FromScaleAndTranslation(new Vector2DF(-2.5f, -3.75f), new Vector2DF(4.25f, -5.5f));
            DecomposedTransform decomp = transform.Decompose();
            DecomposedTransform expected = new();

            expected.SetTranslate(4.25f, -5.5f, 0f);
            expected.SetScale(2.5f, 3.75f, 1f);
            expected.SetSkew(0f, 0f, 0f);
            expected.SetPerspective(0f, 0f, 0f, 1f);
            expected.SetQuaternion(0f, 0f, 1f, 0f);

            Debug.Assert(expected == decomp);
            //Debug.Assert(new Transform(transform) == Transform.Compose(decomp));
        }
        {
            var transform =
            AxisTransform2D.FromScaleAndTranslation(new Vector2DF(), new Vector2DF());
            DecomposedTransform decomp = transform.Decompose();
            DecomposedTransform expected = new();

            expected.SetTranslate(0f, 0f, 0f);
            expected.SetScale(0f, 0f, 1f);
            expected.SetSkew(0f, 0f, 0f);
            expected.SetPerspective(0f, 0f, 0f, 1f);
            expected.SetQuaternion(0f, 0f, 0f, 1f);

            Debug.Assert(expected == decomp);
            //Debug.Assert(new Transform(transform) == Transform.Compose(decomp));
        }
    }
}
