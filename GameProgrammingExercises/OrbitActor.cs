using Silk.NET.Input;
using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class OrbitActor : Actor
{
    private readonly MeshComponent _meshComp;
    private readonly OrbitCamera _cameraComp;

    public OrbitActor(Game game) 
        : base(game)
    {
        _meshComp = new MeshComponent(this)
        {
            Mesh = Game.Renderer.GetMesh("Assets/RacingCar.gpmesh")
        };
        Position = new Vector3D<float>(0.0f, 0.0f, -100.0f);

        _cameraComp = new OrbitCamera(this);
    }

    public bool Visible
    {
        get => _meshComp.Visible;
        set => _meshComp.Visible = value;
    }

    protected override void ActorInput(InputState state)
    {
        // Mouse rotation
        // Get relative movement from SDL
        var x = state.Mouse.RelativePosition.X;
        var y = state.Mouse.RelativePosition.Y;

        // Only apply rotation if right-click is held
        if (state.Mouse.GetButtonValue(MouseButton.Right))
        {
            // Assume mouse movement is usually between -500 and +500
            int maxMouseSpeed = 500;
            // Rotation/sec at maximum speed
            float maxOrbitSpeed = Scalar<float>.Pi * 8;
            float yawSpeed = 0.0f;
            if (x != 0)
            {
                // Convert to ~[-1.0, 1.0]
                yawSpeed = x / maxMouseSpeed;

                // Multiply by rotation/sec
                yawSpeed *= maxOrbitSpeed;
            }

            _cameraComp.YawSpeed = -yawSpeed;

            // Compute pitch
            float pitchSpeed = 0.0f;
            if (y != 0)
            {
                // Convert to ~[-1.0, 1.0]
                pitchSpeed = y / maxMouseSpeed;
                pitchSpeed *= maxOrbitSpeed;
            }

            _cameraComp.PitchSpeed = pitchSpeed;
        }
    }
}