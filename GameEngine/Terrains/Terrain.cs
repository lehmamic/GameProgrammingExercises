using GameEngine.Models;
using GameEngine.RenderEngine;
using GameEngine.Textures;

namespace GameEngine.Terrains;

public class Terrain
{
    private const float Size = 800.0f;
    private const int VertexCount = 128;

    public Terrain(int gridX, int gridZ, Loader loader, TerrainTexturePack texturePack, TerrainTexture blendMap)
    {
        TexturePack = texturePack;
        BlendMap = blendMap;
        X = gridX * Size;
        Z = gridZ * Size;
        VAO = GenerateTerrain(loader);
    }

    public float X { get; }

    public float Z { get; }

    public VertexArrayObject VAO { get; }

    public TerrainTexturePack TexturePack { get; }

    public TerrainTexture BlendMap { get; }

    private static VertexArrayObject GenerateTerrain(Loader loader){
        int count = VertexCount * VertexCount;
        float[] vertices = new float[count * 8];
        // float[] vertices = new float[count * 3];
        // float[] normals = new float[count * 3];
        // float[] textureCoords = new float[count * 2];
        uint[] indices = new uint[6 * (VertexCount - 1) * (VertexCount - 1)];
        int vertexPointer = 0;

        for(int i = 0; i < VertexCount; i++){
            for(int j = 0; j < VertexCount; j++){
                // position
                vertices[vertexPointer * 8] = (float)j / ((float)VertexCount - 1) * Size;
                vertices[vertexPointer * 8 + 1] = 0;
                vertices[vertexPointer * 8 + 2] = (float)i / ((float)VertexCount - 1) * Size;

                // normals
                vertices[vertexPointer * 8 + 3] = 0;
                vertices[vertexPointer * 8 + 4] = 1;
                vertices[vertexPointer * 8 + 5] = 0;

                // textureCoords
                vertices[vertexPointer * 8 + 6] = (float)j/((float)VertexCount - 1);
                vertices[vertexPointer * 8 + 7] = (float)i/((float)VertexCount - 1);

                // vertices[vertexPointer*3] = (float)j / ((float)VertexCount - 1) * Size;
                // vertices[vertexPointer*3+1] = 0;
                // vertices[vertexPointer*3+2] = (float)i / ((float)VertexCount - 1) * Size;
                // normals[vertexPointer*3] = 0;
                // normals[vertexPointer*3+1] = 1;
                // normals[vertexPointer*3+2] = 0;
                // textureCoords[vertexPointer*2] = (float)j/((float)VertexCount - 1);
                // textureCoords[vertexPointer*2+1] = (float)i/((float)VertexCount - 1);
                vertexPointer++;
            }
        }
        int pointer = 0;
        for(int gz = 0; gz < VertexCount - 1; gz++){
            for(int gx = 0; gx < VertexCount - 1 ; gx++){
                int topLeft = (gz * VertexCount) + gx;
                int topRight = topLeft + 1;
                int bottomLeft = ((gz + 1) * VertexCount) + gx;
                int bottomRight = bottomLeft + 1;
                indices[pointer++] = (uint)topLeft;
                indices[pointer++] = (uint)bottomLeft;
                indices[pointer++] = (uint)topRight;
                indices[pointer++] = (uint)topRight;
                indices[pointer++] = (uint)bottomLeft;
                indices[pointer++] = (uint)bottomRight;
            }
        }

        return loader.LoadToVAO(vertices, indices);
    }
}