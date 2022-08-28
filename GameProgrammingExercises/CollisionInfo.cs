using Silk.NET.Maths;

namespace GameProgrammingExercises;

/// <summary>
/// Used to give helpful information about collision results
/// </summary>
public class CollisionInfo
{
    // Point of collision
    public Vector3D<float> Point;

    // Normal at collision
    public Vector3D<float> Normal;

    // Component collided with
    public BoxComponent Box;

    // Owning actor of component
    public Actor Actor;

    public CollisionInfo(Vector3D<float> point, Vector3D<float> normal, BoxComponent box, Actor actor)
    {
        Point = point;
        Normal = normal;
        Box = box;
        Actor = actor;
    }
}