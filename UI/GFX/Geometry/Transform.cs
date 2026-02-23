using System.Diagnostics;
using static UI.Numerics.ClampedMath;

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
        
    }

    public Matrix44 AxisTransform2dToMatrix44(in AxisTransform2D axis_2d)
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
            matrix_ = AxisTransform2dToMatrix44(axis_2d_);
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

    public static Transform Compose(in DecomposedTransform decomp)
    {
        throw new NotImplementedException();

        /*
        Transform result = new();

        if (decomp.Perspective.X != 0)
            result.set_rc(3, 0, decomp.Perspective.X);
        
        if (decomp.Perspective.Y != 0)
            result.set_rc(3, 1, decomp.Perspective.Y);
        
        if (decomp.Perspective.Z != 0)
            result.set_rc(3, 2, decomp.Perspective.Z);
        
        if (decomp.Perspective.W != 1)
            result.set_rc(3, 3, decomp.Perspective.W);

        result.Translate3D(decomp.Translate.X, decomp.Translate.Y, decomp.Translate.Z);

        result.PreConcat(new Transform(decomp.Quaternion));

        if (decomp.Skew.X || decomp.Skew.Y || decomp.Skew.Z)
            result.EnsureFullMatrix().ApplyDecomposedSkews(decomp.Skew);

        result.Scale3D(decomp.Scale.X, decomp.Scale.Y, decomp.Scale.Z);

        return result;
        */
    }
}
