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
        if (!AngularSpeed.NearZero())
        {
            Owner.Rotation += AngularSpeed * deltaTime;
        }

        if (!ForwardSpeed.NearZero())
        {
            Vector2D<float> position = Owner.Position;
            position += Owner.Forward * ForwardSpeed * deltaTime;

            // Screen wrapping (for asteroids)
            if (position.X < -512.0f)
            {
                position.X = 510.0f;
            }
            else if (position.X > 512.0f)
            {
                position.X = -510.0f;
            }

            if (position.Y < -384.0f)
            {
                position.Y = 382.0f;
            }
            else if (position.Y > 384.0f)
            {
                position.Y = -382.0f;
            }
            
            Owner.Position = position;
        }
    }
}