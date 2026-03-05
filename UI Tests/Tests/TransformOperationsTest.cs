using Xunit;

using UI.GFX.Geometry;

using static UI.GFX.Geometry.TransformOperation;
using static UI.Tests.GeometryUtil;
using System.Diagnostics;

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

    [Fact]
    private static void TestMatchingPrefixSameLength()
    {
        TransformOperations translates = new();
        translates.AppendTranslate(1, 0, 0);
        translates.AppendTranslate(1, 0, 0);
        translates.AppendTranslate(1, 0, 0);

        TransformOperations skews = new();
        skews.AppendSkew(0, 2);
        skews.AppendSkew(0, 2);
        skews.AppendSkew(0, 2);

        TransformOperations translates2 = new();
        translates2.AppendTranslate(0, 2, 0);
        translates2.AppendTranslate(0, 2, 0);
        translates2.AppendTranslate(0, 2, 0);

        TransformOperations mixed = new();
        mixed.AppendTranslate(0, 2, 0);
        mixed.AppendScale(2, 1, 1);
        mixed.AppendSkew(0, 2);

        TransformOperations translates3 = translates2;

        Assert.Equal(0, translates.MatchingPrefixLength(skews));
        Assert.Equal(3, translates.MatchingPrefixLength(translates2));
        Assert.Equal(3, translates.MatchingPrefixLength(translates3));
        Assert.Equal(1, translates.MatchingPrefixLength(mixed));
    }

    [Fact]
    private static void TestMatchingPrefixDifferentLength()
    {
        TransformOperations translates = new();
        translates.AppendTranslate(1, 0, 0);
        translates.AppendTranslate(1, 0, 0);
        translates.AppendTranslate(1, 0, 0);

        TransformOperations skews = new();
        skews.AppendSkew(2, 0);
        skews.AppendSkew(2, 0);

        TransformOperations translates2 = new();
        translates2.AppendTranslate(0, 2, 0);
        translates2.AppendTranslate(0, 2, 0);

        TransformOperations none = new();

        Assert.Equal(0, translates.MatchingPrefixLength(skews));
        // Pad the length of the shorter list provided all previous operation-pairs match per spec
        // (https://drafts.csswg.org/css-transforms/#interpolation-of-transforms).
        Assert.Equal(3, translates.MatchingPrefixLength(translates2));
        Assert.Equal(3, translates.MatchingPrefixLength(none));
    }

    [Fact]
    private static void TestMatchingPrefixLengthOrder()
    {
        TransformOperations mix_order_identity = new();
        mix_order_identity.AppendTranslate(0, 0, 0);
        mix_order_identity.AppendScale(1, 1, 1);
        mix_order_identity.AppendTranslate(0, 0, 0);

        TransformOperations mix_order_one = new();
        mix_order_one.AppendTranslate(0, 1, 0);
        mix_order_one.AppendScale(2, 1, 3);
        mix_order_one.AppendTranslate(1, 0, 0);

        TransformOperations mix_order_two = new();
        mix_order_two.AppendTranslate(0, 1, 0);
        mix_order_two.AppendTranslate(1, 0, 0);
        mix_order_two.AppendScale(2, 1, 3);

        Assert.Equal(3, mix_order_identity.MatchingPrefixLength(mix_order_one));
        Assert.Equal(1, mix_order_identity.MatchingPrefixLength(mix_order_two));
        Assert.Equal(1, mix_order_one.MatchingPrefixLength(mix_order_two));
    }

    private static List<TransformOperations> GetIdentityOperations()
    {
        List<TransformOperations> operations = new();
        TransformOperations to_add = new();
        operations.Add(to_add);

        to_add = new();
        to_add.AppendTranslate(0, 0, 0);
        operations.Add(to_add);

        to_add = new();
        to_add.AppendTranslate(0, 0, 0);
        to_add.AppendTranslate(0, 0, 0);
        operations.Add(to_add);

        to_add = new();
        to_add.AppendScale(1, 1, 1);
        operations.Add(to_add);

        to_add = new();
        to_add.AppendScale(1, 1, 1);
        to_add.AppendScale(1, 1, 1);
        operations.Add(to_add);

        to_add = new();
        to_add.AppendSkew(0, 0);
        operations.Add(to_add);

        to_add = new();
        to_add.AppendSkew(0, 0);
        to_add.AppendSkew(0, 0);
        operations.Add(to_add);

        to_add = new();
        to_add.AppendRotate(0, 0, 1, 0);
        operations.Add(to_add);

        to_add = new();
        to_add.AppendRotate(0, 0, 1, 0);
        to_add.AppendRotate(0, 0, 1, 0);
        operations.Add(to_add);

        to_add = new();
        to_add.AppendMatrix(new Transform());
        operations.Add(to_add);

        to_add = new();
        to_add.AppendMatrix(new Transform());
        to_add.AppendMatrix(new Transform());
        operations.Add(to_add);

        return operations;
    }

    [Fact]
    private static void TestNoneAlwaysMatches()
    {
        List<TransformOperations> operations = GetIdentityOperations();

        TransformOperations none_operation = new();
        foreach (var operation in operations)
        {
            Assert.Equal(operation.Size, operation.MatchingPrefixLength(none_operation));
        }
    }

    [Fact]
    private static void TestApplyTranslate()
    {
        float x = 1;
        float y = 2;
        float z = 3;
        TransformOperations operations = new();
        operations.AppendTranslate(x, y, z);
        Transform expected = new();
        expected.Translate3D(x, y, z);
        AssertTransformEqual(expected, operations.Apply());
    }
}
