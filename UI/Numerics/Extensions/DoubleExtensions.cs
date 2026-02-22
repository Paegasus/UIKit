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
        public static double MachineEpsilon => Math.Pow(2, -52);
    }
}
