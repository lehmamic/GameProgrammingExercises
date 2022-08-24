using System.Text.Json;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace GameProgrammingExercises;

public class Mesh : IDisposable
{
    public Mesh(float radius, float specularPower, string shaderName, IReadOnlyList<Texture> textures, VertexArrayObject vertexArray)
    {
        Radius = radius;
        SpecularPower = specularPower;
        ShaderName = shaderName;
        Textures = textures;
        VertexArray = vertexArray;
    }

    public float Radius { get; }

    public float SpecularPower { get; }

    public string ShaderName { get; }

    public IReadOnlyList<Texture> Textures { get; }

    public VertexArrayObject VertexArray { get; }

    public static Mesh Load(string fileName, Game game)
    {
        // Load Textures
        var jsonString = File.ReadAllText(fileName);
        var raw = JsonSerializer.Deserialize<RawMesh>(jsonString, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        if (raw?.Version != 1)
        {
            throw new MeshException($"Mesh {fileName} is not version 1.");
        }

        // Skip the vertex format/shader for now
        // (This is changed in a later chapter's code)
        var vertSize = 8;

        if (raw.Textures is null || !raw.Textures.Any())
        {
            throw new MeshException($"Mesh {fileName} has not textures, there should be at least one.");
        }

        // Load the textures
        var textures = new List<Texture>();
        foreach (var textureName in raw.Textures)
        {
            Texture texture;
            try
            {
                // Is this texture already loaded?
                texture = game.Renderer.GetTexture(textureName);
            }
            catch
            {
                // If it's still null, just use the default texture
                texture = game.Renderer.GetTexture("Assets/Default.png");
            }

            textures.Add(texture);
        }

        // Load in the vertices
        if (raw.Vertices is null || !raw.Vertices.Any())
        {
            throw new MeshException($"Mesh {fileName} has no vertices.");
        }

        float radius = 0.0f;
        var vertices = new float[raw.Vertices.Length * vertSize];
        for (int i = 0; i < raw.Vertices.Length; i++)
        {
            var vertex = raw.Vertices[i];
            if (vertex is null || vertex.Length != vertSize)
            {
                throw new MeshException($"Unexpected vertex format for {fileName}.");
            }
        
            var position = new Vector3D<float>(vertex[0], vertex[1], vertex[2]);
            radius = Scalar.Max(radius, position.LengthSquared);

            // Add the floats
            var offset = i * vertSize;
            for (int j = 0; j < vertex.Length; j++)
            {
                vertices[offset + j] = vertex[j];
            }
        }
        
        // We where computing length squared earlier
        radius = Scalar.Sqrt(radius);

        var rawIndices = new uint[][]
        {
            new uint[] { 2,1,0 },
            new uint[] { 3,9,8 },
            new uint[] { 4,11,10 },
            new uint[] { 5,11,12 },
            new uint[] { 6,14,13 },
            new uint[] { 7,14,15 },
            new uint[] { 18,17,16 },
            new uint[] { 19,17,18 },
            new uint[] { 22,21,20 },
            new uint[] { 23,21,22 },
            new uint[] { 26,25,24 },
            new uint[] { 27,25,26 }
        };

        // Load in the indices
        if (raw.Indices is null || !raw.Indices.Any())
        {
            throw new MeshException($"Mesh {fileName} has no indices.");
        }

        var indices = new uint[raw.Indices.Length * 3];
        for (int i = 0; i < raw.Indices.Length; i++)
        {
            var index = raw.Indices[i];
            if (index is null || index.Length != 3)
            {
                throw new MeshException($"Invalid indices for {fileName}.");
            }

            var offset = i * 3;
            for (int j = 0; j < index.Length; j++)
            {
                indices[offset + j] = index[j];
            }
        }

        var vertices2 = new float[]
        {
            -0.5f, -0.5f, -0.5f, 0, 0, -1, 0, 0,
            0.5f, -0.5f, -0.5f, 0, 0, -1, 1, 0,
            -0.5f, 0.5f, -0.5f, 0, 0, -1, 0, -1,
            0.5f, 0.5f, -0.5f, 0, 0, -1, 1, -1,
            -0.5f, 0.5f, 0.5f, 0, 1, 0, 0, -1,
            0.5f, 0.5f, 0.5f, 0, 1, 0, 1, -1,
            -0.5f, -0.5f, 0.5f, 0, 0, 1, 0, 0,
            0.5f, -0.5f, 0.5f, 0, 0, 1, 1, 0,
            -0.5f, 0.5f, -0.5f, 0, 0, -1, 0, -1,
            0.5f, -0.5f, -0.5f, 0, 0, -1, 1, 0,
            -0.5f, 0.5f, -0.5f, 0, 1, 0, 0, -1,
            0.5f, 0.5f, -0.5f, 0, 1, 0, 1, -1,
            -0.5f, 0.5f, 0.5f, 0, 1, 0, 0, -1,
            -0.5f, 0.5f, 0.5f, 0, 0, 1, 0, -1,
            0.5f, 0.5f, 0.5f, 0, 0, 1, 1, -1,
            -0.5f, -0.5f, 0.5f, 0, 0, 1, 0, 0,
            -0.5f, -0.5f, 0.5f, 0, -1, 0, 0, 0,
            0.5f, -0.5f, 0.5f, 0, -1, 0, 1, 0,
            -0.5f, -0.5f, -0.5f, 0, -1, 0, 0, 0,
            0.5f, -0.5f, -0.5f, 0, -1, 0, 1, 0,
            0.5f, -0.5f, -0.5f, 1, 0, 0, 1, 0,
            0.5f, -0.5f, 0.5f, 1, 0, 0, 1, 0,
            0.5f, 0.5f, -0.5f, 1, 0, 0, 1, -1,
            0.5f, 0.5f, 0.5f, 1, 0, 0, 1, -1,
            -0.5f, -0.5f, 0.5f, -1, 0, 0, 0, 0,
            -0.5f, -0.5f, -0.5f, -1, 0, 0, 0, 0,
            -0.5f, 0.5f, 0.5f, -1, 0, 0, 0, -1,
            -0.5f, 0.5f, -0.5f, -1, 0, 0, 0, -1,
        };

        var indices2 = new uint[]
        {
            2, 1, 0,
            3, 9, 8,
            4, 11, 10,
            5, 11, 12,
            6, 14, 13,
            7, 14, 15,
            18, 17, 16,
            19, 17, 18,
            22, 21, 20,
            23, 21, 22,
            26, 25, 24,
            27, 25, 26,
        };

        var indices3 = new uint[raw.Indices.Length * 3];
        indices3[0] = (uint)(int)2;
        indices3[1] = (uint)(int)1;
        indices3[2] = (uint)(int)0;
        indices3[3] = (uint)(int)3;
        indices3[4] = (uint)(int)9;
        indices3[5] = (uint)(int)8;
        indices3[6] =  (uint)(int)4;
        indices3[7] = (uint)(int)11;
        indices3[8] = (uint)(int)10;
        indices3[9] = (uint)(int)5;
        indices3[10] = (uint)(int)11;
        indices3[11] = (uint)(int)12;
        indices3[12] = (uint)(int)6;
        indices3[13] = (uint)(int)14;
        indices3[14] = (uint)(int)13;
        indices3[15] = (uint)(int)7;
        indices3[16] = (uint)(int)14;
        indices3[17] = (uint)(int)15;
        indices3[18] = (uint)(int)18;
        indices3[19] = (uint)(int)17;
        indices3[20] = (uint)(int)16;
        indices3[21] = (uint)(int)19;
        indices3[22] = (uint)(int)17;
        indices3[23] = (uint)(int)18;
        indices3[24] = (uint)(int)22;
        indices3[25] = (uint)(int)21;
        indices3[26] = (uint)(int)20;
        indices3[27] = (uint)(int)23;
        indices3[28] = (uint)(int)21;
        indices3[29] = (uint)(int)22;
        indices3[30] = (uint)(int)26;
        indices3[31] = (uint)(int)25;
        indices3[32] = (uint)(int)24;
        indices3[33] = (uint)(int)27;
        indices3[34] = (uint)(int)25;
        indices3[35] = (uint)(int)26;

        var vbo = new BufferObject<float>(game.Renderer.GL, vertices, BufferTargetARB.ArrayBuffer);
        var ebo = new BufferObject<uint>(game.Renderer.GL, indices, BufferTargetARB.ElementArrayBuffer);
        var vao = new VertexArrayObject(game.Renderer.GL, vbo, ebo);

        return new Mesh(radius, raw.SpecularPower, raw.Shader, textures, vao);
    }

    public void Dispose()
    {
        VertexArray.Dispose();
    }

    private class RawMesh
    {
        public int Version { get; set; }

        public string Shader { get; set; } = String.Empty;

        public string[] Textures { get; set; } = null!;

        public float SpecularPower { get; set; }

        public float[][] Vertices { get; set; } = null!;

        public uint[][] Indices { get; set; } = null!;
    }
}