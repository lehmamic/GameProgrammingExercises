using Silk.NET.Maths;

namespace GameProgrammingExercises.Maths.Geometry;

public struct Capsule
{
    public LineSegment Segment;
    public float Radius;
    
    public Capsule(Vector3D<float> start, Vector3D<float> end, float radius)
    {
        Segment = new LineSegment(start, end);
        Radius = radius;
    }

    /// <summary>
    /// Get point along segment where 0 <= t <= 1
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public Vector3D<float> PointOnSegment(float t)
    {
        return Segment.PointOnSegment(t);
    }

    public bool Contains(Vector3D<float> point)
    {
        // Get minimal dist. sq. between point and line segment
        float distSq = Segment.MinDistSq(point);
        return distSq <= (Radius * Radius);
    }
}