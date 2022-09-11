using GameEngine.Models;
using GameEngine.RenderEngine;
using GameEngine.Textures;
using GameEngine.Toolbox;
using Silk.NET.Maths;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace GameEngine.Terrains;

public class Terrain
{
    private const float Size = 150.0f;
    private const float MaxHeight = 40.0f;
    private const float MaxPixelColor = 256.0f; // 256.0f * 256.0f * 256.0f; its grayscale so we use only one pixel attribute

    private float[,]? _heights;

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

    public float GetHeightOfTerrain(float worldX, float worldZ)
    {
        float terrainX = worldX - X;
        float terrainZ = worldZ - Z;

        float gridSquareSize = Size / ((float) _heights!.GetLength(0) - 1);
        int gridX = (int) Scalar.Floor(terrainX / gridSquareSize);
        int gridZ = (int) Scalar.Floor(terrainZ / gridSquareSize);
        if (gridX >= _heights!.GetLength(0) - 1 || gridZ >= _heights!.GetLength(0) - 1 || gridX < 0 || gridZ < 0)
        {
            return 0;
        }

        // calculate the coord on the square
        float xCoord = (terrainX % gridSquareSize) / gridSquareSize;
        float zCoord = (terrainZ % gridSquareSize) / gridSquareSize;
        
        // We will du Barycentryc interpolation
        float answer;
        
        // First, find out on which triangle we are
        if (xCoord <= (1-zCoord)) {
            answer = Maths
                .BarryCentric(
                    new Vector3D<float>(0, _heights[gridX, gridZ], 0),
                    new Vector3D<float>(1, _heights[gridX + 1, gridZ], 0),
                    new Vector3D<float>(0, _heights[gridX, gridZ + 1], 1),
                    new Vector2D<float>(xCoord, zCoord));
        } else {
            answer = Maths
                .BarryCentric(
                    new Vector3D<float>(1, _heights[gridX + 1, gridZ], 0),
                    new Vector3D<float>(1, _heights[gridX + 1, gridZ + 1], 1),
                    new Vector3D<float>(0, _heights[gridX, gridZ + 1], 1),
                    new Vector2D<float>(xCoord, zCoord));
        }

        return answer;
    }

    private VertexArrayObject GenerateTerrain(Loader loader, string heightMap)
    {
        var imageConfig = Configuration.Default.Clone();
        imageConfig.PreferContiguousImageBuffers = true;

        using var image = Image.Load<Rgba32>(imageConfig, heightMap);
        
        var vertexCount = image.Height;

        _heights = new float[vertexCount, vertexCount];

        int count = vertexCount * vertexCount;
        float[] vertices = new float[count * 8];
        uint[] indices = new uint[6 * (vertexCount - 1) * (vertexCount - 1)];
        int vertexPointer = 0;

        for(int i = 0; i < vertexCount; i++){
            for(int j = 0; j < vertexCount; j++){
                // position
                var height = -GetHeight(j, i, image);
                _heights[j, i] = height;
                vertices[vertexPointer * 8] = (float)j / ((float)vertexCount - 1) * Size;
                vertices[vertexPointer * 8 + 1] = height;
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
        // its grayscale so we an use only one color attribute
        float height = -1 * pixelColor.R;
        height += MaxPixelColor / 2.0f;
        height /= MaxPixelColor / 2.0f;
        height *= MaxHeight;

        return height;
    }
}