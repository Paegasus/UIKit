using Xunit;

using UI.GFX.Geometry;
using System.Diagnostics;

namespace UI.Tests;

public static class LinearGradientTest
{
    [Fact]
    private static void TestBasic()
    {
        LinearGradient gradient = new(45);
        Assert.True(gradient.IsEmpty);

        gradient.AddStep(0.1f, 0);
        gradient.AddStep(0.5f, 50);
        gradient.AddStep(0.8f, 1);

        Assert.False(gradient.IsEmpty);
        Assert.Equal(45, gradient.Angle);
        Assert.Equal(3, gradient.StepCount);
        Assert.Equal(0.1f, gradient.Steps[0].Fraction);
        Assert.Equal(0, gradient.Steps[0].Alpha);
        Assert.Equal(0.5f, gradient.Steps[1].Fraction);
        Assert.Equal(50, gradient.Steps[1].Alpha);
        Assert.Equal(0.8f, gradient.Steps[2].Fraction);
        Assert.Equal(1, gradient.Steps[2].Alpha);

        
        LinearGradient gradient2 = new(90);
        gradient2.AddStep(0.1f, 0);
        gradient2.AddStep(0.5f, 50);
        gradient2.AddStep(0.8f, 1);

        Assert.NotEqual(gradient, gradient2);

        gradient2.Angle = 45;
        Assert.Equal(gradient, gradient2);

        gradient2.AddStep(0.9f, 0);
        Assert.NotEqual(gradient, gradient2);
    }

    [Fact]
    private static void TestReverse()
    {
        LinearGradient gradient = new(45);

        // Make sure reversing an empty LinearGradient doesn't cause an issue.
        gradient.ReverseSteps();

        gradient.AddStep(0.1f, 0);
        gradient.AddStep(0.5f, 50);
        gradient.AddStep(0.8f, 1);

        gradient.ReverseSteps();

        Assert.Equal(45, gradient.Angle);
        Assert.Equal(3, gradient.StepCount);

        float tolerance = 1E-7f;

        Assert.Equal(0.2f, gradient.Steps[0].Fraction, tolerance);
        Assert.Equal(1, gradient.Steps[0].Alpha);
        Assert.Equal(0.5f, gradient.Steps[1].Fraction, tolerance);
        Assert.Equal(50, gradient.Steps[1].Alpha);
        Assert.Equal(0.9f, gradient.Steps[2].Fraction, tolerance);
        Assert.Equal(0, gradient.Steps[2].Alpha);
    }

    [Fact]
    private static void TestApplyTransform()
    {
        {
            LinearGradient gradient = new(45);
            Transform transform = new();
            transform.Translate(10, 50);
            gradient.ApplyTransform(transform);
            Assert.Equal(45, gradient.Angle);
        }
        // Scale can change the angle.
        {
            LinearGradient gradient = new(45);
            Transform transform = new();
            transform.Scale(1, 10);
            gradient.ApplyTransform(transform);
            Assert.Equal(84, gradient.Angle);
        }
        {
            LinearGradient gradient = new(45);
            Transform transform = new();
            transform.Rotate(45);

            //Debug.WriteLine(transform.GetFullMatrix());

            gradient.ApplyTransform(transform);
            Assert.Equal(0, gradient.Angle); // Fails, Expected: 0, Actual: -23
        }
    }
}
