using System.Diagnostics;

namespace UI.GFX.Geometry;

// Transform operations are a decomposed transformation matrix. It can be
// applied to obtain a Transform at any time, and can be blended
// intelligently with other transform operations, so long as they represent
// the same decomposition. For example, if we have a transform that is made
// up of a rotation followed by skew, it can be blended intelligently with
// another transform made up of a rotation followed by a skew. Blending is
// possible if we have two dissimilar sets of transform operations, but the
// effect may not be what was intended.
public class TransformOperations
{
    private readonly List<TransformOperation> operations_ = new();

    // For efficiency, we cache the decomposed transforms.
    private readonly Dictionary<int, DecomposedTransform> decomposed_transforms_ = new();

    public TransformOperations() { }

    public TransformOperations(TransformOperations other)
    {
        operations_ = new List<TransformOperation>(other.operations_);
    }

    // Returns a transformation matrix representing these transform operations.
    public Transform Apply() => ApplyRemaining(0);

    // Returns a transformation matrix representing the set of transform
    // operations from index |start| to the end of the list.
    public Transform ApplyRemaining(int start)
    {
        Transform to_return = new();
#if DEBUG
        //Debug.WriteLine($"to_return before: {to_return}");
#endif
        for (int i = start; i < operations_.Count; i++)
        {
            to_return.PreConcat(operations_[i].Matrix);
        }
#if DEBUG
        //Debug.WriteLine($"operations_[i].Matrix: {operations_[0].Matrix}");
        //Debug.WriteLine($"to_return after: {to_return}");
#endif
        return to_return;
    }

    /*
    public Transform ApplyRemaining(int start)
    {
        Transform to_return = new();
        for (int i = start; i < operations_.Count; i++)
            to_return.PreConcat(operations_[i].Matrix);
        return to_return;
    }
    */

    // Given another set of transform operations and a progress in the range
    // [0, 1], returns a transformation matrix representing the intermediate
    // value. If this->MatchesTypes(from), then each of the operations are
    // blended separately and then combined. Otherwise, the two sets of
    // transforms are baked to matrices (using apply), and the matrices are
    // then decomposed and interpolated.
    //
    // If either of the matrices are non-decomposable for the blend, Blend
    // applies discrete interpolation between them based on the progress value.
    public TransformOperations Blend(TransformOperations from, float progress)
    {
        TransformOperations to_return = new();
        if (!BlendInternal(from, progress, to_return))
        {
            // If the matrices cannot be blended, fallback to discrete animation
            // logic. See https://drafts.csswg.org/css-transforms/#matrix-interpolation
            to_return = progress < 0.5f ? new TransformOperations(from)
                                        : new TransformOperations(this);
        }
        return to_return;
    }

    // Sets |bounds| to be the bounding box for the region within which |box|
    // will exist when transformed by the result of calling Blend on |from| with
    // progress in the range [min_progress, max_progress].
    public bool BlendedBoundsForBox(in BoxF box, TransformOperations from,
                                    float min_progress, float max_progress,
                                    out BoxF bounds)
    {
        bounds = box;

        bool from_identity = from.IsIdentity();
        bool to_identity   = IsIdentity();
        if (from_identity && to_identity)
            return true;

        if (!MatchesTypes(from))
            return false;

        int num_operations = Math.Max(from_identity ? 0 : from.operations_.Count,
                                      to_identity   ? 0 : operations_.Count);

        // Because we are squashing all of the matrices together when applying
        // them to the animation, we must apply them in reverse order when
        // not squashing them.
        for (int i = 0; i < num_operations; ++i)
        {
            int operation_index = num_operations - 1 - i;
            TransformOperation? from_op = from_identity
                ? null
                : from.operations_[operation_index];
            TransformOperation? to_op = to_identity
                ? null
                : operations_[operation_index];

            if (!TransformOperation.BlendedBoundsForBox(
                    bounds, from_op, to_op,
                    min_progress, max_progress,
                    out BoxF bounds_for_operation))
                return false;

            bounds = bounds_for_operation;
        }

        return true;
    }

    // Returns true if these operations are only translations.
    public bool IsTranslation()
    {
        foreach (var operation in operations_)
        {
            switch (operation.type)
            {
                case TransformOperation.TransformOperationType.Identity:
                case TransformOperation.TransformOperationType.Translate:
                    continue;
                case TransformOperation.TransformOperationType.Matrix:
                    if (!operation.Matrix.IsIdentityOrTranslation)
                        return false;
                    continue;
                default:
                    return false;
            }
        }
        return true;
    }

