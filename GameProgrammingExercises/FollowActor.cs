using GameProgrammingExercises.Maths;
using Silk.NET.Input;
using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class FollowActor : Actor
{
    private readonly MeshComponent _meshComp;
    private readonly MoveComponent _moveComp;
    private readonly FollowCamera _cameraComp;

    public FollowActor(Game game)
        : base(game)
    {
        _meshComp = new MeshComponent(this)
        {
            Mesh = Game.Renderer.GetMesh("Assets/RacingCar.gpmesh")
        };
        Position = new Vector3D<float>(0.0f, 0.0f, -100.0f);

        _moveComp = new MoveComponent(this);
        _cameraComp = new FollowCamera(this);
        _cameraComp.SnapToIdeal();
    }
    
    public bool Visible
    {
        get => _meshComp.Visible;
        set => _meshComp.Visible = value;
    }

    protected override void ActorInput(InputState state)
    {
        float forwardSpeed = 0.0f;
        float angularSpeed = 0.0f;

        // wasd movement
        if (state.Keyboard.GetKeyValue(Key.W))
        {
            forwardSpeed += 400.0f;
        }
        if (state.Keyboard.GetKeyValue(Key.S))
        {
            forwardSpeed -= 400.0f;
        }
        if (state.Keyboard.GetKeyValue(Key.A))
        {
            angularSpeed -= Scalar<float>.Pi;
        }
        if (state.Keyboard.GetKeyValue(Key.D))
        {
            angularSpeed += Scalar<float>.Pi;
        }

        _moveComp.ForwardSpeed = forwardSpeed;
        _moveComp.AngularSpeed = angularSpeed;

        // Adjust horizontal distance of camera based on speed
        if (!forwardSpeed.NearZero())
        {
            _cameraComp.HorzDist = 500.0f;
        }
        else
        {
            _cameraComp.HorzDist = 350.0f;
        }
    }
}