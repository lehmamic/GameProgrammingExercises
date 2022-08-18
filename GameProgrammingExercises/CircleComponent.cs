using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class CircleComponent : Component
{
    public CircleComponent(Actor owner)
        : base(owner)
    {
    }
    
    public float Radius { get; set; }

    public Vector2D<float> Center => Owner.Position;

    public static bool Intersect(CircleComponent a, CircleComponent b)
    {
        // Calculate distance squared
        Vector2D<float> diff = a.Center - b.Center;
        float distSq = diff.LengthSquared;

        // Calculate sum of radii squared
        float radiiSq = a.Radius + b.Radius;
        radiiSq *= radiiSq;

        return distSq <= radiiSq;
    }
}