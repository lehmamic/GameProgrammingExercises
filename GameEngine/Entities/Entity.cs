using GameEngine.Models;
using Silk.NET.Maths;

namespace GameEngine.Entities;

public class Entity
{
    private readonly int _textureIndex;

    private Vector3D<float> _position;
    private float _rotX;
    private float _rotY;
    private float _rotZ;
    private float _scale;

    public Entity(TexturedModel model, Vector3D<float> position, float rotX, float rotY, float rotZ, float scale)
    : this(model, 0, position, rotX, rotY, rotZ, scale)
    {
    }
    
    public Entity(TexturedModel model, int textureIndex, Vector3D<float> position, float rotX, float rotY, float rotZ, float scale)
    {
        Model = model;
        _textureIndex = textureIndex;
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

    public float TextureXOffset
    {
        get
        {
            int column = _textureIndex % Model.Texture.NumberOfRows;
            return (float) column / (float) Model.Texture.NumberOfRows;
        }
    }
    
    public float TextureYOffset
    {
        get
        {
            int row = _textureIndex / Model.Texture.NumberOfRows;
            return (float) row / (float) Model.Texture.NumberOfRows;
        }
    }
}