using System.Diagnostics;

namespace UI.GFX.Geometry;

// 4x4 Transformation matrix. Depending on the complexity of the matrix, it may
// be internally stored as an AxisTransform2d (float precision) or a full
// Matrix44 (4x4 double precision). Which one is used only affects precision and
// performance.
// - On construction (including constructors and static functions returning a
//   new Transform object), AxisTransform2d will be used if it the matrix will
//   be 2d scale and/or translation, otherwise Matrix44, with some exceptions
//   (e.g. ColMajor()) described in the method comments.
// - On mutation, if the matrix has been using AxisTransform2d and the result
//   can still be 2d scale and/or translation, AxisTransform2d will still be
//   used, otherwise Matrix44, with some exceptions (e.g. set_rc()) described
//   in the method comments.
// - On assignment, the new matrix will keep the choice of the rhs matrix.
//
public struct Transform
{
    // axis_2d_ is used if full_matrix_ is false, otherwise matrix_ is used.
    // See the class documentation for more details about how we use them.
    private bool full_matrix_ = false;

    // Each constructor must explicitly initialize one of the following,
    // according to the value of full_matrix_.
    AxisTransform2D axis_2d_;
    Matrix44 matrix_;

    public Transform()
    {
        axis_2d_ = new AxisTransform2D();
    }

    public Transform(in AxisTransform2D axis_2d)
    {
        axis_2d_ = axis_2d;
    }

    // Used internally to construct Transform with parameters in col-major order.
    Transform(double r0c0, double r1c0, double r2c0, double r3c0,
              double r0c1, double r1c1, double r2c1, double r3c1,
              double r0c2, double r1c2, double r2c2, double r3c2,
              double r0c3, double r1c3, double r2c3, double r3c3)
    {
        full_matrix_ = true;

        matrix_ = new(r0c0, r1c0, r2c0, r3c0,
                      r0c1, r1c1, r2c1, r3c1,
                      r0c2, r1c2, r2c2, r3c2,
                      r0c3, r1c3, r2c3, r3c3);
    }

    Transform(in Quaternion q) : this(
          // Col 0.
          1.0 - 2.0 * (q.Y * q.Y + q.Z * q.Z),
          2.0 * (q.X * q.Y + q.Z * q.W),
          2.0 * (q.X * q.Z - q.Y * q.W),
          0,
          // Col 1.
          2.0 * (q.X * q.Y - q.Z * q.W),
          1.0 - 2.0 * (q.X * q.X + q.Z * q.Z),
          2.0 * (q.Y * q.Z + q.X * q.W),
          0,
          // Col 2.
          2.0 * (q.X * q.Z + q.Y * q.W),
          2.0 * (q.Y * q.Z - q.X * q.W),
          1.0 - 2.0 * (q.X * q.X + q.Y * q.Y),
          0,
          // Col 3.
          0, 0, 0, 1) {}

    public static Matrix44 AxisTransform2DToMatrix44(in AxisTransform2D axis_2d)
    {
        return new Matrix44(axis_2d.Scale.X, 0, 0, 0,  // col 0
                            0, axis_2d.Scale.Y, 0, 0,  // col 1
                            0, 0, 1, 0,                // col 2
                            axis_2d.Translation.X, axis_2d.Translation.Y, 0, 1);
    }

    public Matrix44 EnsureFullMatrix()
    {
        if (!full_matrix_)
        {
            full_matrix_ = true;
            matrix_ = AxisTransform2DToMatrix44(axis_2d_);
        }
        
        return matrix_;
    }

    // Sets a value in the matrix at |row|, |col|. It forces full double precision 4x4 matrix.
    public void set_rc(int row, int col, double v)
    {
#if DEBUG
        Debug.Assert((uint)row <= 3u);
        Debug.Assert((uint)col <= 3u);
#endif
        EnsureFullMatrix().set_rc(row, col, v);
    }

    public void Translate(float x, float y)
    {
        if (!full_matrix_)
        {
            axis_2d_.PreTranslate(new Vector2DF(x, y));
        }
        else
        {
            matrix_.PreTranslate(x, y);
        }
    }

    public void Translate(in Vector2DF offset)
    {
        Translate(offset.X, offset.Y);
    }

    void PostTranslate(float x, float y)
    {
        if (!full_matrix_)
        {
            axis_2d_.PostTranslate(new Vector2DF(x, y));
        }
        else
        {
            matrix_.PostTranslate(x, y);
        }
    }

