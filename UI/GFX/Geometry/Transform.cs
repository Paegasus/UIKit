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

    private static double TanDegrees(double degrees) => Math.Tan(double.DegreesToRadians(degrees));

    public static bool ApproximatelyZero(double x, double tolerance) => Math.Abs(x) <= tolerance;

    public static bool ApproximatelyOne(double x, double tolerance) => Math.Abs(x - 1) <= tolerance;

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

    // Constructs Transform from a float col-major array.
    // Creates an AxisTransform2d or a Matrix44 depending on the values. 
    // GetColMajorF() and ColMajorF() are used when passing a Transform through mojo.
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

    public readonly void AxisTransform2dToColMajor(in AxisTransform2D axis_2d, Span<double> a)
    {
        a[0] = axis_2d.Scale.X;
        a[5] = axis_2d.Scale.Y;
        a[12] = axis_2d.Translation.X;
        a[13] = axis_2d.Translation.Y;
        a[1] = a[2] = a[3] = a[4] = a[6] = a[7] = a[8] = a[9] = a[11] = a[14] = 0;
        a[10] = a[15] = 1;
    }

    public readonly void AxisTransform2dToColMajor(in AxisTransform2D axis_2d, Span<float> a)
    {
        a[0] = axis_2d.Scale.X;
        a[5] = axis_2d.Scale.Y;
        a[12] = axis_2d.Translation.X;
        a[13] = axis_2d.Translation.Y;
        a[1] = a[2] = a[3] = a[4] = a[6] = a[7] = a[8] = a[9] = a[11] = a[14] = 0;
        a[10] = a[15] = 1;
    }

    // Gets col-major data.

    public readonly void GetColMajor(Span<double> a)
    {
        if (!full_matrix_)
        {
            AxisTransform2dToColMajor(axis_2d_, a);
        }
        else
        {
            matrix_.GetColMajor(a);
        }
    }

    public readonly void GetColMajorF(Span<float> a)
    {
        if (!full_matrix_)
        {
            AxisTransform2dToColMajor(axis_2d_, a);
        }
        else
        {
            matrix_.GetColMajorF(a);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly double ColMajorData(int index) => rc(index % 4, index / 4);

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

    // Returns true if this is the identity matrix.
    // This function modifies a mutable variable in |matrix_|.
    // Note: Consider creating a static AxisTransform2D.Identity to compare against
    // instead of creating a new AxisTransform2D every time
    public readonly bool IsIdentity => !full_matrix_ ? (axis_2d_ == new AxisTransform2D()) : matrix_.IsIdentity;

    // Returns true if the matrix is either identity or pure translation.
    public readonly bool IsIdentityOrTranslation => !full_matrix_ ? axis_2d_.Scale == new Vector2DF(1, 1) : matrix_.IsIdentityOrTranslation;

    // Returns true if the matrix is either the identity or a 2d translation.
    public readonly bool IsIdentityOr2dTranslation =>
    
        !full_matrix_ ?
        (axis_2d_.Scale == new Vector2DF(1, 1)) :
        (matrix_.IsIdentityOrTranslation && matrix_.rc(2, 3) == 0);

    // Returns true if the matrix is either identity or pure translation,
    // allowing for an amount of inaccuracy as specified by the parameter.
    public readonly bool IsApproximatelyIdentityOrTranslation(double tolerance)
    {
#if DEBUG
        Debug.Assert(tolerance >= 0);
#endif
  if (!full_matrix_)
  {
    return ApproximatelyOne(axis_2d_.Scale.X, tolerance) &&
           ApproximatelyOne(axis_2d_.Scale.Y, tolerance);
  }

  if (!ApproximatelyOne(matrix_.rc(0, 0), tolerance) ||
      !ApproximatelyZero(matrix_.rc(1, 0), tolerance) ||
      !ApproximatelyZero(matrix_.rc(2, 0), tolerance) ||
      !ApproximatelyZero(matrix_.rc(0, 1), tolerance) ||
      !ApproximatelyOne(matrix_.rc(1, 1), tolerance) ||
      !ApproximatelyZero(matrix_.rc(2, 1), tolerance) ||
      !ApproximatelyZero(matrix_.rc(0, 2), tolerance) ||
      !ApproximatelyZero(matrix_.rc(1, 2), tolerance) ||
      !ApproximatelyOne(matrix_.rc(2, 2), tolerance)) {
    return false;
  }

    // Check perspective components more strictly by using the smaller of float
    // epsilon and |tolerance|.
    double perspective_tolerance = Math.Min(kEpsilon, tolerance);
    return ApproximatelyZero(matrix_.rc(3, 0), perspective_tolerance) &&
           ApproximatelyZero(matrix_.rc(3, 1), perspective_tolerance) &&
           ApproximatelyZero(matrix_.rc(3, 2), perspective_tolerance) &&
           ApproximatelyOne(matrix_.rc(3, 3), perspective_tolerance);
    }

    public readonly bool IsApproximatelyIdentityOrIntegerTranslation(double tolerance = 0)
    {
        if (!IsApproximatelyIdentityOrTranslation(tolerance))
            return false;

        static bool IsApproximateIntegerTranslation(double t, double tolerance) =>
            t >= int.MinValue && t <= int.MaxValue &&
            Math.Abs(Math.Round(t, MidpointRounding.AwayFromZero) - t) <= tolerance;
        
        if (!full_matrix_)
        {
            return IsApproximateIntegerTranslation(axis_2d_.Translation.X, tolerance) &&
                   IsApproximateIntegerTranslation(axis_2d_.Translation.Y, tolerance);
        }

        return IsApproximateIntegerTranslation(matrix_.rc(0, 3), tolerance) &&
               IsApproximateIntegerTranslation(matrix_.rc(1, 3), tolerance) &&
               IsApproximateIntegerTranslation(matrix_.rc(2, 3), tolerance);
    }

    // Returns true if the matrix has only x and y scaling components, including
    // identity.
    public readonly bool IsScale2D => !full_matrix_ ?
                                       axis_2d_.Translation.IsZero() :
                                       matrix_.IsScale && matrix_.rc(2, 2) == 1;

    // Returns true if the matrix is has only scaling and translation components,
    // including identity.
    public readonly bool IsScaleOrTranslation => !full_matrix_ ? true : matrix_.IsScaleOrTranslation;

    // Returns true if, for 2d rects on the x/y plane, this matrix can be
    // represented as a 2d affine transform on the x/y plane.
    public readonly bool Preserves2dAffine()
    {
        if (!full_matrix_)
        {
            return true;
        }

        bool is_flat_ignore_z = matrix_.rc(2, 0) == 0 && matrix_.rc(2, 1) == 0 && matrix_.rc(2, 3) == 0;

        bool has_no_perspective_ignore_z = matrix_.rc(3, 0) == 0 && matrix_.rc(3, 1) == 0 && matrix_.rc(3, 3) == 1;

        if (is_flat_ignore_z && has_no_perspective_ignore_z)
        {
            return true;
        }

        return false;
    }

    // Returns true if the matrix is identity or, if the matrix consists only
    // of a translation whose components can be represented as integers. Returns
    // false if the translation contains a fractional component or is too large to
    // fit in an integer.
    public readonly bool IsIdentityOrIntegerTranslation()
    {
        if (!IsIdentityOrTranslation)
            return false;

        static bool IsIntegerTranslation(double t) => t >= int.MinValue &&
                                                      t <= int.MaxValue &&
                                                 (int)t == t;

        if (!full_matrix_)
        {
            return IsIntegerTranslation(axis_2d_.Translation.X) &&
                   IsIntegerTranslation(axis_2d_.Translation.Y);
        }

        return IsIntegerTranslation(matrix_.rc(0, 3)) &&
               IsIntegerTranslation(matrix_.rc(1, 3)) &&
               IsIntegerTranslation(matrix_.rc(2, 3));
    }

    public readonly bool IsIdentityOrInteger2dTranslation() => IsIdentityOrIntegerTranslation() && rc(2, 3) == 0;

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

    // Returns the point with reverse transformation applied to `point`, clamped
    // with ClampFloatGeometry(), or `std::nullopt` if the transformation cannot be inverted.
    public readonly PointF? InverseMapPoint(in PointF point)
    {
        if (!full_matrix_)
        {
            if (!axis_2d_.IsInvertible())
                return null;
            return axis_2d_.InverseMapPoint(point);
        }
        Matrix44 inverse = new(Matrix44.UninitializedTag.kUninitialized);
        if (!matrix_.GetInverse(out inverse))
            return null;
        return MapPointInternal(inverse, point);
    }

    public readonly Point3F? InverseMapPoint(in Point3F point)
    {
        if (!full_matrix_)
        {
            if (!axis_2d_.IsInvertible())
                return null;
            PointF result = axis_2d_.InverseMapPoint(point.AsPointF());
            return new Point3F(result.X, result.Y, ClampFloatGeometry(point.Z));
        }
        Matrix44 inverse = new(Matrix44.UninitializedTag.kUninitialized);
        if (!matrix_.GetInverse(out inverse))
            return null;
        return MapPointInternal(inverse, point);
    }

    // Applies the reverse transformation on `point`. Returns `std::nullopt` if
    // the transformation cannot be inverted. Rounds the result to the nearest  point.
    public readonly Point? InverseMapPoint(in Point point)
    {
        PointF? point_f = InverseMapPoint(new PointF(point));
        if (point_f.HasValue)
            return ToRoundedPoint(point_f.Value);
        return null;
    }

    // Returns the rect that is the smallest axis aligned bounding rect
    // containing the transformed rect, clamped with ClampFloatGeometry().
    public readonly RectF MapRect(in RectF rect)
    {
        if (IsIdentity)
            return rect;

        if (!full_matrix_)
        {
            if (axis_2d_.Scale.X >= 0 && axis_2d_.Scale.Y >= 0)
            {
                return axis_2d_.MapRect(rect);
            }
        }

        return MapQuad(new QuadF(rect)).BoundingBox();
    }

    public readonly Rect MapRect(in Rect rect)
    {
        if (IsIdentity)
            return rect;

        return RectConversions.ToEnclosingRect(MapRect(new RectF(rect)));
    }

    // Applies the reverse transformation on the given rect. Returns
    // `std::nullopt` if the transformation cannot be inverted, or the rect that
    // is the smallest axis aligned bounding rect containing the transformed rect,
    // clamped with ClampFloatGeometry().
    public readonly bool InverseMapRect(in RectF rect, out RectF result)
    {
        result = default;

        if (IsIdentity)
        {
            result = rect;
            return true;
        }

        if (!full_matrix_)
        {
            if (!axis_2d_.IsInvertible())
            {
                return false;
            }
            if (axis_2d_.Scale.X > 0 && axis_2d_.Scale.Y > 0)
            {
                result = axis_2d_.InverseMapRect(rect);
                return true;
            }
        }

        Transform inverse;
        if (!GetInverse(out inverse))
        {
            return false;
        }

        result = inverse.MapQuad(new QuadF(rect)).BoundingBox();
        return true;
    }

    public readonly bool InverseMapRect(in Rect rect, out Rect result)
    {
        result = default;
        
        if (IsIdentity)
        {
            result = rect;
            return true;
        }

        RectF mapped;
        if (InverseMapRect(new RectF(rect), out mapped))
        {
            result = RectConversions.ToEnclosingRect(mapped);
            return true;
        }

        return false;
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

    // Applies a scale to the current transformation and assigns the result to
    // |this|, i.e. this = scaling * this.

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

    public void PostScale3D(float x, float y, float z)
    {
        if (z == 1)
            PostScale(x, y);
        else
            EnsureFullMatrix();
            matrix_.PostScale3D(x, y, z);
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

    public readonly double Determinant => !full_matrix_ ? axis_2d_.Determinant() : matrix_.Determinant();

    // Rounds translation components to integers, and all other components to
    // identity. Normally this function is meaningful only if
    // IsApproximatelyIdentityOrIntegerTranslation() is true.
    public void RoundToIdentityOrIntegerTranslation()
    {
        if (!full_matrix_)
        {
            axis_2d_ = AxisTransform2D.FromScaleAndTranslation(new Vector2DF(1, 1),
                                                               new Vector2DF(MathF.Round(axis_2d_.Translation.X, MidpointRounding.AwayFromZero),
                                                                             MathF.Round(axis_2d_.Translation.Y, MidpointRounding.AwayFromZero)));
        } else
        {
            matrix_ = new Matrix44(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0,          // col0-2
                Math.Round(matrix_.rc(0, 3), MidpointRounding.AwayFromZero),    // col3
                Math.Round(matrix_.rc(1, 3), MidpointRounding.AwayFromZero),
                Math.Round(matrix_.rc(2, 3), MidpointRounding.AwayFromZero), 1);
        }
    }

    // Applies the current transformation on a 2d rotation and assigns the result
    // to |this|, i.e. this = this * rotation.
    public void Rotate(double degrees) => RotateAboutZAxis(degrees);

    // Applies the current transformation on a skew and assigns the result
    // to |this|, i.e. this = this * skew.

    public void Skew(double degrees_x, double degrees_y)
    {
        if (degrees_x == 0 && degrees_y == 0)
            return;
        
        EnsureFullMatrix();
        matrix_.Skew(TanDegrees(degrees_x), TanDegrees(degrees_y));
    }

    public void SkewX(double degrees) => Skew(degrees, 0);
    public void SkewY(double degrees) => Skew(0, degrees);

    // Applies the current transformation on a perspective transform and assigns
    // the result to |this|.
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

    // Returns true if axis-aligned 2d rects will remain axis-aligned and not
    // clipped by perspective (w > 0) after being transformed by this matrix,
    // and distinct points in the x/y plane will remain distinct after being
    // transformed by this matrix and mapped back to the x/y plane.
    public readonly bool NonDegeneratePreserves2dAxisAlignment()
    {
        if (!full_matrix_)
        {
            return axis_2d_.Scale.X > kEpsilon && axis_2d_.Scale.Y > kEpsilon;
        }

        // See comments above for Preserves2dAxisAlignment.

        // This function differs from it by requiring:
        //  (1) that there are exactly two nonzero values on a diagonal in
        //      the upper left 2x2 submatrix, and
        //  (2) that the w perspective value is positive.

        bool has_x_or_y_perspective = matrix_.rc(3, 0) != 0 || matrix_.rc(3, 1) != 0;
        bool positive_w_perspective = matrix_.rc(3, 3) > kEpsilon;

        bool have_0_0 = Math.Abs(matrix_.rc(0, 0)) > kEpsilon;
        bool have_0_1 = Math.Abs(matrix_.rc(0, 1)) > kEpsilon;
        bool have_1_0 = Math.Abs(matrix_.rc(1, 0)) > kEpsilon;
        bool have_1_1 = Math.Abs(matrix_.rc(1, 1)) > kEpsilon;

        return have_0_0 == have_1_1 && have_0_1 == have_1_0 && have_0_0 != have_0_1 &&
            !has_x_or_y_perspective && positive_w_perspective;
    }

    // Returns true if the matrix has any perspective component that would
    // change the w-component of a homogeneous point.
    public readonly bool HasPerspective => !full_matrix_ ? false : matrix_.HasPerspective;

    // Returns true if this transform is non-singular.
    public readonly bool IsInvertible => !full_matrix_ ? axis_2d_.IsInvertible() : matrix_.IsInvertible();

    // If |this| is invertible, inverts |this| and stores the result in
    // |*transform|, and returns true. Otherwise sets |*transform| to identity and returns false.
    public readonly bool GetInverse(out Transform transform)
    {
        // Snapshot this entirely first, since transform may alias this.
        Transform self = this;

        if (!self.full_matrix_)
        {
            transform.full_matrix_ = false;
            transform.matrix_ = default;

            if (self.axis_2d_.IsInvertible())
            {
                transform.axis_2d_ = self.axis_2d_;
                transform.axis_2d_.Invert();
                return true;
            }

            transform.axis_2d_ = new AxisTransform2D();
            return false;
        }

        transform.axis_2d_ = default;

        if (self.matrix_.GetInverse(out transform.matrix_))
        {
            transform.full_matrix_ = true;
            return true;
        }

        // Initialize the return value to identity if this matrix turned out to be un-invertible.
        transform = new Transform();
        transform.MakeIdentity();
        return false;
    }

    // Same as GetInverse() except that it returns the the inverse of |this| or
    // identity, instead of a bool. This is suitable when it's good to fallback
    // to identity silently.
    public readonly Transform InverseOrIdentity()
    {
        Transform inverse;
        bool invertible = GetInverse(out inverse);
#if DEBUG
        Debug.Assert(invertible || inverse.IsIdentity);
#endif
        return inverse;
    }

    // Returns true if a layer with a forward-facing normal of (0, 0, 1)
    // would have its back side facing frontwards after applying the transform.
    public readonly bool IsBackFaceVisible()
    {
        if (!full_matrix_)
        {
            return false;
        }

        // Compute whether a layer with a forward-facing normal of (0, 0, 1, 0)
        // would have its back face visible after applying the transform.
        // This is done by transforming the normal and seeing if the resulting z
        // value is positive or negative. However, note that transforming a normal
        // actually requires using the inverse-transpose of the original transform.
        //
        // We can avoid inverting and transposing the matrix since we know we want
        // to transform only the specific normal vector (0, 0, 1, 0). In this case,
        // we only need the 3rd row, 3rd column of the inverse-transpose. We can
        // calculate only the 3rd row 3rd column element of the inverse, skipping
        // everything else.
        //
        // For more information, refer to:
        //   http://en.wikipedia.org/wiki/Invertible_matrix#Analytic_solution
        //

        double determinant = matrix_.Determinant();

        // If matrix was not invertible, then just assume back face is not visible.
        if (determinant == 0)
            return false;

        // Compute the cofactor of the 3rd row, 3rd column.
        double cofactor_part_1 =
            matrix_.rc(0, 0) * matrix_.rc(1, 1) * matrix_.rc(3, 3);

        double cofactor_part_2 =
            matrix_.rc(0, 1) * matrix_.rc(1, 3) * matrix_.rc(3, 0);

        double cofactor_part_3 =
            matrix_.rc(0, 3) * matrix_.rc(1, 0) * matrix_.rc(3, 1);

        double cofactor_part_4 =
            matrix_.rc(0, 0) * matrix_.rc(1, 3) * matrix_.rc(3, 1);

        double cofactor_part_5 =
            matrix_.rc(0, 1) * matrix_.rc(1, 0) * matrix_.rc(3, 3);

        double cofactor_part_6 =
            matrix_.rc(0, 3) * matrix_.rc(1, 1) * matrix_.rc(3, 0);

        double cofactor33 = cofactor_part_1 + cofactor_part_2 + cofactor_part_3 -
                            cofactor_part_4 - cofactor_part_5 - cofactor_part_6;

        // Technically the transformed z component is cofactor33 / determinant.  But
        // we can avoid the costly division because we only care about the resulting
        // +/- sign; we can check this equivalently by multiplication.
        return cofactor33 * determinant < -kEpsilon;
    }

    public void Transpose()
    {
        if (!IsScale2D)
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
        Debug.Assert(IsFlat);
#endif
    }

    public readonly bool IsFlat => !full_matrix_ ? true : matrix_.IsFlat;

    // Returns the x and y scale components of the matrix, clamped with
    // ClampFloatGeometry().
    public readonly Vector2DF To2DScale()
    {
        if (!full_matrix_)
        {
            return new Vector2DF(ClampFloatGeometry(axis_2d_.Scale.X), ClampFloatGeometry(axis_2d_.Scale.Y));
        }
        return new Vector2DF(ClampFloatGeometry(matrix_.rc(0, 0)), ClampFloatGeometry(matrix_.rc(1, 1)));
    }

    public readonly bool Is2dTransform => !full_matrix_ ? true : matrix_.Is2DTransform;

    // Returns the x and y translation components of the matrix, clamped with
    // ClampFloatGeometry().
    public readonly Vector2DF To2dTranslation()
    {
        if (!full_matrix_)
        {
            return new Vector2DF(ClampFloatGeometry(axis_2d_.Translation.X), 
                                 ClampFloatGeometry(axis_2d_.Translation.Y));
        }

        return new Vector2DF(ClampFloatGeometry(matrix_.rc(0, 3)),
                             ClampFloatGeometry(matrix_.rc(1, 3)));
    }

    // Returns the x, y and z translation components of the matrix, clampe with
    // ClampFloatGeometry().
    public readonly Vector3DF To3dTranslation()
    {
        if (!full_matrix_)
        {
            return new Vector3DF(ClampFloatGeometry(axis_2d_.Translation.X),
                                 ClampFloatGeometry(axis_2d_.Translation.Y), 0);
        }

        return new Vector3DF(ClampFloatGeometry(matrix_.rc(0, 3)),
                             ClampFloatGeometry(matrix_.rc(1, 3)),
                             ClampFloatGeometry(matrix_.rc(2, 3)));
    }

    // Returns the x and y scale components of the matrix, clamped with
    // ClampFloatGeometry().
    public readonly Vector2DF To2dScale()
    {
        if (!full_matrix_)
        {
            return new Vector2DF(ClampFloatGeometry(axis_2d_.Scale.X),
                                 ClampFloatGeometry(axis_2d_.Scale.Y));
        }
        return new Vector2DF(ClampFloatGeometry(matrix_.rc(0, 0)),
                             ClampFloatGeometry(matrix_.rc(1, 1)));
    }

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

    // Decomposes |this| into |decomp|. Returns nullopt if |this| can't be
    // decomposed. |decomp| must be identity on input.
    //
    // Uses routines described in the following specs:
    // 2d: https://www.w3.org/TR/css-transforms-1/#decomposing-a-2d-matrix
    // 3d: https://www.w3.org/TR/css-transforms-2/#decomposing-a-3d-matrix
    //<
    // Note: when the determinant is negative, the 2d spec calls for flipping one
    // of the axis, while the general 3d spec calls for flipping all of the
    // scales. The latter not only introduces rotation in the case of a trivial
    // scale inversion, but causes transformed objects to needlessly shrink and
    // grow as they transform through scale = 0 along multiple axes. Thus 2d
    // transforms should follow the 2d spec regarding matrix decomposition.
    public readonly bool Decompose(out DecomposedTransform result)
    {
        if (!full_matrix_)
        {
            if (!axis_2d_.IsInvertible())
            {
                result = default;
                return false;
            }
            
            result = axis_2d_.Decompose();
            return true;
        }

        return matrix_.Decompose(out result);
    }

    // Decomposes |this| and |from|, interpolates the decomposed values, and
    // sets |this| to the reconstituted result. Returns false and leaves |this|
    // unchanged if either matrix can't be decomposed.
    // Uses routines described in this spec:
    // https://www.w3.org/TR/css-transforms-2/#matrix-interpolation
    //
    // Note: this call is expensive for complex transforms since we need to
    // decompose the transforms. If you're going to be calling this rapidly
    // (e.g., in an animation) for complex transforms, you should decompose once
    // using Decompose() and reuse your DecomposedTransform with
    // BlendDecomposedTransforms() (see transform_util.h).
    public bool Blend(in Transform from, double progress)
    {
        if (!Decompose(out DecomposedTransform to_decomp))
            return false;

        if (!from.Decompose(out DecomposedTransform from_decomp))
            return false;

        to_decomp = TransformUtil.BlendDecomposedTransforms(to_decomp, from_decomp, progress);

        this = Compose(to_decomp);

        return true;
    }

    // Returns a string in the format of "[ row0\n, row1\n, row2\n, row3 ]\n".
    public override readonly string ToString()
    {
        return $"[ {rc(0, 0)} {rc(0, 1)} {rc(0, 2)} {rc(0, 3)}\n  {rc(1, 0)} {rc(1, 1)} {rc(1, 2)} {rc(1, 3)}\n  {rc(2, 0)} {rc(2, 1)} {rc(2, 2)} {rc(2, 3)}\n  {rc(3, 0)} {rc(3, 1)} {rc(3, 2)} {rc(3, 3)} ]\n";
    }

    // It's not easy to get a hash code
    // because a Transform either uses a AxisTransform2D or a Matrix44
    //public override int GetHashCode() => HashCode.Combine();

    public readonly bool Equals(in Transform other)
    {
        // If both use AxisTransform2D, compare by axis_2d_
        if (!full_matrix_ && !other.full_matrix_)
        {
            return axis_2d_ == other.axis_2d_;
        }
        // If both use Matrix44, compare by matrix_
        if (full_matrix_ && other.full_matrix_)
        {
            return matrix_ == other.matrix_;
        }
        // If one uses AxisTransform2D and the other Matrix44 or vice versa,
        // compare by GetFullMatrix()
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
