using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UI.GFX.Geometry;

[StructLayout(LayoutKind.Sequential)]
public struct Matrix44
{
    // 16 explicit fields, column-major layout:
    // _c0r0 = column 0, row 0, etc.
    private double _c0r0, _c0r1, _c0r2, _c0r3;
    private double _c1r0, _c1r1, _c1r2, _c1r3;
    private double _c2r0, _c2r1, _c2r2, _c2r3;
    private double _c3r0, _c3r1, _c3r2, _c3r3;

    public double this[int row, int col]
    {
        // Fields are contiguous in struct memory, so we can index directly
        // into them via a ref to the first field.
        readonly get => Unsafe.Add(ref Unsafe.AsRef(in _c0r0), col * 4 + row);
        set => Unsafe.Add(ref _c0r0, col * 4 + row) = value;
    }

    public Matrix44()
    {
        // Construct identity matrix:
        //
        // [1][0][0][0]
        // [0][1][0][0]
        // [0][0][1][0]
        // [0][0][0][1]

        // Column 0
        _c0r0 = 1; _c0r1 = 0; _c0r2 = 0; _c0r3 = 0;

        // Column 1
        _c1r0 = 0; _c1r1 = 1; _c1r2 = 0; _c1r3 = 0;

        // Column 2
        _c2r0 = 0; _c2r1 = 0; _c2r2 = 1; _c2r3 = 0;

        // Column 3
        _c3r0 = 0; _c3r1 = 0; _c3r2 = 0; _c3r3 = 1;
    }

    public Matrix44(double r0c0, double r1c0, double r2c0, double r3c0,
                    double r0c1, double r1c1, double r2c1, double r3c1,
                    double r0c2, double r1c2, double r2c2, double r3c2,
                    double r0c3, double r1c3, double r2c3, double r3c3)
    {
        // Column 0
        _c0r0 = r0c0; _c0r1 = r1c0; _c0r2 = r2c0; _c0r3 = r3c0;

        // Column 1
        _c1r0 = r0c1; _c1r1 = r1c1; _c1r2 = r2c1; _c1r3 = r3c1;

        // Column 2
        _c2r0 = r0c2; _c2r1 = r1c2; _c2r2 = r2c2; _c2r3 = r3c2;

        // Column 3
        _c3r0 = r0c3; _c3r1 = r1c3; _c3r2 = r2c3; _c3r3 = r3c3;
    }

    // Returns true if the matrix is identity.
    // {1, 0, 0, 0}
    // {0, 1, 0, 0}
    // {0, 0, 1, 0}
    // {0, 0, 0, 1}
    public readonly bool IsIdentity => this == new Matrix44();

    // Returns true if the matrix contains translate or is identity.
    // {1, 0, 0, -}
    // {0, 1, 0, -}
    // {0, 0, 1, -}
    // {0, 0, 0, 1}
    public readonly bool IsIdentityOrTranslation =>

        _c0r0 == 1 && _c0r1 == 0 && _c0r2 == 0 && _c0r3 == 0 &&
        _c1r0 == 0 && _c1r1 == 1 && _c1r2 == 0 && _c1r3 == 0 &&
        _c2r0 == 0 && _c2r1 == 0 && _c2r2 == 1 && _c2r3 == 0 &&
                                                  _c3r3 == 1;

    // Returns true if the matrix only contains scale or translate or is identity.
    // {-, 0, 0, -}
    // {0, -, 0, -}
    // {0, 0, -, -}
    // {0, 0, 0, 1}
    public readonly bool IsScaleOrTranslation =>
    
        _c0r1 == 0 && _c0r2 == 0 && _c0r3 == 0 && _c1r0 == 0 &&
        _c1r2 == 0 && _c1r3 == 0 && _c2r0 == 0 && _c2r1 == 0 &&
        _c2r3 == 0 && _c3r3 == 1;
    
    // Returns true if the matrix only contains scale or is identity.
    // {-, 0, 0, 0}
    // {0, -, 0, 0}
    // {0, 0, -, 0}
    // {0, 0, 0, 1}
    public readonly bool IsScale =>

        IsScaleOrTranslation &&
        _c3r0 == 0 && _c3r1 == 0 && _c3r2 == 0 && _c3r3 == 1;
    
    // {-, -, 0, -}
    // {-, -, 0, -}
    // {0, 0, 1, 0}
    // {-, -, 0, -}
    public readonly bool IsFlat =>

