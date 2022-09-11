using Silk.NET.Maths;

namespace GameEngine.ObjConverter;

public class Vertex
{
    private const int NoIndex = -1;

    public Vertex(uint index, Vector3D<float> position)
    {
        Index = index;
        Position = position;
        Length = position.Length;
    }


    public uint Index { get; }

    public Vector3D<float> Position { get; }

    public float Length { get; }

    public bool IsSet => TextureIndex != NoIndex && NormalIndex != NoIndex;

    public int TextureIndex { get; set; } = NoIndex;

    public int NormalIndex { get; set; } = NoIndex;

    public Vertex? DuplicateVertex { get; set; } = null;

    public bool HasSameTextureAndNormal(int textureIndexOther,int normalIndexOther){
        return textureIndexOther == TextureIndex && normalIndexOther == NormalIndex;
    }
}