using Silk.NET.Maths;

namespace GameEngine.Entities;

public class Light
{
    public Light(Vector3D<float> position, Vector3D<float> color)
    : this(position, color, new(1.0f, 0.0f, 0.0f))
    {
    }

    public Light(Vector3D<float> position, Vector3D<float> color, Vector3D<float> attenuation)
    {
        Position = position;
        Color = color;
        Attenuation = attenuation;
    }

    public Vector3D<float> Position { get; set; }

    public Vector3D<float> Color { get; set; }

    public Vector3D<float> Attenuation { get; set; }
}