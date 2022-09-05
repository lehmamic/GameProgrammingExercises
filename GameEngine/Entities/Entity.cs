using GameEngine.Models;
using Silk.NET.Maths;

namespace GameEngine.Entities;

public class Entity
{
    private Vector3D<float> _position;
    private float _rotX;
    private float _rotY;
    private float _rotZ;
    private float _scale;

    public Entity(TexturedModel model, Vector3D<float> position, float rotX, float rotY, float rotZ, float scale)
    {
        Model = model;
        Position = position;
        RotX = rotX;
        RotY = rotY;
        RotZ = rotZ;
        Scale = scale;
    }

    public void IncreasePosition(float dx, float dy, float dz)
    {
        _position.X += dx;
        _position.Y += dy;
        _position.Z += dz;
    }
    
    public void IncreaseRotation(float dx, float dy, float dz)
    {
        _rotX += dx;
        _rotY += dy;
        _rotZ += dz;
    }

    public TexturedModel Model { get; set; }

    public Vector3D<float> Position
    {
        get => _position;
        set => _position = value;
    }

    public float RotX
    {
        get => _rotX;
        set => _rotX = value;
    }

    public float RotY
    {
        get => _rotY;
        set => _rotY = value;
    }

    public float RotZ
    {
        get => _rotZ;
        set => _rotZ = value;
    }

    public float Scale
    {
        get => _scale;
        set => _scale = value;
    }
}