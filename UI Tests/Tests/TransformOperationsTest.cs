using Xunit;

using UI.GFX.Geometry;
using UI.GFX.Animation;

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

    [Fact]
    private static void TestApplyRotate()
    {
        float x = 1;
        float y = 2;
        float z = 3;
        float degrees = 80;
        TransformOperations operations = new();
        operations.AppendRotate(x, y, z, degrees);
        Transform expected = new();
        expected.RotateAbout(new Vector3DF(x, y, z), degrees);
        AssertTransformEqual(expected, operations.Apply());
    }

    [Fact]
    private static void TestApplyScale()
    {
        float x = 1;
        float y = 2;
        float z = 3;
        TransformOperations operations = new();
        operations.AppendScale(x, y, z);
        Transform expected = new();
        expected.Scale3D(x, y, z);
        AssertTransformEqual(expected, operations.Apply());
    }

    [Fact]
    private static void TestApplySkew()
    {
        float x = 1;
        float y = 2;
        TransformOperations operations = new();
        operations.AppendSkew(x, y);
        Transform expected = new();
        expected.Skew(x, y);
        AssertTransformEqual(expected, operations.Apply());
    }

    [Fact]
    private static void TestApplyPerspective()
    {
        float depth = 800;
        TransformOperations operations = new();
        operations.AppendPerspective(depth);
        Transform expected = new();
        expected.ApplyPerspectiveDepth(depth);
        AssertTransformEqual(expected, operations.Apply());
    }

    [Fact]
    private static void TestApplyMatrix()
    {
        float dx = 1;
        float dy = 2;
        float dz = 3;
        Transform expected_matrix = new();
        expected_matrix.Translate3D(dx, dy, dz);
        TransformOperations matrix_transform = new();
        matrix_transform.AppendMatrix(expected_matrix);
        AssertTransformEqual(expected_matrix, matrix_transform.Apply());
    }

    [Fact]
    private static void TestApplyOrder()
    {
        float sx = 2;
        float sy = 4;
        float sz = 8;

        float dx = 1;
        float dy = 2;
        float dz = 3;

        TransformOperations operations = new();
        operations.AppendScale(sx, sy, sz);
        operations.AppendTranslate(dx, dy, dz);

        Transform expected_scale_matrix = new();
        expected_scale_matrix.Scale3D(sx, sy, sz);

        Transform expected_translate_matrix = new();
        expected_translate_matrix.Translate3D(dx, dy, dz);

        Transform expected_combined_matrix = expected_scale_matrix;
        expected_combined_matrix.PreConcat(expected_translate_matrix);

        AssertTransformEqual(expected_combined_matrix, operations.Apply());
    }

    [Fact]
    private static void TestBlendOrder()
    {
        float sx1 = 2;
        float sy1 = 4;
        float sz1 = 8;

        float dx1 = 1;
        float dy1 = 2;
        float dz1 = 3;

        float sx2 = 4;
        float sy2 = 8;
        float sz2 = 16;

        float dx2 = 10;
        float dy2 = 20;
        float dz2 = 30;

        float sx3 = 2;
        float sy3 = 1;
        float sz3 = 1;

        TransformOperations operations_from = new();
        operations_from.AppendScale(sx1, sy1, sz1);
        operations_from.AppendTranslate(dx1, dy1, dz1);

        TransformOperations operations_to = new();
        operations_to.AppendScale(sx2, sy2, sz2);
        operations_to.AppendTranslate(dx2, dy2, dz2);

        Transform scale_from = new();
        scale_from.Scale3D(sx1, sy1, sz1);
        Transform translate_from = new();
        translate_from.Translate3D(dx1, dy1, dz1);

        Transform scale_to = new();
        scale_to.Scale3D(sx2, sy2, sz2);
        Transform translate_to = new();
        translate_to.Translate3D(dx2, dy2, dz2);

        float progress = 0.25f;

        TransformOperations operations_expected = new();
        operations_expected.AppendScale(
            Tween.FloatValueBetween(progress, sx1, sx2),
            Tween.FloatValueBetween(progress, sy1, sy2),
            Tween.FloatValueBetween(progress, sz1, sz2));

        operations_expected.AppendTranslate(
            Tween.FloatValueBetween(progress, dx1, dx2),
            Tween.FloatValueBetween(progress, dy1, dy2),
            Tween.FloatValueBetween(progress, dz1, dz2));

        Transform blended_scale = scale_to;
        blended_scale.Blend(scale_from, progress);

        Transform blended_translate = translate_to;
        blended_translate.Blend(translate_from, progress);

        Transform expected = blended_scale;
        expected.PreConcat(blended_translate);

        TransformOperations blended = operations_to.Blend(operations_from, progress);

        AssertTransformEqual(expected, blended.Apply());
        AssertTransformEqual(operations_expected.Apply(), blended.Apply());
        Assert.Equal(operations_expected.Size, blended.Size);
        for (int i = 0; i < operations_expected.Size; ++i)
        {
            TransformOperation expected_op = operations_expected.At(i);
            TransformOperation blended_op = blended.At(i);
            ExpectTransformOperationEqual(expected_op, blended_op);
        }

        // Note: In C++ = on a class-type calls the copy constructor — a deep copy.
        //
        //       TransformOperations base_operations_expected = operations_expected;
        //
        //       In C# we need to do this explicitly:
        TransformOperations base_operations_expected = new(operations_expected);

        // Create a mismatch in number of operations. Pairwise interpolation is still
        // used when the operations match up to the length of the shorter list.
        operations_to.AppendScale(sx3, sy3, sz3);

        Transform appended_scale = new();
        appended_scale.Scale3D(sx3, sy3, sz3);

        Transform blended_append_scale = appended_scale;
        blended_append_scale.Blend(new Transform(), progress);
        expected.PreConcat(blended_append_scale);

        operations_expected.AppendScale(
            Tween.FloatValueBetween(progress, 1, sx3),
            Tween.FloatValueBetween(progress, 1, sy3),
            Tween.FloatValueBetween(progress, 1, sz3));

        blended = operations_to.Blend(operations_from, progress);

        AssertTransformEqual(expected, blended.Apply());
        AssertTransformEqual(operations_expected.Apply(), blended.Apply());
        Assert.Equal(operations_expected.Size, blended.Size);
        for (int i = 0; i < operations_expected.Size; ++i)
        {
            TransformOperation expected_op = operations_expected.At(i);
            TransformOperation blended_op = blended.At(i);
            ExpectTransformOperationEqual(expected_op, blended_op);
        }

        // Create a mismatch, forcing matrix interpolation for the last operator pair.
        operations_from.AppendRotate(0, 0, 1, 90);

        blended = operations_to.Blend(operations_from, progress);

        Transform transform_from = new();
        transform_from.RotateAboutZAxis(90);
        Transform transform_to = new();
        transform_to.Scale3D(sx3, sy3, sz3);
        Transform blended_matrix = transform_to;
        blended_matrix.Blend(transform_from, progress);

        expected = blended_scale;
        expected.PreConcat(blended_translate);
        expected.PreConcat(blended_matrix);

        operations_expected = base_operations_expected;
        operations_expected.AppendMatrix(blended_matrix);

        AssertTransformEqual(expected, blended.Apply());
        AssertTransformEqual(operations_expected.Apply(), blended.Apply());
        Assert.Equal(operations_expected.Size, blended.Size);
        for (int i = 0; i < operations_expected.Size; ++i)
        {
            TransformOperation expected_op = operations_expected.At(i);
            TransformOperation blended_op = blended.At(i);
            ExpectTransformOperationEqual(expected_op, blended_op);
        }
    }
}
