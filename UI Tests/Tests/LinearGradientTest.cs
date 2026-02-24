using Xunit;

using UI.GFX.Geometry;

using static UI.GFX.Geometry.PointConversions;

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

        /*
        EXPECT_EQ(45, gradient.angle());
        EXPECT_EQ(3u, gradient.step_count());
        EXPECT_FLOAT_EQ(gradient.steps()[0].fraction, .1);
        EXPECT_EQ(gradient.steps()[0].alpha, 0);
        EXPECT_FLOAT_EQ(gradient.steps()[1].fraction, .5);
        EXPECT_EQ(gradient.steps()[1].alpha, 50);
        EXPECT_FLOAT_EQ(gradient.steps()[2].fraction, .8);
        EXPECT_EQ(gradient.steps()[2].alpha, 1);

        LinearGradient gradient2(90);
        gradient2.AddStep(.1, 0);
        gradient2.AddStep(.5, 50);
        gradient2.AddStep(.8, 1);

        EXPECT_NE(gradient, gradient2);

        gradient2.set_angle(45);
        EXPECT_EQ(gradient, gradient2);

        gradient2.AddStep(.9, 0);
        EXPECT_NE(gradient, gradient2);
        */
    }
}
