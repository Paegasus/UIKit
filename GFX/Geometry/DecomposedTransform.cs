namespace UI.GFX.Geometry;

using System.Runtime.CompilerServices;
using static Numerics.ClampedMath;

// Contains the components of a factored transform.
// These components may be blended and recomposed.
public struct DecomposedTransform
{
    // The default constructor initializes the components in such a way that
    // will compose the identity transform.
    public DecomposedTransform(int x, int y)
    {
        x_ = x;
        y_ = y;
    }
}
