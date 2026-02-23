using Xunit;

using UI.GFX.Geometry;
using UI.Extensions;

namespace UI.Tests;

public static class RoundedCornersFTest
{
    [Fact]
    private static void TestDefaultConstructor()
    {
        RoundedCornersF rc = new();
        Assert.Equal(0.0f, rc.UpperLeft);
        Assert.Equal(0.0f, rc.UpperRight);
        Assert.Equal(0.0f, rc.LowerRight);
        Assert.Equal(0.0f, rc.LowerLeft);
    }

    [Fact]
    private static void TestFromSingleValue()
    {
        float kValue = 1.33f;
        RoundedCornersF rc = new(kValue);
        Assert.Equal(kValue, rc.UpperLeft);
        Assert.Equal(kValue, rc.UpperRight);
        Assert.Equal(kValue, rc.LowerRight);
        Assert.Equal(kValue, rc.LowerLeft);
    }

    [Fact]
    private static void TestFromFourValues()
    {
        float kValue1 = 1.33f;
        float kValue2 = 2.66f;
        float kValue3 = 0.1f;
        float kValue4 = 50.0f;
        RoundedCornersF rc = new(kValue1, kValue2, kValue3, kValue4);
        Assert.Equal(kValue1, rc.UpperLeft);
        Assert.Equal(kValue2, rc.UpperRight);
        Assert.Equal(kValue3, rc.LowerRight);
        Assert.Equal(kValue4, rc.LowerLeft);
    }

    [Fact]
    private static void TestIsEmpty()
    {
        Assert.True(new RoundedCornersF().IsEmpty);
        Assert.False(new RoundedCornersF(1.0f).IsEmpty);
        Assert.False(new RoundedCornersF(1.0f, 0.0f, 0.0f, 0.0f).IsEmpty);
        Assert.False(new RoundedCornersF(0.0f, 1.0f, 0.0f, 0.0f).IsEmpty);
        Assert.False(new RoundedCornersF(0.0f, 0.0f, 1.0f, 0.0f).IsEmpty);
        Assert.False(new RoundedCornersF(0.0f, 0.0f, 0.0f, 1.0f).IsEmpty);
    }

    [Fact]
    private static void TestEquality()
    {
        RoundedCornersF kCorners = new(1.33f, 2.66f, 0.1f, 50.0f);
        RoundedCornersF rc = kCorners;
        // Using Assert.True and Assert.False to explicitly test == and != operators
        // (rather than Assert.Equal, EXPECT_NE).
        Assert.True(rc == kCorners);
        Assert.False(rc != kCorners);
        rc.UpperLeft = 2.0f;
        Assert.False(rc == kCorners);
        Assert.True(rc != kCorners);
        rc = kCorners;
        rc.UpperRight = 2.0f;
        Assert.False(rc == kCorners);
        Assert.True(rc != kCorners);
        rc = kCorners;
        rc.LowerLeft = 2.0f;
        Assert.False(rc == kCorners);
        Assert.True(rc != kCorners);
        rc = kCorners;
        rc.LowerRight = 2.0f;
        Assert.False(rc == kCorners);
        Assert.True(rc != kCorners);
    }

    [Fact]
    private static void TestSet()
    {
        RoundedCornersF rc = new(1.0f, 2.0f, 3.0f, 4.0f);
        rc.Set(4.0f, 3.0f, 2.0f, 1.0f);
        Assert.Equal(4.0f, rc.UpperLeft);
        Assert.Equal(3.0f, rc.UpperRight);
        Assert.Equal(2.0f, rc.LowerRight);
        Assert.Equal(1.0f, rc.LowerLeft);
    }

    [Fact]
    private static void TestSetProperties()
    {
        RoundedCornersF rc = new(1.0f, 2.0f, 3.0f, 4.0f);

        rc.UpperLeft = 50.0f;
        Assert.Equal(50.0f, rc.UpperLeft);
        Assert.Equal(2.0f, rc.UpperRight);
        Assert.Equal(3.0f, rc.LowerRight);
        Assert.Equal(4.0f, rc.LowerLeft);

        rc.UpperRight = 40.0f;
        Assert.Equal(50.0f, rc.UpperLeft);
        Assert.Equal(40.0f, rc.UpperRight);
        Assert.Equal(3.0f, rc.LowerRight);
        Assert.Equal(4.0f, rc.LowerLeft);

        rc.LowerRight = 30.0f;
        Assert.Equal(50.0f, rc.UpperLeft);
        Assert.Equal(40.0f, rc.UpperRight);
        Assert.Equal(30.0f, rc.LowerRight);
        Assert.Equal(4.0f, rc.LowerLeft);

        rc.LowerLeft = 20.0f;
        Assert.Equal(50.0f, rc.UpperLeft);
        Assert.Equal(40.0f, rc.UpperRight);
        Assert.Equal(30.0f, rc.LowerRight);
        Assert.Equal(20.0f, rc.LowerLeft);
    }

    // Verify that IsEmpty() returns true and that all values are exactly zero.
    private static void VerifyEmptyAndZero(in RoundedCornersF rc)
    {
        Assert.True(rc.IsEmpty);
        Assert.Equal(0.0f, rc.UpperLeft);
        Assert.Equal(0.0f, rc.UpperRight);
        Assert.Equal(0.0f, rc.LowerRight);
        Assert.Equal(0.0f, rc.LowerLeft);
    }

    [Fact]
    private static void TestEpsilon()
    {
        float kEpsilon = float.MachineEpsilon;
        RoundedCornersF rc = new(kEpsilon, kEpsilon, kEpsilon, kEpsilon);
        VerifyEmptyAndZero(rc);

        rc.UpperLeft = kEpsilon;
        VerifyEmptyAndZero(rc);
        rc.UpperRight = kEpsilon;
        VerifyEmptyAndZero(rc);
        rc.LowerRight = kEpsilon;
        VerifyEmptyAndZero(rc);
        rc.LowerLeft = kEpsilon;
        VerifyEmptyAndZero(rc);

        rc.Set(kEpsilon, kEpsilon, kEpsilon, kEpsilon);
        VerifyEmptyAndZero(rc);
    }

    [Fact]
    private static void TestNegative()
    {
        float kNegative = -0.5f;
        RoundedCornersF rc = new(kNegative, kNegative, kNegative, kNegative);
        VerifyEmptyAndZero(rc);

        rc.UpperLeft =kNegative;
        VerifyEmptyAndZero(rc);
        rc.UpperRight = kNegative;
        VerifyEmptyAndZero(rc);
        rc.LowerRight = kNegative;
        VerifyEmptyAndZero(rc);
        rc.LowerLeft = kNegative;
        VerifyEmptyAndZero(rc);

        rc.Set(kNegative, kNegative, kNegative, kNegative);
        VerifyEmptyAndZero(rc);
    }
}
