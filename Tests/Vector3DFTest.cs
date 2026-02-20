using System.Diagnostics;
using UI.GFX.Geometry;

namespace UI.Tests;

public static class Vector3DFTest
{
    public static void Run()
    {
        TestIsZero();
        TestAdd();
        TestNegative();
        TestScale();
        TestLength();
        TestDotProduct();
        TestCrossProduct();
        TestClampVector3dF();
        TestAngleBetweenVectorsInDegress();
        TestClockwiseAngleBetweenVectorsInDegress();
        TestGetNormalized();
        TestToString();

        Debug.WriteLine("All Vector3DF tests passed!");
    }

    private static bool FloatEqual(float a, float b) => MathF.Abs(a - b) <= 1e-6f;

    private static bool FloatNear(float val1, float val2, float abs_error) => MathF.Abs(val1 - val2) <= abs_error;

    public static void TestIsZero()
    {
        Vector3DF float_zero = new(0, 0, 0);
        Vector3DF float_nonzero = new(0.1f, -0.1f, 0.1f);

        Debug.Assert(float_zero.IsZero());
        Debug.Assert(!float_nonzero.IsZero());
    }

    public static void TestAdd()
    {
        Vector3DF f1 = new(3.1f, 5.1f, 2.7f);
        Vector3DF f2 = new(4.3f, -1.3f, 8.1f);

        (Vector3DF expected, Vector3DF actual)[] values =
        [
            (new Vector3DF(3.1F, 5.1F, 2.7f), f1 + new Vector3DF()),
            (new Vector3DF(3.1f + 4.3f, 5.1f - 1.3f, 2.7f + 8.1f), f1 + f2),
            (new Vector3DF(3.1f - 4.3f, 5.1f + 1.3f, 2.7f - 8.1f), f1 - f2)
        ];

        foreach (var (expected, actual) in values)
        {
            Debug.Assert(expected.ToString() == actual.ToString());
        }
    }

    public static void TestNegative()
    {
        (Vector3DF expected, Vector3DF actual)[] values =
        [
            (new Vector3DF(-0.0f, -0.0f, -0.0f), -new Vector3DF(0, 0, 0)),
            (new Vector3DF(-0.3f, -0.3f, -0.3f), -new Vector3DF(0.3f, 0.3f, 0.3f)),
            (new Vector3DF(0.3f, 0.3f, 0.3f), -new Vector3DF(-0.3f, -0.3f, -0.3f)),
            (new Vector3DF(-0.3f, 0.3f, -0.3f), -new Vector3DF(0.3f, -0.3f, 0.3f)),
            (new Vector3DF(0.3f, -0.3f, -0.3f), -new Vector3DF(-0.3f, 0.3f, 0.3f)),
            (new Vector3DF(-0.3f, -0.3f, 0.3f), -new Vector3DF(0.3f, 0.3f, -0.3f))
        ];

        foreach (var (expected, actual) in values)
            Debug.Assert(expected.ToString() == actual.ToString());
    }

