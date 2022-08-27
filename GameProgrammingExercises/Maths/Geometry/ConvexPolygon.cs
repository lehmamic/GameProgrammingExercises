using Silk.NET.Maths;

namespace GameProgrammingExercises.Maths.Geometry;

public struct ConvexPolygon
{
    // Vertices have a clockwise ordering
    public Vector2D<float>[] Vertices;
    
    public ConvexPolygon(Vector2D<float>[] vertices)
    {
        Vertices = vertices;
    }

    public bool Contains(Vector2D<float> point)
    {
        float sum = 0.0f;
        Vector2D<float> a, b;
        for (int i = 0; i < Vertices.Length - 1; i++)
        {
            // From point to first vertex
            a = Vertices[i] - point;
            a = Vector2D.Normalize(a);
            // From point to second vertex
            b = Vertices[i + 1] - point;
            b = Vector2D.Normalize(b);
            // Add angle to sum
            sum += Scalar.Acos(Vector2D.Dot(a, b));
        }
        // Have to add angle for last vertex and first vertex
        a = Vertices.Last() - point;
        a = Vector2D.Normalize(a);
        b = Vertices.First() - point;
        b = Vector2D.Normalize(b);
        sum += Scalar.Acos(Vector2D.Dot(a, b));
        // Return true if approximately 2pi
        return (sum - GameMath.TwoPi).NearZero();
    }
}