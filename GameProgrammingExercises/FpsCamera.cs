using GameProgrammingExercises.Maths;
using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class FpsCamera : CameraComponent
{
    public FpsCamera(Actor owner)
        : base(owner)
    {
    }
    
    /// <summary>
    /// Rotation/sec speed of pitch
    /// </summary>
    public float PitchSpeed { get; set; } = 0.0f;

    /// <summary>
    /// Maximum pitch deviation from forward
    /// </summary>
    public float MaxPitch { get; set; } = Scalar<float>.Pi / 3.0f;

    /// <summary>
    /// Current pitch
    /// </summary>
    public float Pitch { get; private set; } = 0.0f;

    public override void Update(float deltaTime)
    {
        // Call parent update (doesn't do anything right now)
        base.Update(deltaTime);
    
        // Camera position is owner position
        Vector3D<float> cameraPos = Owner.Position;
    
        // Update pitch based on pitch speed
        Pitch += PitchSpeed * deltaTime;
    
        // Clamp pitch to [-max, +max]
        Pitch = Math.Clamp(Pitch, -MaxPitch, MaxPitch);
    
        // Make a quaternion representing pitch rotation
        // which is about owner's right vector
        var q = GameMath.CreateQuaternion(Owner.Right, Pitch);
    
        // Rotate owner forward by pitch quaternion
        Vector3D<float> viewForward = Vector3D.Transform(Owner.Forward, q);
    
        // Target position 100 units in front of owner
        Vector3D<float> target = cameraPos + viewForward * 100.0f;
    
        // Also rotate up vector by pitch quaternion
        Vector3D<float> up = Vector3D.Transform(Vector3D<float>.UnitZ, q);
    
        // Create look at Matrix, set as view
        Matrix4X4<float> view = GameMath.CreateLookAt(cameraPos, target, up);
        SetViewMatrix(view);
    }
}