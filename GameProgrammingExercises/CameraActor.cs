using GameProgrammingExercises.Maths;
using Silk.NET.Input;
using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class CameraActor : Actor
{
    private readonly MoveComponent _move;

    public CameraActor(Game game)
        : base(game)
    {
        _move = new MoveComponent(this);
    }

    protected override void UpdateActor(float deltaTime)
    {
        base.UpdateActor(deltaTime);
        
        // Compute new camera from this actor
        Vector3D<float> cameraPosition = Position;
        Vector3D<float> target = Position + Forward * 100.0f;
        Vector3D<float> up = Vector3D<float>.UnitZ;

        Matrix4X4<float> view = GameMath.CreateLookAt(cameraPosition, target, up);
        Game.Renderer.ViewMatrix = view;
    }

    protected override void ActorInput(IKeyboard keyboard)
    {
        float forwardSpeed = 0.0f;
        float angularSpeed = 0.0f;

        // wasd movement
        if (keyboard.IsKeyPressed(Key.W))
        {
            forwardSpeed += 300.0f;
        }
    
        if (keyboard.IsKeyPressed(Key.S))
        {
            forwardSpeed -= 300.0f;
        }
    
        if (keyboard.IsKeyPressed(Key.A))
        {
            angularSpeed -= GameMath.TwoPi;
        }
    
        if (keyboard.IsKeyPressed(Key.D))
        {
            angularSpeed += GameMath.TwoPi;
        }

        _move.ForwardSpeed = forwardSpeed;
        _move.AngularSpeed = angularSpeed;
    }
}