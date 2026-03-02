using System.Diagnostics;
using System.Runtime.CompilerServices;
using UI.Extensions;

using static UI.GFX.Geometry.ClampFloatGeometryHelper;
using static UI.GFX.Geometry.PointConversions;

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

    public static double kEpsilon = float.MachineEpsilon;

    public Transform()
    {
        axis_2d_ = new AxisTransform2D();
    }

    public Transform(in AxisTransform2D axis_2d)
    {
        axis_2d_ = axis_2d;
    }

    // Used internally to construct a Transform with uninitialized full matrix.
    private Transform(Matrix44.UninitializedTag uninitializedTag)
    {
        full_matrix_ = true;
        matrix_ = new Matrix44(uninitializedTag);
    }

    // Creates a transform from explicit 16 matrix elements in row-major order.
    // Always creates a double precision 4x4 matrix.
    public static Transform RowMajor(
        double r0c0, double r0c1, double r0c2, double r0c3,
        double r1c0, double r1c1, double r1c2, double r1c3,
        double r2c0, double r2c1, double r2c2, double r2c3,
        double r3c0, double r3c1, double r3c2, double r3c3)
    {
        return new Transform(r0c0, r1c0, r2c0, r3c0,  // col 0
                             r0c1, r1c1, r2c1, r3c1,  // col 1
                             r0c2, r1c2, r2c2, r3c2,  // col 2
                             r0c3, r1c3, r2c3, r3c3); // col 3
    }

    // Creates a transform from explicit 16 matrix elements in col-major order.
    // Always creates a double precision 4x4 matrix.
    // See also ColMajor(double[]) and ColMajorF(float[]).
    public static Transform ColMajor(
      double r0c0, double r1c0, double r2c0, double r3c0,
      double r0c1, double r1c1, double r2c1, double r3c1,
      double r0c2, double r1c2, double r2c2, double r3c2,
      double r0c3, double r1c3, double r2c3, double r3c3)
      {
        return new Transform(r0c0, r1c0, r2c0, r3c0,   // col 0
                             r0c1, r1c1, r2c1, r3c1,   // col 1
                             r0c2, r1c2, r2c2, r3c2,   // col 2
                             r0c3, r1c3, r2c3, r3c3);  // col 3
    }

    // Constructs Transform from a float col-major array. Creates an
    // AxisTransform2d or a Matrix44 depending on the values. GetColMajorF() and
    // ColMajorF() are used when passing a Transform through mojo.
    public static Transform ColMajor(ReadOnlySpan<double> a)
    {
        return new Transform(a[0], a[1], a[2], a[3], a[4], a[5], a[6], a[7], a[8], a[9],
                   a[10], a[11], a[12], a[13], a[14], a[15]);
    }

    // Constructs Transform from a float col-major array.
    // Creates an AxisTransform2d or a Matrix44 depending on the values.
    // GetColMajorF() and ColMajorF() are used when passing a Transform through mojo.
    public static Transform ColMajorF(ReadOnlySpan<float> a)
    {
#if DEBUG
    Debug.Assert(a.Length >= 16);
#endif
        if (a[1]  == 0 && a[2]  == 0 && a[3]  == 0 && a[4]  == 0 &&
            a[6]  == 0 && a[7]  == 0 && a[8]  == 0 && a[9]  == 0 &&
            a[10] == 1 && a[11] == 0 && a[14] == 0 && a[15] == 1)
        {
            return new Transform(a[0], a[5], a[12], a[13]);
        }

        return new Transform(a[0],  a[1],  a[2],  a[3],
                             a[4],  a[5],  a[6],  a[7],
                             a[8],  a[9],  a[10], a[11],
                             a[12], a[13], a[14], a[15]);
    }

    // Used internally to construct Transform with parameters in col-major order.
    private Transform(double r0c0, double r1c0, double r2c0, double r3c0,
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

    public Transform(float scale_x, float scale_y, float trans_x, float trans_y)
    {
        axis_2d_ = AxisTransform2D.FromScaleAndTranslation(new Vector2DF(scale_x, scale_y), new Vector2DF(trans_x, trans_y));
    }

    // Creates a transform from explicit 2d elements. All other matrix elements
    // remain the same as the corresponding elements of an identity matrix.
    // Always creates a double precision 4x4 matrix.
    public static Transform Affine(double a,    // a.k.a. r0c0 or scale_x
                                    double b,    // a.k.a. r1c0 or tan(skew_y)
                                    double c,    // a.k.a. r0c1 or tan(skew_x)
                                    double d,    // a.k.a  r1c1 or scale_y
                                    double e,    // a.k.a  r0c3 or translation_x
                                    double f) {  // a.k.a  r1c3 or translaiton_y
        return ColMajor(a, b, 0, 0, c, d, 0, 0, 0, 0, 1, 0, e, f, 0, 1);
    }

    // Resets this transform to the identity transform.
    public void MakeIdentity()
    {
        full_matrix_ = false;
        axis_2d_ = new AxisTransform2D();
    }

    public Transform(in Quaternion q) : this(
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

    // Creates a transform as a 2d translation.
    public static Transform MakeTranslation(float tx, float ty) => new Transform(1, 1, tx, ty);

    public static Transform MakeTranslation(in Vector2DF v) => MakeTranslation(v.X, v.Y);

    public static Transform MakeScale(float sx, float sy) => new Transform(sx, sy, 0, 0);

    // Creates a transform as a 2d scale.
    public static Transform MakeScale(float scale) => MakeScale(scale, scale);

    // Accurately rotate by 90, 180 or 270 degrees about the z axis.
    public static Transform Make90degRotation() => Affine(0, 1, -1, 0, 0, 0);

    public static Transform Make180degRotation() => MakeScale(-1);

    public static Transform Make270degRotation() => Affine(0, -1, 1, 0, 0, 0);

    // Gets a value at |row|, |col| from the matrix.
    public readonly double rc(int row, int col)
    {
#if DEBUG
        Debug.Assert((uint)row <= 3u);
        Debug.Assert((uint)col <= 3u);
#endif
        if (!full_matrix_)
        {
            return (row, col) switch
            {
                (0, 0) => axis_2d_.Scale.X,
                (1, 1) => axis_2d_.Scale.Y,
                (2, 2) => 1,
                (3, 3) => 1,
                (0, 3) => axis_2d_.Translation.X,
                (1, 3) => axis_2d_.Translation.Y,
                _ => 0
            };
        }

        return matrix_.rc(row, col);
    }
    
    // Sets a value in the matrix at |row|, |col|. It forces full double precision
    // 4x4 matrix.
    public void set_rc(int row, int col, double v)
    {
#if DEBUG
        Debug.Assert((uint)row <= 3u);
        Debug.Assert((uint)col <= 3u);
#endif
        EnsureFullMatrix();
        matrix_.set_rc(row, col, v);
    }

    // Returns true if the matrix is either identity or pure translation.
    public readonly bool IsIdentityOrTranslation => !full_matrix_ ? axis_2d_.Scale == new Vector2DF(1, 1) : matrix_.IsIdentityOrTranslation;

    // Returns true if the matrix has only x and y scaling components, including
    // identity.
    public readonly bool IsScale2D() => !full_matrix_ ?
                                       axis_2d_.Translation.IsZero() :
                                       matrix_.IsScale && matrix_.rc(2, 2) == 1;

    // Returns true if the matrix is has only scaling and translation components,
    // including identity.
    public readonly bool IsScaleOrTranslation()
    {
        if (!full_matrix_)
        {
            return true;
        }
        return matrix_.IsScaleOrTranslation;
    }

    public readonly PointF MapPointInternal(in Matrix44 matrix, in PointF point)
    {
#if DEBUG
        Debug.Assert(full_matrix_);
#endif

        Span<double> p = [point.X, point.Y];

        double w = matrix.MapVector2(p);

        if (w != 1.0 && double.IsNormal(w))
        {
            double w_inverse = 1.0 / w;

            return new PointF(ClampFloatGeometry(p[0] * w_inverse), ClampFloatGeometry(p[1] * w_inverse));
        }

        return new PointF(ClampFloatGeometry(p[0]), ClampFloatGeometry(p[1]));
    }

    public readonly Point MapPoint(in Point point)
    {
        return ToRoundedPoint(MapPoint(new PointF(point)));
    }

    public readonly PointF MapPoint(in PointF point)
    {
        if (!full_matrix_)
        {
            return axis_2d_.MapPoint(point);
        }
        
        return MapPointInternal(matrix_, point);
    }

    public readonly Point3F MapPoint(in Point3F point)
    {
        if (!full_matrix_)
        {
            PointF result = axis_2d_.MapPoint(point.AsPointF());
            return new Point3F(result.X, result.Y, ClampFloatGeometry(point.Z));
        }

        return MapPointInternal(matrix_, point);
    }

    public readonly Point3F MapPointInternal(in Matrix44 matrix, in Point3F point)
    {
#if DEBUG
        Debug.Assert(full_matrix_);
#endif
        double[] p = [point.X, point.Y, point.Z, 1];

        matrix.MapVector4(p);

        if (p[3] != 1.0 && double.IsNormal(p[3]))
        {
            double w_inverse = 1.0 / p[3];
            return new Point3F(ClampFloatGeometry(p[0] * w_inverse),
                   ClampFloatGeometry(p[1] * w_inverse),
                   ClampFloatGeometry(p[2] * w_inverse));
        }

        return new Point3F(ClampFloatGeometry(p[0]), ClampFloatGeometry(p[1]),
                 ClampFloatGeometry(p[2]));
    }

    public readonly QuadF MapQuad(in QuadF quad) => new QuadF(MapPoint(quad.p1), MapPoint(quad.p2), MapPoint(quad.p3), MapPoint(quad.p4));

    public static Matrix44 AxisTransform2DToMatrix44(in AxisTransform2D axis_2d)
    {
        return new Matrix44(axis_2d.Scale.X, 0, 0, 0,  // col 0
                            0, axis_2d.Scale.Y, 0, 0,  // col 1
                            0, 0, 1, 0,                // col 2
                            axis_2d.Translation.X, axis_2d.Translation.Y, 0, 1);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EnsureFullMatrix()
    {
        if (!full_matrix_)
        {
            full_matrix_ = true;
            matrix_ = AxisTransform2DToMatrix44(axis_2d_);
        }
    }

    /*
    public Matrix44 EnsureFullMatrix()
    {
        if (!full_matrix_)
        {
            full_matrix_ = true;
            matrix_ = AxisTransform2DToMatrix44(axis_2d_);
        }
        
        return matrix_;
    }
    */

    public readonly Matrix44 GetFullMatrix()
    {
        if (!full_matrix_)
        {
            return AxisTransform2DToMatrix44(axis_2d_);
        }
        
        return matrix_;
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

    public void PostTranslate(float x, float y)
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

    public void PostTranslate(in Vector2DF offset)
    {
        PostTranslate(offset.X, offset.Y);
    }

    public void PostTranslate3D(float x, float y, float z)
    {
        if (z == 0)
            PostTranslate(x, y);
        else
            EnsureFullMatrix();
        matrix_.PostTranslate3D(x, y, z);
    }

    public void PostTranslate3D(in Vector3DF offset) => PostTranslate3D(offset.X, offset.Y, offset.Z);

    public void Translate3D(in Vector3DF offset)
    {
        Translate3D(offset.X, offset.Y, offset.Z);
    }

    public void Translate3D(float x, float y, float z)
    {
        if (z == 0)
        {
            Translate(x, y);
        }
        else
        {
            EnsureFullMatrix();
            matrix_.PreTranslate3D(x, y, z);
        }
    }

    // Applies the current transformation on a scaling and assigns the result
    // to |this|, i.e. this = this * scaling.

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

    public void Scale(float scale) => Scale(scale, scale);

    public void Scale3D(float x, float y, float z)
    {
        if (z == 1)
        {
            Scale(x, y);
        }
        else
        {
            EnsureFullMatrix();
            matrix_.PreScale3D(x, y, z);
        }
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
            // Copy from the transform
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
            // Copy from the trasnform
            this = transform;
            PreConcat(self);
        }
        else
        {
            matrix_.PostConcat(transform.matrix_);
        }
    }

    // Applies the current transformation on an axis-angle rotation and assigns
    // the result to |this|.
    public void RotateAboutXAxis(double degrees)
    {
        SinCos sin_cos = SinCos.SinCosDegrees(degrees);
        if (sin_cos.IsZeroAngle())
            return;
        EnsureFullMatrix();
        matrix_.RotateAboutXAxisSinCos(sin_cos.sin, sin_cos.cos);
    }

    public void RotateAboutYAxis(double degrees)
    {
        SinCos sin_cos = SinCos.SinCosDegrees(degrees);
        if (sin_cos.IsZeroAngle())
            return;
        EnsureFullMatrix();
        matrix_.RotateAboutYAxisSinCos(sin_cos.sin, sin_cos.cos);
    }

    public void RotateAboutZAxis(double degrees)
    {
        SinCos sin_cos = SinCos.SinCosDegrees(degrees);
        if (sin_cos.IsZeroAngle())
            return;
        EnsureFullMatrix();
        matrix_.RotateAboutZAxisSinCos(sin_cos.sin, sin_cos.cos);
    }

    public void RotateAbout(double x, double y, double z, double degrees)
    {
        SinCos sin_cos = SinCos.SinCosDegrees(degrees);
        if (sin_cos.IsZeroAngle())
            return;

        double square_length = x * x + y * y + z * z;
        if (square_length == 0)
            return;
        if (square_length != 1)
        {
            double scale = 1.0 / Math.Sqrt(square_length);
            x *= scale;
            y *= scale;
            z *= scale;
        }
        EnsureFullMatrix();
        matrix_.RotateUnitSinCos(x, y, z, sin_cos.sin, sin_cos.cos);
    }

    public void RotateAbout(in Vector3DF axis, double degrees) => RotateAbout(axis.X, axis.Y, axis.Z, degrees);

    // Applies the current transformation on a 2d rotation and assigns the result
    // to |this|, i.e. this = this * rotation.
    public void Rotate(double degrees)
    {
        RotateAboutZAxis(degrees);
    }

    private static double TanDegrees(double degrees)
    {
        return Math.Tan(double.DegreesToRadians(degrees));
    }

    public void Skew(double degrees_x, double degrees_y)
    {
        if (degrees_x == 0 && degrees_y == 0)
            return;
        
        EnsureFullMatrix();
        matrix_.Skew(TanDegrees(degrees_x), TanDegrees(degrees_y));
    }

    public void SkewX(double degrees) => Skew(degrees, 0);
    public void SkewY(double degrees) => Skew(0, degrees);

    public void ApplyPerspectiveDepth(double depth)
    {
        if (depth == 0)
            return;

        EnsureFullMatrix();
        matrix_.ApplyPerspectiveDepth(depth);
    }

    // Returns true if axis-aligned 2d rects will remain axis-aligned after being
    // transformed by this matrix.
    public readonly bool Preserves2dAxisAlignment()
    {
        if (!full_matrix_)
        {
            return true;
        }

        // Check whether an axis aligned 2-dimensional rect would remain axis-aligned
        // after being transformed by this matrix (and implicitly projected by
        // dropping any non-zero z-values).
        //
        // The 4th column can be ignored because translations don't affect axis
        // alignment. The 3rd column can be ignored because we are assuming 2d
        // inputs, where z-values will be zero. The 3rd row can also be ignored
        // because we are assuming 2d outputs, and any resulting z-value is dropped
        // anyway. For the inner 2x2 portion, the only effects that keep a rect axis
        // aligned are (1) swapping axes and (2) scaling axes. This can be checked by
        // verifying only 1 element of every column and row is non-zero.  Degenerate
        // cases that project the x or y dimension to zero are considered to preserve
        // axis alignment.
        //
        // If the matrix does have perspective component that is affected by x or y
        // values: The current implementation conservatively assumes that axis
        // alignment is not preserved.

        bool has_x_or_y_perspective = matrix_.rc(3, 0) != 0 || matrix_.rc(3, 1) != 0;

        int num_non_zero_in_row_0 = 0;
        int num_non_zero_in_row_1 = 0;
        int num_non_zero_in_col_0 = 0;
        int num_non_zero_in_col_1 = 0;

        if (Math.Abs(matrix_.rc(0, 0)) > kEpsilon)
        {
            num_non_zero_in_row_0++;
            num_non_zero_in_col_0++;
        }

        if (Math.Abs(matrix_.rc(0, 1)) > kEpsilon)
        {
            num_non_zero_in_row_0++;
            num_non_zero_in_col_1++;
        }

        if (Math.Abs(matrix_.rc(1, 0)) > kEpsilon)
        {
            num_non_zero_in_row_1++;
            num_non_zero_in_col_0++;
        }

        if (Math.Abs(matrix_.rc(1, 1)) > kEpsilon) {
            num_non_zero_in_row_1++;
            num_non_zero_in_col_1++;
        }

        return num_non_zero_in_row_0 <= 1 && num_non_zero_in_row_1 <= 1 &&
            num_non_zero_in_col_0 <= 1 && num_non_zero_in_col_1 <= 1 &&
            !has_x_or_y_perspective;
    }

    public void Transpose()
    {
        if (!IsScale2D())
            EnsureFullMatrix();
        matrix_.Transpose();
    }

    public void ApplyTransformOrigin(float x, float y, float z)
    {
        PostTranslate3D(x, y, z);
        Translate3D(-x, -y, -z);
    }

    public void Zoom(float zoom_factor)
    {
        if (!full_matrix_)
        {
            axis_2d_.Zoom(zoom_factor);
        }
        else
        {
            matrix_.Zoom(zoom_factor);
        }
    }

    public void Flatten()
    {
        if (full_matrix_)
        {
            matrix_.Flatten();
        }
#if DEBUG
        Debug.Assert(IsFlat());
#endif
    }

    public readonly bool IsFlat() => !full_matrix_ ? true : matrix_.IsFlat;

    public readonly bool Is2dTransform() => !full_matrix_ ? true : matrix_.Is2DTransform;

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
        {
            result.EnsureFullMatrix();
            result.matrix_.ApplyDecomposedSkews(decomp.Skew);
        }

        result.Scale3D((float)decomp.Scale.X, (float)decomp.Scale.Y, (float)decomp.Scale.Z);

        return result;
    }

    // It's not easy to get a hash code
    // because a Transform either uses a AxisTransform2D or a Matrix44
    //public override int GetHashCode() => HashCode.Combine();

    public readonly bool Equals(in Transform other)
    {
        if (!full_matrix_ && !other.full_matrix_)
        {
            return axis_2d_ == other.axis_2d_;
        }
        if (full_matrix_ && other.full_matrix_)
        {
            return matrix_ == other.matrix_;
        }
        return GetFullMatrix() == other.GetFullMatrix();
    }

    public override readonly bool Equals(object? obj) => obj is Transform other && Equals(other);

    public static bool operator ==(in Transform left, in Transform right) => left.Equals(right);
    public static bool operator !=(in Transform left, in Transform right) => !left.Equals(right);

    // Returns |this| * |other|.
    public static Transform operator *(in Transform left, in Transform right)
    {
        Transform result;

        if (!right.full_matrix_)
        {
            result = left;
            result.PreConcat(right.axis_2d_);
            return result;
        }
        if (!left.full_matrix_)
        {
            result = right;
            result.PostConcat(left.axis_2d_);
            return result;
        }
        result = new(Matrix44.UninitializedTag.kUninitialized);
        result.matrix_.SetConcat(left.matrix_, right.matrix_);
        return result;
    }

    // Sets |this| = |this| * |other|
    public void operator *=(in Transform other)
    {
        PreConcat(other);
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}
