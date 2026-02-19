using System.Diagnostics;
using UI.Geometry;

namespace UI.Tests;

public static class LayoutUnitTests
{
    public static void RunAllTests()
    {
        TestConstructors();
        TestConversionsAndRounding();
        TestArithmeticOperators();
        TestIncrementDecrementOperators();
        TestComparisonOperators();
        TestUtilityFunctions();
        TestSpecialValues();
        
        Debug.WriteLine("All LayoutUnit tests passed!");
    }

    private static void TestConstructors()
    {
        Debug.WriteLine("Running Constructor Tests...");

        // From int
        var from_int = new LayoutUnit(10);
        Debug.Assert(from_int.RawValue() == 10 << LayoutUnit.FractionalBits);

        // From float
        var from_float = new LayoutUnit(10.5f);
        Debug.Assert(from_float.RawValue() == (int)(10.5f * LayoutUnit.FixedPointDenominator));

        // From double
        var from_double = new LayoutUnit(20.25);
        Debug.Assert(from_double.RawValue() == (int)(20.25 * LayoutUnit.FixedPointDenominator));

        // Saturation from int
        var from_int_max = new LayoutUnit(LayoutUnit.IntegerMax + 100);
        Debug.Assert(from_int_max == LayoutUnit.MaxValue);

        var from_int_min = new LayoutUnit(LayoutUnit.IntegerMin - 100);
        Debug.Assert(from_int_min == LayoutUnit.MinValue);
    }

    private static void TestConversionsAndRounding()
    {
        Debug.WriteLine("Running Conversion and Rounding Tests...");
        
        var val1 = new LayoutUnit(10.5f); // Raw = 672
        var val2 = new LayoutUnit(10.49f); // Raw = 671 -> constructor truncates to (int)(10.49*64)=671
        var val3 = new LayoutUnit(-10.5f); // Raw = -672

        // Conversions
        Debug.Assert(val1.ToInteger() == 10);
        Debug.Assert(val1.ToFloat() == 10.5f);
        Debug.Assert(val1.ToDouble() == 10.5);

        // Rounding
        Debug.Assert(val1.Round() == 11);
        Debug.Assert(val2.Round() == 10);
        // Corrected: -10.5 rounds to the nearest even number, which is -10.
        Debug.Assert(val3.Round() == -10);

        // Ceiling
        // Corrected: Constructor truncates 10.01*64 = 640.64 to 640. Ceil(640/64) is 10.
        Debug.Assert(new LayoutUnit(10.01f).Ceil() == 10);
        Debug.Assert(new LayoutUnit(10.99f).Ceil() == 11, "Ceil(10.99) should be 11");
        Debug.Assert(new LayoutUnit(10.0f).Ceil() == 10);
        Debug.Assert(new LayoutUnit(-10.01f).Ceil() == -10);
        Debug.Assert(new LayoutUnit(-10.99f).Ceil() == -10);

        // Floor
        Debug.Assert(new LayoutUnit(10.01f).Floor() == 10);
        Debug.Assert(new LayoutUnit(10.99f).Floor() == 10);
        // Corrected: Constructor truncates -10.01*64 = -640.64 to -640. Floor(-640/64) is -10.
        Debug.Assert(new LayoutUnit(-10.01f).Floor() == -10);
        Debug.Assert(new LayoutUnit(-10.99f).Floor() == -11, "Floor(-10.99) should be -11");
    }

    private static void TestArithmeticOperators()
    {
        Debug.WriteLine("Running Arithmetic Operator Tests...");

        var a = new LayoutUnit(10.5f);
        var b = new LayoutUnit(2.25f);

        // Addition
        Debug.Assert(a + b == new LayoutUnit(12.75f));
        // Subtraction
        Debug.Assert(a - b == new LayoutUnit(8.25f));
        // Multiplication
        Debug.Assert(a * b == new LayoutUnit(10.5f * 2.25f));
        Debug.Assert(a * 2 == new LayoutUnit(21.0f));
        // Division
        Debug.Assert(a / b == new LayoutUnit(10.5f / 2.25f));
        Debug.Assert(a / 2 == new LayoutUnit(5.25f));
        // Unary negation
        Debug.Assert(-a == new LayoutUnit(-10.5f));

        // Test saturation
        var max = LayoutUnit.MaxValue;
        var min = LayoutUnit.MinValue;
        var one = new LayoutUnit(1);

        Debug.Assert(max + one == max, "Addition saturation failed");
        Debug.Assert(min - one == min, "Subtraction saturation failed");

        // Test int.MinValue negation
        unchecked
        {
            var minValue = LayoutUnit.MinValue;
            var negatedMinValue = -minValue;
            Debug.Assert(negatedMinValue.RawValue() == -minValue.RawValue(), "Negation of MinValue should wrap");
        }
    }
    
