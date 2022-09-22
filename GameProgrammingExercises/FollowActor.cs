using GameProgrammingExercises.Maths;
using Silk.NET.Input;
using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class FollowActor : Actor
{
    private readonly SkeletalMeshComponent _meshComp;
    private readonly MoveComponent _moveComp;
    private readonly FollowCamera _cameraComp;

    private bool _moving = false;

    public FollowActor(Game game)
        : base(game)
    {
        _meshComp = new SkeletalMeshComponent(this)
        {
            Mesh = Game.Renderer.GetMesh("Assets/CatWarrior.gpmesh"),
            Skeleton = Game.GetSkeleton("Assets/CatWarrior.gpskel"),
        };
        _meshComp.PlayAnimation(Game.GetAnimation("Assets/CatActionIdle.gpanim"));
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

        // Did we just start moving?
        if (!_moving && !forwardSpeed.NearZero())
        {
            _moving = true;
            _meshComp.PlayAnimation(Game.GetAnimation("Assets/CatRunSprint.gpanim"), 1.25f);
        }
        // Or did we just stop moving?
        else if (_moving && forwardSpeed.NearZero())
        {
            _moving = false;
            _meshComp.PlayAnimation(Game.GetAnimation("Assets/CatActionIdle.gpanim"));
        }

        _moveComp.ForwardSpeed = forwardSpeed;
        _moveComp.AngularSpeed = angularSpeed;
    }
}