    public static void TestScale()
    {
        (float, float, float, float, float, float)[] triple_values =
        [
            (4.5f, 1.2f, 1.8f, 3.3f, 5.6f, 4.2f),
            (4.5f, -1.2f, -1.8f, 3.3f, 5.6f, 4.2f),
            (4.5f, 1.2f, -1.8f, 3.3f, 5.6f, 4.2f),
            (4.5f, -1.2f, 1.8f, 3.3f, 5.6f, 4.2f),

            (4.5f, 1.2f, 1.8f, 3.3f, -5.6f, -4.2f),
            (4.5f, 1.2f, 1.8f, -3.3f, -5.6f, -4.2f),
            (4.5f, 1.2f, -1.8f, 3.3f, -5.6f, -4.2f),
            (4.5f, 1.2f, -1.8f, -3.3f, -5.6f, -4.2f),

            (-4.5f, 1.2f, 1.8f, 3.3f, 5.6f, 4.2f),
            (-4.5f, 1.2f, 1.8f, 0, 5.6f, 4.2f),
            (-4.5f, 1.2f, -1.8f, 3.3f, 5.6f, 4.2f),
            (-4.5f, 1.2f, -1.8f, 0, 5.6f, 4.2f),

            (-4.5f, 1.2f, 1.8f, 3.3f, 0, 4.2f),
            (4.5f, 0, 1.8f, 3.3f, 5.6f, 4.2f),
            (-4.5f, 1.2f, -1.8f, 3.3f, 0, 4.2f),
            (4.5f, 0, -1.8f, 3.3f, 5.6f, 4.2f),
            (-4.5f, 1.2f, 1.8f, 3.3f, 5.6f, 0),
            (-4.5f, 1.2f, -1.8f, 3.3f, 5.6f, 0),

            (0, 1.2f, 0, 3.3f, 5.6f, 4.2f),
            (0, 1.2f, 1.8f, 3.3f, 5.6f, 4.2f)
        ];

        foreach (var (ONE, TWO, THREE, FOUR, FIVE, SIX) in triple_values)
        {
            Vector3DF v = new(ONE, TWO, THREE);
            v.Scale(FOUR, FIVE, SIX);
            Debug.Assert(ONE * FOUR == v.X);
            Debug.Assert(TWO * FIVE == v.Y);
            Debug.Assert(THREE * SIX == v.Z);

            Vector3DF v2 = Vector3DF.ScaleVector3D(new Vector3DF(ONE, TWO, THREE), FOUR, FIVE, SIX);
            Debug.Assert(ONE * FOUR == v2.X);
            Debug.Assert(TWO * FIVE == v2.Y);
            Debug.Assert(THREE * SIX == v2.Z);
        }

        (float, float, float, float)[] single_values =
        [
            (4.5f, 1.2f, 1.8f, 3.3f),
            (4.5f, -1.2f, 1.8f, 3.3f),
            (4.5f, 1.2f, -1.8f, 3.3f),
            (4.5f, -1.2f, -1.8f, 3.3f),
            (-4.5f, 1.2f, 3.3f, 0),
            (-4.5f, 1.2f, 0, 0),
            (-4.5f, 1.2f, 1.8f, 3.3f),
            (-4.5f, 1.2f, 1.8f, 0),
            (4.5f, 0, 1.8f, 3.3f),
            (0, 1.2f, 1.8f, 3.3f),
            (4.5f, 0, 1.8f, 3.3f),
            (0, 1.2f, 1.8f, 3.3f),
            (4.5f, 1.2f, 0, 3.3f),
            (4.5f, 1.2f, 0, 3.3f)
        ];

        foreach(var (ONE, TWO, THREE, FOUR) in single_values)
        {
            Vector3DF v = new(ONE, TWO, THREE);
            v.Scale(FOUR);
            Debug.Assert(ONE * FOUR == v.X);
            Debug.Assert(TWO * FOUR == v.Y);
            Debug.Assert(THREE * FOUR == v.Z);

            Vector3DF v2 = Vector3DF.ScaleVector3D(new Vector3DF(ONE, TWO, THREE), FOUR);
            Debug.Assert(ONE * FOUR == v2.X);
            Debug.Assert(TWO * FOUR == v2.Y);
            Debug.Assert(THREE * FOUR == v2.Z);
        }
    }

    public static void TestLength()
    {
        
    }

    public static void TestDotProduct()
    {
        
    }
    public static void TestCrossProduct()
    {
        
    }
    public static void TestClampVector3dF()
    {
        
    }
    public static void TestAngleBetweenVectorsInDegress()
    {
        
    }

    public static void TestClockwiseAngleBetweenVectorsInDegress()
    {
        
    }

    public static void TestGetNormalized()
    {
        
    }

    public static void TestToString()
    {
        Debug.Assert("[1.03125 2.5 -3]" == new Vector3DF(1.03125f, 2.5f, -3f).ToString());
    }
}
