using GameProgrammingExercises.Maths.Geometry;
using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class BallMove : MoveComponent
{
    public BallMove(Actor owner)
        : base(owner)
    {
    }
    
    public Actor Player { get; set; }

    public override void Update(float deltaTime)
    {
        // Construct segment in direction of travel
        float segmentLength = 30.0f;
        Vector3D<float> start = Owner.Position;
        Vector3D<float> dir = Owner.Forward;
        Vector3D<float> end = start + dir * segmentLength;

        // Create line segment
        LineSegment l = new(start, end);

        // Test segment vs world
        PhysWorld phys = Owner.Game.PhysWorld;

        // (Don't collide vs player)
        if (phys.SegmentCast(l, out var info) && info.Actor != Player)
        {
            // If we collided, reflect the ball about the normal
            dir = Vector3D.Reflect(dir, info.Normal);
            Owner.RotateToNewForward(dir);

            // Did we hit a target?
            if (info.Actor is TargetActor target)
            {
                ((BallActor)Owner).HitTarget();
            }
        }

        // Base class update moves based on forward speed
        base.Update(deltaTime);
    }
}