using GameProgrammingExercises.Maths;
using Silk.NET.Input;
using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class CameraActor : Actor
{
    private readonly MoveComponent _move;
    private readonly AudioComponent _audio;
    private readonly SoundEvent _footstep;

    private float _lastFootstep;

    public CameraActor(Game game)
        : base(game)
    {
        _move = new MoveComponent(this);
        _audio = new AudioComponent(this);
        _lastFootstep = 0.0f;
        _footstep = _audio.PlayEvent("event:/Footstep");
        _footstep.SetPaused(true);
    }
    
    public void SetFootstepSurface(float value)
    {
        // Pause here because the way I setup the parameter in FMOD
        // changing it will play a footstep
        _footstep.SetPaused(true);
        _footstep.SetParameter("Surface", value);
    }

    protected override void UpdateActor(float deltaTime)
    {
        base.UpdateActor(deltaTime);

        // Play the footstep if we're moving and haven't recently
        _lastFootstep -= deltaTime;
        if (!_move.ForwardSpeed.NearZero() && _lastFootstep <= 0.0f)
        {
            _footstep.SetPaused(false);
            _footstep.Restart();
            _lastFootstep = 0.5f;
        }
        
        // Compute new camera from this actor
        Vector3D<float> cameraPosition = Position;
        Vector3D<float> target = Position + Forward * 100.0f;
        Vector3D<float> up = Vector3D<float>.UnitZ;

        Matrix4X4<float> view = GameMath.CreateLookAt(cameraPosition, target, up);
        Game.Renderer.ViewMatrix = view;
        Game.AudioSystem.SetListener(view);
    }

    protected override void ActorInput(InputState state)
    {
        float forwardSpeed = 0.0f;
        float angularSpeed = 0.0f;

        // wasd movement
        if (state.Keyboard.GetKeyValue(Key.W))
        {
            forwardSpeed += 300.0f;
        }
    
        if (state.Keyboard.GetKeyValue(Key.S))
        {
            forwardSpeed -= 300.0f;
        }
    
        if (state.Keyboard.GetKeyValue(Key.A))
        {
            angularSpeed -= GameMath.TwoPi;
        }

        if (state.Keyboard.GetKeyValue(Key.D))
        {
            angularSpeed += GameMath.TwoPi;
        }

        _move.ForwardSpeed = forwardSpeed;
        _move.AngularSpeed = angularSpeed;
    }
}