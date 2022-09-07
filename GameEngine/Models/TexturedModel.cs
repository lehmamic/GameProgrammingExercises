using GameEngine.Textures;

namespace GameEngine.Models;

public class TexturedModel
{
    public VertexArrayObject VAO { get; }
    public ModelTexture Texture { get; }

    public TexturedModel(VertexArrayObject vao, ModelTexture texture)
    {
        VAO = vao;
        Texture = texture;
    }
}