using Silk.NET.OpenGL;

namespace GameEngine.Textures;

public class ModelTexture : Texture
{
    public ModelTexture(GL gl, string path)
        : base(gl, path)
    {
    }

    // Spec Power

    public float ShineDamper { get; set; } = 1.0f;

    public float Reflectivity { get; set; } = 0.0f;

    public bool HasTransparency { get; set; }

    public bool UseFakeLighting { get; set; }
}