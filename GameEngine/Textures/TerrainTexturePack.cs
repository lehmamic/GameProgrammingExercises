namespace GameEngine.Textures;

public class TerrainTexturePack
{
    public TerrainTexturePack(TerrainTexture backgroundTexture, TerrainTexture rTexture, TerrainTexture gTexture, TerrainTexture bTexture)
    {
        BackgroundTexture = backgroundTexture;
        RTexture = rTexture;
        GTexture = gTexture;
        BTexture = bTexture;
    }

    public TerrainTexture BackgroundTexture { get; }

    public TerrainTexture RTexture { get; }

    public TerrainTexture GTexture { get; }

    public TerrainTexture BTexture { get; }
}