        _c2r0 == 0 && _c2r1 == 0 && _c2r2 == 1 && _c2r3 == 0 &&
        _c0r2 == 0 && _c1r2 == 0 && _c3r2 == 0;

    public readonly bool HasPerspective =>
    
        _c0r3 != 0 || _c1r3 != 0 || _c2r3 != 0 || _c3r3 != 1;

    public readonly bool Is2DTransform => IsFlat && !HasPerspective;

    // Gets a value at |row|, |col| from the matrix.
  public readonly double rc(int row, int col)
  {
#if DEBUG
        Debug.Assert((uint)row <= 3u);
        Debug.Assert((uint)col <= 3u);
#endif
    return this[col, row];
  }

    // Set a value in the matrix at |row|, |col|.
    public void set_rc(int row, int col, double value)
    {
#if DEBUG
        Debug.Assert((uint)row <= 3u);
        Debug.Assert((uint)col <= 3u);
#endif
        this[col, row] = value;
    }

    // this = this * translation.
    public void PreTranslate(double dx, double dy)
    {
        _c3r0 = _c0r0 * dx + _c1r0 * dy + _c3r0;
        _c3r1 = _c0r1 * dx + _c1r1 * dy + _c3r1;
        _c3r2 = _c0r2 * dx + _c1r2 * dy + _c3r2;
        _c3r3 = _c0r3 * dx + _c1r3 * dy + _c3r3;
    }

    // this = this * translation.
    public void PreTranslate3D(double dx, double dy, double dz)
    {
        if(dx == 0 && dy == 0 && dz == 0)
            return;

        _c3r0 = _c0r0 * dx + _c1r0 * dy + _c2r0 * dz + _c3r0;
        _c3r1 = _c0r1 * dx + _c1r1 * dy + _c2r1 * dz + _c3r1;
        _c3r2 = _c0r2 * dx + _c1r2 * dy + _c2r2 * dz + _c3r2;
        _c3r3 = _c0r3 * dx + _c1r3 * dy + _c2r3 * dz + _c3r3;
    }

    // this = translation * this.
    public void PostTranslate(double dx, double dy)
    {
        if (!HasPerspective)
        {
            _c3r0 += dx;
            _c3r1 += dy;
        }
        else
        {
            if (dx != 0)
            {
                _c0r0 += _c0r3 * dx;
                _c1r0 += _c1r3 * dx;
                _c2r0 += _c2r3 * dx;
                _c3r0 += _c3r3 * dx;
            }
            if (dy != 0)
            {
                _c0r1 += _c0r3 * dy;
                _c1r1 += _c1r3 * dy;
                _c2r1 += _c2r3 * dy;
                _c3r1 += _c3r3 * dy;
            }
        }
    }

    // this = translation * this.
    public void PostTranslate3D(double dx, double dy, double dz)
    {
        if (dx == 0 && dy == 0 && dz == 0)
            return;

        if (!HasPerspective)
        {
            _c3r0 += dx;
            _c3r1 += dy;
            _c3r2 += dz;
        }
        else
        {
            _c0r0 += dx * _c0r3;
            _c0r1 += dy * _c0r3;
            _c0r2 += dz * _c0r3;
            
            _c1r0 += dx * _c1r3;
            _c1r1 += dy * _c1r3;
            _c1r2 += dz * _c1r3;

            _c2r0 += dx * _c2r3;
            _c2r1 += dy * _c2r3;
            _c2r2 += dz * _c2r3;

            _c3r0 += dx * _c3r3;
            _c3r1 += dy * _c3r3;
            _c3r2 += dz * _c3r3;
        }
    }

    // this = this * scale.
    public void PreScale(double sx, double sy)
    {
        _c0r0 *= sx;
        _c0r1 *= sx;
        _c0r2 *= sx;
        _c0r3 *= sx;

        _c1r0 *= sy;
        _c1r1 *= sy;
        _c1r2 *= sy;
        _c1r3 *= sy;
    }

    // this = this * scale.
    public void PreScale3D(double sx, double sy, double sz)
    {
        if (sx == 1 && sy == 1 && sz == 1)
            return;
        
        _c0r0 *= sx;
        _c0r1 *= sx;
        _c0r2 *= sx;
        _c0r3 *= sx;

        _c1r0 *= sy;
        _c1r1 *= sy;
        _c1r2 *= sy;
        _c1r3 *= sy;

        _c2r0 *= sz;
        _c2r1 *= sz;
        _c2r2 *= sz;
        _c2r3 *= sz;
    }

