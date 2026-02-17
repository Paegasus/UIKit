namespace UI.GFX.Geometry;

// Contains the components of a factored transform.
// These components may be blended and recomposed.
public struct DecomposedTransform
{
    public Vector3D Translate;
    public Vector3D Scale;
    public Vector3D Skew;
    public Vector4D Perspective;
    public Quaternion Quaternion;

    // The default constructor initializes the components in such a way
    // that will compose the identity transform.
    public DecomposedTransform()
    {
        Translate = new Vector3D(0, 0, 0);
        Scale = new Vector3D(1, 1, 1);
        Skew = new Vector3D(0, 0, 0);
        Perspective = new Vector4D(0, 0, 0, 1);
        Quaternion = new Quaternion();
    }

    public override readonly string ToString()
    {
      return $"translate: {Translate.X} {Translate.Y} {Translate.Z}\nscale: {Scale.X} {Scale.Y} {Scale.Z}\nskew: {Skew.X} {Skew.Y} {Skew.Z}\nperspective: {Perspective.X} {Perspective.Y} {Perspective.Z} {Perspective.W}\nquaternion: {Quaternion.X} {Quaternion.Y} {Quaternion.Z} {Quaternion.W}\n";
    }
}

// We need these vector structs because doing double[] instead would lead to heap allocations.

public readonly struct Vector3D
{
    public readonly double X, Y, Z;

    public Vector3D(double x, double y, double z) => (X, Y, Z) = (x, y, z);
}

public readonly struct Vector4D
{
    public readonly double X, Y, Z, W;

    public Vector4D(double x, double y, double z, double w) => (X, Y, Z, W) = (x, y, z, w);
}
