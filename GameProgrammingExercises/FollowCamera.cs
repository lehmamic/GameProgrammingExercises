using GameProgrammingExercises.Maths;
using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class FollowCamera : CameraComponent
{
    // Actual position of camera
    private Vector3D<float> _actualPos;

    // Velocity of actual camera
    private Vector3D<float> _velocity;

    public FollowCamera(Actor owner)
        : base(owner)
    {
    }

    /// <summary>
    /// Horizontal follow distance
    /// </summary>
    public float HorzDist { get; set; } = 350.0f;

    /// <summary>
    /// Vertical follow distance
    /// </summary>
    public float VertDist { get; set; } = 150.0f;

    /// <summary>
    /// Target distance
    /// </summary>
    public float TargetDist { get; set; } = 100.0f;

    /// <summary>
    /// Spring constant (higher is more stiff)
    /// </summary>
    public float SpringConstant { get; set; } = 64.0f;

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        // Compute dampening from spring constant
        float dampening = 2.0f * Scalar.Sqrt(SpringConstant);
    
        // Compute ideal position
        Vector3D<float> idealPos = ComputeCameraPos();
    
        // Compute difference between actual and ideal
        Vector3D<float> diff = _actualPos - idealPos;
    
        // Compute acceleration of spring
        Vector3D<float> acel = -SpringConstant * diff - dampening * _velocity;
    
        // Update velocity
        _velocity += acel * deltaTime;
    
        // Update actual camera position
        _actualPos += _velocity * deltaTime;
    
        // Target is target dist in front of owning actor
        Vector3D<float> target = Owner.Position + Owner.Forward * TargetDist;
    
        // Use actual position here, not ideal
        // (Up is just UnitZ since we don't flip the camera)
        Vector3D<float> up = Vector3D<float>.UnitZ;
        Matrix4X4<float> view = GameMath.CreateLookAt(_actualPos, target, up);
        SetViewMatrix(view);
    }

    public void SnapToIdeal()
    {
        // Set actual position to ideal
        _actualPos = ComputeCameraPos();

        // Zero velocity
        _velocity = Vector3D<float>.Zero;
    
        // Compute target and view
        Vector3D<float> target = Owner.Position + Owner.Forward * TargetDist;

        Matrix4X4<float> view = GameMath.CreateLookAt(_actualPos, target, Vector3D<float>.UnitZ);
        SetViewMatrix(view);
    }

    private Vector3D<float> ComputeCameraPos()
    {
        // Set camera position behind and above owner
        Vector3D<float> cameraPos = Owner.Position;
        cameraPos -= Owner.Forward * HorzDist;
        cameraPos += Vector3D<float>.UnitZ * VertDist;
    
        return cameraPos;
    }
}