    // this = scale * this.
    public void PostScale(double sx, double sy)
    {
        if (sx != 1)
        {
            _c0r0 *= sx;
            _c1r0 *= sx;
            _c2r0 *= sx;
            _c3r0 *= sx;
        }
        if (sy != 1)
        {
            _c0r1 *= sy;
            _c1r1 *= sy;
            _c2r1 *= sy;
            _c3r1 *= sy;
        }
    }

    // this = scale * this.
    public void PostScale3D(double sx, double sy, double sz)
    {
        if (sx == 1 && sy == 1 && sz == 1)
            return;
        
        _c0r0 *= sx;
        _c0r1 *= sx;
        _c0r2 *= sx;
        _c0r3 *= sx;

        _c1r0 *= sy;
        _c1r1 *= sy;
        _c1r2 *= sy;
        _c1r3 *= sy;

        _c2r0 *= sz;
        _c2r1 *= sz;
        _c2r2 *= sz;
        _c2r3 *= sz;
    }

    // this = this * m.
    public void PreConcat(in Matrix44 m)
    {
        SetConcat(this, m);
    }

    // this = m * this.
    public void PostConcat(in Matrix44 m)
    {
        SetConcat(m, this);
    }
    
    // this = a * b.
    public void SetConcat(in Matrix44 x, in Matrix44 y)
    {
        if (x.Is2DTransform && y.Is2DTransform)
        {
            double a = x._c0r0;
            double b = x._c0r1;
            double c = x._c1r0;
            double d = x._c1r1;
            double e = x._c3r0;
            double f = x._c3r1;
            double ya = y._c0r0;
            double yb = y._c0r1;
            double yc = y._c1r0;
            double yd = y._c1r1;
            double ye = y._c3r0;
            double yf = y._c3r1;

            this = new Matrix44(a * ya + c * yb, b * ya + d * yb, 0, 0,           // col 0
                                a * yc + c * yd, b * yc + d * yd, 0, 0,           // col 1
                                0, 0, 1, 0,                                       // col 2
                                a * ye + c * yf + e, b * ye + d * yf + f, 0, 1);  // col 3
            return;
        }

        _c0r0 = x._c0r0 * y._c0r0 + x._c1r0 * y._c0r1 + x._c2r0 * y._c0r2 + x._c3r0 * y._c0r3;
        _c0r1 = x._c0r1 * y._c0r0 + x._c1r1 * y._c0r1 + x._c2r1 * y._c0r2 + x._c3r1 * y._c0r3;
        _c0r2 = x._c0r2 * y._c0r0 + x._c1r2 * y._c0r1 + x._c2r2 * y._c0r2 + x._c3r2 * y._c0r3;
        _c0r3 = x._c0r3 * y._c0r0 + x._c1r3 * y._c0r1 + x._c2r3 * y._c0r2 + x._c3r3 * y._c0r3;

        _c1r0 = x._c0r0 * y._c1r0 + x._c1r0 * y._c1r1 + x._c2r0 * y._c1r2 + x._c3r0 * y._c1r3;
        _c1r1 = x._c0r1 * y._c1r0 + x._c1r1 * y._c1r1 + x._c2r1 * y._c1r2 + x._c3r1 * y._c1r3;
        _c1r2 = x._c0r2 * y._c1r0 + x._c1r2 * y._c1r1 + x._c2r2 * y._c1r2 + x._c3r2 * y._c1r3;
        _c1r3 = x._c0r3 * y._c1r0 + x._c1r3 * y._c1r1 + x._c2r3 * y._c1r2 + x._c3r3 * y._c1r3;

        _c2r0 = x._c0r0 * y._c2r0 + x._c1r0 * y._c2r1 + x._c2r0 * y._c2r2 + x._c3r0 * y._c2r3;
        _c2r1 = x._c0r1 * y._c2r0 + x._c1r1 * y._c2r1 + x._c2r1 * y._c2r2 + x._c3r1 * y._c2r3;
        _c2r2 = x._c0r2 * y._c2r0 + x._c1r2 * y._c2r1 + x._c2r2 * y._c2r2 + x._c3r2 * y._c2r3;
        _c2r3 = x._c0r3 * y._c2r0 + x._c1r3 * y._c2r1 + x._c2r3 * y._c2r2 + x._c3r3 * y._c2r3;

        _c3r0 = x._c0r0 * y._c3r0 + x._c1r0 * y._c3r1 + x._c2r0 * y._c3r2 + x._c3r0 * y._c3r3;
        _c3r1 = x._c0r1 * y._c3r0 + x._c1r1 * y._c3r1 + x._c2r1 * y._c3r2 + x._c3r1 * y._c3r3;
        _c3r2 = x._c0r2 * y._c3r0 + x._c1r2 * y._c3r1 + x._c2r2 * y._c3r2 + x._c3r2 * y._c3r3;
        _c3r3 = x._c0r3 * y._c3r0 + x._c1r3 * y._c3r1 + x._c2r3 * y._c3r2 + x._c3r3 * y._c3r3;
    }

