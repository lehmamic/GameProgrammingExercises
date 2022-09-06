using GameEngine.Models;
using Silk.NET.Input;
using Silk.NET.Maths;

namespace GameEngine.Entities;

public class Player : Entity
{
    private const float RunSpeed = 20.0f;
    private const float TurnSpeed = 160.0f;
    private const float Gravity = -50.0f;
    private const float JumpPower = 30.0f;

    private static float TerrainHeight = 0.0f;

    private float _currentSpeed = 0.0f;
    private float _currentTurnSpeed = 0.0f;
    private float _upwardsSpeed = 0.0f;

    private bool _isInAir = false;

    public Player(TexturedModel model, Vector3D<float> position, float rotX, float rotY, float rotZ, float scale)
        : base(model, position, rotX, rotY, rotZ, scale)
    {
    }

    public void Move(float deltaTime, IKeyboard keyboard)
    {
        CheckInputs(keyboard);

        IncreaseRotation(0.0f, _currentTurnSpeed * deltaTime, 0.0f);

        float distance = _currentSpeed * deltaTime;
        float dx = distance * Scalar.Sin(Scalar.DegreesToRadians(RotY));
        float dz = distance * Scalar.Cos(Scalar.DegreesToRadians(RotY));
        IncreasePosition(dx, 0.0f, dz);

        _upwardsSpeed += Gravity * deltaTime;
        IncreasePosition(0.0f, _upwardsSpeed * deltaTime, 0.0f);
        if (Position.Y < TerrainHeight)
        {
            _upwardsSpeed = 0.0f;
            _isInAir = false;
            Position = new Vector3D<float>(Position.X, TerrainHeight, Position.Z);
        }
    }

    private void Jump()
    {
        if (!_isInAir)
        {
            _upwardsSpeed = JumpPower;
            _isInAir = true;
        }
    }

    private void CheckInputs(IKeyboard keyboard)
    {
        if (keyboard.IsKeyPressed(Key.W))
        {
            _currentSpeed = RunSpeed;
        }
        else if (keyboard.IsKeyPressed(Key.S))
        {
            _currentSpeed = -RunSpeed;
        }
        else
        {
            _currentSpeed = 0;
        }

        if (keyboard.IsKeyPressed(Key.D))
        {
            _currentTurnSpeed = -TurnSpeed;
        }
        else if (keyboard.IsKeyPressed(Key.A))
        {
            _currentTurnSpeed = TurnSpeed;
        }
        else
        {
            _currentTurnSpeed = 0;
        }

        if (keyboard.IsKeyPressed(Key.Space))
        {
            Jump();
        }
    }
}