    private static void TestIncrementDecrementOperators()
    {
        Debug.WriteLine("Running Increment/Decrement Tests...");

        var a = new LayoutUnit(5.5f);
        var b = a;

        // Test equivalent behavior
        var incremented_a = a++;
        var added_one_b = b + new LayoutUnit(1);
        Debug.Assert(a == added_one_b, "Post-increment failed equivalence test");
        Debug.Assert(incremented_a.RawValue() == new LayoutUnit(5.5f).RawValue()); // post-increment returns original

        a = new LayoutUnit(5.5f);
        b = a;
        var decremented_a = a--;
        var subtracted_one_b = b - new LayoutUnit(1);
        Debug.Assert(a == subtracted_one_b, "Post-decrement failed equivalence test");
        Debug.Assert(decremented_a.RawValue() == new LayoutUnit(5.5f).RawValue()); // post-decrement returns original
        
        a = new LayoutUnit(5.5f);
        b = a;
        ++a;
        b += new LayoutUnit(1);
        Debug.Assert(a == b, "Pre-increment failed equivalence test");
    }

    private static void TestComparisonOperators()
    {
        Debug.WriteLine("Running Comparison Operator Tests...");

        var a = new LayoutUnit(10);
        var b = new LayoutUnit(20);
        var c = new LayoutUnit(10);

        Debug.Assert(a == c);
        Debug.Assert(a != b);
        Debug.Assert(a < b);
        Debug.Assert(b > a);
        Debug.Assert(a <= c);
        Debug.Assert(a >= c);

        Debug.Assert(a < 20);
        Debug.Assert(b > 10);
        Debug.Assert(a == 10);
    }

    private static void TestUtilityFunctions()
    {
        Debug.WriteLine("Running Utility Function Tests...");

        var neg = new LayoutUnit(-5.25f);
        Debug.Assert(neg.Abs() == new LayoutUnit(5.25f));

        var frac = new LayoutUnit(12.75f); // Raw = 816
        Debug.Assert(frac.HasFraction);
        Debug.Assert(!new LayoutUnit(12).HasFraction);
        Debug.Assert(frac.Fraction().RawValue() == 48); // 0.75 * 64 = 48

        // SnapSizeToPixel
        var loc1 = new LayoutUnit(10.25f);
        var size1 = new LayoutUnit(20.5f);
        Debug.Assert(LayoutUnit.SnapSizeToPixel(size1, loc1) == 21);

        var loc2 = new LayoutUnit(10.75f);
        var size2 = new LayoutUnit(0.1f); // Raw value is 6
        // Corrected: result is 0, but since size > 4, it must return 1.
        Debug.Assert(LayoutUnit.SnapSizeToPixel(size2, loc2) == 1);
        
        // MulDiv
        var a = new LayoutUnit(100);
        var b = new LayoutUnit(50);
        var c = new LayoutUnit(10);
        Debug.Assert(LayoutUnit.MulDiv(a,b,c) == new LayoutUnit(500));
    }
    
    private static void TestSpecialValues()
    {
        Debug.WriteLine("Running Special Value Tests...");
        
        Debug.Assert(LayoutUnit.IndefiniteSize.RawValue() == -LayoutUnit.FixedPointDenominator);
        
        var clamped = LayoutUnit.IndefiniteSize.ClampIndefiniteToZero();
        Debug.Assert(clamped.RawValue() == 0);
        
        var positive = new LayoutUnit(10);
        Debug.Assert(positive.ClampIndefiniteToZero() == positive);
    }
}
