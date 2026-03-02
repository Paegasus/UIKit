using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UI.Extensions;

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
    public readonly double rc(int row, int col) => this[row, col];

    // Set a value in the matrix at |row|, |col|.
    public void set_rc(int row, int col, double value) => this[row, col] = value;

/*
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
*/

    /*
    void Matrix44::GetColMajor(base::span<double, 16> dst) const
    {
        base::span UNSAFE_TODO(src{&matrix_[0][0], base::fixed_extent<16>()});
        dst.copy_from(src);
    }
    */
    /*
    void Matrix44::GetColMajorF(base::span<float, 16> dst) const
    {
        base::span UNSAFE_TODO(src{&matrix_[0][0], base::fixed_extent<16>()});

        // TODO: It's surprising that this isn't flagged as unsafe.
        //       It'd be nice if copy_from() supported differing element types,
        //       then this would be statically safe.
        std::ranges::copy(src, dst.begin());
    }
    */
    
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

        // Multiplies each column element-wise by {sx, sy, sz, 1},
        // equivalent to diag(sx, sy, sz, 1) * this (post-multiply).
        _c0r0 *= sx; _c0r1 *= sy; _c0r2 *= sz;
        _c1r0 *= sx; _c1r1 *= sy; _c1r2 *= sz;
        _c2r0 *= sx; _c2r1 *= sy; _c2r2 *= sz;
        _c3r0 *= sx; _c3r1 *= sy; _c3r2 *= sz;
        // row 3 is multiplied by 1, so _c*r3 are unchanged.
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

        // Snapshot x entirely before any writes, since x may alias this.
        double x_c0r0 = x._c0r0, x_c0r1 = x._c0r1, x_c0r2 = x._c0r2, x_c0r3 = x._c0r3;
        double x_c1r0 = x._c1r0, x_c1r1 = x._c1r1, x_c1r2 = x._c1r2, x_c1r3 = x._c1r3;
        double x_c2r0 = x._c2r0, x_c2r1 = x._c2r1, x_c2r2 = x._c2r2, x_c2r3 = x._c2r3;
        double x_c3r0 = x._c3r0, x_c3r1 = x._c3r1, x_c3r2 = x._c3r2, x_c3r3 = x._c3r3;

        _c0r0 = x_c0r0 * y._c0r0 + x_c1r0 * y._c0r1 + x_c2r0 * y._c0r2 + x_c3r0 * y._c0r3;
        _c0r1 = x_c0r1 * y._c0r0 + x_c1r1 * y._c0r1 + x_c2r1 * y._c0r2 + x_c3r1 * y._c0r3;
        _c0r2 = x_c0r2 * y._c0r0 + x_c1r2 * y._c0r1 + x_c2r2 * y._c0r2 + x_c3r2 * y._c0r3;
        _c0r3 = x_c0r3 * y._c0r0 + x_c1r3 * y._c0r1 + x_c2r3 * y._c0r2 + x_c3r3 * y._c0r3;

        _c1r0 = x_c0r0 * y._c1r0 + x_c1r0 * y._c1r1 + x_c2r0 * y._c1r2 + x_c3r0 * y._c1r3;
        _c1r1 = x_c0r1 * y._c1r0 + x_c1r1 * y._c1r1 + x_c2r1 * y._c1r2 + x_c3r1 * y._c1r3;
        _c1r2 = x_c0r2 * y._c1r0 + x_c1r2 * y._c1r1 + x_c2r2 * y._c1r2 + x_c3r2 * y._c1r3;
        _c1r3 = x_c0r3 * y._c1r0 + x_c1r3 * y._c1r1 + x_c2r3 * y._c1r2 + x_c3r3 * y._c1r3;

        _c2r0 = x_c0r0 * y._c2r0 + x_c1r0 * y._c2r1 + x_c2r0 * y._c2r2 + x_c3r0 * y._c2r3;
        _c2r1 = x_c0r1 * y._c2r0 + x_c1r1 * y._c2r1 + x_c2r1 * y._c2r2 + x_c3r1 * y._c2r3;
        _c2r2 = x_c0r2 * y._c2r0 + x_c1r2 * y._c2r1 + x_c2r2 * y._c2r2 + x_c3r2 * y._c2r3;
        _c2r3 = x_c0r3 * y._c2r0 + x_c1r3 * y._c2r1 + x_c2r3 * y._c2r2 + x_c3r3 * y._c2r3;

        _c3r0 = x_c0r0 * y._c3r0 + x_c1r0 * y._c3r1 + x_c2r0 * y._c3r2 + x_c3r0 * y._c3r3;
        _c3r1 = x_c0r1 * y._c3r0 + x_c1r1 * y._c3r1 + x_c2r1 * y._c3r2 + x_c3r1 * y._c3r3;
        _c3r2 = x_c0r2 * y._c3r0 + x_c1r2 * y._c3r1 + x_c2r2 * y._c3r2 + x_c3r2 * y._c3r3;
        _c3r3 = x_c0r3 * y._c3r0 + x_c1r3 * y._c3r1 + x_c2r3 * y._c3r2 + x_c3r3 * y._c3r3;
    }

    // Special case for x axis of RotateUnitSinCos().
    public void RotateAboutXAxisSinCos(double sin_angle, double cos_angle)
    {
        // Snapshot c1 and c2 before overwriting
        double c1r0 = _c1r0, c1r1 = _c1r1, c1r2 = _c1r2, c1r3 = _c1r3;
        double c2r0 = _c2r0, c2r1 = _c2r1, c2r2 = _c2r2, c2r3 = _c2r3;

        _c1r0 = c1r0 * cos_angle + c2r0 * sin_angle;
        _c1r1 = c1r1 * cos_angle + c2r1 * sin_angle;
        _c1r2 = c1r2 * cos_angle + c2r2 * sin_angle;
        _c1r3 = c1r3 * cos_angle + c2r3 * sin_angle;

        _c2r0 = c2r0 * cos_angle - c1r0 * sin_angle;
        _c2r1 = c2r1 * cos_angle - c1r1 * sin_angle;
        _c2r2 = c2r2 * cos_angle - c1r2 * sin_angle;
        _c2r3 = c2r3 * cos_angle - c1r3 * sin_angle;
    }

    // Special case for y axis of RotateUnitSinCos().
    public void RotateAboutYAxisSinCos(double sin_angle, double cos_angle)
    {
        // Snapshot c0 and c2 before overwriting
        double c0r0 = _c0r0, c0r1 = _c0r1, c0r2 = _c0r2, c0r3 = _c0r3;
        double c2r0 = _c2r0, c2r1 = _c2r1, c2r2 = _c2r2, c2r3 = _c2r3;

        _c0r0 = c0r0 * cos_angle - c2r0 * sin_angle;
        _c0r1 = c0r1 * cos_angle - c2r1 * sin_angle;
        _c0r2 = c0r2 * cos_angle - c2r2 * sin_angle;
        _c0r3 = c0r3 * cos_angle - c2r3 * sin_angle;

        _c2r0 = c2r0 * cos_angle + c0r0 * sin_angle;
        _c2r1 = c2r1 * cos_angle + c0r1 * sin_angle;
        _c2r2 = c2r2 * cos_angle + c0r2 * sin_angle;
        _c2r3 = c2r3 * cos_angle + c0r3 * sin_angle;
    }

    // Special case for z axis of RotateUnitSinCos().
    public void RotateAboutZAxisSinCos(double sin_angle, double cos_angle)
    {
        // Snapshot c0 and c1 before overwriting
        double c0r0 = _c0r0, c0r1 = _c0r1, c0r2 = _c0r2, c0r3 = _c0r3;
        double c1r0 = _c1r0, c1r1 = _c1r1, c1r2 = _c1r2, c1r3 = _c1r3;

        _c0r0 = c0r0 * cos_angle + c1r0 * sin_angle;
        _c0r1 = c0r1 * cos_angle + c1r1 * sin_angle;
        _c0r2 = c0r2 * cos_angle + c1r2 * sin_angle;
        _c0r3 = c0r3 * cos_angle + c1r3 * sin_angle;

        _c1r0 = c1r0 * cos_angle - c0r0 * sin_angle;
        _c1r1 = c1r1 * cos_angle - c0r1 * sin_angle;
        _c1r2 = c1r2 * cos_angle - c0r2 * sin_angle;
        _c1r3 = c1r3 * cos_angle - c0r3 * sin_angle;
    }

    // Rotates this matrix about the specified unit-length axis vector,
    // by an angle specified by its sin() and cos(). This does not attempt to
    // verify that axis(x, y, z).length() == 1 or that the sin, cos values are
    // correct. this = this * rotation.
    public void RotateUnitSinCos(double x, double y, double z, double sin_angle, double cos_angle)
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
        double c0r0 = _c0r0, c0r1 = _c0r1, c0r2 = _c0r2, c0r3 = _c0r3;
        double c1r0 = _c1r0, c1r1 = _c1r1, c1r2 = _c1r2, c1r3 = _c1r3;

        _c0r0 = c0r0 + c1r0 * tan_skew_y;
        _c0r1 = c0r1 + c1r1 * tan_skew_y;
        _c0r2 = c0r2 + c1r2 * tan_skew_y;
        _c0r3 = c0r3 + c1r3 * tan_skew_y;

        _c1r0 = c1r0 + c0r0 * tan_skew_x;
        _c1r1 = c1r1 + c0r1 * tan_skew_x;
        _c1r2 = c1r2 + c0r2 * tan_skew_x;
        _c1r3 = c1r3 + c0r3 * tan_skew_x;
    }

    //               |1 skew[0] skew[1] 0|
    // this = this * |0    1    skew[2] 0|
    //               |0    0      1     0|
    //               |0    0      0     1|
    public void ApplyDecomposedSkews(in Vector3D skews)
    {
        //                  / |1 0 0  0|   |1 0 s1 0|   |1 s0 0 0|   |1 s0 s1 0| \
        // |c0 c1 c2 c3| * |  |0 1 s2 0| * |0 1  0 0| * |0  1 0 0| = |0  1 s2 0|  |
        //                 |  |0 0 1  0|   |0 0  1 0|   |0  0 1 0|   |0  0  1 0|  |
        //                  \ |0 0 0  1|   |0 0  0 1|   |0  0 0 1|   |0  0  0 1| /

        double c0r0 = _c0r0, c0r1 = _c0r1, c0r2 = _c0r2, c0r3 = _c0r3;
        double c1r0 = _c1r0, c1r1 = _c1r1, c1r2 = _c1r2, c1r3 = _c1r3;

        _c1r0 = c1r0 + c0r0 * skews.X;
        _c1r1 = c1r1 + c0r1 * skews.X;
        _c1r2 = c1r2 + c0r2 * skews.X;
        _c1r3 = c1r3 + c0r3 * skews.X;

        _c2r0 = c0r0 * skews.Y + c1r0 * skews.Z + _c2r0;
        _c2r1 = c0r1 * skews.Y + c1r1 * skews.Z + _c2r1;
        _c2r2 = c0r2 * skews.Y + c1r2 * skews.Z + _c2r2;
        _c2r3 = c0r3 * skews.Y + c1r3 * skews.Z + _c2r3;
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
    public readonly double MapVector2(Span<double> vec)
    {
        double v0 = vec[0];
        double v1 = vec[1];
        double x = v0 * _c0r0 + v1 * _c1r0 + _c3r0;
        double y = v0 * _c0r1 + v1 * _c1r1 + _c3r1;
        double w = v0 * _c0r3 + v1 * _c1r3 + _c3r3;
        vec[0] = x;
        vec[1] = y;
        return w;
    }

    // Applies the matrix to the vector in place.
    public readonly void MapVector4(double[] vec)
    {
        Double4 v = new(vec[0], vec[1], vec[2], vec[3]);

        Double4 r0 = new(_c0r0, _c1r0, _c2r0, _c3r0);
        Double4 r1 = new(_c0r1, _c1r1, _c2r1, _c3r1);
        Double4 r2 = new(_c0r2, _c1r2, _c2r2, _c3r2);
        Double4 r3 = new(_c0r3, _c1r3, _c2r3, _c3r3);
        
        vec[0] = Double4.Sum(r0 * v);
        vec[1] = Double4.Sum(r1 * v);
        vec[2] = Double4.Sum(r2 * v);
        vec[3] = Double4.Sum(r3 * v);
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
    public readonly DecomposedTransform? Decompose2D()
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

        double m11 = _c0r0;
        double m21 = _c1r0;
        double m12 = _c0r1;
        double m22 = _c1r1;

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
        decomp.Translate.X = _c3r0;
        decomp.Translate.Y = _c3r1;

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

    public readonly DecomposedTransform? Decompose()
    {
        // See documentation of Transform::Decompose() for why we need the 2d branch.
        if (Is2DTransform)
            return Decompose2D();

        // https://www.w3.org/TR/css-transforms-2/#decomposing-a-3d-matrix.

        Double4 c0 = new(_c0r0, _c0r1, _c0r2, _c0r3);
        Double4 c1 = new(_c1r0, _c1r1, _c1r2, _c1r3);
        Double4 c2 = new(_c2r0, _c2r1, _c2r2, _c2r3);
        Double4 c3 = new(_c3r0, _c3r1, _c3r2, _c3r3);

        // Normalize the matrix.
        if (!double.IsNormal(c3.V3))
            return null;

        double inv_w = 1.0 / c3.V3;
        c0 *= inv_w;
        c1 *= inv_w;
        c2 *= inv_w;
        c3 *= inv_w;

        Double4 perspective = new (c0.V3, c1.V3, c2.V3, 1.0);

        // Clear the perspective partition.
        c0.V3 = c1.V3 = c2.V3 = 0;
        c3.V3 = 1;

        Double4 inverse_c0 = c0;
        Double4 inverse_c1 = c1;
        Double4 inverse_c2 = c2;
        Double4 inverse_c3 = c3;

        if (!InverseWithDouble4Cols(ref inverse_c0, ref inverse_c1, ref inverse_c2, ref inverse_c3))
            return null;

        DecomposedTransform decomp = new();

        // First, isolate perspective.
        if (perspective.V0 != 0 || perspective.V1 != 0 || perspective.V2 != 0 || perspective.V3 != 1)
        {
            // Solve the equation by multiplying perspective by the inverse.
            decomp.Perspective.X = Double4.Sum(perspective * inverse_c0);
            decomp.Perspective.Y = Double4.Sum(perspective * inverse_c1);
            decomp.Perspective.Z = Double4.Sum(perspective * inverse_c2);
            decomp.Perspective.W = Double4.Sum(perspective * inverse_c3);
        }

        // Next take care of translation (easy).
        decomp.Translate.X = c3.V0;
        c3.V0 = 0;
        decomp.Translate.Y = c3.V1;
        c3.V1 = 0;
        decomp.Translate.Z = c3.V2;
        c3.V2 = 0;

        // Note: Deviating from the spec in terms of variable naming. The matrix is
        // stored on column major order and not row major. Using the variable 'row'
        // instead of 'column' in the spec pseudocode has been the source of
        // confusion, specifically in sorting out rotations.

        // From now on, only the first 3 components of the Double4 column is used.

        static double sum3(in Double4 c)
        {
            return c.V0 + c.V1 + c.V2;
        }

        static bool extract_scale(ref Double4 c, out double scale)
        {
            scale = Math.Sqrt(sum3(c * c));

            if (!double.IsNormal(scale))
                return false;

            c *= 1.0 / scale;

            return true;
        }

        static double epsilon_to_zero(double d)
        {
            return Math.Abs(d) < float.MachineEpsilon ? 0.0 : d;
        }

        // Compute X scale factor and normalize the first column.
        if (!extract_scale(ref c0, out decomp.Scale.X))
            return null;

        // Compute XY shear factor and make 2nd column orthogonal to 1st.
        decomp.Skew.X = epsilon_to_zero(sum3(c0 * c1));
        c1 -= c0 * decomp.Skew.X;

        // Now, compute Y scale and normalize 2nd column.
        if (!extract_scale(ref c1, out decomp.Scale.Y))
            return null;

        decomp.Skew.X /= decomp.Scale.Y;

        // Compute XZ and YZ shears, and orthogonalize the 3rd column.
        decomp.Skew.Y = epsilon_to_zero(sum3(c0 * c2));
        c2 -= c0 * decomp.Skew.Y;
        decomp.Skew.Z = epsilon_to_zero(sum3(c1 * c2));
        c2 -= c1 * decomp.Skew.Z;

        // Next, get Z scale and normalize the 3rd column.
        if (!extract_scale(ref c2, out decomp.Scale.Z))
            return null;

        decomp.Skew.Y /= decomp.Scale.Z;
        decomp.Skew.Z /= decomp.Scale.Z;

        // At this point, the matrix is orthonormal.
        // Check for a coordinate system flip.  If the determinant is -1, then negate
        // the matrix and the scaling factors.
        static Double4 cross3(in Double4 a, in Double4 b)
        {
            return new Double4(a.V1, a.V2, a.V0, a.V3) * new Double4(b.V2, b.V0, b.V1, b.V3) - new Double4(a.V2, a.V0, a.V1, a.V3) * new Double4(b.V1, b.V2, b.V0, b.V3);
        };

        Double4 pdum3 = cross3(c1, c2);

        if (sum3(c0 * pdum3) < 0)
        {
            // Flip all 3 scaling factors, following the 3d decomposition spec. See
            // documentation of Transform::Decompose() about the difference between
            // the 2d spec and and 3d spec about scale flipping.
            decomp.Scale.X *= -1;
            decomp.Scale.Y *= -1;
            decomp.Scale.Z *= -1;
            c0 *= -1;
            c1 *= -1;
            c2 *= -1;
        }

        // Lastly, compute the quaternions.
        // See https://en.wikipedia.org/wiki/Rotation_matrix#Quaternion.
        // Note: deviating from spec (http://www.w3.org/TR/css3-transforms/)
        // which has a degenerate case when the trace (t) of the orthonormal matrix
        // (Q) approaches -1. In the Wikipedia article, Q_ij is indexing on row then
        // column. Thus, Q_ij = column[j][i].

        // The following are equivalent representations of the rotation matrix:
        //
        // Axis-angle form:
        //
        //      [ c+(1-c)x^2  (1-c)xy-sz  (1-c)xz+sy ]    c = cos theta
        // R =  [ (1-c)xy+sz  c+(1-c)y^2  (1-c)yz-sx ]    s = sin theta
        //      [ (1-c)xz-sy  (1-c)yz+sx  c+(1-c)z^2 ]    [x,y,z] = axis or rotation
        //
        // The sum of the diagonal elements (trace) is a simple function of the cosine
        // of the angle. The w component of the quaternion is cos(theta/2), and we
        // make use of the double angle formula to directly compute w from the
        // trace. Differences between pairs of skew symmetric elements in this matrix
        // isolate the remaining components. Since w can be zero (also numerically
        // unstable if near zero), we cannot rely solely on this approach to compute
        // the quaternion components.
        //
        // Quaternion form:
        //
        //       [ 1-2(y^2+z^2)    2(xy-zw)      2(xz+yw)   ]
        //  r =  [   2(xy+zw)    1-2(x^2+z^2)    2(yz-xw)   ]    q = (x,y,z,w)
        //       [   2(xz-yw)      2(yz+xw)    1-2(x^2+y^2) ]
        //
        // Different linear combinations of the diagonal elements isolates x, y or z.
        // Sums or differences between skew symmetric elements isolate the remainder.

        double r, s, t, x, y, z, w;

        t = c0.V0 + c1.V1 + c2.V2;  // trace of Q

        // https://en.wikipedia.org/wiki/Rotation_matrix#Quaternion
        if (1 + t > 0.001)
        {
            // Numerically stable as long as 1+t is not close to zero. Otherwise use the
            // diagonal element with the greatest value to compute the quaternions.
            r = Math.Sqrt(1.0 + t);
            s = 0.5 / r;
            w = 0.5 * r;
            x = (c1.V2 - c2.V1) * s;
            y = (c2.V0 - c0.V2) * s;
            z = (c0.V1 - c1.V0) * s;
        }
        else if (c0.V0 > c1.V1 && c0.V0 > c2.V2)
        {
            // Q_xx is largest.
            r = Math.Sqrt(1.0 + c0.V0 - c1.V1 - c2.V2);
            s = 0.5 / r;
            x = 0.5 * r;
            y = (c1.V0 + c0.V1) * s;
            z = (c2.V0 + c0.V2) * s;
            w = (c1.V2 - c2.V1) * s;
        }
        else if (c1.V1 > c2.V2)
        {
            // Q_yy is largest.
            r = Math.Sqrt(1.0 - c0.V0 + c1.V1 - c2.V2);
            s = 0.5 / r;
            x = (c1.V0 + c0.V1) * s;
            y = 0.5 * r;
            z = (c2.V1 + c1.V2) * s;
            w = (c2.V0 - c0.V2) * s;
        }
        else
        {
            // Q_zz is largest.
            r = Math.Sqrt(1.0 - c0.V0 - c1.V1 + c2.V2);
            s = 0.5 / r;
            x = (c2.V0 + c0.V2) * s;
            y = (c2.V1 + c1.V2) * s;
            z = 0.5 * r;
            w = (c0.V1 - c1.V0) * s;
        }

        decomp.Quaternion.X = x;
        decomp.Quaternion.Y = y;
        decomp.Quaternion.Z = z;
        decomp.Quaternion.W = w;

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
        Double4 c1 = new(_c1r0, _c1r1, _c1r2, _c1r3);
        Double4 c2 = new(_c2r0, _c2r1, _c2r2, _c2r3);
        Double4 c3 = new(_c3r0, _c3r1, _c3r2, _c3r3);

        if(!InverseWithDouble4Cols(ref c0, ref c1, ref c2, ref c3))
            return false;
        
        result._c0r0 = c0.V0;
        result._c0r1 = c0.V1;
        result._c0r2 = c0.V2;
        result._c0r3 = c0.V3;

        result._c1r0 = c1.V0;
        result._c1r1 = c1.V1;
        result._c1r2 = c1.V2;
        result._c1r3 = c1.V3;

        result._c2r0 = c2.V0;
        result._c2r1 = c2.V1;
        result._c2r2 = c2.V2;
        result._c2r3 = c2.V3;
        
        result._c3r0 = c3.V0;
        result._c3r1 = c3.V1;
        result._c3r2 = c3.V2;
        result._c3r3 = c3.V3;

        return true;
    }

    // HashCode.Add<double>() uses the bit pattern of the value internally,
    // since 0.0 and -0.0 have different bit patterns, they'd produce different hashes if we added them directly,
    // and because Equals(Matrix44 other) treats them as equal, that would violate the contract that equal objects must have equal hash codes.
    public override readonly int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(_c0r0 == 0.0 ? 0.0 : _c0r0);
        hc.Add(_c0r1 == 0.0 ? 0.0 : _c0r1);
        hc.Add(_c0r2 == 0.0 ? 0.0 : _c0r2);
        hc.Add(_c0r3 == 0.0 ? 0.0 : _c0r3);

        hc.Add(_c1r0 == 0.0 ? 0.0 : _c1r0);
        hc.Add(_c1r1 == 0.0 ? 0.0 : _c1r1);
        hc.Add(_c1r2 == 0.0 ? 0.0 : _c1r2);
        hc.Add(_c1r3 == 0.0 ? 0.0 : _c1r3);

        hc.Add(_c2r0 == 0.0 ? 0.0 : _c2r0);
        hc.Add(_c2r1 == 0.0 ? 0.0 : _c2r1);
        hc.Add(_c2r2 == 0.0 ? 0.0 : _c2r2);
        hc.Add(_c2r3 == 0.0 ? 0.0 : _c2r3);

        hc.Add(_c3r0 == 0.0 ? 0.0 : _c3r0);
        hc.Add(_c3r1 == 0.0 ? 0.0 : _c3r1);
        hc.Add(_c3r2 == 0.0 ? 0.0 : _c3r2);
        hc.Add(_c3r3 == 0.0 ? 0.0 : _c3r3);

        return hc.ToHashCode();
    }

    public readonly bool Equals(in Matrix44 other)
    {
        return _c0r0 == other._c0r0 && _c0r1 == other._c0r1 && _c0r2 == other._c0r2 && _c0r3 == other._c0r3 &&
               _c1r0 == other._c1r0 && _c1r1 == other._c1r1 && _c1r2 == other._c1r2 && _c1r3 == other._c1r3 &&
               _c2r0 == other._c2r0 && _c2r1 == other._c2r1 && _c2r2 == other._c2r2 && _c2r3 == other._c2r3 &&
               _c3r0 == other._c3r0 && _c3r1 == other._c3r1 && _c3r2 == other._c3r2 && _c3r3 == other._c3r3;
    }

    public override readonly bool Equals(object? obj) => obj is Matrix44 other && Equals(other);

    public static bool operator ==(in Matrix44 left, in Matrix44 right) => left.Equals(right);
    public static bool operator !=(in Matrix44 left, in Matrix44 right) => !left.Equals(right);

    public override readonly string ToString()
    {
        return $"{_c0r0} {_c1r0} {_c2r0} {_c3r0}\n{_c0r1} {_c1r1} {_c2r1} {_c3r1}\n{_c0r2} {_c1r2} {_c2r2} {_c3r2}\n{_c0r3} {_c1r3} {_c2r3} {_c3r3}\n";
    }
}
