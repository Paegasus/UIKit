using UI.Geometry;
using SkiaSharp;

using UI.GFX.Geometry;
using System.Diagnostics;

namespace UI;

public static class Program
{
    public static void Main(string[] args)
    {
        Transform t = new();

        Debug.WriteLine(t);

        TransformOperation to = new();

        Debug.WriteLine(to.Matrix);
    }
}
