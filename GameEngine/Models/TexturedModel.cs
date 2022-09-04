using GameEngine.Textures;

namespace GameEngine.Models;

public class TexturedModel
{
    public VertexArrayObject VAO { get; }
    public Texture Texture { get; }

    public TexturedModel(VertexArrayObject vao, Texture texture)
    {
        VAO = vao;
        Texture = texture;
    }
}