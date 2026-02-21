using System.Diagnostics;
using UI.GFX.Geometry;

namespace UI.Tests;

public static class Vector2DTest
{
    public static void Run()
    {
        TestIsZero();
        TestAdd();
        TestNegative();
        TestLength();
        TestSetToMinMax();
        TestIntegerOverflow();
        TestTranspose();

        Debug.WriteLine("All Vector2D tests passed!");
    }

    private static void TestIsZero()
    {
        Debug.Assert(new Vector2D().IsZero());
        Debug.Assert(new Vector2D(0, 0).IsZero());
        Debug.Assert(!new Vector2D(1, 0).IsZero());
        Debug.Assert(!new Vector2D(0, -2).IsZero());
        Debug.Assert(!new Vector2D(1, -2).IsZero());
    }

    private static void TestAdd()
    {
        Vector2D i1 = new(3, 5);
        Vector2D i2 = new(4, -1);
        Debug.Assert(new Vector2D(3, 5) == i1 + new Vector2D());
        Debug.Assert(new Vector2D(3 + 4, 5 - 1) == i1 + i2);
        Debug.Assert(new Vector2D(3 - 4, 5 + 1) == i1 - i2);
    }

    private static void TestNegative()
    {
        Debug.Assert(new Vector2D(0, 0) == -new Vector2D(0, 0));
        Debug.Assert(new Vector2D(-3, -3) == -new Vector2D(3, 3));
        Debug.Assert(new Vector2D(3, 3) == -new Vector2D(-3, -3));
        Debug.Assert(new Vector2D(-3, 3) == -new Vector2D(3, -3));
        Debug.Assert(new Vector2D(3, -3) == -new Vector2D(-3, 3));
    }

    private static void TestLength()
    {
        (int X, int Y)[] values =
        [
            (0, 0),
            (10, 20),
            (20, 10),
            (-10, -20),
            (-20, 10),
            (10, -20)
        ];

        foreach (var (X, Y) in values)
        {
            double length_squared =  (double)X * X + (double)Y * Y;
            double length = Math.Sqrt(length_squared);
            Vector2D vector = new(X, Y);
            Debug.Assert((float)length_squared == vector.LengthSquared());
            Debug.Assert((float)length == vector.Length());
        }
    }

    private static void TestSetToMinMax()
    {
        Vector2D a;

        a = new Vector2D(3, 5);
        Debug.Assert(new Vector2D(3, 5) == a);
        a.SetToMax(new Vector2D(2, 4));
        Debug.Assert(new Vector2D(3, 5) == a);
        a.SetToMax(new Vector2D(3, 5));
        Debug.Assert(new Vector2D(3, 5) == a);
        a.SetToMax(new Vector2D(4, 2));
        Debug.Assert(new Vector2D(4, 5) == a);
        a.SetToMax(new Vector2D(8, 10));
        Debug.Assert(new Vector2D(8, 10) == a);

        a.SetToMin(new Vector2D(9, 11));
        Debug.Assert(new Vector2D(8, 10) == a);
        a.SetToMin(new Vector2D(8, 10));
        Debug.Assert(new Vector2D(8, 10) == a);
        a.SetToMin(new Vector2D(11, 9));
        Debug.Assert(new Vector2D(8, 9) == a);
        a.SetToMin(new Vector2D(7, 11));
        Debug.Assert(new Vector2D(7, 9) == a);
        a.SetToMin(new Vector2D(3, 5));
        Debug.Assert(new Vector2D(3, 5) == a);
    }

    private static void TestIntegerOverflow()
    {
        int int_max = int.MaxValue;
        int int_min = int.MinValue;

        Vector2D max_vector = new(int_max, int_max);
        Vector2D min_vector = new(int_min, int_min);
        Vector2D test;

        test = new Vector2D();
        test += new Vector2D(int_max, int_max);
        Debug.Assert(test == max_vector);

        test = new Vector2D();
        test += new Vector2D(int_min, int_min);
        Debug.Assert(test == min_vector);

        test = new Vector2D(10, 20);
        test += new Vector2D(int_max, int_max);
        Debug.Assert(test == max_vector);

        test = new Vector2D(-10, -20);
        test += new Vector2D(int_min, int_min);
        Debug.Assert(test == min_vector);

        test = new Vector2D();
        test -= new Vector2D(int_max, int_max);
        Debug.Assert(test == new Vector2D(-int_max, -int_max));

        test = new Vector2D();
        test -= new Vector2D(int_min, int_min);
        Debug.Assert(test == max_vector);

        test = new Vector2D(10, 20);
        test -= new Vector2D(int_min, int_min);
        Debug.Assert(test == max_vector);

        test = new Vector2D(-10, -20);
        test -= new Vector2D(int_max, int_max);
        Debug.Assert(test == min_vector);

        test = new Vector2D();
        test -= new Vector2D(int_min, int_min);
        Debug.Assert(test == max_vector);

        test = -new Vector2D(int_min, int_min);
        Debug.Assert(test == max_vector);
    }

    private static void TestTranspose()
    {
        Vector2D v = new(1, -2);
        Debug.Assert(new Vector2D(-2, 1) == Vector2D.TransposeVector2D(v));
        v.Transpose();
        Debug.Assert(new Vector2D(-2, 1) == v);
    }
}
