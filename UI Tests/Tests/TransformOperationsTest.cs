using Xunit;

using UI.GFX.Geometry;

using static UI.GFX.Geometry.TransformOperation;
using static UI.Tests.GeometryUtil;

namespace UI.Tests;

public static class TransformOperationsTest
{

    private static void ExpectTransformOperationEqual(in TransformOperation lhs, in TransformOperation rhs)
    {
        Assert.Equal(lhs.type, rhs.type);
        AssertTransformEqual(lhs.Matrix, rhs.Matrix);
        
        switch (lhs.type)
        {
            case TransformOperationType.Translate:
                AssertFloatEqual(lhs.X, rhs.X);
                AssertFloatEqual(lhs.Y, rhs.Y);
                AssertFloatEqual(lhs.Z, rhs.Z);
                break;
            case TransformOperationType.Rotate:
                AssertFloatEqual(lhs.X, rhs.X);
                AssertFloatEqual(lhs.Y, rhs.Y);
                AssertFloatEqual(lhs.Z, rhs.Z);
                AssertFloatEqual(lhs.Angle, rhs.Angle);
                break;
            case TransformOperationType.Scale:
                AssertFloatEqual(lhs.X, rhs.X);
                AssertFloatEqual(lhs.Y, rhs.Y);
                AssertFloatEqual(lhs.Z, rhs.Z);
                break;
            case TransformOperationType.SkewX:
            case TransformOperationType.SkewY:
            case TransformOperationType.Skew:
                AssertFloatEqual(lhs.X, rhs.X);
                AssertFloatEqual(lhs.Y, rhs.Y);
                break;
            case TransformOperationType.Perspective:
                AssertFloatEqual(lhs.PerspectiveM43, rhs.PerspectiveM43);
                break;
            case TransformOperationType.Matrix:
            case TransformOperationType.Identity:
                break;
        }
    }

    [Fact]
    private static void TestTransformTypesAreUnique()
    {
        List<TransformOperations> transforms = [];

        TransformOperations to_add = new();
        to_add.AppendTranslate(1, 0, 0);
        transforms.Add(to_add);

        to_add = new();
        to_add.AppendRotate(0, 0, 1, 2);
        transforms.Add(to_add);

        to_add = new();
        to_add.AppendScale(2, 2, 2);
        transforms.Add(to_add);

        to_add = new();
        to_add.AppendSkew(1, 0);
        transforms.Add(to_add);

        to_add = new();
        to_add.AppendPerspective(800);
        transforms.Add(to_add);

        for (int i = 0; i < transforms.Count; ++i)
        {
            for (int j = 0; j < transforms.Count; ++j)
            {
                bool matches_type = transforms[i].MatchesTypes(transforms[j]);
                Assert.True((i == j && matches_type) || !matches_type);
            }
        }
    }
}
