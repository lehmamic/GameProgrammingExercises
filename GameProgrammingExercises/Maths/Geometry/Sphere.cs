using Silk.NET.Maths;

namespace GameProgrammingExercises.Maths.Geometry;

public struct Sphere
{
    public Vector3D<float> Center;
    public float Radius;

    public Sphere(Vector3D<float> center, float radius)
    {
        Center = center;
        Radius = radius;
    }

    public bool Contains(Vector3D<float> point)
    {
        // Get distance squared between center and point
        float distSq = (Center - point).LengthSquared;
        return distSq <= (Radius * Radius);
    }
}