    // Returns false if the operations affect 2d axis alignment.
    public bool PreservesAxisAlignment()
    {
        foreach (var operation in operations_)
        {
            switch (operation.type)
            {
                case TransformOperation.TransformOperationType.Identity:
                case TransformOperation.TransformOperationType.Translate:
                case TransformOperation.TransformOperationType.Scale:
                    continue;
                case TransformOperation.TransformOperationType.Matrix:
                    if (!operation.Matrix.IsIdentity &&
                        !operation.Matrix.IsScaleOrTranslation)
                        return false;
                    continue;
                default:
                    return false;
            }
        }
        return true;
    }

    // Returns true if this operation and its descendants have the same types
    // as other and its descendants.
    public bool MatchesTypes(TransformOperations other)
    {
        if (operations_.Count == 0 || other.operations_.Count == 0)
            return true;

        if (operations_.Count != other.operations_.Count)
            return false;

        for (int i = 0; i < operations_.Count; ++i)
            if (operations_[i].type != other.operations_[i].type)
                return false;

        return true;
    }

    // Returns the number of matching transform operations at the start of the
    // transform lists.
    public int MatchingPrefixLength(TransformOperations other)
    {
        int num_operations = Math.Min(operations_.Count, other.operations_.Count);
        for (int i = 0; i < num_operations; ++i)
        {
            if (operations_[i].type != other.operations_[i].type)
                return i;
        }
        return Math.Max(operations_.Count, other.operations_.Count);
    }

    // Returns true if these operations can be blended.
    public bool CanBlendWith(TransformOperations other)
    {
        TransformOperations dummy = new();
        return BlendInternal(other, 0.5f, dummy);
    }

    // If none of these operations have a perspective component, sets |scale| to
    // be the product of the scale component of every operation.
    public bool ScaleComponent(out float scale)
    {
        scale = 1f;
        foreach (var operation in operations_)
        {
            switch (operation.type)
            {
                case TransformOperation.TransformOperationType.Identity:
                case TransformOperation.TransformOperationType.Translate:
                case TransformOperation.TransformOperationType.Rotate:
                    continue;
                case TransformOperation.TransformOperationType.Matrix:
                {
                    if (operation.Matrix.HasPerspective)
                        return false;
                    Vector2DF scale_components =
                        TransformUtil.ComputeTransform2dScaleComponents(
                            operation.Matrix, 1f);
                    scale *= Math.Max(scale_components.X, scale_components.Y);
                    break;
                }
                case TransformOperation.TransformOperationType.SkewX:
                case TransformOperation.TransformOperationType.SkewY:
                case TransformOperation.TransformOperationType.Skew:
                {
                    float x_component = TanDegrees(operation.X);
                    float y_component = TanDegrees(operation.Y);
                    float x_scale = MathF.Sqrt(x_component * x_component + 1);
                    float y_scale = MathF.Sqrt(y_component * y_component + 1);
                    scale *= Math.Max(x_scale, y_scale);
                    break;
                }
                case TransformOperation.TransformOperationType.Perspective:
                    return false;
                case TransformOperation.TransformOperationType.Scale:
                    scale *= Math.Max(MathF.Abs(operation.X),
                             Math.Max(MathF.Abs(operation.Y),
                                      MathF.Abs(operation.Z)));
                    break;
            }
        }
        return true;
    }

    public void AppendTranslate(float x, float y, float z)
    {
        TransformOperation to_add = new();
#if DEBUG
        //Debug.WriteLine($"AppendTranslate(), to_add.Matrix: {to_add.Matrix}");
#endif
        to_add.Matrix.Translate3D(x, y, z);
        to_add.type = TransformOperation.TransformOperationType.Translate;
        to_add.X = x;
        to_add.Y = y;
        to_add.Z = z;
        operations_.Add(to_add);
        decomposed_transforms_.Clear();
    }

    public void AppendRotate(float x, float y, float z, float degrees)
    {
        TransformOperation to_add = new();
        to_add.type = TransformOperation.TransformOperationType.Rotate;
        to_add.X     = x;
        to_add.Y     = y;
        to_add.Z     = z;
        to_add.Angle = degrees;
        to_add.Bake();
        operations_.Add(to_add);
        decomposed_transforms_.Clear();
    }

    public void AppendScale(float x, float y, float z)
    {
        TransformOperation to_add = new();
        to_add.type = TransformOperation.TransformOperationType.Scale;
        to_add.X = x;
        to_add.Y = y;
        to_add.Z = z;
        to_add.Bake();
        operations_.Add(to_add);
        decomposed_transforms_.Clear();
    }

