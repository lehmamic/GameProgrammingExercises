using GameEngine.Textures;

namespace GameEngine.Models;

public class TexturedModel
{
    public RawModel RawModel { get; }
    public ModelTexture Texture { get; }

    public TexturedModel(RawModel rawRawModel, ModelTexture texture)
    {
        RawModel = rawRawModel;
        Texture = texture;
    }
}