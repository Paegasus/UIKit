using System.Diagnostics;

namespace UI.GFX.Geometry;

public struct TransformOperation
{
    public enum Type
    {
        Translate,
        Rotate,
        Scale,
        SkewX,
        SkewY,
        Skew,
        Perspective,
        Matrix,
        Identity
    }

    private const float kAngleEpsilon = 1e-4f;

    public Type OperationType = Type.Identity;
    public Transform Matrix;

    // Union fields — only fields relevant to OperationType are valid.
    // Perspective: PerspectiveM43 (-1/depth)
    // Skew:        X, Y
    // Scale:       X, Y, Z
    // Translate:   X, Y, Z
    // Rotate:      X, Y, Z (axis), Angle
    public float X;
    public float Y;
    public float Z;
    public float Angle;
    public float PerspectiveM43;

    public TransformOperation() { }

    public readonly bool IsIdentity()
    {
        if (OperationType == Type.Rotate)
        {
            // We can't use Matrix.IsIdentity() because rotate(n*360) is not
            // identity, but the matrix is identity.
            return Angle == 0 ||
                   // Rotation with zero length axis is treated as identity.
                   (X == 0 && Y == 0 && Z == 0);
        }
        return Matrix.IsIdentity;
    }

    // Sets |Matrix| based on OperationType and the union values.
    public void Bake()
    {
        Matrix.MakeIdentity();
        switch (OperationType)
        {
            case Type.Translate:
                Matrix.Translate3D(X, Y, Z);
                break;
            case Type.Rotate:
                Matrix.RotateAbout(new Vector3DF(X, Y, Z), Angle);
                break;
            case Type.Scale:
                Matrix.Scale3D(X, Y, Z);
                break;
            case Type.SkewX:
            case Type.SkewY:
            case Type.Skew:
                Matrix.Skew(X, Y);
                break;
            case Type.Perspective:
            {
                Transform m = new();
                m.set_rc(3, 2, PerspectiveM43);
                Matrix.PreConcat(m);
                break;
            }
            case Type.Matrix:
            case Type.Identity:
                break;
        }
    }

    public readonly bool ApproximatelyEqual(in TransformOperation other, float tolerance)
    {
        Debug.Assert(tolerance >= 0);
        if (OperationType != other.OperationType)
            return false;
        switch (OperationType)
        {
            case Type.Translate:
                return MathF.Abs(X - other.X) <= tolerance &&
                       MathF.Abs(Y - other.Y) <= tolerance &&
                       MathF.Abs(Z - other.Z) <= tolerance;
            case Type.Rotate:
                return MathF.Abs(X     - other.X)     <= tolerance &&
                       MathF.Abs(Y     - other.Y)     <= tolerance &&
                       MathF.Abs(Z     - other.Z)     <= tolerance &&
                       MathF.Abs(Angle - other.Angle) <= tolerance;
            case Type.Scale:
                return MathF.Abs(X - other.X) <= tolerance &&
                       MathF.Abs(Y - other.Y) <= tolerance &&
                       MathF.Abs(Z - other.Z) <= tolerance;
            case Type.SkewX:
            case Type.SkewY:
            case Type.Skew:
                return MathF.Abs(X - other.X) <= tolerance &&
                       MathF.Abs(Y - other.Y) <= tolerance;
            case Type.Perspective:
                return MathF.Abs(PerspectiveM43 - other.PerspectiveM43) <= tolerance;
            case Type.Matrix:
                return Matrix.ApproximatelyEqual(other.Matrix, tolerance);
            case Type.Identity:
                return other.Matrix.IsIdentity;
        }
        throw new InvalidOperationException("Unknown TransformOperation type");
    }

    private static bool IsOperationIdentity(in TransformOperation? operation) =>
        !operation.HasValue || operation.Value.IsIdentity();

    private static float BlendFloats(float from, float to, float progress) =>
        from * (1 - progress) + to * progress;

