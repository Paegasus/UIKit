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
        Assert.Equal(a.X, b.X, 1e-7);
        Assert.Equal(a.Y, b.Y, 1e-7);
        Assert.Equal(a.Z, b.Z, 1e-7);
        Assert.Equal(a.W, b.W, 1e-7);
    }

    [Fact]
    private static void TestDefaultConstruction()
    {
        CompareQuaternions(new Quaternion(0, 0, 0, 1), new Quaternion());
    }

    [Fact]
    private static void TestAxisAngleCommon()
    {
        double radians = 0.5;
        Quaternion q = new(new Vector3DF(1, 0, 0), radians);
        CompareQuaternions(new Quaternion(Math.Sin(radians / 2), 0, 0, Math.Cos(radians / 2)), q);
    }

    [Fact]
    private static void TestVectorToVectorRotation()
    {
        var q = new Quaternion(new Vector3DF(1.0f, 0.0f, 0.0f), new Vector3DF(0.0f, 1.0f, 0.0f));
        var r = new Quaternion(new Vector3DF(0.0f, 0.0f, 1.0f), MathF.PI / 2);

        Assert.Equal(r.X, q.X, 1e-7);
        Assert.Equal(r.Y, q.Y, 1e-7);
        Assert.Equal(r.Z, q.Z, 1e-7);
        Assert.Equal(r.W, q.W, 1e-7);
    }

    [Fact]
    private static void TestAxisAngleWithZeroLengthAxis()
    {
        var q = new Quaternion(new Vector3DF(0, 0, 0), 0.5);

        // If the axis is zero length, we should assume the default values.
        CompareQuaternions(q, new Quaternion());
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
