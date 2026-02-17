namespace UI.GFX.Geometry;

// Contains the components of a factored transform.
// These components may be blended and recomposed.
public struct DecomposedTransform
{
    public Vector3D translate;
    public Vector3D scale;
    public Vector3D skew;
    public Vector4D perspective;
    public Quaternion quaternion;

    // The default constructor initializes the components in such a way
    // that will compose the identity transform.
    public DecomposedTransform()
    {
        translate = new Vector3D(0, 0, 0);
        scale = new Vector3D(1, 1, 1);
        skew = new Vector3D(0, 0, 0);
        perspective = new Vector4D(0, 0, 0, 1);
        quaternion = new Quaternion();
    }

    public override readonly string ToString()
    {
      return $"translate: {translate.X} {translate.Y} {translate.Z}\nscale: {scale.X} {scale.Y} {scale.Z}\nskew: {skew.X} {skew.Y} {skew.Z}\nperspective: {perspective.X} {perspective.Y} {perspective.Z} {perspective.W}\nquaternion: {quaternion.X} {quaternion.Y} {quaternion.Z} {quaternion.W}\n";
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
