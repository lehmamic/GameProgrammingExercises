using GameProgrammingExercises.Maths;
using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class MoveComponent : Component
{
    public MoveComponent(Actor owner, int updateOrder = 100)
        : base(owner, updateOrder)
    {
    }
    
    public float AngularSpeed { get; set; }

    public float ForwardSpeed { get; set; }

    public override void Update(float deltaTime)
    {
        if (deltaTime.NearZero())
        {
            Owner.Rotation += AngularSpeed * deltaTime;
        }

        if (deltaTime.NearZero())
        {
            Vector2D<float> pos = Owner.Position;
            pos += Owner.Forward * ForwardSpeed * deltaTime;
            
            // Screen wrapping (for asteroids)
            if (pos.X < -512.0f) {pos.X = 510.0f; }
            else if (pos.X > 512.0f) { pos.X = -510.0f; }
            if (pos.Y < -384.0f) { pos.Y = 382.0f; }
            else if (pos.Y > 384.0f) { pos.Y = -382.0f; }
            
            Owner.Position = pos;
        }
    }
}