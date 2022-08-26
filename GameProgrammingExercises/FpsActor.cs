using GameProgrammingExercises.Maths;
using Silk.NET.Input;
using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class FpsActor : Actor
{
    private readonly MoveComponent _move;
    private readonly AudioComponent _audio;
    private readonly SoundEvent _footstep;
    private readonly FpsCamera _cameraComp;
    private readonly Actor _fpsModel;
    private readonly MeshComponent _meshComp;

    private float _lastFootstep;

    public FpsActor(Game game)
        : base(game)
    {
        _move = new MoveComponent(this);
        _audio = new AudioComponent(this);
        _lastFootstep = 0.0f;
        _footstep = _audio.PlayEvent("event:/Footstep");
        _footstep.SetPaused(true);

        _cameraComp = new FpsCamera(this);

        _fpsModel = new Actor(game)
        {
            Scale = 0.75f
        };
        _meshComp = new MeshComponent(_fpsModel)
        {
            Mesh = Game.Renderer.GetMesh("Assets/Rifle.gpmesh")
        };
    }

    public bool Visible
    {
        get => _meshComp.Visible;
        set => _meshComp.Visible = value;
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

        // Update position of FPS model relative to actor position
        var modelOffset = new Vector3D<float>(10.0f, 10.0f, -10.0f);
        Vector3D<float> modelPos = Position;
        modelPos += Forward * modelOffset.X;
        modelPos += Right * modelOffset.Y;
        modelPos.Z += modelOffset.Z;
        _fpsModel.Position = modelPos;
    
        // Initialize rotation to actor rotation
        var q = Rotation;
    
        // Rotate by pitch from camera
        q = Quaternion<float>.Concatenate(q, GameMath.CreateQuaternion(Right, _cameraComp.Pitch));

        _fpsModel.Rotation = q;
    }

    protected override void ActorInput(InputState state)
    {
        float forwardSpeed = 0.0f;
        float strafeSpeed = 0.0f;

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
            strafeSpeed -= 400.0f;
        }

        if (state.Keyboard.GetKeyValue(Key.D))
        {
            strafeSpeed += 400.0f;
        }

        _move.ForwardSpeed = forwardSpeed;
        _move.StrafeSpeed = strafeSpeed;

        // Mouse movement
    
        // Get relative movement from SDL
        var x = state.Mouse.Position.X;
        var y = state.Mouse.Position.Y;

        // Assume mouse movement is usually between -500 and +500
        int maxMouseSpeed = 500;
    
        // Rotation/sec at maximum speed
        float maxAngularSpeed = Scalar<float>.Pi * 8;

        float angularSpeed = 0.0f;
        if (x != 0)
        {
            // Convert to ~[-1.0, 1.0]
            angularSpeed = x / maxMouseSpeed;
        
            // Multiply by rotation/sec
            angularSpeed *= maxAngularSpeed;
        }

        _move.AngularSpeed = angularSpeed;

        // Compute pitch
        float maxPitchSpeed = Scalar<float>.Pi * 8;
        float pitchSpeed = 0.0f;
        if (y != 0)
        {
            // Convert to ~[-1.0, 1.0]
            pitchSpeed = y / maxMouseSpeed;
            pitchSpeed *= maxPitchSpeed;
        }

        _cameraComp.PitchSpeed = pitchSpeed;
    }
}