    // Special case for x axis of RotateUnitSinCos().
    public void RotateAboutXAxisSinCos(double sin_angle, double cos_angle)
    {
        _c1r0 = _c1r0 * cos_angle + _c2r0 * sin_angle;
        _c1r1 = _c1r1 * cos_angle + _c2r1 * sin_angle;
        _c1r2 = _c1r2 * cos_angle + _c2r2 * sin_angle;
        _c1r3 = _c1r3 * cos_angle + _c2r3 * sin_angle;

        _c2r0 = _c2r0 * cos_angle - _c1r0 * sin_angle;
        _c2r1 = _c2r1 * cos_angle - _c1r1 * sin_angle;
        _c2r2 = _c2r2 * cos_angle - _c1r2 * sin_angle;
        _c2r3 = _c2r3 * cos_angle - _c1r3 * sin_angle;
    }

    // Special case for y axis of RotateUnitSinCos().
    public void RotateAboutYAxisSinCos(double sin_angle, double cos_angle)
    {
        _c0r0 = _c0r0 * cos_angle - _c2r0 * sin_angle;
        _c0r1 = _c0r1 * cos_angle - _c2r1 * sin_angle;
        _c0r2 = _c0r2 * cos_angle - _c2r2 * sin_angle;
        _c0r3 = _c0r3 * cos_angle - _c2r3 * sin_angle;

        _c2r0 = _c2r0 * cos_angle + _c0r0 * sin_angle;
        _c2r1 = _c2r1 * cos_angle + _c0r1 * sin_angle;
        _c2r2 = _c2r2 * cos_angle + _c0r2 * sin_angle;
        _c2r3 = _c2r3 * cos_angle + _c0r3 * sin_angle;
    }

    // Special case for z axis of RotateUnitSinCos().
    public void RotateAboutZAxisSinCos(double sin_angle, double cos_angle)
    {
        _c0r0 = _c0r0 * cos_angle + _c1r0 * sin_angle;
        _c0r1 = _c0r1 * cos_angle + _c1r1 * sin_angle;
        _c0r2 = _c0r2 * cos_angle + _c1r2 * sin_angle;
        _c0r3 = _c0r3 * cos_angle + _c1r3 * sin_angle;

        _c1r0 = _c1r0 * cos_angle - _c0r0 * sin_angle;
        _c1r1 = _c1r1 * cos_angle - _c0r1 * sin_angle;
        _c1r2 = _c1r2 * cos_angle - _c0r2 * sin_angle;
        _c1r3 = _c1r3 * cos_angle - _c0r3 * sin_angle;
    }

    // Rotates this matrix about the specified unit-length axis vector,
    // by an angle specified by its sin() and cos(). This does not attempt to
    // verify that axis(x, y, z).length() == 1 or that the sin, cos values are
    // correct. this = this * rotation.
    void RotateUnitSinCos(double x, double y, double z, double sin_angle, double cos_angle)
    {
        // Optimize cases where the axis is along a major axis.
        // Since we've already normalized the vector we don't need to check that the other two dimensions are zero.
        // Tiny errors of the other two dimensions are ignored.
        if (z == 1.0)
        {
            RotateAboutZAxisSinCos(sin_angle, cos_angle);
            return;
        }
        if (y == 1.0)
        {
            RotateAboutYAxisSinCos(sin_angle, cos_angle);
            return;
        }
        if (x == 1.0)
        {
            RotateAboutXAxisSinCos(sin_angle, cos_angle);
            return;
        }

        double c = cos_angle;
        double s = sin_angle;
        double C = 1 - c;
        double xs = x * s;
        double ys = y * s;
        double zs = z * s;
        double xC = x * C;
        double yC = y * C;
        double zC = z * C;
        double xyC = x * yC;
        double yzC = y * zC;
        double zxC = z * xC;

        PreConcat(new Matrix44(x * xC + c, xyC + zs, zxC - ys, 0,  // col 0
                               xyC - zs, y * yC + c, yzC + xs, 0,  // col 1
                               zxC + ys, yzC - xs, z * zC + c, 0,  // col 2
                               0, 0, 0, 1));                       // col 3
    }

