using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using GameProgrammingExercises.Maths.Geometry;
using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class Mesh : IDisposable
{
    public Mesh(float radius, float specularPower, string shaderName, IReadOnlyList<Texture> textures, AABB box, VertexArrayObject vertexArray)
    {
        Radius = radius;
        SpecularPower = specularPower;
        ShaderName = shaderName;
        Textures = textures;
        Box = box;
        VertexArray = vertexArray;
    }

    public float Radius { get; }

    public float SpecularPower { get; }

    public string ShaderName { get; }

    public IReadOnlyList<Texture> Textures { get; }

    public AABB Box { get; }

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

        var shaderName = raw.Shader;

        var layout = raw.VertexFormat;
        var vertSize = layout switch
        {
            VertexArrayObjectLayout.PosNormTex => 8,
            VertexArrayObjectLayout.PosNormSkinTex => 16,
            _ => throw new NotImplementedException($"The VAO layout {layout} has not been implemented yet."),
        };

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

        var specPower = raw.SpecularPower;
        
        // Load in the indices (we load the first, this is different to the book)
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

        // Load in the vertices
        if (raw.Vertices is null || !raw.Vertices.Any())
        {
            throw new MeshException($"Mesh {fileName} has no vertices.");
        }

        float radius = 0.0f;
        AABB box = new(
            new Vector3D<float>(Scalar<float>.PositiveInfinity, Scalar<float>.PositiveInfinity, Scalar<float>.PositiveInfinity),
            new Vector3D<float>(Scalar<float>.NegativeInfinity, Scalar<float>.NegativeInfinity, Scalar<float>.NegativeInfinity));

        VertexArrayObject? vao = null;
        if (layout == VertexPosNormTex.Layout)
        {
            var vertices = new VertexPosNormTex[raw.Vertices.Length];
            for (int i = 0; i < raw.Vertices.Length; i++)
            {
                var vertex = raw.Vertices[i];
                if (vertex is null || vertex.Length != vertSize)
                {
                    throw new MeshException($"Unexpected vertex format for {fileName}.");
                }

                var position = new Vector3D<float>(vertex[0], vertex[1], vertex[2]);
                var normal = new Vector3D<float>(vertex[3], vertex[4], vertex[5]);
                var texCoords = new Vector2D<float>(vertex[6], vertex[7]);

                vertices[i] = new VertexPosNormTex(position, normal, texCoords);

                radius = Scalar.Max(radius, position.LengthSquared);
                box.UpdateMinMax(position);
            }
            
            // We where computing length squared earlier
            radius = Scalar.Sqrt(radius);

            vao = new VertexArrayObject(game.Renderer.GL, vertices, indices);
        }
        else if (layout == VertexPosNormSkinTex.Layout)
        {
            var vertices = new VertexPosNormSkinTex[raw.Vertices.Length];
            for (int i = 0; i < raw.Vertices.Length; i++)
            {
                var vertex = raw.Vertices[i];
                if (vertex is null || vertex.Length != vertSize)
                {
                    throw new MeshException($"Unexpected vertex format for {fileName}.");
                }

                var position = new Vector3D<float>(vertex[0], vertex[1], vertex[2]);
                var normal = new Vector3D<float>(vertex[3], vertex[4], vertex[5]);
                var skinningIndices = new Vector4D<byte>((byte) vertex[6], (byte) vertex[7], (byte) vertex[8], (byte) vertex[9]);
                var skinningWeights = new Vector4D<byte>((byte) vertex[10], (byte) vertex[11], (byte) vertex[12], (byte) vertex[13]);
                var texCoords = new Vector2D<float>(vertex[14], vertex[15]);

                vertices[i] = new VertexPosNormSkinTex(position, normal, skinningIndices, skinningWeights, texCoords);

                radius = Scalar.Max(radius, position.LengthSquared);
                box.UpdateMinMax(position);
            }
            
            // We where computing length squared earlier
            radius = Scalar.Sqrt(radius);

            vao = new VertexArrayObject(game.Renderer.GL, vertices, indices);
        }
        else
        {
            throw new NotImplementedException($"The VAO layout {layout} has not been implemented yet.");
        }

        return new Mesh(radius, raw.SpecularPower, raw.Shader, textures, box, vao);
    }
    
    public Texture? GetTexture(int index)
    {
        if (index < Textures.Count)
        {
            return Textures[index];
        }

        return null;
    }

    public void Dispose()
    {
        VertexArray.Dispose();
    }

    private class RawMesh
    {
        public int Version { get; set; }
        
        [JsonPropertyName("vertexformat")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public VertexArrayObjectLayout VertexFormat { get; set; }

        public string Shader { get; set; } = String.Empty;

        public string[] Textures { get; set; } = null!;

        public float SpecularPower { get; set; }

        public float[][] Vertices { get; set; } = null!;

        public uint[][] Indices { get; set; } = null!;
    }
}