using System.Runtime.InteropServices;

namespace UI.GFX.Geometry;

[StructLayout(LayoutKind.Sequential)]
public struct SinCos
{
    public double sin;
    public double cos;

    public SinCos(double sin_, double cos_) => (sin, cos) = (sin_, cos_);

    public readonly bool IsZeroAngle() => sin == 0 && cos == 1;

    public static SinCos SinCosDegrees(double degrees)
    {
        double sqrt2 = 1.4142135623730951d;

        if (degrees > -90000000.0d && degrees < 90000000.0d)
        {
            double n45degrees = degrees / 45.0d;
            int octant = (int)n45degrees;
            if (octant == n45degrees)
            {
                var kSinCosN45 = new SinCos[]
                {
                    new(0, 1),
                    new( sqrt2 / 2, sqrt2 / 2),
                    new( 1, 0),
                    new( sqrt2 / 2, -sqrt2 / 2),
                    new( 0, -1),
                    new( -sqrt2 / 2, -sqrt2 / 2),
                    new( -1, 0),
                    new( -sqrt2 / 2, sqrt2 / 2)
                };

                return kSinCosN45[octant & 7];
            }

            if (degrees < 0)
            {
                --octant;
            }
            degrees -= octant * 45.0;

            if ((octant & 1) == 1)
            {
                degrees = 45.0 - degrees;
            }

            double rad = double.DegreesToRadians(degrees);
            double s = Math.Sin(rad);
            double c = Math.Cos(rad);

            if (((octant + 1) & 2) != 0)
            {
                (s, c) = (c, s);
            }

            if ((octant & 4) != 0)
            {
                s = -s;
            }

            if (((octant + 2) & 4) != 0)
            {
                c = -c;
            }

            return new SinCos() { sin = s, cos = c };
        }

        degrees %= 360.0d;
        double rads = double.DegreesToRadians(degrees);
        return new SinCos() { sin = Math.Sin(rads), cos = Math.Cos(rads) };
    }
}