    // this = this * skew.
    public void Skew(double tan_skew_x, double tan_skew_y)
    {
        _c0r0 += _c1r0 * tan_skew_y;
        _c0r1 += _c1r1 * tan_skew_y;
        _c0r2 += _c1r2 * tan_skew_y;
        _c0r3 += _c1r3 * tan_skew_y;

        _c1r0 += _c0r0 * tan_skew_x;
        _c1r1 += _c0r1 * tan_skew_x;
        _c1r2 += _c0r2 * tan_skew_x;
        _c1r3 += _c0r3 * tan_skew_x;
    }

    //               |1 skew[0] skew[1] 0|
    // this = this * |0    1    skew[2] 0|
    //               |0    0      1     0|
    //               |0    0      0     1|
    public void ApplyDecomposedSkews(ReadOnlySpan<double> skews)
    {
        //                  / |1 0 0  0|   |1 0 s1 0|   |1 s0 0 0|   |1 s0 s1 0| \
        // |c0 c1 c2 c3| * |  |0 1 s2 0| * |0 1  0 0| * |0  1 0 0| = |0  1 s2 0|  |
        //                 |  |0 0 1  0|   |0 0  1 0|   |0  0 1 0|   |0  0  1 0|  |
        //                  \ |0 0 0  1|   |0 0  0 1|   |0  0 0 1|   |0  0  0 1| /

        _c1r0 += _c0r0 * skews[0];
        _c1r1 += _c0r1 * skews[0];
        _c1r2 += _c0r2 * skews[0];
        _c1r3 += _c0r3 * skews[0];

        _c2r0 = _c0r0 * skews[1] + _c1r0 * skews[2] + _c2r0;
        _c2r1 = _c0r1 * skews[1] + _c1r1 * skews[2] + _c2r1;
        _c2r2 = _c0r2 * skews[1] + _c1r2 * skews[2] + _c2r2;
        _c2r3 = _c0r3 * skews[1] + _c1r3 * skews[2] + _c2r3;
    }

    // this = this * perspective.
    public void ApplyPerspectiveDepth(double perspective)
    {
#if DEBUG
        Debug.Assert(perspective != 0.0);
#endif
        double p = -1.0 / perspective;

        _c2r0 += _c3r0 * p;
        _c2r1 += _c3r1 * p;
        _c2r2 += _c3r2 * p;
        _c2r3 += _c3r3 * p;
    }

    // This is a simplified version of InverseWithDouble4Cols().
    public readonly double Determinant()
    {
        if (Is2DTransform)
            return _c0r0 * _c1r1 - _c0r1 * _c1r0;

        Double4 c0 = new(_c0r0, _c0r1, _c0r2, _c0r3);
        Double4 c1 = new(_c1r0, _c1r1, _c1r2, _c1r3);
        Double4 c2 = new(_c2r0, _c2r1, _c2r2, _c2r3);
        Double4 c3 = new(_c3r0, _c3r1, _c3r2, _c3r3);

        // Note that r1 and r3 have components 2/3 and 0/1 swapped.
        Double4 r0 = new(c0.V0, c1.V0, c2.V0, c3.V0);
        Double4 r1 = new(c2.V1, c3.V1, c0.V1, c1.V1);
        Double4 r2 = new(c0.V2, c1.V2, c2.V2, c3.V2);
        Double4 r3 = new(c2.V3, c3.V3, c0.V3, c1.V3);

        Double4 t = Double4.SwapInPairs(r2 * r3);
        c0 = r1 * t;
        t = Double4.SwapHighLow(t);
        c0 = r1 * t - c0;
        t = Double4.SwapInPairs(r1 * r2);
        c0 += r3 * t;
        t = Double4.SwapHighLow(t);
        c0 -= r3 * t;
        t = Double4.SwapInPairs(Double4.SwapHighLow(r1) * r3);
        r2 = Double4.SwapHighLow(r2);
        c0 += r2 * t;
        t = Double4.SwapHighLow(t);
        c0 -= r2 * t;

        return Double4.Sum(r0 * c0);
    }

