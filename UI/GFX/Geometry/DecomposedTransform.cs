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

    public void SetTranslate(double x, double y, double z)
    {
        Translate.X = x;
        Translate.Y = y;
        Translate.Z = z;
    }

    public void SetScale(double x, double y, double z)
    {
        Scale.X = x;
        Scale.Y = y;
        Scale.Z = z;
    }

    public void SetSkew(double x, double y, double z)
    {
        Skew.X = x;
        Skew.Y = y;
        Skew.Z = z;
    }

    public void SetPerspective(double x, double y, double z, double w)
    {
        Perspective.X = x;
        Perspective.Y = y;
        Perspective.Z = z;
        Perspective.W = w;
    }

    public void SetQuaternion(double x, double y, double z, double w)
    {
        Quaternion.X = x;
        Quaternion.Y = y;
        Quaternion.Z = z;
        Quaternion.W = w;
    }

    public override readonly string ToString()
    {
      return $"translate: {Translate.X} {Translate.Y} {Translate.Z}\nscale: {Scale.X} {Scale.Y} {Scale.Z}\nskew: {Skew.X} {Skew.Y} {Skew.Z}\nperspective: {Perspective.X} {Perspective.Y} {Perspective.Z} {Perspective.W}\nquaternion: {Quaternion.X} {Quaternion.Y} {Quaternion.Z} {Quaternion.W}\n";
    }

    public override readonly int GetHashCode() => HashCode.Combine(Translate, Scale, Skew, Perspective, Quaternion);
    
    public readonly bool Equals(DecomposedTransform other) => Translate == other.Translate && Scale == other.Scale && Skew == other.Skew && Perspective == other.Perspective && Quaternion == other.Quaternion;

    public override readonly bool Equals(object? obj) => obj is DecomposedTransform other && Equals(other);

    public static bool operator == (in DecomposedTransform left, in DecomposedTransform right) => left.Equals(right);
    public static bool operator != (in DecomposedTransform left, in DecomposedTransform right) => !left.Equals(right);
}

// We need these vector structs because doing double[] instead would lead to heap allocations.

public struct Vector3D
{
    public double X, Y, Z;

    public Vector3D() => (X, Y, Z) = (0, 0, 0);

    public Vector3D(double x, double y, double z) => (X, Y, Z) = (x, y, z);

    public void Set(double x, double y, double z) => (X, Y, Z) = (x, y, z);

    public override readonly int GetHashCode() => HashCode.Combine(X, Y, Z);
    
    public readonly bool Equals(in Vector3D other) => X == other.X && Y == other.Y && Z == other.Z;

    public override readonly bool Equals(object? obj) => obj is Vector3D other && Equals(other);

    public static bool operator == (in Vector3D left, in Vector3D right) => left.Equals(right);
    public static bool operator != (in Vector3D left, in Vector3D right) => !left.Equals(right);
}

public struct Vector4D
{
    public double X, Y, Z, W;

    public Vector4D() => (X, Y, Z, W) = (0, 0, 0, 0);

    public Vector4D(double x, double y, double z, double w) => (X, Y, Z, W) = (x, y, z, w);

    public void Set(double x, double y, double z, double w) => (X, Y, Z, W) = (x, y, z, w);

    public override readonly int GetHashCode() => HashCode.Combine(X, Y, Z, W);
    
    public readonly bool Equals(Vector4D other) => X == other.X && Y == other.Y && Z == other.Z && W == other.W;

    public override readonly bool Equals(object? obj) => obj is Vector4D other && Equals(other);

    public static bool operator == (in Vector4D left, in Vector4D right) => left.Equals(right);
    public static bool operator != (in Vector4D left, in Vector4D right) => !left.Equals(right);
}
