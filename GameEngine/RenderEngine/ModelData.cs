namespace GameEngine.RenderEngine;

public class ModelData
{
    public ModelData(float[] vertices, uint[] indices, float furthestPoint)
    {
        Vertices = vertices;
        Indices = indices;
        FurthestPoint = furthestPoint;
    }

    public float[] Vertices { get; }

    public uint[] Indices { get; }

    public float FurthestPoint { get; }
}