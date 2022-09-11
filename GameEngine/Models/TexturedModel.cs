using GameEngine.Textures;

namespace GameEngine.Models;

public class TexturedModel
{
    public VertexArrayObject VAO { get; }
    public ModelTexture Texture { get; }

    public ModelTexture? NormalMap { get; }

    public TexturedModel(VertexArrayObject vao, ModelTexture texture, ModelTexture? normalMap = null)
    {
        VAO = vao;
        Texture = texture;
        NormalMap = normalMap;
    }
}