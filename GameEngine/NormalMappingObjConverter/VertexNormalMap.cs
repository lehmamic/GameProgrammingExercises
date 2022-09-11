using Silk.NET.Maths;

namespace GameEngine.NormalMappingObjConverter;

public class VertexNormalMap
{
    private const int NoIndex = -1;

    private readonly List<Vector3D<float>> _tangents = new();

    private Vector3D<float> _averagedTangent = Vector3D<float>.Zero;

    public VertexNormalMap(uint index, Vector3D<float> position)
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

    public VertexNormalMap? DuplicateVertex { get; set; } = null;

    public Vector3D<float> AveragedTangent => _averagedTangent;

    public void AddTangent(Vector3D<float> tangent)
    {
        _tangents.Add(tangent);
    }

    public void AverageTangents()
    {
        if(!_tangents.Any())
        {
            return;
        }

        foreach (var tangent in _tangents){
            _averagedTangent += tangent;
        }

        _averagedTangent = Vector3D.Normalize(_averagedTangent);
    }

    public bool HasSameTextureAndNormal(int textureIndexOther,int normalIndexOther){
        return textureIndexOther == TextureIndex && normalIndexOther == NormalIndex;
    }
}