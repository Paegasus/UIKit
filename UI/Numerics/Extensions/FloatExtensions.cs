using System.Numerics;

namespace UI.Extensions;

public static class FloatExtensions
{
    extension(float source)
    {
        /// <summary>
        /// The difference between 1.0f and the next representable value.
        /// Also known as the "machine epsilon" for IEEE-754 float.
        /// Equivalent to C++ std::numeric_limits<float>::epsilon().
        /// </summary>
        public static float MachineEpsilon => MathF.BitIncrement(1f) - 1f;
    }
}