    public readonly bool IsInvertible()
    {
        return float.IsNormal((float)Determinant());
    }

    // Transposes this matrix in place.
    public void Transpose()
    {
        (_c0r1, _c1r0) = (_c1r0, _c0r1);
        (_c0r2, _c2r0) = (_c2r0, _c0r2);
        (_c0r3, _c3r0) = (_c3r0, _c0r3);
        (_c1r2, _c2r1) = (_c2r1, _c1r2);
        (_c1r3, _c3r1) = (_c3r1, _c1r3);
        (_c2r3, _c3r2) = (_c3r2, _c2r3);
    }

    // See Transform::Zoom().
    public void Zoom(double zoom_factor)
    {
        _c0r3 /= zoom_factor;
        _c1r3 /= zoom_factor;
        _c2r3 /= zoom_factor;
        _c3r0 *= zoom_factor;
        _c3r1 *= zoom_factor;
        _c3r2 *= zoom_factor;
    }

    // Same as above, but assumes the vec[2] is 0 and vec[3] is 1, discards
    // vec[2], and returns vec[3].
    public readonly double MapVector2(ReadOnlySpan<double> vec)
    {
        
    }

    // Applies the matrix to the vector in place.
    public readonly void MapVector4(double[] vec)
    {
        
    }

    public void Flatten()
    {
        _c0r2 = 0;
        _c1r2 = 0;
        _c3r2 = 0;

        _c2r0 = 0;
        _c2r1 = 0;
        _c2r2 = 1;
        _c2r3 = 0;
    }

