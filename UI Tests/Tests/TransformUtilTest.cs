using Xunit;

using UI.GFX.Geometry;

using static UI.GFX.Geometry.TransformUtil;

namespace UI.Tests;

public static class TransformUtilTest
{
    [Fact]
    private static void TestGetScaleTransform()
    {
        Point kAnchor = new(20, 40);
        const float kScale = 0.5f;

        Transform scale = GetScaleTransform(kAnchor, kScale);

        const int kOffset = 10;

        for (int sign_x = -1; sign_x <= 1; ++sign_x)
        {
            for (int sign_y = -1; sign_y <= 1; ++sign_y)
            {
                Point test = scale.MapPoint(new Point(kAnchor.X + sign_x * kOffset, kAnchor.Y + sign_y * kOffset));

                Assert.Equal(new Point((int)(kAnchor.X + sign_x * kOffset * kScale), (int)(kAnchor.Y + sign_y * kOffset * kScale)), test);
            }
        }
    }
}
