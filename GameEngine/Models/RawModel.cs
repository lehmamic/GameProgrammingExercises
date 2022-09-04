namespace GameEngine.Models;

public class RawModel
{
    public RawModel(uint vaoId, uint vertexCount)
    {
        VaoId = vaoId;
        VertexCount = vertexCount;
    }

    public uint VaoId { get; }

    public uint VertexCount { get; }
}