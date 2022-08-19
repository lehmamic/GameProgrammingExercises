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
        var vertices = new List<float>();
        foreach (var vertex in raw.Vertices)
        {
            if (vertex is null || vertex.Length != vertSize)
            {
                throw new MeshException($"Unexpected vertex format for {fileName}.");
            }
        
            var position = new Vector3D<float>(vertex[0], vertex[1], vertex[2]);
            radius = Scalar.Max(radius, position.LengthSquared);
        
            foreach (var value in vertex)
            {
                vertices.Add(value);
            }
        }

        // Load in the indices
        if (raw.Indices is null || !raw.Indices.Any())
        {
            throw new MeshException($"Mesh {fileName} has no indices.");
        }

        var indices = new List<uint>();
        foreach (var index in raw.Indices)
        {
            if (index is null || index.Length != 3)
            {
                throw new MeshException($"Invalid indices for {fileName}.");
            }
        
            foreach (var value in index)
            {
                indices.Add(value);
            }
        }

        var vbo = new BufferObject<float>(game.Renderer.GL, vertices.ToArray(), BufferTargetARB.ArrayBuffer);
        var ebo = new BufferObject<uint>(game.Renderer.GL, indices.ToArray(), BufferTargetARB.ElementArrayBuffer);
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