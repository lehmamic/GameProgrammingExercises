using GameProgrammingExercises.Maths;
using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class OrbitCamera : CameraComponent
{
    // Offset from target
    private Vector3D<float> _offset = new(-400.0f, 0.0f, 0.0f);

    // Up vector of camera
    private Vector3D<float> _up = Vector3D<float>.UnitZ;

    public OrbitCamera(Actor owner)
        : base(owner)
    {
    }

    /// <summary>
    /// Rotation/sec speed of pitch
    /// </summary>
    public float PitchSpeed { get; set; }

    /// <summary>
    /// Rotation/sec speed of yaw
    /// </summary>
    public float YawSpeed { get; set; }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        // Create a quaternion for yaw about world up
        var yaw = GameMath.CreateQuaternion(Vector3D<float>.UnitZ, YawSpeed * deltaTime);

        // Transform offset and up by yaw
        _offset = Vector3D.Transform(_offset, yaw);
        _up = Vector3D.Transform(_up, yaw);

        // Compute camera forward/right from these vectors
        // Forward owner.position - (owner.position + offset)
        // = -offset
        Vector3D<float> forward = -1.0f * _offset;
        forward = Vector3D.Normalize(forward);
        Vector3D<float> right = Vector3D.Cross(_up, forward);
        right = Vector3D.Normalize(right);

        // Create quaternion for pitch about camera right
        var pitch = GameMath.CreateQuaternion(right, PitchSpeed * deltaTime);

        // Transform camera offset and up by pitch
        _offset = Vector3D.Transform(_offset, pitch);
        _up = Vector3D.Transform(_up, pitch);

        // Compute transform matrix
        Vector3D<float> target = Owner.Position;
        Vector3D<float> cameraPos = target + _offset;
        Matrix4X4<float> view = GameMath.CreateLookAt(cameraPos, target, _up);
        SetViewMatrix(view);
    }
}