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

        // In C++, assignments for both classes and structs create a copy by default (value semantics):
        //
        // TransformOperations base_operations_expected = operations_expected;
        //
        // In C#, the behavior is fundamentally different, assignments for classes copy the reference:
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

        operations_expected = new(base_operations_expected);
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

    private static void CheckProgress(float progress, in Transform from_matrix, in Transform to_matrix, in TransformOperations from_transform, in TransformOperations to_transform)
    {
        Transform expected_matrix = to_matrix;
        expected_matrix.Blend(from_matrix, progress);
        AssertTransformEqual(expected_matrix, to_transform.Blend(from_transform, progress).Apply());
    }

    [Fact]
    private static void TestBlendProgress()
    {
        float sx = 2;
        float sy = 4;
        float sz = 8;
        TransformOperations operations_from = new();
        operations_from.AppendScale(sx, sy, sz);

        Transform matrix_from = new();
        matrix_from.Scale3D(sx, sy, sz);

        sx = 4;
        sy = 8;
        sz = 16;
        TransformOperations operations_to = new();
        operations_to.AppendScale(sx, sy, sz);

        Transform matrix_to = new();
        matrix_to.Scale3D(sx, sy, sz);

        CheckProgress(-1, matrix_from, matrix_to, operations_from, operations_to);
        CheckProgress(0, matrix_from, matrix_to, operations_from, operations_to);
        CheckProgress(0.25f, matrix_from, matrix_to, operations_from, operations_to);
        CheckProgress(0.5f, matrix_from, matrix_to, operations_from, operations_to);
        CheckProgress(1, matrix_from, matrix_to, operations_from, operations_to);
        CheckProgress(2, matrix_from, matrix_to, operations_from, operations_to);
    }

    [Fact]
    private static void TestBlendWhenTypesDoNotMatch()
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

        TransformOperations operations_from = new();
        operations_from.AppendScale(sx1, sy1, sz1);
        operations_from.AppendTranslate(dx1, dy1, dz1);

        TransformOperations operations_to = new();
        operations_to.AppendTranslate(dx2, dy2, dz2);
        operations_to.AppendScale(sx2, sy2, sz2);

        Transform from = new();
        from.Scale3D(sx1, sy1, sz1);
        from.Translate3D(dx1, dy1, dz1);

        Transform to = new();
        to.Translate3D(dx2, dy2, dz2);
        to.Scale3D(sx2, sy2, sz2);

        float progress = 0.25f;

        Transform expected = to;
        expected.Blend(from, progress);

        AssertTransformEqual(expected, operations_to.Blend(operations_from, progress).Apply());
    }

    [Fact]
    private static void TestLargeRotationsWithSameAxis()
    {
        TransformOperations operations_from = new();
        operations_from.AppendRotate(0, 0, 1, 0);

        TransformOperations operations_to = new();
        operations_to.AppendRotate(0, 0, 2, 360);

        float progress = 0.5f;

        Transform expected = new();
        expected.RotateAbout(new Vector3DF(0, 0, 1), 180);

        AssertTransformEqual(expected, operations_to.Blend(operations_from, progress).Apply());
    }

    [Fact]
    private static void TestLargeRotationsWithSameAxisInDifferentDirection()
    {
        TransformOperations operations_from = new();
        operations_from.AppendRotate(0, 0, 1, 180);

        TransformOperations operations_to = new();
        operations_to.AppendRotate(0, 0, -1, 180);

        float progress = 0.5f;

        Transform expected = new();

        AssertTransformEqual(expected,
                            operations_to.Blend(operations_from, progress).Apply());
    }

    [Fact]
    private static void TestLargeRotationsWithDifferentAxes()
    {
        TransformOperations operations_from = new();
        operations_from.AppendRotate(0, 0, 1, 175);

        TransformOperations operations_to = new();
        operations_to.AppendRotate(0, 1, 0, 175);

        float progress = 0.5f;
        Transform matrix_from = new();
        matrix_from.RotateAbout(new Vector3DF(0, 0, 1), 175);

        Transform matrix_to = new();
        matrix_to.RotateAbout(new Vector3DF(0, 1, 0), 175);

        Transform expected = matrix_to;
        expected.Blend(matrix_from, progress);

        AssertTransformEqual(expected, operations_to.Blend(operations_from, progress).Apply());
    }

    [Fact]
    private static void TestRotationFromZeroDegDifferentAxes()
    {
        TransformOperations operations_from = new();
        operations_from.AppendRotate(0, 0, 1, 0);

        TransformOperations operations_to = new();
        operations_to.AppendRotate(0, 1, 0, 450);

        float progress = 0.5f;
        Transform expected = new();
        expected.RotateAbout(new Vector3DF(0, 1, 0), 225);
        AssertTransformEqual(expected, operations_to.Blend(operations_from, progress).Apply());
    }

    [Fact]
    private static void TestRotationFromZeroDegSameAxes()
    {
        TransformOperations operations_from = new();
        operations_from.AppendRotate(0, 0, 1, 0);

        TransformOperations operations_to = new();
        operations_to.AppendRotate(0, 0, 1, 450);

        float progress = 0.5f;
        Transform expected = new();
        expected.RotateAbout(new Vector3DF(0, 0, 1), 225);
        AssertTransformEqual(expected, operations_to.Blend(operations_from, progress).Apply());
    }

    [Fact]
    private static void TestRotationToZeroDegDifferentAxes()
    {
        TransformOperations operations_from = new();
        operations_from.AppendRotate(0, 1, 0, 450);

        TransformOperations operations_to = new();
        operations_to.AppendRotate(0, 0, 1, 0);

        float progress = 0.5f;
        Transform expected = new();
        expected.RotateAbout(new Vector3DF(0, 1, 0), 225);
        AssertTransformEqual(expected, operations_to.Blend(operations_from, progress).Apply());
    }

    [Fact]
    private static void TestRotationToZeroDegSameAxes()
    {
        TransformOperations operations_from = new();
        operations_from.AppendRotate(0, 0, 1, 450);

        TransformOperations operations_to = new();
        operations_to.AppendRotate(0, 0, 1, 0);

        float progress = 0.5f;
        Transform expected = new();
        expected.RotateAbout(new Vector3DF(0, 0, 1), 225);
        AssertTransformEqual(expected, operations_to.Blend(operations_from, progress).Apply());
    }

    [Fact]
    private static void TestBlendRotationFromIdentity()
    {
        List<TransformOperations> identity_operations = GetIdentityOperations();

        foreach (var identity_operation in identity_operations)
        {
            TransformOperations operations = new();
            operations.AppendRotate(0, 0, 1, 90);

            float progress = 0.5f;

            Transform expected = new();
            expected.RotateAbout(new Vector3DF(0, 0, 1), 45);

            AssertTransformEqual(expected, operations.Blend(identity_operation, progress).Apply());

            progress = -0.5f;

            expected.MakeIdentity();
            expected.RotateAbout(new Vector3DF(0, 0, 1), -45);

            AssertTransformEqual(expected, operations.Blend(identity_operation, progress).Apply());

            progress = 1.5f;

            expected.MakeIdentity();
            expected.RotateAbout(new Vector3DF(0, 0, 1), 135);

            AssertTransformEqual(expected, operations.Blend(identity_operation, progress).Apply());
        }
    }

    [Fact]
    private static void TestBlendTranslationFromIdentity()
    {
        List<TransformOperations> identity_operations = GetIdentityOperations();

        foreach (var identity_operation in identity_operations)
        {
            TransformOperations operations = new();
            operations.AppendTranslate(2, 2, 2);

            float progress = 0.5f;

            Transform expected = new();
            expected.Translate3D(1, 1, 1);

            AssertTransformEqual(
                expected, operations.Blend(identity_operation, progress).Apply());

            progress = -0.5f;

            expected.MakeIdentity();
            expected.Translate3D(-1, -1, -1);

            AssertTransformEqual(
                expected, operations.Blend(identity_operation, progress).Apply());

            progress = 1.5f;

            expected.MakeIdentity();
            expected.Translate3D(3, 3, 3);

            AssertTransformEqual(
                expected, operations.Blend(identity_operation, progress).Apply());
        }
    }

    [Fact]
    private static void TestBlendScaleFromIdentity()
    {
        List<TransformOperations> identity_operations = GetIdentityOperations();

        foreach (var identity_operation in identity_operations)
        {
            TransformOperations operations = new();
            operations.AppendScale(3, 3, 3);

            float progress = 0.5f;

            Transform expected = new();
            expected.Scale3D(2, 2, 2);

            AssertTransformEqual(
                expected, operations.Blend(identity_operation, progress).Apply());

            progress = -0.5f;

            expected.MakeIdentity();
            expected.Scale3D(0, 0, 0);

            AssertTransformEqual(
                expected, operations.Blend(identity_operation, progress).Apply());

            progress = 1.5f;

            expected.MakeIdentity();
            expected.Scale3D(4, 4, 4);

            AssertTransformEqual(
                expected, operations.Blend(identity_operation, progress).Apply());
        }
    }

    [Fact]
    private static void TestBlendSkewFromEmpty()
    {
        TransformOperations empty_operation = new();

        TransformOperations operations = new();
        operations.AppendSkew(2, 2);

        float progress = 0.5f;

        Transform expected = new();
        expected.Skew(1, 1);

        AssertTransformEqual(expected, operations.Blend(empty_operation, progress).Apply());

        progress = -0.5f;

        expected.MakeIdentity();
        expected.Skew(-1, -1);

        AssertTransformEqual(expected, operations.Blend(empty_operation, progress).Apply());

        progress = 1.5f;

        expected.MakeIdentity();
        expected.Skew(3, 3);

        AssertTransformEqual(expected, operations.Blend(empty_operation, progress).Apply());
    }

    [Fact]
    private static void TestBlendPerspectiveFromIdentity()
    {
        List<TransformOperations> identity_operations = GetIdentityOperations();

        foreach (var identity_operation in identity_operations)
        {
            TransformOperations operations = new();
            operations.AppendPerspective(1000);

            float progress = 0.5f;

            Transform expected = new();
            expected.ApplyPerspectiveDepth(2000);

            AssertTransformEqual(expected, operations.Blend(identity_operation, progress).Apply());
        }
    }

    [Fact]
    private static void TestBlendRotationToIdentity()
    {
        List<TransformOperations> identity_operations =
      GetIdentityOperations();

        foreach (var identity_operation in identity_operations)
        {
            TransformOperations operations = new();
            operations.AppendRotate(0, 0, 1, 90);

            float progress = 0.5f;

            Transform expected = new();
            expected.RotateAbout(new Vector3DF(0, 0, 1), 45);

            AssertTransformEqual(expected, identity_operation.Blend(operations, progress).Apply());
        }
    }

    [Fact]
    private static void TestBlendTranslationToIdentity()
    {
        List<TransformOperations> identity_operations = GetIdentityOperations();

        foreach (var identity_operation in identity_operations)
        {
            TransformOperations operations = new();
            operations.AppendTranslate(2, 2, 2);

            float progress = 0.5f;

            Transform expected = new();
            expected.Translate3D(1, 1, 1);

            AssertTransformEqual(expected, identity_operation.Blend(operations, progress).Apply());
        }
    }

    [Fact]
    private static void TestBlendScaleToIdentity()
    {
        List<TransformOperations> identity_operations =
      GetIdentityOperations();

        foreach (var identity_operation in identity_operations)
        {
            TransformOperations operations = new();
            operations.AppendScale(3, 3, 3);

            float progress = 0.5f;

            Transform expected = new();
            expected.Scale3D(2, 2, 2);

            AssertTransformEqual(expected, identity_operation.Blend(operations, progress).Apply());
        }
    }

    [Fact]
    private static void TestBlendSkewToEmpty()
    {
        TransformOperations empty_operation = new();

        TransformOperations operations = new();
        operations.AppendSkew(2, 2);

        float progress = 0.5f;

        Transform expected = new();
        expected.Skew(1, 1);

        AssertTransformEqual(expected, empty_operation.Blend(operations, progress).Apply());
    }

    [Fact]
    private static void TestBlendPerspectiveToIdentity()
    {
        List<TransformOperations> identity_operations = GetIdentityOperations();

        foreach (var identity_operation in identity_operations)
        {
            TransformOperations operations = new();
            operations.AppendPerspective(1000);

            float progress = 0.5f;

            Transform expected = new();
            expected.ApplyPerspectiveDepth(2000);

            AssertTransformEqual(expected, identity_operation.Blend(operations, progress).Apply());
        }
    }

    [Fact]
    private static void TestExtrapolatePerspectiveBlending()
    {
        TransformOperations operations1 = new();
        operations1.AppendPerspective(1000);

        TransformOperations operations2 = new();
        operations2.AppendPerspective(500);

        Transform expected = new();
        expected.ApplyPerspectiveDepth(400);

        AssertTransformEqual(expected, operations1.Blend(operations2, -0.5f).Apply());

        expected.MakeIdentity();
        expected.ApplyPerspectiveDepth(2000);

        AssertTransformEqual(expected, operations1.Blend(operations2, 1.5f).Apply());
    }

    [Fact]
    private static void TestExtrapolateMatrixBlending()
    {
        Transform transform1 = new();
        transform1.Translate3D(1, 1, 1);
        TransformOperations operations1 = new();
        operations1.AppendMatrix(transform1);

        Transform transform2 = new();
        transform2.Translate3D(3, 3, 3);
        TransformOperations operations2 = new();
        operations2.AppendMatrix(transform2);

        Transform expected = new();
        AssertTransformEqual(expected, operations1.Blend(operations2, 1.5f).Apply());

        expected.Translate3D(4, 4, 4);
        AssertTransformEqual(expected, operations1.Blend(operations2, -0.5f).Apply());
    }

    [Fact]
    private static void TestNonDecomposableBlend()
    {
        TransformOperations non_decomposible_transform = new();
        Transform non_decomposible_matrix = Transform.MakeScale(0);
        non_decomposible_transform.AppendMatrix(non_decomposible_matrix);

        TransformOperations identity_transform = new();
        Transform identity_matrix = new();
        identity_transform.AppendMatrix(identity_matrix);

        // Before the half-way point, we should return the 'from' matrix.
        AssertTransformEqual(
            non_decomposible_matrix,
            identity_transform.Blend(non_decomposible_transform, 0.0f).Apply());
        AssertTransformEqual(
            non_decomposible_matrix,
            identity_transform.Blend(non_decomposible_transform, 0.49f).Apply());

        // After the half-way point, we should return the 'to' matrix.
        AssertTransformEqual(
            identity_matrix,
            identity_transform.Blend(non_decomposible_transform, 0.5f).Apply());
        AssertTransformEqual(
            identity_matrix,
            identity_transform.Blend(non_decomposible_transform, 1.0f).Apply());
    }

    [Fact]
    private static void TestBlendedBoundsWhenTypesDoNotMatch()
    {
        TransformOperations operations_from = new();
        operations_from.AppendScale(2.0f, 4.0f, 8.0f);
        operations_from.AppendTranslate(1.0f, 2.0f, 3.0f);

        TransformOperations operations_to = new();
        operations_to.AppendTranslate(10.0f, 20.0f, 30.0f);
        operations_to.AppendScale(4.0f, 8.0f, 16.0f);

        BoxF box = new(1.0f, 1.0f, 1.0f);
        BoxF bounds;

        float min_progress = 0.0f;
        float max_progress = 1.0f;

        Assert.False(operations_to.BlendedBoundsForBox(box, operations_from, min_progress, max_progress, out bounds));
    }

    [Fact]
    private static void TestBlendedBoundsForIdentity()
    {
        TransformOperations operations_from = new();
        operations_from.AppendIdentity();
        TransformOperations operations_to = new();
        operations_to.AppendIdentity();

        BoxF box = new(1.0f, 2.0f, 3.0f);
        BoxF bounds;

        float min_progress = 0.0f;
        float max_progress = 1.0f;

        Assert.True(operations_to.BlendedBoundsForBox(box, operations_from, min_progress, max_progress, out bounds));
        Assert.Equal(box.ToString(), bounds.ToString());
    }

    [Fact]
    private static void TestBlendedBoundsForTranslate()
    {
        TransformOperations operations_from = new();
        operations_from.AppendTranslate(3.0f, -4.0f, 2.0f);
        TransformOperations operations_to = new();
        operations_to.AppendTranslate(7.0f, 4.0f, -2.0f);

        BoxF box = new(1.0f, 2.0f, 3.0f, 4.0f, 4.0f, 4.0f);
        BoxF bounds;

        float min_progress = -0.5f;
        float max_progress = 1.5f;
        Assert.True(operations_to.BlendedBoundsForBox(box, operations_from, min_progress, max_progress, out bounds));
        Assert.Equal(new BoxF(2.0f, -6.0f, -1.0f, 12.0f, 20.0f, 12.0f).ToString(), bounds.ToString());

        min_progress = 0.0f;
        max_progress = 1.0f;
        Assert.True(operations_to.BlendedBoundsForBox(box, operations_from, min_progress, max_progress, out bounds));
        Assert.Equal(new BoxF(4.0f, -2.0f, 1.0f, 8.0f, 12.0f, 8.0f).ToString(), bounds.ToString());

        TransformOperations identity = new();
        Assert.True(operations_to.BlendedBoundsForBox(box, identity, min_progress, max_progress, out bounds));
        Assert.Equal(new BoxF(1.0f, 2.0f, 1.0f, 11.0f, 8.0f, 6.0f).ToString(), bounds.ToString());

        Assert.True(identity.BlendedBoundsForBox(box, operations_from, min_progress, max_progress, out bounds));
        Assert.Equal(new BoxF(1.0f, -2.0f, 3.0f, 7.0f, 8.0f, 6.0f).ToString(), bounds.ToString());
    }

    [Fact]
    private static void TestBlendedBoundsForScale()
    {
        TransformOperations operations_from = new();
        operations_from.AppendScale(3.0f, 0.5f, 2.0f);
        TransformOperations operations_to = new();
        operations_to.AppendScale(7.0f, 4.0f, -2.0f);

        BoxF box = new(1.0f, 2.0f, 3.0f, 4.0f, 4.0f, 4.0f);
        BoxF bounds;

        float min_progress = -0.5f;
        float max_progress = 1.5f;
        Assert.True(operations_to.BlendedBoundsForBox(box, operations_from, min_progress, max_progress, out bounds));
        Assert.Equal(new BoxF(1.0f, -7.5f, -28.0f, 44.0f, 42.0f, 56.0f).ToString(), bounds.ToString());

        min_progress = 0.0f;
        max_progress = 1.0f;
        Assert.True(operations_to.BlendedBoundsForBox(box, operations_from, min_progress, max_progress, out bounds));
        Assert.Equal(new BoxF(3.0f, 1.0f, -14.0f, 32.0f, 23.0f, 28.0f).ToString(), bounds.ToString());

        TransformOperations identity = new();
        Assert.True(operations_to.BlendedBoundsForBox(box, identity, min_progress, max_progress, out bounds));
        Assert.Equal(new BoxF(1.0f, 2.0f, -14.0f, 34.0f, 22.0f, 21.0f).ToString(), bounds.ToString());

        Assert.True(identity.BlendedBoundsForBox(box, operations_from, min_progress, max_progress, out bounds));
        Assert.Equal(new BoxF(1.0f, 1.0f, 3.0f, 14.0f, 5.0f, 11.0f).ToString(), bounds.ToString());
    }

    [Fact]
    private static void TestBlendedBoundsWithZeroScale()
    {
        TransformOperations zero_scale = new();
        zero_scale.AppendScale(0.0f, 0.0f, 0.0f);
        TransformOperations non_zero_scale = new();
        non_zero_scale.AppendScale(2.0f, -4.0f, 5.0f);

        BoxF box = new(1.0f, 2.0f, 3.0f, 4.0f, 4.0f, 4.0f);
        BoxF bounds;

        float min_progress = 0.0f;
        float max_progress = 1.0f;
        Assert.True(zero_scale.BlendedBoundsForBox(box, non_zero_scale, min_progress, max_progress, out bounds));
        Assert.Equal(new BoxF(0.0f, -24.0f, 0.0f, 10.0f, 24.0f, 35.0f).ToString(), bounds.ToString());

        Assert.True(non_zero_scale.BlendedBoundsForBox(box, zero_scale, min_progress, max_progress, out bounds));
        Assert.Equal(new BoxF(0.0f, -24.0f, 0.0f, 10.0f, 24.0f, 35.0f).ToString(), bounds.ToString());

        Assert.True(zero_scale.BlendedBoundsForBox(box, zero_scale, min_progress, max_progress, out bounds));
        Assert.Equal(new BoxF().ToString(), bounds.ToString());
    }

    [Fact]
    private static void TestBlendedBoundsForRotationTrivial()
    {
        TransformOperations operations_from = new();
        operations_from.AppendRotate(0.0f, 0.0f, 1.0f, 0.0f);
        TransformOperations operations_to = new();
        operations_to.AppendRotate(0.0f, 0.0f, 1.0f, 360.0f);

        float sqrt_2 = MathF.Sqrt(2.0f);
        BoxF box = new(-sqrt_2, -sqrt_2, 0.0f, sqrt_2, sqrt_2, 0.0f);
        BoxF bounds;

        // Since we're rotating 360 degrees,
        // any box with dimensions between 0 and 2 * sqrt(2) should give the same result.
        float[] sizes = [0.0f, 0.1f, sqrt_2, 2.0f * sqrt_2];

        foreach (float size in sizes)
        {
            box.SetSize(size, size, 0.0f);
            float min_progress = 0.0f;
            float max_progress = 1.0f;
            Assert.True(operations_to.BlendedBoundsForBox(box, operations_from, min_progress, max_progress, out bounds));
            Assert.Equal(new BoxF(-2.0f, -2.0f, 0.0f, 4.0f, 4.0f, 0.0f).ToString(), bounds.ToString());
        }
    }

    [Fact]
    private static void TestBlendedBoundsForRotationAllExtrema()
    {
        // If the normal is out of the plane, we can have up to 6 extrema (a min/max
        // in each dimension) between the endpoints of the arc. This test ensures that
        // we consider all 6.
        TransformOperations operations_from = new();
        operations_from.AppendRotate(1.0f, 1.0f, 1.0f, 30.0f);
        TransformOperations operations_to = new();
        operations_to.AppendRotate(1.0f, 1.0f, 1.0f, 390.0f);

        BoxF box = new(1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
        BoxF bounds;

        float min = -1.0f / 3.0f;
        float max = 1.0f;
        float size = max - min;
        Assert.True(operations_to.BlendedBoundsForBox(box, operations_from, 0.0f, 1.0f, out bounds));
        string a = new BoxF(min, min, min, size, size, size).ToString();
        string b = bounds.ToString();
        Assert.Equal(a, b);
    }

    [Fact]
    private static void TestBlendedBoundsForRotationDifferentAxes()
    {
        // We can handle rotations about a single axis. If the axes are different,
        // we revert to matrix interpolation for which inflated bounds cannot be
        // computed.
        TransformOperations operations_from = new();
        operations_from.AppendRotate(1.0f, 1.0f, 1.0f, 30.0f);
        TransformOperations operations_to_same = new();
        operations_to_same.AppendRotate(1.0f, 1.0f, 1.0f, 390.0f);
        TransformOperations operations_to_opposite = new();
        operations_to_opposite.AppendRotate(-1.0f, -1.0f, -1.0f, 390.0f);
        TransformOperations operations_to_different = new();
        operations_to_different.AppendRotate(1.0f, 3.0f, 1.0f, 390.0f);

        BoxF box = new(1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
        BoxF bounds;

        Assert.True(operations_to_same.BlendedBoundsForBox(box, operations_from, 0.0f, 1.0f, out bounds));
        Assert.True(operations_to_opposite.BlendedBoundsForBox(box, operations_from, 0.0f, 1.0f, out bounds));
        Assert.False(operations_to_different.BlendedBoundsForBox(box, operations_from, 0.0f, 1.0f, out bounds));
    }

    [Fact]
    private static void TestBlendedBoundsForRotationPointOnAxis()
    {
        // Checks that if the point to rotate is sitting on the axis of rotation, that
        // it does not get affected.
        TransformOperations operations_from = new();
        operations_from.AppendRotate(1.0f, 1.0f, 1.0f, 30.0f);
        TransformOperations operations_to = new();
        operations_to.AppendRotate(1.0f, 1.0f, 1.0f, 390.0f);

        BoxF box = new(1.0f, 1.0f, 1.0f, 0.0f, 0.0f, 0.0f);
        BoxF bounds;

        Assert.True(operations_to.BlendedBoundsForBox(box, operations_from, 0.0f, 1.0f, out bounds));
        Assert.Equal(box.ToString(), bounds.ToString());
    }

    [Fact]
    private static void TestBlendedBoundsForRotationProblematicAxes()
    {
        // Zeros in the components of the axis of rotation
        // turned out to be tricky to deal with in practice.
        // This function tests some potentially problematic axes to ensure sane behavior.

        // Some common values used in the expected boxes.
        float dim1 = 0.292893f;
        float dim2 = MathF.Sqrt(2.0f);
        float dim3 = 2.0f * dim2;

        (float x, float y, float z, BoxF expected)[] tests =
        [
            (0.0f, 0.0f, 0.0f, new BoxF(1.0f, 1.0f, 1.0f, 0.0f, 0.0f, 0.0f)),
            (1.0f, 0.0f, 0.0f, new BoxF(1.0f, -dim2, -dim2, 0.0f, dim3, dim3)),
            (0.0f, 1.0f, 0.0f, new BoxF(-dim2, 1.0f, -dim2, dim3, 0.0f, dim3)),
            (0.0f, 0.0f, 1.0f, new BoxF(-dim2, -dim2, 1.0f, dim3, dim3, 0.0f)),
            (1.0f, 1.0f, 0.0f, new BoxF(dim1, dim1, -1.0f, dim2, dim2, 2.0f)),
            (0.0f, 1.0f, 1.0f, new BoxF(-1.0f, dim1, dim1, 2.0f, dim2, dim2)),
            (1.0f, 0.0f, 1.0f, new BoxF(dim1, -1.0f, dim1, dim2, 2.0f, dim2))
        ];

        foreach (var test in tests)
        {
            float x = test.x;
            float y = test.y;
            float z = test.z;
            TransformOperations operations_from = new();
            operations_from.AppendRotate(x, y, z, 0.0f);
            TransformOperations operations_to = new();
            operations_to.AppendRotate(x, y, z, 360.0f);
            BoxF box = new(1.0f, 1.0f, 1.0f, 0.0f, 0.0f, 0.0f);
            BoxF bounds;

            Assert.True(operations_to.BlendedBoundsForBox(box, operations_from, 0.0f, 1.0f, out bounds));
            Assert.Equal(test.expected.ToString(), bounds.ToString());
        }
    }

    private static void ExpectBoxesApproximatelyEqual(in BoxF lhs, in BoxF rhs, float tolerance)
    {
        Assert.Equal(lhs.X, rhs.X, tolerance);
        Assert.Equal(lhs.Y, rhs.Y, tolerance);
        Assert.Equal(lhs.Z, rhs.Z, tolerance);
        Assert.Equal(lhs.Width, rhs.Width, tolerance);
        Assert.Equal(lhs.Height, rhs.Height, tolerance);
        Assert.Equal(lhs.Depth, rhs.Depth, tolerance);
    }

    private static void EmpiricallyTestBounds(in TransformOperations from, in TransformOperations to, float min_progress, float max_progress, bool test_containment_only)
    {
        BoxF box = new(200.0f, 500.0f, 100.0f, 100.0f, 300.0f, 200.0f);
        BoxF bounds;
        Assert.True(to.BlendedBoundsForBox(box, from, min_progress, max_progress, out bounds));

        bool first_time = true;
        BoxF empirical_bounds = new();
        const int kNumSteps = 10;
        for (int step = 0; step < kNumSteps; ++step)
        {
            float t = step / (kNumSteps - 1.0f);
            t = Tween.FloatValueBetween(t, min_progress, max_progress);
            Transform partial_transform = to.Blend(from, t).Apply();
            BoxF transformed = partial_transform.MapBox(box);

            if (first_time)
            {
                empirical_bounds = transformed;
                first_time = false;
            }
            else
            {
                empirical_bounds.Union(transformed);
            }
        }

        if (test_containment_only)
        {
            BoxF unified_bounds = bounds;
            unified_bounds.Union(empirical_bounds);
            // Convert to the screen space rects these boxes represent.
            Rect bounds_rect = RectConversions.ToEnclosingRect(new RectF(bounds.X, bounds.Y, bounds.Width, bounds.Height));
            Rect unified_bounds_rect = RectConversions.ToEnclosingRect(new RectF(unified_bounds.X, unified_bounds.Y, unified_bounds.Width, unified_bounds.Height));
            Assert.Equal(bounds_rect.ToString(), unified_bounds_rect.ToString());
        }
        else
        {
            // Our empirical estimate will be a little rough since we're only doing
            // 100 samples.
            const float kTolerance = 1e-2f;
            ExpectBoxesApproximatelyEqual(empirical_bounds, bounds, kTolerance);
        }
    }

    private static void EmpiricallyTestBoundsEquality(in TransformOperations from, in TransformOperations to, float min_progress, float max_progress)
    {
        EmpiricallyTestBounds(from, to, min_progress, max_progress, false);
    }

    private static void EmpiricallyTestBoundsContainment(in TransformOperations from, in TransformOperations to, float min_progress, float max_progress)
    {
        EmpiricallyTestBounds(from, to, min_progress, max_progress, true);
    }

    [Fact]
    private static void TestBlendedBoundsForRotationEmpiricalTests()
    {
        // Sets up various axis angle combinations, computes the bounding box 
        // and empirically tests that the transformed bounds are indeed contained
        // by the computed bounding box.

        (float x, float y, float z)[] axes =
        [
            (1.0f, 1.0f, 1.0f),   (-1.0f, -1.0f, -1.0f), (-1.0f, 2.0f, 3.0f),
            (1.0f, -2.0f, 3.0f),  (1.0f, 2.0f, -3.0f),   (0.0f, 0.0f, 0.0f),
            (1.0f, 0.0f, 0.0f),   (0.0f, 1.0f, 0.0f),    (0.0f, 0.0f, 1.0f),
            (1.0f, 1.0f, 0.0f),   (0.0f, 1.0f, 1.0f),    (1.0f, 0.0f, 1.0f),
            (-1.0f, 0.0f, 0.0f),  (0.0f, -1.0f, 0.0f),   (0.0f, 0.0f, -1.0f),
            (-1.0f, -1.0f, 0.0f), (0.0f, -1.0f, -1.0f),  (-1.0f, 0.0f, -1.0f)
        ];

        (float theta_from, float theta_to)[] angles =
        [
            (5.0f, 10.0f),
            (10.0f, 5.0f),
            (0.0f, 360.0f),
            (20.0f, 180.0f),
            (-20.0f, -180.0f),
            (180.0f, -220.0f),
            (220.0f, 320.0f)
        ];

        // We can go beyond the range [0, 1] (the bezier might slide out of this range
        // at either end), but since the first and last knots are at (0, 0) and (1, 1)
        // we will never go within it, so these tests are sufficient.
        (float min_progress, float max_progress)[] progresses =
        [
            (0.0f, 1.0f),
            (-.25f, 1.25f)
        ];

        foreach (var axis in axes)
        {
            foreach (var angle in angles)
            {
                foreach (var progress in progresses)
                {
                    float x = axis.x;
                    float y = axis.y;
                    float z = axis.z;
                    TransformOperations operations_from = new();
                    operations_from.AppendRotate(x, y, z, angle.theta_from);
                    TransformOperations operations_to = new();
                    operations_to.AppendRotate(x, y, z, angle.theta_to);
                    EmpiricallyTestBoundsContainment(operations_from, operations_to,
                                         progress.min_progress,
                                         progress.max_progress);
                }
            }
        }
    }

    [Fact]
    private static void TestPerspectiveMatrixAndTransformBlendingEquivalency()
    {
        TransformOperations from_operations = new();
        from_operations.AppendPerspective(200);

        TransformOperations to_operations = new();
        to_operations.AppendPerspective(1000);

        Transform from_transform = new();
        from_transform.ApplyPerspectiveDepth(200);

        Transform to_transform = new();
        to_transform.ApplyPerspectiveDepth(1000);

        const int steps = 20;
        for (int i = 0; i < steps; ++i)
        {
            double progress = (double)i / (steps - 1);

            Transform blended_matrix = to_transform;
            Assert.True(blended_matrix.Blend(from_transform, progress));

            Transform blended_transform = to_operations.Blend(from_operations, (float)progress).Apply();

            AssertTransformEqual(blended_matrix, blended_transform);
        }
    }

    [Fact]
    private static void TestBlendedBoundsForPerspective()
    {
        (float from_depth, float to_depth)[] perspective_depths =
        [
            (600.0f, 400.0f),
            (800.0f, 1000.0f),
            (800.0f, float.PositiveInfinity)
        ];

        (float min_progress, float max_progress)[] progresses =
        [
            (0.0f, 1.0f),
            (-0.1f, 1.1f)
        ];

        foreach (var perspective_depth in perspective_depths)
        {
            foreach (var progress in progresses)
            {
                TransformOperations operations_from = new();
                operations_from.AppendPerspective(perspective_depth.from_depth);
                TransformOperations operations_to = new();
                operations_to.AppendPerspective(perspective_depth.to_depth);
                EmpiricallyTestBoundsEquality(operations_from, operations_to, progress.min_progress, progress.max_progress);
            }
        }
    }

    [Fact]
    private static void TestBlendedBoundsForSkew()
    {
        (float from_x, float from_y, float to_x, float to_y)[] skews =
        [
            (1.0f, 0.5f, 0.5f, 1.0f),
            (2.0f, 1.0f, 0.5f, 0.5f)
        ];

        (float min_progress, float max_progress)[] progresses =
        [
            (0.0f, 1.0f),
            (-0.1f, 1.1f)
        ];

        foreach (var skew in skews)
        {
            foreach (var progress in progresses)
            {
                TransformOperations operations_from = new();
                operations_from.AppendSkew(skew.from_x, skew.from_y);
                TransformOperations operations_to = new();
                operations_to.AppendSkew(skew.to_x, skew.to_y);
                EmpiricallyTestBoundsEquality(operations_from, operations_to,
                                              progress.min_progress,
                                              progress.max_progress);
            }
        }
    }

    [Fact]
    private static void TestNonCommutativeRotations()
    {
        TransformOperations operations_from = new();
        operations_from.AppendRotate(1.0f, 0.0f, 0.0f, 0.0f);
        operations_from.AppendRotate(0.0f, 1.0f, 0.0f, 0.0f);
        TransformOperations operations_to = new();
        operations_to.AppendRotate(1.0f, 0.0f, 0.0f, 45.0f);
        operations_to.AppendRotate(0.0f, 1.0f, 0.0f, 135.0f);

        BoxF box = new(0f, 0f, 0f, 1f, 1f, 1f);
        BoxF bounds;

        float min_progress = 0.0f;
        float max_progress = 1.0f;
        Assert.True(operations_to.BlendedBoundsForBox(
            box, operations_from, min_progress, max_progress, out bounds));
        Transform blended_transform =
            operations_to.Blend(operations_from, max_progress).Apply();
        Point3F blended_point = new(0.9f, 0.9f, 0.0f);
        blended_point = blended_transform.MapPoint(blended_point);
        BoxF expanded_bounds = bounds;
        expanded_bounds.ExpandTo(blended_point);
        Assert.Equal(bounds.ToString(), expanded_bounds.ToString());
    }

    [Fact]
    private static void TestBlendedBoundsForSequence()
    {
        TransformOperations operations_from = new();
        operations_from.AppendTranslate(1.0f, -5.0f, 1.0f);
        operations_from.AppendScale(-1.0f, 2.0f, 3.0f);
        operations_from.AppendTranslate(2.0f, 4.0f, -1.0f);
        TransformOperations operations_to = new();
        operations_to.AppendTranslate(13.0f, -1.0f, 5.0f);
        operations_to.AppendScale(-3.0f, -2.0f, 5.0f);
        operations_to.AppendTranslate(6.0f, -2.0f, 3.0f);

        BoxF box = new(1.0f, 2.0f, 3.0f, 4.0f, 4.0f, 4.0f);
        BoxF bounds;

        float min_progress = -0.5f;
        float max_progress = 1.5f;
        Assert.True(operations_to.BlendedBoundsForBox(box, operations_from, min_progress, max_progress, out bounds));
        Assert.Equal(new BoxF(-57.0f, -59.0f, -1.0f, 76.0f, 112.0f, 80.0f).ToString(), bounds.ToString());

        min_progress = 0.0f;
        max_progress = 1.0f;
        Assert.True(operations_to.BlendedBoundsForBox(box, operations_from, min_progress, max_progress, out bounds));
        Assert.Equal(new BoxF(-32.0f, -25.0f, 7.0f, 42.0f, 44.0f, 48.0f).ToString(), bounds.ToString());

        TransformOperations identity = new();
        Assert.True(operations_to.BlendedBoundsForBox(box, identity, min_progress, max_progress, out bounds));
        Assert.Equal(new BoxF(-33.0f, -13.0f, 3.0f, 57.0f, 19.0f, 52.0f).ToString(),bounds.ToString());

        Assert.True(identity.BlendedBoundsForBox(box, operations_from, min_progress, max_progress, out bounds));
        Assert.Equal(new BoxF(-7.0f, -3.0f, 2.0f, 15.0f, 23.0f, 20.0f).ToString(), bounds.ToString());
    }

    [Fact]
    private static void TestIsTranslationWithSingleOperation()
    {
        TransformOperations empty_operations = new();
        Assert.True(empty_operations.IsTranslation());

        TransformOperations identity = new();
        identity.AppendIdentity();
        Assert.True(identity.IsTranslation());

        TransformOperations translate = new();
        translate.AppendTranslate(1.0f, 2.0f, 3.0f);
        Assert.True(translate.IsTranslation());

        TransformOperations rotate = new();
        rotate.AppendRotate(1.0f, 2.0f, 3.0f, 4.0f);
        Assert.False(rotate.IsTranslation());

        TransformOperations scale = new();
        scale.AppendScale(1.0f, 2.0f, 3.0f);
        Assert.False(scale.IsTranslation());

        TransformOperations skew = new();
        skew.AppendSkew(1.0f, 2.0f);
        Assert.False(skew.IsTranslation());

        TransformOperations perspective = new();
        perspective.AppendPerspective(1.0f);
        Assert.False(perspective.IsTranslation());

        TransformOperations identity_matrix = new();
        identity_matrix.AppendMatrix(new Transform());
        Assert.True(identity_matrix.IsTranslation());

        TransformOperations translation_matrix = new();
        Transform translation_transform = new();
        translation_transform.Translate3D(1.0f, 2.0f, 3.0f);
        translation_matrix.AppendMatrix(translation_transform);
        Assert.True(translation_matrix.IsTranslation());

        TransformOperations scaling_matrix = new();
        Transform scaling_transform = new();
        scaling_transform.Scale(2.0f, 2.0f);
        scaling_matrix.AppendMatrix(scaling_transform);
        Assert.False(scaling_matrix.IsTranslation());
    }

    [Fact]
    private static void TestIsTranslationWithMultipleOperations()
    {
        TransformOperations operations1 = new();
        operations1.AppendSkew(1.0f, 2.0f);
        operations1.AppendTranslate(1.0f, 2.0f, 3.0f);
        operations1.AppendIdentity();
        Assert.False(operations1.IsTranslation());

        TransformOperations operations2 = new();
        operations2.AppendIdentity();
        operations2.AppendTranslate(3.0f, 2.0f, 1.0f);
        Transform translation_transform = new();
        translation_transform.Translate3D(1.0f, 2.0f, 3.0f);
        operations2.AppendMatrix(translation_transform);
        Assert.True(operations2.IsTranslation());
    }

    [Fact]
    private static void TestScaleComponent()
    {
        float scale;

        // Scale.
        TransformOperations operations1 = new();
        operations1.AppendScale(-3.0f, 2.0f, 5.0f);
        Assert.True(operations1.ScaleComponent(out scale));
        Assert.Equal(5.0f, scale);

        // Translate.
        TransformOperations operations2 = new();
        operations2.AppendTranslate(1.0f, 2.0f, 3.0f);
        Assert.True(operations2.ScaleComponent(out scale));
        Assert.Equal(1.0f, scale);

        // Rotate.
        TransformOperations operations3 = new();
        operations3.AppendRotate(1.0f, 2.0f, 3.0f, 4.0f);
        Assert.True(operations3.ScaleComponent(out scale));
        Assert.Equal(1.0f, scale);

        // Matrix that's only a translation.
        TransformOperations operations4 = new();
        Transform translation_transform = new();
        translation_transform.Translate3D(1.0f, 2.0f, 3.0f);
        operations4.AppendMatrix(translation_transform);
        Assert.True(operations4.ScaleComponent(out scale));
        Assert.Equal(1.0f, scale);

        // Matrix that includes scale.
        TransformOperations operations5 = new();
        Transform matrix = new();
        matrix.RotateAboutZAxis(30.0);
        matrix.Scale(-7.0f, 6.0f);
        matrix.Translate3D(new Vector3DF(3.0f, 7.0f, 1.0f));
        operations5.AppendMatrix(matrix);
        Assert.True(operations5.ScaleComponent(out scale));
        Assert.Equal(7.0f, scale);

        // Matrix with perspective.
        TransformOperations operations6 = new();
        matrix.ApplyPerspectiveDepth(2000.0f);
        operations6.AppendMatrix(matrix);
        Assert.False(operations6.ScaleComponent(out scale));

        // Skew.
        TransformOperations operations7 = new();
        operations7.AppendSkew(30.0f, 60.0f);
        Assert.True(operations7.ScaleComponent(out scale));
        Assert.Equal(2.0f, scale);

        // Perspective.
        TransformOperations operations8 = new();
        operations8.AppendPerspective(500.0f);
        Assert.False(operations8.ScaleComponent(out scale));

        // Translate + Scale.
        TransformOperations operations9 = new();
        operations9.AppendTranslate(1.0f, 2.0f, 3.0f);
        operations9.AppendScale(2.0f, 5.0f, 4.0f);
        Assert.True(operations9.ScaleComponent(out scale));
        Assert.Equal(5.0f, scale);

        // Translate + Scale + Matrix with translate.
        operations9.AppendMatrix(translation_transform);
        Assert.True(operations9.ScaleComponent(out scale));
        Assert.Equal(5.0f, scale);

        // Scale + translate.
        TransformOperations operations10 = new();
        operations10.AppendScale(2.0f, 3.0f, 2.0f);
        operations10.AppendTranslate(1.0f, 2.0f, 3.0f);
        Assert.True(operations10.ScaleComponent(out scale));
        Assert.Equal(3.0f, scale);

        // Two Scales.
        TransformOperations operations11 = new();
        operations11.AppendScale(2.0f, 3.0f, 2.0f);
        operations11.AppendScale(-3.0f, -2.0f, -3.0f);
        Assert.True(operations11.ScaleComponent(out scale));
        Assert.Equal(9.0f, scale);

        // Scale + Matrix.
        TransformOperations operations12 = new();
        operations12.AppendScale(2.0f, 2.0f, 2.0f);
        Transform scaling_transform = new();
        scaling_transform.Scale(2.0f, 2.0f);
        operations12.AppendMatrix(scaling_transform);
        Assert.True(operations12.ScaleComponent(out scale));
        Assert.Equal(4.0f, scale);

        // Scale + Rotate.
        TransformOperations operations13 = new();
        operations13.AppendScale(2.0f, 2.0f, 2.0f);
        operations13.AppendRotate(1.0f, 2.0f, 3.0f, 4.0f);
        Assert.True(operations13.ScaleComponent(out scale));
        Assert.Equal(2.0f, scale);

        // Scale + Skew.
        TransformOperations operations14 = new();
        operations14.AppendScale(2.0f, 2.0f, 2.0f);
        operations14.AppendSkew(60.0f, 45.0f);
        Assert.True(operations14.ScaleComponent(out scale));
        Assert.Equal(4.0f, scale);

        // Scale + Perspective.
        TransformOperations operations15 = new();
        operations15.AppendScale(2.0f, 2.0f, 2.0f);
        operations15.AppendPerspective(1.0f);
        Assert.False(operations15.ScaleComponent(out scale));

        // Matrix with skew.
        TransformOperations operations16 = new();
        Transform skew_transform = new();
        skew_transform.Skew(50.0f, 60.0f);
        operations16.AppendMatrix(skew_transform);
        Assert.True(operations16.ScaleComponent(out scale));
        Assert.Equal(2.0f, scale);
    }

    [Fact]
    private static void TestApproximateEquality()
    {
        float noise = 1e-7f;
        float tolerance = 1e-5f;
        TransformOperations lhs = new();
        TransformOperations rhs = new();

        // Empty lists of operations are trivially equal.
        Assert.True(lhs.ApproximatelyEqual(rhs, tolerance));

        rhs.AppendIdentity();
        rhs.AppendTranslate(0, 0, 0);
        rhs.AppendRotate(1, 0, 0, 0);
        rhs.AppendScale(1, 1, 1);
        rhs.AppendSkew(0, 0);
        rhs.AppendMatrix(new Transform());

        // Even though both lists operations are effectively the identity matrix, rhs
        // has a different number of operations and is therefore different.
        Assert.False(lhs.ApproximatelyEqual(rhs, tolerance));

        rhs.AppendPerspective(800);

        // Assignment should produce equal lists of operations.
        lhs = new(rhs);
        Assert.True(lhs.ApproximatelyEqual(rhs, tolerance));

        // Cannot affect identity operations.
        lhs.At(0).X = 1;
        Assert.True(lhs.ApproximatelyEqual(rhs, tolerance));

        lhs.At(1).X += noise;
        Assert.True(lhs.ApproximatelyEqual(rhs, tolerance));
        lhs.At(1).X += 1;
        Assert.False(lhs.ApproximatelyEqual(rhs, tolerance));

        lhs = new(rhs);
        lhs.At(2).Angle += noise;
        Assert.True(lhs.ApproximatelyEqual(rhs, tolerance));
        lhs.At(2).Angle = 1;
        Assert.False(lhs.ApproximatelyEqual(rhs, tolerance));

        lhs = new(rhs);
        lhs.At(3).X += noise;
        Assert.True(lhs.ApproximatelyEqual(rhs, tolerance));
        lhs.At(3).X += 1;
        Assert.False(lhs.ApproximatelyEqual(rhs, tolerance));

        lhs = new(rhs);
        lhs.At(4).X += noise;
        Assert.True(lhs.ApproximatelyEqual(rhs, tolerance));
        lhs.At(4).X = 2;
        Assert.False(lhs.ApproximatelyEqual(rhs, tolerance));

        lhs = new(rhs);
        lhs.At(5).Matrix.Translate3D(noise, 0, 0);
        Assert.True(lhs.ApproximatelyEqual(rhs, tolerance));
        lhs.At(5).Matrix.Translate3D(1, 1, 1);
        Assert.False(lhs.ApproximatelyEqual(rhs, tolerance));

        lhs = new(rhs);
        lhs.At(6).PerspectiveM43 += noise;
        Assert.True(lhs.ApproximatelyEqual(rhs, tolerance));
        lhs.At(6).PerspectiveM43 = -1.0f / 810;
        Assert.False(lhs.ApproximatelyEqual(rhs, tolerance));
    }
}
