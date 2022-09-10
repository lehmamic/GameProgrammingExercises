namespace GameEngine.Water;

public class WaterTile
{
    public const float TileSize = 60.0f;

    public WaterTile(float centerX, float centerZ, float height)
    {
        Height = height;
        X = centerX;
        Z = centerZ;
    }

    public float Height { get; }

    public float X { get; }

    public float Z { get; }
}