    // TODO(crbug.com/40237414): Consider letting this function always succeed.
    public readonly DecomposedTransform Decompose2D()
    {
#if DEBUG
        Debug.Assert(Is2DTransform);
#endif
        // https://www.w3.org/TR/css-transforms-1/#decomposing-a-2d-matrix.
        // Decompose a 2D transformation matrix of the form:
        // [m11 m21 0 m41]
        // [m12 m22 0 m42]
        // [ 0   0  1  0 ]
        // [ 0   0  0  1 ]
        //
        // The decomposition is of the form:
        // M = translate * rotate * skew * scale
        //     [1 0 0 Tx] [cos(R) -sin(R) 0 0] [1 K 0 0] [Sx 0  0 0]
        //   = [0 1 0 Ty] [sin(R)  cos(R) 0 0] [0 1 0 0] [0  Sy 0 0]
        //     [0 0 1 0 ] [  0       0    1 0] [0 0 1 0] [0  0  1 0]
        //     [0 0 0 1 ] [  0       0    0 1] [0 0 0 1] [0  0  0 1]

        double m11 = matrix_[0][0];
        double m21 = matrix_[1][0];
        double m12 = matrix_[0][1];
        double m22 = matrix_[1][1];

        double determinant = m11 * m22 - m12 * m21;
        // Test for matrix being singular.
        if (determinant == 0)
            return null;

        DecomposedTransform decomp = new();

        // Translation transform.
        // [m11 m21 0 m41]    [1 0 0 Tx] [m11 m21 0 0]
        // [m12 m22 0 m42]  = [0 1 0 Ty] [m12 m22 0 0]
        // [ 0   0  1  0 ]    [0 0 1 0 ] [ 0   0  1 0]
        // [ 0   0  0  1 ]    [0 0 0 1 ] [ 0   0  0 1]
        decomp.Translate.X = matrix_[3][0];
        decomp.Translate.Y = matrix_[3][1];

        // For the remainder of the decomposition process, we can focus on the upper
        // 2x2 submatrix
        // [m11 m21] = [cos(R) -sin(R)] [1 K] [Sx 0 ]
        // [m12 m22]   [sin(R)  cos(R)] [0 1] [0  Sy]
        //           = [Sx*cos(R) Sy*(K*cos(R) - sin(R))]
        //             [Sx*sin(R) Sy*(K*sin(R) + cos(R))]

        // Determine sign of the x and y scale.
        if (determinant < 0)
        {
            // If the determinant is negative, we need to flip either the x or y scale.
            // Flipping both is equivalent to rotating by 180 degrees.
            if (m11 < m22)
            {
                decomp.Scale.X *= -1;
            }
            else
            {
                decomp.Scale.Y *= -1;
            }
        }

        // X Scale.
        // m11^2 + m12^2 = Sx^2*(cos^2(R) + sin^2(R)) = Sx^2.
        // Sx = +/-sqrt(m11^2 + m22^2)
        decomp.Scale.X *= Math.Sqrt(m11 * m11 + m12 * m12);
        m11 /= decomp.Scale.X;
        m12 /= decomp.Scale.X;

        // Post normalization, the submatrix is now of the form:
        // [m11 m21] = [cos(R)  Sy*(K*cos(R) - sin(R))]
        // [m12 m22]   [sin(R)  Sy*(K*sin(R) + cos(R))]

        // XY Shear.
        // m11 * m21 + m12 * m22 = Sy*K*cos^2(R) - Sy*sin(R)*cos(R) +
        //                         Sy*K*sin^2(R) + Sy*cos(R)*sin(R)
        //                       = Sy*K
        double scaled_shear = m11 * m21 + m12 * m22;
        m21 -= m11 * scaled_shear;
        m22 -= m12 * scaled_shear;

        // Post normalization, the submatrix is now of the form:
        // [m11 m21] = [cos(R)  -Sy*sin(R)]
        // [m12 m22]   [sin(R)   Sy*cos(R)]

        // Y Scale.
        // Similar process to determining x-scale.
        decomp.Scale.Y *= Math.Sqrt(m21 * m21 + m22 * m22);
        m21 /= decomp.Scale.Y;
        m22 /= decomp.Scale.Y;
        decomp.Skew.X = scaled_shear / decomp.Scale.Y;

        // Rotation transform.
        // [1-2(yy+zz)  2(xy-zw)    2(xz+yw) ]   [cos(R) -sin(R)  0]
        // [2(xy+zw)   1-2(xx+zz)   2(yz-xw) ] = [sin(R)  cos(R)  0]
        // [2(xz-yw)    2*(yz+xw)  1-2(xx+yy)]   [  0       0     1]
        // Comparing terms, we can conclude that x = y = 0.
        // [1-2zz   -2zw  0]   [cos(R) -sin(R)  0]
        // [ 2zw   1-2zz  0] = [sin(R)  cos(R)  0]
        // [  0     0     1]   [  0       0     1]
        // cos(R) = 1 - 2*z^2
        // From the double angle formula: cos(2a) = 1 - 2 sin(a)^2
        // cos(R) = 1 - 2*sin(R/2)^2 = 1 - 2*z^2 ==> z = sin(R/2)
        // sin(R) = 2*z*w
        // But sin(2a) = 2 sin(a) cos(a)
        // sin(R) = 2 sin(R/2) cos(R/2) = 2*z*w ==> w = cos(R/2)
        double angle = Math.Atan2(m12, m11);
        decomp.Quaternion.X = 0;
        decomp.Quaternion.Y = 0;
        decomp.Quaternion.Z = Math.Sin(0.5 * angle);
        decomp.Quaternion.W = Math.Cos(0.5 * angle);

        return decomp;
    }

