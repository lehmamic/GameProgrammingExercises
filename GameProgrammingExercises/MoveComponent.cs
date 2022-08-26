using GameProgrammingExercises.Maths;
using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class MoveComponent : Component
{
    public MoveComponent(Actor owner, int updateOrder = 100)
        : base(owner, updateOrder)
    {
    }

    /// <summary>
    /// Controls rotation (radians/second)
    /// </summary>
    public float AngularSpeed { get; set; }

    /// <summary>
    /// Controls forward movement
    /// </summary>
    public float ForwardSpeed { get; set; }
    
    /// <summary>
    /// Controls strafe movement
    /// </summary>
    public float StrafeSpeed { get; set; }

    public override void Update(float deltaTime)
    {
        if (!AngularSpeed.NearZero())
        {
            float angle = AngularSpeed * deltaTime;

            // Create quaternion for incremental (Rotate about up axis)
            var increment = GameMath.CreateQuaternion(Vector3D<float>.UnitZ, angle);
            
            // Concatenate old an new quaternion
            Owner.Rotation = Quaternion<float>.Concatenate(Owner.Rotation, increment);
        }

        // Update position based on forward speed stays the same
        if (!ForwardSpeed.NearZero() || !StrafeSpeed.NearZero())
        {
            var pos = Owner.Position;
            pos += Owner.Forward * ForwardSpeed * deltaTime;

            // Update position based on strafe
            pos += Owner.Right * StrafeSpeed * deltaTime;

            Owner.Position = pos;
        }
    }
}