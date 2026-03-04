namespace UI.Extensions;

public static class DoubleExtensions
{
    extension(double source)
    {
        /// <summary>
        /// The difference between 1.0 and the next representable value.
        /// Also known as the "machine epsilon" for IEEE-754 double.
        /// Equivalent to C++ std::numeric_limits<double>::epsilon().
        /// </summary>
        public static double MachineEpsilon => Math.BitIncrement(1f) - 1f;

        /// <summary>
        /// Equivalent to C++ std::numeric_limits<double>::min().
        /// </summary>
        public static double MinNormal => BitConverter.Int64BitsToDouble(0x0010_0000_0000_0000);

        /// <summary>
        /// Number of significant binary digits (mantissa bits + 1).
        /// Equivalent to C++ std::numeric_limits<float>::digits.
        /// </summary>
        public static int Digits => 53;
    }
}
