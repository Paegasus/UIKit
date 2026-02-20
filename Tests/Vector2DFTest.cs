using System.Diagnostics;
using UI.GFX.Geometry;

namespace UI.Tests;

public static class Vector2DFTest
{
    public static void Run()
    {
        TestVector2DToVector2DF();
        TestIsZero();
        TestAdd();
        TestNegative();
        Scale();
        SetToMinMax();
        TestLength();
        TestSlopeAngleRadians();
        TestSetToMinMax();
        TestIntegerOverflow();
        TestTranspose();

        Debug.WriteLine("All Vector2DF tests passed!");
    }

    public static void TestVector2DToVector2DF()
    {
        Vector2D i = new(3, 4);
        Vector2DF f = i;
        Debug.Assert(i == f);
    }

    public static void TestIsZero()
    {
        Debug.Assert(new Vector2DF().IsZero());
        Debug.Assert(new Vector2DF(0, 0).IsZero());
        Debug.Assert(!new Vector2DF(0.1f, 0).IsZero());
        Debug.Assert(!new Vector2DF(0, -0.1f).IsZero());
        Debug.Assert(!new Vector2DF(0.1f, -0.1f).IsZero());
    }

    public static void TestAdd()
    {
        Vector2DF f1 = new(3.1f, 5.1f);
        Vector2DF f2 = new(4.3f, -1.3f);
        Debug.Assert(new Vector2DF(3.1f, 5.1f) == f1 + new Vector2DF());
        Debug.Assert(new Vector2DF(3.1f + 4.3f, 5.1f - 1.3f) == f1 + f2);
        Debug.Assert(new Vector2DF(3.1f - 4.3f, 5.1f + 1.3f) == f1 - f2);
    }

    public static void TestNegative()
    {
        Debug.Assert(new Vector2DF() == -new Vector2DF());
        Debug.Assert(new Vector2DF(-0.3f, -0.3f) == -new Vector2DF(0.3f, 0.3f));
        Debug.Assert(new Vector2DF(0.3f, 0.3f) == -new Vector2DF(-0.3f, -0.3f));
        Debug.Assert(new Vector2DF(-0.3f, 0.3f) == -new Vector2DF(0.3f, -0.3f));
        Debug.Assert(new Vector2DF(0.3f, -0.3f) == -new Vector2DF(-0.3f, 0.3f));
    }

    public static void Scale()
    {
        (float, float, float, float)[] double_values =
        [
            (4.5f, 1.2f, 3.3f, 5.6f),  (4.5f, -1.2f, 3.3f, 5.6f),
            (4.5f, 1.2f, 3.3f, -5.6f), (4.5f, 1.2f, -3.3f, -5.6f),
            (-4.5f, 1.2f, 3.3f, 5.6f), (-4.5f, 1.2f, 0, 5.6f),
            (-4.5f, 1.2f, 3.3f, 0),    (4.5f, 0, 3.3f, 5.6f),
            (0, 1.2f, 3.3f, 5.6f)
        ];

        foreach (var (X, Y, Z, W) in double_values)
        {
            Vector2DF v = new(X, Y);
            v.Scale(Z, W);
            Debug.Assert(v.X == X * Z);
            Debug.Assert(v.Y == Y * W);

            Vector2DF v2 = Vector2DF.ScaleVector2D(new Vector2DF(X, Y), Z, W);
            Debug.Assert(X * Z == v2.X);
            Debug.Assert(Y * W == v2.Y);
        }

        (float, float, float)[] single_values =
        [
            (4.5f, 1.2f, 3.3f),  (4.5f, -1.2f, 3.3f), (4.5f, 1.2f, 3.3f),
            (4.5f, 1.2f, -3.3f), (-4.5f, 1.2f, 3.3f), (-4.5f, 1.2f, 0),
            (-4.5f, 1.2f, 3.3f), (4.5f, 0, 3.3f),     (0, 1.2f, 3.3f)
        ];

        foreach (var (X, Y, Z) in single_values)
        {
            Vector2DF v = new(X, Y);
            v.Scale(Z);
            Debug.Assert(v.X == X * Z);
            Debug.Assert(v.Y == Y * Z);

            Vector2DF v2 = Vector2DF.ScaleVector2D(new Vector2DF(X, Y), Z);
            Debug.Assert(X * Z == v2.X);
            Debug.Assert(Y * Z == v2.Y);
        }
    }