    private static bool ShareSameAxis(
        in TransformOperation? from, bool is_identity_from,
        in TransformOperation? to,   bool is_identity_to,
        out float axis_x, out float axis_y, out float axis_z,
        out float angle_from)
    {
        Debug.Assert(is_identity_from == IsOperationIdentity(from));
        Debug.Assert(is_identity_to   == IsOperationIdentity(to));
        Debug.Assert(!is_identity_from || !is_identity_to);

        if (is_identity_from && !is_identity_to)
        {
            axis_x = to!.Value.X;
            axis_y = to!.Value.Y;
            axis_z = to!.Value.Z;
            angle_from = 0;
            return true;
        }

        if (!is_identity_from && is_identity_to)
        {
            axis_x     = from!.Value.X;
            axis_y     = from!.Value.Y;
            axis_z     = from!.Value.Z;
            angle_from = from!.Value.Angle;
            return true;
        }

        float length_2 = from!.Value.X * from.Value.X +
                         from!.Value.Y * from.Value.Y +
                         from!.Value.Z * from.Value.Z;
        float other_length_2 = to!.Value.X * to.Value.X +
                               to!.Value.Y * to.Value.Y +
                               to!.Value.Z * to.Value.Z;

        if (length_2 <= kAngleEpsilon || other_length_2 <= kAngleEpsilon)
        {
            axis_x = axis_y = axis_z = angle_from = 0;
            return false;
        }

        float dot = to.Value.X * from.Value.X +
                    to.Value.Y * from.Value.Y +
                    to.Value.Z * from.Value.Z;
        float error = MathF.Abs(1f - (dot * dot) / (length_2 * other_length_2));
        bool result = error < kAngleEpsilon;

        if (result)
        {
            axis_x     = to.Value.X;
            axis_y     = to.Value.Y;
            axis_z     = to.Value.Z;
            // If the axes are pointing in opposite directions, reverse the angle.
            angle_from = dot > 0 ? from.Value.Angle : -from.Value.Angle;
        }
        else
        {
            axis_x = axis_y = axis_z = angle_from = 0;
        }
        return result;
    }

    public static bool BlendTransformOperations(
        in TransformOperation? from,
        in TransformOperation? to,
        float progress,
        out TransformOperation result)
    {
        result = new TransformOperation();

        bool is_identity_from = IsOperationIdentity(from);
        bool is_identity_to   = IsOperationIdentity(to);
        if (is_identity_from && is_identity_to)
            return true;

        Type interpolation_type = is_identity_to ? from!.Value.OperationType
                                                 : to!.Value.OperationType;
        result.OperationType = interpolation_type;

        switch (interpolation_type)
        {
            case Type.Translate:
            {
                float from_x = is_identity_from ? 0 : from!.Value.X;
                float from_y = is_identity_from ? 0 : from!.Value.Y;
                float from_z = is_identity_from ? 0 : from!.Value.Z;
                float to_x   = is_identity_to   ? 0 : to!.Value.X;
                float to_y   = is_identity_to   ? 0 : to!.Value.Y;
                float to_z   = is_identity_to   ? 0 : to!.Value.Z;
                result.X = BlendFloats(from_x, to_x, progress);
                result.Y = BlendFloats(from_y, to_y, progress);
                result.Z = BlendFloats(from_z, to_z, progress);
                result.Bake();
                break;
            }
            case Type.Rotate:
            {
                float axis_x = 0, axis_y = 0, axis_z = 1, from_angle = 0;
                float to_angle = is_identity_to ? 0 : to!.Value.Angle;
                if (ShareSameAxis(from, is_identity_from, to, is_identity_to,
                                  out axis_x, out axis_y, out axis_z, out from_angle))
                {
                    result.X     = axis_x;
                    result.Y     = axis_y;
                    result.Z     = axis_z;
                    result.Angle = BlendFloats(from_angle, to_angle, progress);
                    result.Bake();
                }
                else
                {
                    if (!is_identity_to)
                        result.Matrix = to!.Value.Matrix;
                    Transform from_matrix = new();
                    if (!is_identity_from)
                        from_matrix = from!.Value.Matrix;
                    if (!result.Matrix.Blend(from_matrix, progress))
                        return false;
                }
                break;
            }
            case Type.Scale:
            {
                float from_x = is_identity_from ? 1 : from!.Value.X;
                float from_y = is_identity_from ? 1 : from!.Value.Y;
                float from_z = is_identity_from ? 1 : from!.Value.Z;
                float to_x   = is_identity_to   ? 1 : to!.Value.X;
                float to_y   = is_identity_to   ? 1 : to!.Value.Y;
                float to_z   = is_identity_to   ? 1 : to!.Value.Z;
                result.X = BlendFloats(from_x, to_x, progress);
                result.Y = BlendFloats(from_y, to_y, progress);
                result.Z = BlendFloats(from_z, to_z, progress);
                result.Bake();
                break;
            }
            case Type.SkewX:
            case Type.SkewY:
            case Type.Skew:
            {
                float from_x = is_identity_from ? 0 : from!.Value.X;
                float from_y = is_identity_from ? 0 : from!.Value.Y;
                float to_x   = is_identity_to   ? 0 : to!.Value.X;
                float to_y   = is_identity_to   ? 0 : to!.Value.Y;
                result.X = BlendFloats(from_x, to_x, progress);
                result.Y = BlendFloats(from_y, to_y, progress);
                result.Bake();
                break;
            }
            case Type.Perspective:
            {
                float from_m43 = is_identity_from ? 0f : from!.Value.PerspectiveM43;
                float to_m43   = is_identity_to   ? 0f : to!.Value.PerspectiveM43;
                Debug.Assert(is_identity_from || (from_m43 <= 0f && from_m43 >= -1f));
                Debug.Assert(is_identity_to   || (to_m43   <= 0f && to_m43   >= -1f));
                result.PerspectiveM43 = Math.Clamp(
                    BlendFloats(from_m43, to_m43, progress), -1f, 0f);
                result.Bake();
                break;
            }
            case Type.Matrix:
            {
                if (!is_identity_to)
                    result.Matrix = to!.Value.Matrix;
                Transform from_matrix = new();
                if (!is_identity_from)
                    from_matrix = from!.Value.Matrix;
                if (!result.Matrix.Blend(from_matrix, progress))
                    return false;
                break;
            }
            case Type.Identity:
                break;
        }

        return true;
    }

