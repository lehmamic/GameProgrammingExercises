using Silk.NET.Maths;

namespace GameProgrammingExercises.Maths.Geometry;

/// <summary>
/// Axis-aligned bounding box
/// </summary>
public struct AABB
{
    public Vector3D<float> Min;
    public Vector3D<float> Max;

    public AABB(Vector3D<float> min, Vector3D<float> max)
    {
        Min = min;
        Max = max;
    }

    /// <summary>
    /// Update min and max accounting for this point
    /// (used when loading a model)
    /// </summary>
    /// <param name="point"></param>
    public void UpdateMinMax(Vector3D<float> point)
    {
        // Update each component separately
        Min.X = Scalar.Min(Min.X, point.X);
        Min.Y = Scalar.Min(Min.Y, point.Y);
        Min.Z = Scalar.Min(Min.Z, point.Z);

        Max.X = Scalar.Max(Max.X, point.X);
        Max.Y = Scalar.Max(Max.Y, point.Y);
        Max.Z = Scalar.Max(Max.Z, point.Z);
    }

    /// <summary>
    /// Rotated by a quaternion
    /// </summary>
    public void Rotate(Quaternion<float> q)
    {
        // Construct the 8 points for the corners of the box
        var points = new Vector3D<float>[8];

        // Min point is always a corner
        points[0] = new Vector3D<float>(Min.X, Min.Y, Min.Z);

        // Permutations with 2 min and 1 max
        points[1] = new Vector3D<float>(Max.X, Min.Y, Min.Z);
        points[2] = new Vector3D<float>(Min.X, Max.Y, Min.Z);
        points[3] = new Vector3D<float>(Min.X, Min.Y, Max.Z);

        // Permutations with 2 max and 1 min
        points[4] = new Vector3D<float>(Min.X, Max.Y, Max.Z);
        points[5] = new Vector3D<float>(Max.X, Min.Y, Max.Z);
        points[6] = new Vector3D<float>(Max.X, Max.Y, Min.Z);

        // Max point corner
        points[7] = new Vector3D<float>(Max.X, Max.Y, Max.Z);

        // Rotate first point
        Vector3D<float> p = Vector3D.Transform(points[0], q);

        // Reset min/max to first point rotated
        Min = p;
        Max = p;

        // Update min/max based on remaining points, rotated
        for (int i = 1; i < points.Length; i++)
        {
            p = Vector3D.Transform(points[i], q);
            UpdateMinMax(p);
        }
    }

    public bool Contains(Vector3D<float> point)
    {
        bool outside = point.X < Min.X ||
                       point.Y < Min.Y ||
                       point.Z < Min.Z ||
                       point.X > Max.X ||
                       point.Y > Max.Y ||
                       point.Z > Max.Z;
        // If none of these are true, the point is inside the box
        return !outside;
    }

    public float MinDistSq(Vector3D<float> point)
    {
        // Compute differences for each axis
        float dx = Scalar.Max(Min.X - point.X, 0.0f);
        dx = Scalar.Max(dx, point.X - Max.X);
        float dy = Scalar.Max(Min.Y - point.Y, 0.0f);
        dy = Scalar.Max(dy, point.Y - Max.Y);
        float dz = Scalar.Max(Min.Z - point.Z, 0.0f);
        dz = Scalar.Max(dy, point.Z - Max.Z);
        // Distance squared formula
        return dx * dx + dy * dy + dz * dz;
    }
}