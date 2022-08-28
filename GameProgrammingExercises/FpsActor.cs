using System.Numerics;
using GameProgrammingExercises.Maths;
using GameProgrammingExercises.Maths.Geometry;
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
    private readonly BoxComponent _boxComp;

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
        
        // Add a box component
        AABB myBox = new (new Vector3D<float>(-25.0f, -25.0f, -87.5f), new Vector3D<float>(25.0f, 25.0f, 87.5f));
        _boxComp = new BoxComponent(this)
        {
            ObjectBox = myBox,
            ShouldRotate = false
        };
    }

    public bool Visible
    {
        get => _meshComp.Visible;
        set => _meshComp.Visible = value;
    }
    
    public void Shoot()
    {
        // Get start point (in center of screen on near plane)
        Vector3D<float> screenPoint = new(0.0f, 0.0f, 0.0f);
        Vector3D<float> start = Game.Renderer.Unproject(screenPoint);

        // Get end point (in center of screen, between near and far)
        screenPoint.Z = 0.9f;
        Vector3D<float> end = Game.Renderer.Unproject(screenPoint);

        // Get direction vector
        Vector3D<float> dir = end - start;
        dir = Vector3D.Normalize(dir);

        // Spawn a ball
        BallActor ball = new BallActor(Game);
        ball.Player = this;
        ball.Position = start + dir * 20.0f;

        // Rotate the ball to face new direction
        ball.RotateToNewForward(dir);

        // Play shooting sound
        _audio.PlayEvent("event:/Shot");
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

        FixCollisions();

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
        var x = state.Mouse.RelativePosition.X;
        var y = state.Mouse.RelativePosition.Y;

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

    private void FixCollisions()
    {
        // Need to recompute my world transform to update world box
        ComputeWorldTransform();

        AABB playerBox = _boxComp.WorldBox;
        Vector3D<float> pos = Position;

        var planes = Game.Planes;
        foreach (var pa in planes)
        {
            // Do we collide with this PlaneActor?
            AABB planeBox = pa.Box.WorldBox;
            if (Collision.Intersect(playerBox, planeBox))
            {
                // Calculate all our differences
                float dx1 = planeBox.Max.X - playerBox.Min.X;
                float dx2 = planeBox.Min.X - playerBox.Max.X;
                float dy1 = planeBox.Max.Y - playerBox.Min.Y;
                float dy2 = planeBox.Min.Y - playerBox.Max.Y;
                float dz1 = planeBox.Max.Z - playerBox.Min.Z;
                float dz2 = planeBox.Min.Z - playerBox.Max.Z;

                // Set dx to whichever of dx1/dx2 have a lower abs
                float dx = Scalar.Abs(dx1) < Scalar.Abs(dx2) ? dx1 : dx2;
                // Ditto for dy
                float dy = Scalar.Abs(dy1) < Scalar.Abs(dy2) ? dy1 : dy2;
                // Ditto for dz
                float dz = Scalar.Abs(dz1) < Scalar.Abs(dz2) ? dz1 : dz2;

                // Whichever is closest, adjust x/y position
                if (Scalar.Abs(dx) <= Scalar.Abs(dy) && Scalar.Abs(dx) <= Scalar.Abs(dz))
                {
                    pos.X += dx;
                }
                else if (Scalar.Abs(dy) <= Scalar.Abs(dx) && Scalar.Abs(dy) <= Scalar.Abs(dz))
                {
                    pos.Y += dy;
                }
                else
                {
                    pos.Z += dz;
                }

                // Need to set position and update box component
                Position = pos;
                _boxComp.OnUpdateWorldTransform();
            }
        }
    }
}