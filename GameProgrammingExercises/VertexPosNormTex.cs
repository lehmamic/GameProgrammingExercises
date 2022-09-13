using Silk.NET.Maths;

namespace GameProgrammingExercises;

public struct VertexPosNormTex
{
    public const uint SizeInBytes = 8 * sizeof(float);
    public const VertexArrayObjectLayout Layout = VertexArrayObjectLayout.PosNormTex;

    public Vector3D<float> Position;

    public Vector3D<float> Normal;

    public Vector2D<float> TexCoords;

    public VertexPosNormTex(Vector3D<float> position, Vector3D<float> normal, Vector2D<float> texCoords)
    {
        Position = position;
        Normal = normal;
        TexCoords = texCoords;
    }
}