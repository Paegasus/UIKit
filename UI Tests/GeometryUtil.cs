using Xunit;

using UI.GFX.Geometry;

namespace UI.Tests;

public static class GeometryUtil
{
    public static void AssertRectFEqual(in RectF lhs, in RectF rhs)
    {
        float epsilon = 1e-5f;
        Assert.Equal(lhs.X,      rhs.X,      epsilon);
        Assert.Equal(lhs.Y,      rhs.Y,      epsilon);
        Assert.Equal(lhs.Width,  rhs.Width,  epsilon);
        Assert.Equal(lhs.Height, rhs.Height, epsilon);
    }
}

/*
::testing::AssertionResult AssertRectFloatEqual(const char* lhs_expr,
                                                const char* rhs_expr,
                                                const RectF& lhs,
                                                const RectF& rhs) {
  if (FloatAlmostEqual(lhs.x(), rhs.x()) &&
      FloatAlmostEqual(lhs.y(), rhs.y()) &&
      FloatAlmostEqual(lhs.width(), rhs.width()) &&
      FloatAlmostEqual(lhs.height(), rhs.height())) {
    return ::testing::AssertionSuccess();
  }
  return EqFailure(lhs_expr, rhs_expr, lhs, rhs);
}
*/