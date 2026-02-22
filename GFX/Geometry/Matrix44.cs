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
            double a = x.matrix_[0][0];
            double b = x.matrix_[0][1];
            double c = x.matrix_[1][0];
            double d = x.matrix_[1][1];
            double e = x.matrix_[3][0];
            double f = x.matrix_[3][1];
            double ya = y.matrix_[0][0];
            double yb = y.matrix_[0][1];
            double yc = y.matrix_[1][0];
            double yd = y.matrix_[1][1];
            double ye = y.matrix_[3][0];
            double yf = y.matrix_[3][1];
            this = new Matrix44(a * ya + c * yb, b * ya + d * yb, 0, 0,           // col 0
                             a * yc + c * yd, b * yc + d * yd, 0, 0,           // col 1
                             0, 0, 1, 0,                                       // col 2
                             a * ye + c * yf + e, b * ye + d * yf + f, 0, 1);  // col 3
            return;
        }

        var c0 = x.Col(0);
        var c1 = x.Col(1);
        var c2 = x.Col(2);
        var c3 = x.Col(3);

        var mc0 = y.Col(0);
        var mc1 = y.Col(1);
        var mc2 = y.Col(2);
        var mc3 = y.Col(3);

        SetCol(0, c0 * mc0[0] + c1 * mc0[1] + c2 * mc0[2] + c3 * mc0[3]);
        SetCol(1, c0 * mc1[0] + c1 * mc1[1] + c2 * mc1[2] + c3 * mc1[3]);
        SetCol(2, c0 * mc2[0] + c1 * mc2[1] + c2 * mc2[2] + c3 * mc2[3]);
        SetCol(3, c0 * mc3[0] + c1 * mc3[1] + c2 * mc3[2] + c3 * mc3[3]);
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
