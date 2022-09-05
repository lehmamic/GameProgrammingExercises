using Silk.NET.Maths;

namespace GameEngine.Entities;

public class Light
{
    public Light(Vector3D<float> position, Vector3D<float> color)
    {
        Position = position;
        Color = color;
    }

    public Vector3D<float> Position { get; set; }

    public Vector3D<float> Color { get; set; }
}