    void PostTranslate(in Vector2DF offset)
    {
        PostTranslate(offset.X, offset.Y);
    }

    public void Translate3D(in Vector3DF offset)
    {
        Translate3D(offset.X, offset.Y, offset.Z);
    }

    public void Translate3D(float x, float y, float z)
    {
        if (z == 0)
            Translate(x, y);
        else
            EnsureFullMatrix().PreTranslate3D(x, y, z);
    }

    public void Scale(float x, float y)
    {
        if (!full_matrix_)
        {
            axis_2d_.PreScale(new Vector2DF(x, y));
        }
        else
        {
            matrix_.PreScale(x, y);
        }
    }

    public void Scale3D(float x, float y, float z)
    {
        if (z == 1)
            Scale(x, y);
        else
            EnsureFullMatrix().PreScale3D(x, y, z);
    }

    public void PostScale(float x, float y)
    {
        if (!full_matrix_)
        {
            axis_2d_.PostScale(new Vector2DF(x, y));
        }
        else
        {
            matrix_.PostScale(x, y);
        }
    }

    public void PreConcat(in AxisTransform2D transform)
    {
        Translate(transform.Translation);
        Scale(transform.Scale.X, transform.Scale.Y);
    }

    public void PreConcat(in Transform transform)
    {
        if (!transform.full_matrix_)
        {
            PreConcat(transform.axis_2d_);
        }
        else if (!full_matrix_)
        {
            AxisTransform2D self = axis_2d_;
            this = transform;
            PostConcat(self);
        }
        else
        {
            matrix_.PreConcat(transform.matrix_);
        }
    }

    public void PostConcat(in AxisTransform2D transform)
    {
        PostScale(transform.Scale.X, transform.Scale.Y);
        PostTranslate(transform.Translation);
    }

    public void PostConcat(in Transform transform)
    {
        if (!transform.full_matrix_)
        {
            PostConcat(transform.axis_2d_);
        }
        else if (!full_matrix_)
        {
            AxisTransform2D self = axis_2d_;
            this = transform;
            PreConcat(self);
        }
        else
        {
            matrix_.PostConcat(transform.matrix_);
        }
    }

    // Translate3D
    // Scale3D

    // Composes a transform from the given |decomp|, following the routines
    // detailed in this specs:
    // https://www.w3.org/TR/css-transforms-2/#recomposing-to-a-3d-matrix
    public static Transform Compose(in DecomposedTransform decomp)
    {
        Transform result = new();

        if (decomp.Perspective.X != 0)
            result.set_rc(3, 0, decomp.Perspective.X);
        
        if (decomp.Perspective.Y != 0)
            result.set_rc(3, 1, decomp.Perspective.Y);
        
        if (decomp.Perspective.Z != 0)
            result.set_rc(3, 2, decomp.Perspective.Z);
        
        if (decomp.Perspective.W != 1)
            result.set_rc(3, 3, decomp.Perspective.W);

        result.Translate3D((float)decomp.Translate.X, (float)decomp.Translate.Y, (float)decomp.Translate.Z);

        result.PreConcat(new Transform(decomp.Quaternion));

        if (decomp.Skew.X != 0 || decomp.Skew.Y != 0 || decomp.Skew.Z != 0)
            result.EnsureFullMatrix().ApplyDecomposedSkews(decomp.Skew);

        result.Scale3D((float)decomp.Scale.X, (float)decomp.Scale.Y, (float)decomp.Scale.Z);

        return result;
        
    }

    public readonly Matrix44 GetFullMatrix()
    {
        if (!full_matrix_)
        {
            return AxisTransform2DToMatrix44(axis_2d_);
        }
        return matrix_;
    }

    // It's not easy to get a hash code
    // because a Transform either uses a AxisTransform2D or a Matrix44
    //public override readonly int GetHashCode() => HashCode.Combine();

    public override readonly bool Equals(object? obj) => obj is Transform other && Equals(other);

    public readonly bool Equals(in Transform other)
    {
        if (!full_matrix_ && !other.full_matrix_)
        {
            return axis_2d_ == other.axis_2d_;
        }
        if (full_matrix_ && other.full_matrix_)
            return matrix_ == other.matrix_;

        return GetFullMatrix() == other.GetFullMatrix();
    }

    public static bool operator ==(in Transform left, in Transform right) => left.Equals(right);
    public static bool operator !=(in Transform left, in Transform right) => !left.Equals(right);
}