    public static bool BlendedBoundsForBox(
        in BoxF box,
        in TransformOperation? from,
        in TransformOperation? to,
        float min_progress,
        float max_progress,
        out BoxF bounds)
    {
        bounds = new BoxF();

        bool is_identity_from = IsOperationIdentity(from);
        bool is_identity_to   = IsOperationIdentity(to);
        if (is_identity_from && is_identity_to)
        {
            bounds = box;
            return true;
        }

        Type interpolation_type = is_identity_to ? from!.Value.OperationType
                                                 : to!.Value.OperationType;

        switch (interpolation_type)
        {
            case Type.Identity:
                bounds = box;
                return true;

            case Type.Translate:
            case Type.SkewX:
            case Type.SkewY:
            case Type.Skew:
            case Type.Perspective:
            case Type.Scale:
            {
                if (!BlendTransformOperations(from, to, min_progress, out TransformOperation from_op) ||
                    !BlendTransformOperations(from, to, max_progress, out TransformOperation to_op))
                    return false;

                bounds = from_op.Matrix.MapBox(box);
                BoxF to_box = to_op.Matrix.MapBox(box);
                bounds.ExpandTo(to_box);
                return true;
            }

            case Type.Rotate:
            {
                if (!ShareSameAxis(from, is_identity_from, to, is_identity_to,
                                   out _, out _, out _, out _))
                    return false;

                bool first_point = true;
                for (int i = 0; i < 8; ++i)
                {
                    Point3F corner = box.Origin;
                    corner += new Vector3DF(
                        (i & 1) != 0 ? box.Width  : 0f,
                        (i & 2) != 0 ? box.Height : 0f,
                        (i & 4) != 0 ? box.Depth  : 0f);
                    BoundingBoxForArc(corner, from, to, min_progress, max_progress,
                                      out BoxF box_for_arc);
                    if (first_point)
                        bounds = box_for_arc;
                    else
                        bounds.Union(box_for_arc);
                    first_point = false;
                }
                return true;
            }

            case Type.Matrix:
                return false;
        }

        throw new InvalidOperationException("Unknown TransformOperation type");
    }

    // If p = (px, py) is a point in the plane being rotated about (0, 0, nz),
    // computes angles to rotate from p to (length(p), 0), (-length(p), 0),
    // (0, length(p)), (0, -length(p)).
    private static void FindCandidatesInPlane(
        float px, float py, float nz,
        Span<double> candidates, out int num_candidates)
    {
        double phi = Math.Atan2(px, py);
        num_candidates = 4;
        candidates[0] = phi;
        for (int i = 1; i < num_candidates; ++i)
            candidates[i] = candidates[i - 1] + Math.PI / 2;
        if (nz < 0f)
            for (int i = 0; i < num_candidates; ++i)
                candidates[i] *= -1.0;
    }

