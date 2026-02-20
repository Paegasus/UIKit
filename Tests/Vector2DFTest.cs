using System.Diagnostics;
using UI.GFX.Geometry;

namespace UI.Tests;

public static class Vector2DFTest
{
    public static void Run()
    {
        Vector2DFTestVector2DToVector2DF();
        Vector2DFTestIsZero();
        Vector2DFTestAdd();
        Vector2DFTestNegative();
        Vector2DFTestLength();
        Vector2DFTestSetToMinMax();
        Vector2DFTestIntegerOverflow();
        Vector2DFTestTranspose();

        Debug.WriteLine("All Vector2DF tests passed!");
    }

    public static void Vector2DFTestVector2DToVector2DF()
    {
        Vector2D i = new(3, 4);
        Vector2DF f = i;
        Debug.Assert(i == f);
    }

    public static void Vector2DFTestIsZero()
    {
        Debug.Assert(new Vector2DF().IsZero());
        Debug.Assert(new Vector2DF(0, 0).IsZero());
        Debug.Assert(!new Vector2DF(0.1f, 0).IsZero());
        Debug.Assert(!new Vector2DF(0, -0.1f).IsZero());
        Debug.Assert(!new Vector2DF(0.1f, -0.1f).IsZero());
    }

    public static void Vector2DFTestAdd()
    {
        
    }

    public static void Vector2DFTestNegative()
    {
        
    }

    public static void Vector2DFTestLength()
    {
        
    }

    public static void Vector2DFTestSetToMinMax()
    {
        
    }

    public static void Vector2DFTestIntegerOverflow()
    {
        
    }

    public static void Vector2DFTestTranspose()
    {
        
    }
}
