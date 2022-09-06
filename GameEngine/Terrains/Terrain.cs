using GameEngine.Models;
using GameEngine.RenderEngine;
using GameEngine.Textures;
using Silk.NET.Maths;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace GameEngine.Terrains;

public class Terrain
{
    private const float Size = 800.0f;
    private const float MaxHeight = 40.0f;
    private const float MaxPixelColor = 256.0f * 256.0f * 256.0f;

    public Terrain(int gridX, int gridZ, Loader loader, TerrainTexturePack texturePack, TerrainTexture blendMap, string heightMap)
    {
        TexturePack = texturePack;
        BlendMap = blendMap;
        X = gridX * Size;
        Z = gridZ * Size;
        VAO = GenerateTerrain(loader, heightMap);
    }

    public float X { get; }

    public float Z { get; }

    public VertexArrayObject VAO { get; }

    public TerrainTexturePack TexturePack { get; }

    public TerrainTexture BlendMap { get; }

    private static VertexArrayObject GenerateTerrain(Loader loader, string heightMap)
    {
        var imageConfig = Configuration.Default.Clone();
        imageConfig.PreferContiguousImageBuffers = true;

        using var image = Image.Load<Rgba32>(imageConfig, heightMap);
        
        var vertexCount = image.Height;
        
        int count = vertexCount * vertexCount;
        float[] vertices = new float[count * 8];
        uint[] indices = new uint[6 * (vertexCount - 1) * (vertexCount - 1)];
        int vertexPointer = 0;

        for(int i = 0; i < vertexCount; i++){
            for(int j = 0; j < vertexCount; j++){
                // position
                vertices[vertexPointer * 8] = (float)j / ((float)vertexCount - 1) * Size;
                vertices[vertexPointer * 8 + 1] = GetHeight(j, i, image);
                vertices[vertexPointer * 8 + 2] = (float)i / ((float)vertexCount - 1) * Size;

                // normals
                var normal = CalculateNormal(j, i, image);
                vertices[vertexPointer * 8 + 3] = normal.X;
                vertices[vertexPointer * 8 + 4] = normal.Y;
                vertices[vertexPointer * 8 + 5] = normal.Z;

                // textureCoords
                vertices[vertexPointer * 8 + 6] = (float)j/((float)vertexCount - 1);
                vertices[vertexPointer * 8 + 7] = (float)i/((float)vertexCount - 1);

                vertexPointer++;
            }
        }
        int pointer = 0;
        for(int gz = 0; gz < vertexCount - 1; gz++){
            for(int gx = 0; gx < vertexCount - 1 ; gx++){
                int topLeft = (gz * vertexCount) + gx;
                int topRight = topLeft + 1;
                int bottomLeft = ((gz + 1) * vertexCount) + gx;
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

    private static Vector3D<float> CalculateNormal(int x, int z, Image<Rgba32> image)
    {
        float heightL = GetHeight(x - 1, z, image);
        float heightR = GetHeight(x + 1, z, image);
        float heightD = GetHeight(x, z - 1, image);
        float heightU = GetHeight(x, z + 1, image);

        var normal = new Vector3D<float>(heightL - heightR, 2.0f, heightD - heightU);
        return Vector3D.Normalize(normal);
    }

    private static float GetHeight(int x, int z, Image<Rgba32> image)
    {
        if (x < 0 || x >= image.Height || z < 0 || z >= image.Height)
        {
            return 0;
        }

        var pixelColor = image[x, z];
        float height = pixelColor.R * pixelColor.G * pixelColor.B;
        height += MaxPixelColor / 2.0f;
        height /= MaxPixelColor / 2.0f;
        height *= MaxHeight;

        // I don't know why, but my terrain is to height
        height -= 50;

        return height;
    }
}