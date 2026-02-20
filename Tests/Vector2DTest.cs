using System.Diagnostics;
using UI.GFX.Geometry;

namespace UI.Tests;

public static class Vector2DTest
{
    public static void Run()
    {
        Vector2DTestIsZero();
        Vector2DTestAdd();
        Vector2DTestNegative();
        Vector2DTestLength();
        Vector2DTestSetToMinMax();
        Vector2DTestIntegerOverflow();
        Vector2DTestTranspose();

        Debug.WriteLine("All Vector2D tests passed!");
    }

    public static void Vector2DTestIsZero()
    {
        Debug.Assert(new Vector2D().IsZero());
        Debug.Assert(new Vector2D(0, 0).IsZero());
        Debug.Assert(!new Vector2D(1, 0).IsZero());
        Debug.Assert(!new Vector2D(0, -2).IsZero());
        Debug.Assert(!new Vector2D(1, -2).IsZero());
    }

    public static void Vector2DTestAdd()
    {
        Vector2D i1 = new(3, 5);
        Vector2D i2 = new(4, -1);
        Debug.Assert(new Vector2D(3, 5) == i1 + new Vector2D());
        Debug.Assert(new Vector2D(3 + 4, 5 - 1) == i1 + i2);
        Debug.Assert(new Vector2D(3 - 4, 5 + 1) == i1 - i2);
    }

    public static void Vector2DTestNegative()
    {
        Debug.Assert(new Vector2D(0, 0) == -new Vector2D(0, 0));
        Debug.Assert(new Vector2D(-3, -3) == -new Vector2D(3, 3));
        Debug.Assert(new Vector2D(3, 3) == -new Vector2D(-3, -3));
        Debug.Assert(new Vector2D(-3, 3) == -new Vector2D(3, -3));
        Debug.Assert(new Vector2D(3, -3) == -new Vector2D(-3, 3));
    }

    public static void Vector2DTestLength()
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

    public static void Vector2DTestSetToMinMax()
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

    public static void Vector2DTestIntegerOverflow()
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

    public static void Vector2DTestTranspose()
    {
        Vector2D v = new(1, -2);
        Debug.Assert(new Vector2D(-2, 1) == Vector2D.TransposeVector2D(v));
        v.Transpose();
        Debug.Assert(new Vector2D(-2, 1) == v);
    }
}
