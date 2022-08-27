using Silk.NET.Maths;

namespace GameProgrammingExercises.Maths.Geometry;

public struct Plane
{
    public Vector3D<float> Normal;
    public float D;
    
    public Plane(Vector3D<float> normal, float d)
    {
        Normal = normal;
        D = d;
    }

    /// <summary>
    /// Construct plane from three points
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    public Plane(Vector3D<float> a, Vector3D<float> b, Vector3D<float> c)
    {
        // Compute vectors from a to b and a to c
        Vector3D<float> ab = b - a;
        Vector3D<float> ac = c - a;

        // Cross product and normalize to get normal
        Normal = Vector3D.Cross(ab, ac);
        Normal = Vector3D.Normalize(Normal);

        // d = -P dot n
        D = -Vector3D.Dot(a, Normal);
    }

    /// <summary>
    /// Get the signed distance between the point and the plane
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public float SignedDist(Vector3D<float> point)
    {
        return Vector3D.Dot(point, Normal) - D;
    }
}