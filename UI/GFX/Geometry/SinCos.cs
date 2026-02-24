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
        // Some math libraries have poor accuracy with large arguments,
        // so range-reduce explicitly before we call sin() or cos(). However, unless
        // we're _really_ large (out of range of an int), we can do that faster than
        // fmod(), since we have an integer divisor (and as an extra bonus, we've
        // already got it precomputed). We pick a pretty arbitrary limit that should
        // be safe.
        //
        // We range-reduce to [0..45]. This should hit the fast path of sincos()
        // on most platforms (since no further reduction is needed; reducing
        // accurately modulo a trancendental can we slow), using only branches that
        // should be possible to do using conditional operations; using a switch
        // instead would be possible, but benchmarked much slower on M1.
        // For platforms that don't use sincos() (e.g., it seems Clang doesn't
        // manage the rewrite on Linux), we also save on having the range reduction
        // done only once.
        double sqrt2 = 1.4142135623730951d;

        if (degrees > -90000000.0d && degrees < 90000000.0d)
        {
            // Make sure 0, 90, 180 and 270 degrees get exact results. (We also have
            // precomputed values for 45, 135, etc., but only as a side effect of using
            // 45 instead of 90, for the benefit of the range reduction algorithm below.
            // The error for e.g. sin(45 degrees) is typically only 1 ulp.)
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
                // This will cause the range-reduction below to move us
                // into [0..45], as desired, instead of [-45..0].
                --octant;
            }
            degrees -= octant * 45.0; // Range-reduce to [0..45].

            // Deal with 45..90 the same as 45..0. This also covers
            // 135..180, 225..270 and 315..360, i.e. the odd octants.
            // The relevant trigonometric identities is that
            // sin(90 - a) = cos(a) and vice versa; we do the sin/cos
            // flip below.
            if ((octant & 1) == 1)
            {
                degrees = 45.0 - degrees;
            }

            double rad = double.DegreesToRadians(degrees);
            double s = Math.Sin(rad);
            double c = Math.Cos(rad);

            // 45..135 and -135..-45 can be moved into the opposite areas
            // simply by flipping the x and y axis (in conjunction with
            // the conversion from CW to CCW done above).
            if (((octant + 1) & 2) != 0)
            {
                (s, c) = (c, s);
            }

            // For sine, 180..360 (lower half) is the same as 0..180,
            // except negative.
            if ((octant & 4) != 0)
            {
                s = -s;
            }

            // For cosine, 90..270 (right half) is the same as -90..90,
            // except negative.
            if (((octant + 2) & 4) != 0)
            {
                c = -c;
            }

            return new SinCos() { sin = s, cos = c };
        }

        // Slow path for extreme cases.
        degrees %= 360.0d;
        double rads = double.DegreesToRadians(degrees);
        return new SinCos() { sin = Math.Sin(rads), cos = Math.Cos(rads) };
    }
}
