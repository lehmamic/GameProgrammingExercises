using GameEngine.Textures;

namespace GameEngine.Models;

public class TexturedModel
{
    public VertexArrayObject VAO { get; }
    public ModelTexture ModelTexture { get; }

    public TexturedModel(VertexArrayObject vao, ModelTexture modelTexture)
    {
        VAO = vao;
        ModelTexture = modelTexture;
    }
}