    // Based on:
    // https://github.com/niswegmann/small-matrix-inverse/blob/master/invert4x4_llvm.h
    // which is based on Intel AP-928 "Streaming SIMD Extensions - Inverse of 4x4 Matrix"
    private static bool InverseWithDouble4Cols(ref Double4 c0, ref Double4 c1, ref Double4 c2, ref Double4 c3)
    {
        // Note that r1 and r3 have components 2/3 and 0/1 swapped.
        var r0 = new Double4(c0.V0, c1.V0, c2.V0, c3.V0);
        var r1 = new Double4(c2.V1, c3.V1, c0.V1, c1.V1);
        var r2 = new Double4(c0.V2, c1.V2, c2.V2, c3.V2);
        var r3 = new Double4(c2.V3, c3.V3, c0.V3, c1.V3);

        var t = Double4.SwapInPairs(r2 * r3);
        c0 = r1 * t;
        c1 = r0 * t;

        t = Double4.SwapHighLow(t);
        c0 = r1 * t - c0;
        c1 = Double4.SwapHighLow(r0 * t - c1);

        t = Double4.SwapInPairs(r1 * r2);
        c0 += r3 * t;
        c3 = r0 * t;

        t = Double4.SwapHighLow(t);
        c0 -= r3 * t;
        c3 = Double4.SwapHighLow(r0 * t - c3);

        t = Double4.SwapInPairs(Double4.SwapHighLow(r1) * r3);
        r2 = Double4.SwapHighLow(r2);
        c0 += r2 * t;
        c2 = r0 * t;

        t = Double4.SwapHighLow(t);
        c0 -= r2 * t;

        double det = Double4.Sum(r0 * c0);

        if (!float.IsNormal((float)det))
            return false;

        c2 = Double4.SwapHighLow(r0 * t - c2);

        t = Double4.SwapInPairs(r0 * r1);
        c2 = r3 * t + c2;
        c3 = r2 * t - c3;

        t = Double4.SwapHighLow(t);
        c2 = r3 * t - c2;
        c3 -= r2 * t;

        t = Double4.SwapInPairs(r0 * r3);
        c1 -= r2 * t;
        c2 = r1 * t + c2;

        t = Double4.SwapHighLow(t);
        c1 = r2 * t + c1;
        c2 -= r1 * t;

        t = Double4.SwapInPairs(r0 * r2);
        c1 = r3 * t + c1;
        c3 -= r1 * t;

        t = Double4.SwapHighLow(t);
        c1 -= r3 * t;
        c3 = r1 * t + c3;

        det = 1.0 / det;
        c0 *= det;
        c1 *= det;
        c2 *= det;
        c3 *= det;
        return true;
    }

    // Returns true and set |inverse| to the inverted matrix if this matrix
    // is invertible. Otherwise return false and leave the |inverse| parameter unchanged.
    public readonly bool GetInverse(out Matrix44 result)
    {
        result = new Matrix44();

        if (Is2DTransform)
        {
            double determinant = Determinant();
            
            if (!float.IsNormal((float)determinant))
                return false;

            double inv_det = 1.0 / determinant;
            double a = _c0r0;
            double b = _c0r1;
            double c = _c1r0;
            double d = _c1r1;
            double e = _c3r0;
            double f = _c3r1;

            result = new Matrix44(d * inv_det, -b * inv_det, 0, 0,  // col 0
                      -c * inv_det, a * inv_det, 0, 0,  // col 1
                      0, 0, 1, 0,                       // col 2
                      (c * f - d * e) * inv_det, (b * e - a * f) * inv_det, 0,
                      1);  // col 3
            return true;
        }

        Double4 c0 = new(_c0r0, _c0r1, _c0r2, _c0r3);
        Double4 c1 = new(_c0r0, _c0r1, _c0r2, _c0r3);
        Double4 c2 = new(_c0r0, _c0r1, _c0r2, _c0r3);
        Double4 c3 = new(_c0r0, _c0r1, _c0r2, _c0r3);

        if(!InverseWithDouble4Cols(ref c0, ref c1, ref c2, ref c3))
            return false;
        
        result._c0r0 = _c0r0;
        result._c0r1 = _c0r1;
        result._c0r2 = _c0r2;
        result._c0r3 = _c0r3;

        result._c1r0 = _c1r0;
        result._c1r1 = _c1r1;
        result._c1r2 = _c1r2;
        result._c1r3 = _c1r3;

        result._c2r0 = _c2r0;
        result._c2r1 = _c2r1;
        result._c2r2 = _c2r2;
        result._c2r3 = _c2r3;
        
        result._c3r0 = _c3r0;
        result._c3r1 = _c3r1;
        result._c3r2 = _c3r2;
        result._c3r3 = _c3r3;

        return true;
    }

    public override readonly int GetHashCode()
    {
        var span = MemoryMarshal.Cast<double, byte>(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in _c0r0), 16));
        var hc = new HashCode();
        hc.AddBytes(span);
        return hc.ToHashCode();
    }

    public readonly bool Equals(Matrix44 other)
    {
        // Compare all 16 fields as raw bytes via a span — no loops, no boxing
        var left  = MemoryMarshal.Cast<double, byte>(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in _c0r0), 16));
        var right = MemoryMarshal.Cast<double, byte>(MemoryMarshal.CreateReadOnlySpan(ref other._c0r0, 16));
        return left.SequenceEqual(right);
    }

    public override readonly bool Equals(object? obj) => obj is Matrix44 other && Equals(other);

    public static bool operator ==(in Matrix44 left, in Matrix44 right) => left.Equals(right);
    public static bool operator !=(in Matrix44 left, in Matrix44 right) => !left.Equals(right);
}
