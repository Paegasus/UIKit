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
        TestLength();
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
        
    }

    public static void TestNegative()
    {
        
    }

    public static void TestLength()
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
