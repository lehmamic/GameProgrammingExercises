using System.Text.Json;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace GameProgrammingExercises;

public class Mesh : IDisposable
{
    public Mesh(float radius, float specularPower, string shaderName, List<Texture> textures, VertexArrayObject vertexArray)
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

    public List<Texture> Textures { get; }

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
            for (int j = 0; j < vertex.Length; j++)
            {
                vertices[i * vertSize + j] = vertex[j];
            }
        }
        
        // We where computing length squared earlier
        radius = Scalar.Sqrt(radius);

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
        
            for (int j = 0; j < index.Length; j++)
            {
                indices[i * 3 + j] = index[j];
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

        public string[]? Textures { get; set; }

        public float SpecularPower { get; set; }

        public float[][]? Vertices { get; set; }

        public uint[][]? Indices { get; set; }
    }
}