    private static void BoundingBoxForArc(
        in Point3F point,
        in TransformOperation? from,
        in TransformOperation? to,
        float min_progress,
        float max_progress,
        out BoxF box)
    {
        box = new BoxF();

        TransformOperation exemplar = from.HasValue ? from.Value : to!.Value;
        Vector3DF axis = new(exemplar.X, exemplar.Y, exemplar.Z);

        bool x_is_zero = axis.X == 0f;
        bool y_is_zero = axis.Y == 0f;
        bool z_is_zero = axis.Z == 0f;

        if (x_is_zero && y_is_zero && z_is_zero)
            return;

        const int kMaxNumCandidates = 6;
        Span<double> candidates = stackalloc double[kMaxNumCandidates];
        int num_candidates = kMaxNumCandidates;

        float from_angle = from.HasValue ? from.Value.Angle : 0f;
        float to_angle   = to.HasValue   ? to.Value.Angle   : 0f;

        // If axes are pointing in opposite directions, flip one angle.
        if (from.HasValue && to.HasValue)
        {
            Vector3DF other_axis = new(to.Value.X, to.Value.Y, to.Value.Z);
            if (Vector3DF.DotProduct(axis, other_axis) < 0f)
                to_angle *= -1f;
        }

        float min_degrees = BlendFloats(from_angle, to_angle, min_progress);
        float max_degrees = BlendFloats(from_angle, to_angle, max_progress);
        if (max_degrees < min_degrees)
            (min_degrees, max_degrees) = (max_degrees, min_degrees);

        Transform from_transform = new();
        from_transform.RotateAbout(axis, min_degrees);
        Transform to_transform = new();
        to_transform.RotateAbout(axis, max_degrees);

        Point3F point_rotated_from = from_transform.MapPoint(point);
        Point3F point_rotated_to   = to_transform.MapPoint(point);

        box = new BoxF();
        box.Origin = point_rotated_from;
        box.ExpandTo(point_rotated_to);

        if (x_is_zero && y_is_zero)
        {
            FindCandidatesInPlane(point.X, point.Y, axis.Z,
                                  candidates, out num_candidates);
        }
        else if (x_is_zero && z_is_zero)
        {
            FindCandidatesInPlane(point.Z, point.X, axis.Y,
                                  candidates, out num_candidates);
        }
        else if (y_is_zero && z_is_zero)
        {
            FindCandidatesInPlane(point.Y, point.Z, axis.X,
                                  candidates, out num_candidates);
        }
        else
        {
            Vector3DF normal = axis;
            normal.InvScale(normal.Length());

            Point3F origin = new();
            Vector3DF to_point = point - origin;
            Point3F center = origin + Vector3DF.ScaleVector3D(normal, Vector3DF.DotProduct(to_point, normal));

            Vector3DF v1 = point - center;
            float v1_length = v1.Length();
            if (v1_length == 0f)
                return;

            v1.InvScale(v1_length);
            Vector3DF v2 = Vector3DF.CrossProduct(normal, v1);

            candidates[0] = Math.Atan2(v2.X, v1.X);
            candidates[1] = candidates[0] + Math.PI;
            candidates[2] = Math.Atan2(v2.Y, v1.Y);
            candidates[3] = candidates[2] + Math.PI;
            candidates[4] = Math.Atan2(v2.Z, v1.Z);
            candidates[5] = candidates[4] + Math.PI;
        }

        double min_radians = double.DegreesToRadians(min_degrees);
        double max_radians = double.DegreesToRadians(max_degrees);

        for (int i = 0; i < num_candidates; ++i)
        {
            double radians = candidates[i];
            while (radians < min_radians) radians += 2.0 * Math.PI;
            while (radians > max_radians) radians -= 2.0 * Math.PI;
            if (radians < min_radians) continue;

            Transform rotation = new();
            rotation.RotateAbout(axis, double.RadiansToDegrees(radians));
            Point3F rotated = rotation.MapPoint(point);
            box.ExpandTo(rotated);
        }
    }
}