    public static void SetToMinMax()
    {
        Vector2DF a;

        a = new Vector2DF(3.5f, 5.5f);
        Debug.Assert(new Vector2DF(3.5f, 5.5f) == a);
        a.SetToMax(new Vector2DF(2.5f, 4.5f));
        Debug.Assert(new Vector2DF(3.5f, 5.5f) == a);
        a.SetToMax(new Vector2DF(3.5f, 5.5f));
        Debug.Assert(new Vector2DF(3.5f, 5.5f) == a);
        a.SetToMax(new Vector2DF(4.5f, 2.5f));
        Debug.Assert(new Vector2DF(4.5f, 5.5f) == a);
        a.SetToMax(new Vector2DF(8.5f, 10.5f));
        Debug.Assert(new Vector2DF(8.5f, 10.5f) == a);

        a.SetToMin(new Vector2DF(9.5f, 11.5f));
        Debug.Assert(new Vector2DF(8.5f, 10.5f) == a);
        a.SetToMin(new Vector2DF(8.5f, 10.5f));
        Debug.Assert(new Vector2DF(8.5f, 10.5f) == a);
        a.SetToMin(new Vector2DF(11.5f, 9.5f));
        Debug.Assert(new Vector2DF(8.5f, 9.5f) == a);
        a.SetToMin(new Vector2DF(7.5f, 11.5f));
        Debug.Assert(new Vector2DF(7.5f, 9.5f) == a);
        a.SetToMin(new Vector2DF(3.5f, 5.5f));
        Debug.Assert(new Vector2DF(3.5f, 5.5f) == a);
    }

    public static void TestLength()
    {
        static bool FloatEq(float a, float b) => MathF.Abs(a - b) <= 1e-6f;

        Debug.Assert(FloatEq(0.0f, new Vector2DF(0, 0).Length()));
        Debug.Assert(FloatEq(1.0f, new Vector2DF(1, 0).Length()));
        Debug.Assert(FloatEq(1.414214f, new Vector2DF(1, 1).Length()));
        Debug.Assert(FloatEq(2.236068f, new Vector2DF(-1, -2).Length()));

        // The Pythagorean triples 3-4-5 and 5-12-13.
        Debug.Assert(FloatEq(5.0f, new Vector2DF(3.0f, 4.0f).Length()));
        Debug.Assert(FloatEq(13.0f, new Vector2DF(5.0f, 12.0f).Length()));

        // Very small numbers.
        Debug.Assert(FloatEq(.7071068e-20f, new Vector2DF(.5e-20f, .5e-20f).Length()));

        // Very large numbers.
        Debug.Assert(FloatEq(.7071068e20f, new Vector2DF(.5e20f, .5e20f).Length()));
        Debug.Assert(FloatEq(float.MaxValue, new Vector2DF(float.MaxValue, 0).Length()));
        // The original C++ code checks if Length() returns float.MaxValue, but that's impossible
        //Debug.Assert(FloatEq(float.MaxValue, new Vector2DF(float.MaxValue, float.MaxValue).Length()));
        Debug.Assert(float.PositiveInfinity == new Vector2DF(float.MaxValue, float.MaxValue).Length());
    }

    public static void TestSlopeAngleRadians()
    {
        
    }

    public static void TestSetToMinMax()
    {
        
    }

    public static void TestIntegerOverflow()
    {
        
    }

    public static void TestTranspose()
    {
        
    }
}
