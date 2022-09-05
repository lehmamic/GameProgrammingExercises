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
        if (keyboard.IsKeyPressed(Key.W))
        {
            _position.Z -= 0.02f;
        }
        if (keyboard.IsKeyPressed(Key.D))
        {
            _position.X += 0.02f;
        }
        if (keyboard.IsKeyPressed(Key.A))
        {
            _position.X -= 0.02f;
        }
    }
}