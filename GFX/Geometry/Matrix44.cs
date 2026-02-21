namespace UI.GFX.Geometry;

using System.Runtime.CompilerServices;
using static Numerics.ClampedMath;

public struct Matrix44
{
    // length 16, column-major
    private readonly double[] _m;

    public double this[int row, int col]
    {
        readonly get => _m[col * 4 + row];
        set => _m[col * 4 + row] = value;
    }
    
    public Matrix44()
    {
        _m = new double[16];

        // Construct identity matrix:
        //
        //[1][0][0][0]
        //[0][1][0][0]
        //[0][0][1][0]
        //[0][0][0][1]

        // Column 0
        _m[0] = 1;
        _m[1] = 0;
        _m[2] = 0;
        _m[3] = 0;

        // Column 1
        _m[4] = 0;
        _m[5] = 1;
        _m[6] = 0;
        _m[7] = 0;

        // Column 2
        _m[8] = 0;
        _m[9] = 0;
        _m[10] = 1;
        _m[11] = 0;

        // Column 3
        _m[12] = 0;
        _m[13] = 0;
        _m[14] = 0;
        _m[15] = 1;

        //this[0, 0] = 1;
        //this[1, 1] = 1;
        //this[2, 2] = 1;
        //this[3, 3] = 1;
    }

    public Matrix44(double r0c0, double r1c0, double r2c0, double r3c0,
                    double r0c1, double r1c1, double r2c1, double r3c1,
                    double r0c2, double r1c2, double r2c2, double r3c2,
                    double r0c3, double r1c3, double r2c3, double r3c3)
    {
        _m = new double[16];

        // Column 0
        _m[0] = r0c0;
        _m[1] = r1c0;
        _m[2] = r2c0;
        _m[3] = r3c0;

        // Column 1
        _m[4] = r0c1;
        _m[5] = r1c1;
        _m[6] = r2c1;
        _m[7] = r3c1;

        // Column 2
        _m[8] = r0c2;
        _m[9] = r1c2;
        _m[10] = r2c2;
        _m[11] = r3c2;

        // Column 3
        _m[12] = r0c3;
        _m[13] = r1c3;
        _m[14] = r2c3;
        _m[15] = r3c3;

        //this[0,0] = r0c0; this[1,0] = r1c0; this[2,0] = r2c0; this[3,0] = r3c0;
        //this[0,1] = r0c1; this[1,1] = r1c1; this[2,1] = r2c1; this[3,1] = r3c1;
        //this[0,2] = r0c2; this[1,2] = r1c2; this[2,2] = r2c2; this[3,2] = r3c2;
        //this[0,3] = r0c3; this[1,3] = r1c3; this[2,3] = r2c3; this[3,3] = r3c3;
    }

    public readonly bool Equals(Matrix44 other)
    {
        for (int i = 0; i < 16; i++)
        {
            if (_m[i] != other._m[i])
                return false;
        }
        
        return true;
    }
}
