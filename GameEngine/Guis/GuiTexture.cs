using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Texture = GameEngine.Textures.Texture;

namespace GameEngine.Guis;

public class GuiTexture : Texture
{
    public GuiTexture(GL gl, uint handle, Vector2D<float> position, Vector2D<float> scale)
    : base(gl, handle)
    {
        Position = position;
        Scale = scale;
    }

    public GuiTexture(GL gl, string path, Vector2D<float> position, Vector2D<float> scale)
        : base(gl, path)
    {
        Position = position;
        Scale = scale;
    }

    public Vector2D<float> Position { get; set; }

    public Vector2D<float> Scale { get; set; }
}