    public void AppendSkewX(float x)
    {
        TransformOperation to_add = new();
        to_add.type = TransformOperation.TransformOperationType.SkewX;
        to_add.X = x;
        to_add.Y = 0;
        to_add.Bake();
        operations_.Add(to_add);
        decomposed_transforms_.Clear();
    }

    public void AppendSkewY(float y)
    {
        TransformOperation to_add = new();
        to_add.type = TransformOperation.TransformOperationType.SkewY;
        to_add.X = 0;
        to_add.Y = y;
        to_add.Bake();
        operations_.Add(to_add);
        decomposed_transforms_.Clear();
    }

    public void AppendSkew(float x, float y)
    {
        TransformOperation to_add = new();
        to_add.type = TransformOperation.TransformOperationType.Skew;
        to_add.X = x;
        to_add.Y = y;
        to_add.Bake();
        operations_.Add(to_add);
        decomposed_transforms_.Clear();
    }

    public void AppendPerspective(float? depth)
    {
        TransformOperation to_add = new();
        to_add.type = TransformOperation.TransformOperationType.Perspective;
        if (depth.HasValue)
        {
#if DEBUG
            //Debug.Assert(depth.Value >= 1.0f);
#endif
            to_add.PerspectiveM43 = -1.0f / depth.Value;
        }
        else
        {
            to_add.PerspectiveM43 = 0.0f;
        }
        to_add.Bake();
        operations_.Add(to_add);
        decomposed_transforms_.Clear();
    }

    public void AppendMatrix(in Transform matrix)
    {
        TransformOperation to_add = new();
        to_add.Matrix         = matrix;
        to_add.type  = TransformOperation.TransformOperationType.Matrix;
        operations_.Add(to_add);
        decomposed_transforms_.Clear();
    }

    public void AppendIdentity()
    {
        operations_.Add(new TransformOperation());
    }

    public void Append(in TransformOperation operation)
    {
        operations_.Add(operation);
        decomposed_transforms_.Clear();
    }

    public bool IsIdentity()
    {
        foreach (var operation in operations_)
            if (!operation.IsIdentity())
                return false;
        return true;
    }

    public int Size => operations_.Count;

    public ref TransformOperation At(int index)
    {
#if DEBUG
        //Debug.Assert(index < Size);
#endif
        return ref System.Runtime.InteropServices.CollectionsMarshal.AsSpan(operations_)[index];
    }

    public bool ApproximatelyEqual(TransformOperations other, float tolerance)
    {
        if (Size != other.Size)
            return false;
        for (int i = 0; i < operations_.Count; ++i)
            if (!operations_[i].ApproximatelyEqual(other.operations_[i], tolerance))
                return false;
        return true;
    }

    private static float TanDegrees(double degrees) =>
        (float)Math.Tan(double.DegreesToRadians(degrees));

    private bool BlendInternal(TransformOperations from, float progress,
                               TransformOperations result)
    {
        bool from_identity = from.IsIdentity();
        bool to_identity   = IsIdentity();
        if (from_identity && to_identity)
            return true;

        int matching_prefix_length = MatchingPrefixLength(from);
        int from_size      = from_identity ? 0 : from.operations_.Count;
        int to_size        = to_identity   ? 0 : operations_.Count;
        int num_operations = Math.Max(from_size, to_size);

        for (int i = 0; i < matching_prefix_length; ++i)
        {
            TransformOperation? from_op = i >= from_size ? null : from.operations_[i];
            TransformOperation? to_op   = i >= to_size   ? null : operations_[i];

            if (!TransformOperation.BlendTransformOperations(
                    from_op, to_op, progress, out TransformOperation blended))
                return false;

            result.Append(blended);
        }

        if (matching_prefix_length < num_operations)
        {
            if (!ComputeDecomposedTransform(matching_prefix_length) ||
                !from.ComputeDecomposedTransform(matching_prefix_length))
                return false;

            DecomposedTransform matrix_transform = TransformUtil.BlendDecomposedTransforms(
                decomposed_transforms_[matching_prefix_length],
                from.decomposed_transforms_[matching_prefix_length],
                progress);
            result.AppendMatrix(Transform.Compose(matrix_transform));
        }

        return true;
    }

    // FRIEND_TEST_ALL_PREFIXES(TransformOperationsTest, TestDecompositionCache)
    internal bool ComputeDecomposedTransform(int start_offset)
    {
        if (!decomposed_transforms_.ContainsKey(start_offset))
        {
            Transform transform = ApplyRemaining(start_offset);
            if (!transform.Decompose(out DecomposedTransform decomp))
                return false;
            decomposed_transforms_[start_offset] = decomp;
        }
        return true;
    }
}
