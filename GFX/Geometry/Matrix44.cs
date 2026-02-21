using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UI.GFX.Geometry;

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