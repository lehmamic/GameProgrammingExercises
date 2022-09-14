using Silk.NET.Maths;

namespace GameProgrammingExercises;

public struct VertexPosNormSkinTex
{
    public const uint SizeInBytes = 8 * sizeof(float) + 8 * sizeof(byte);
    public const VertexArrayObjectLayout Layout = VertexArrayObjectLayout.PosNormSkinTex;

    public Vector3D<float> Position;

    public Vector3D<float> Normal;

    public Vector4D<byte> SkinningIndices;

    public Vector4D<byte> SkinningWeights;

    public Vector2D<float> TexCoords;

    public VertexPosNormSkinTex(Vector3D<float> position, Vector3D<float> normal, Vector4D<byte> skinningIndices,
        Vector4D<byte> skinningWeights, Vector2D<float> texCoords)
    {
        Position = position;
        Normal = normal;
        SkinningIndices = skinningIndices;
        SkinningWeights = skinningWeights;
        TexCoords = texCoords;
    }
}