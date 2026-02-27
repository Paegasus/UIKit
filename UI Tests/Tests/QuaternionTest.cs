using Xunit;

using UI.GFX.Geometry;

namespace UI.Tests;

public static class QuaternionTest
{
    private static double kEpsilon = 1e-7;

    private static void AssertQuaternion(Quaternion expected, Quaternion actual)
    {
        Assert.Equal(expected.X, actual.X, kEpsilon);
        Assert.Equal(expected.Y, actual.Y, kEpsilon);
        Assert.Equal(expected.Z, actual.Z, kEpsilon);
        Assert.Equal(expected.W, actual.W, kEpsilon);
    }

    private static void CompareQuaternions(Quaternion a, Quaternion b)
    {
        Assert.Equal(a.X, b.X, 1e-6f);
        Assert.Equal(a.Y, b.Y, 1e-6f);
        Assert.Equal(a.Z, b.Z, 1e-6f);
        Assert.Equal(a.W, b.W, 1e-6f);
    }

    [Fact]
    private static void TestDefaultConstruction()
    {
        CompareQuaternions(new Quaternion(0, 0, 0, 1), new Quaternion());
    }

    private static void TestAxisAngleCommon()
    {
        
    }

    private static void TestVectorToVectorRotation()
    {
        
    }

    private static void TestAxisAngleWithZeroLengthAxis()
    {
        
    }

    private static void TestAddition()
    {
        
    }

    private static void TestMultiplication()
    {
        
    }

    private static void TestScaling()
    {
        
    }

    private static void TestNormalization()
    {
        
    }

    private static void TestLerp()
    {
        
    }

    private static void TestSlerp()
    {
        
    }

    private static void TestSlerpOppositeAngles()
    {
        
    }

    private static void TestSlerpRotateXRotateY()
    {
        
    }

    private static void TestSlerp360()
    {
        
    }

    private static void TestSlerpEquivalentQuaternions()
    {
        
    }

    private static void TestSlerpQuaternionWithInverse()
    {
        
    }

    private static void TestSlerpObtuseAngle()
    {
        
    }

    private static void TestEquals()
    {
        
    }

    private static void TestNotEquals()
    {
        
    }
}
