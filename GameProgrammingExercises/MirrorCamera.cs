using GameProgrammingExercises.Maths;
using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class MirrorCamera : CameraComponent
{
    // Actual position of camera
    private Vector3D<float> _actualPos;

    // Velocity of actual camera
    private Vector3D<float> _velocity;

    public MirrorCamera(Actor owner)
        : base(owner)
    {
    }

    /// <summary>
    /// Horizontal follow distance
    /// </summary>
    public float HorzDist { get; set; } = 150.0f;

    /// <summary>
    /// Vertical follow distance
    /// </summary>
    public float VertDist { get; set; } = 200.0f;

    /// <summary>
    /// Target distance
    /// </summary>
    public float TargetDist { get; set; } = 400.0f;

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        // Compute ideal position
        Vector3D<float> idealPos = ComputeCameraPos();

        // Target is target dist in front of owning actor
        Vector3D<float> target = Owner.Position - Owner.Forward * TargetDist;

        // Use actual position here, not ideal
        Matrix4X4<float> view = GameMath.CreateLookAt(idealPos, target, Vector3D<float>.UnitZ);
        Owner.Game.Renderer.MirrorViewMatrix = view;
    }

    public void SnapToIdeal()
    {
        Vector3D<float> idealPos = ComputeCameraPos();

        // Compute target and view
        Vector3D<float> target = Owner.Position - Owner.Forward * TargetDist;

        // Use actual position here, not ideal
        Matrix4X4<float> view = GameMath.CreateLookAt(idealPos, target, Vector3D<float>.UnitZ);
        Owner.Game.Renderer.MirrorViewMatrix = view;
    }

    private Vector3D<float> ComputeCameraPos()
    {
        // Set camera position in front of
        Vector3D<float> cameraPos = Owner.Position;
        cameraPos += Owner.Forward * HorzDist;
        cameraPos += Vector3D<float>.UnitZ * VertDist;;

        return cameraPos;
    }
}