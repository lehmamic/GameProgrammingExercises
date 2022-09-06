using Silk.NET.Input;
using Silk.NET.Maths;

namespace GameEngine.Entities;

public class Camera
{
    private Vector3D<float> _position;

    public Camera()
    {

    }

    public Vector3D<float> Position
    {
        get => _position;
        set => _position = value;
    }

    public float Pitch { get; set; }

    public float Yaw { get; set; }

    public float Roll { get; set; }

    public void Move(IKeyboard keyboard)
    {
        if (keyboard.IsKeyPressed(Key.Number1))
        {
            _position.Y += 0.5f;
        }
        else if (keyboard.IsKeyPressed(Key.Number2))
        {
            _position.Y -= 0.5f;
        }
    }
}