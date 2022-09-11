using GameEngine.Models;
using GameEngine.NormalMappingObjConverter;
using GameEngine.RenderEngine;
using Silk.NET.Maths;

namespace GameEngine.ObjConverter;

public static class NormalMappedObjLoader
{
    public static VertexArrayObject LoadObj(string fileName, Loader loader)
    {
        List<VertexNormalMap> vertices = new();
        List<Vector2D<float>> textures = new();
        List<Vector3D<float>> normals = new();
        List<uint> indices = new();

        var lines = File.ReadAllLines(fileName);
        foreach (var line in lines)
        {
            var currentLine = line.Split(" ");
            if (line.StartsWith("v "))
            {
                var vertex = new Vector3D<float>(
                    float.Parse(currentLine[1]),
                    float.Parse(currentLine[2]),
                    float.Parse(currentLine[3]));
                VertexNormalMap newVertex = new VertexNormalMap((uint)vertices.Count, vertex);
                vertices.Add(newVertex);
            }
            else if (line.StartsWith("vt "))
            {
                var uv = new Vector2D<float>(
                    float.Parse(currentLine[1]),
                    float.Parse(currentLine[2]));
                textures.Add(uv);
            }
            else if (line.StartsWith("vn "))
            {
                var normal = new Vector3D<float>(
                    float.Parse(currentLine[1]),
                    float.Parse(currentLine[2]),
                    float.Parse(currentLine[3]));
                normals.Add(normal);
            }
            else if (line.StartsWith("f "))
            {
                var vertex1 = currentLine[1].Split("/");
                var vertex2 = currentLine[2].Split("/");
                var vertex3 = currentLine[3].Split("/");

                var v0 = ProcessVertex(vertex1, vertices, indices);
                var v1 = ProcessVertex(vertex2, vertices, indices);
                var v2 = ProcessVertex(vertex3, vertices, indices);
                CalculateTangents(v0, v1, v2, textures);
            }
        }

        RemoveUnusedVertices(vertices);

        float[] verticesArray = new float[vertices.Count * 11];
        float furthest = ConvertDataToArrays(vertices, textures, normals, verticesArray);
        uint[] indicesArray = indices.ToArray();

        return loader.LoadToVAO(verticesArray, indicesArray, true);
    }

    private static void CalculateTangents(VertexNormalMap v0, VertexNormalMap v1, VertexNormalMap v2, List<Vector2D<float>> textures)
    {
        var delatPos1 = v1.Position - v0.Position;
        var delatPos2 = v2.Position - v0.Position;
        var uv0 = textures[v0.TextureIndex];
        var uv1 = textures[v1.TextureIndex];
        var uv2 = textures[v2.TextureIndex];
        var deltaUv1 = uv1 - uv0;
        var deltaUv2 = uv2 - uv0;

        float r = 1.0f / (deltaUv1.X * deltaUv2.Y - deltaUv1.Y * deltaUv2.X);
        delatPos1 *= deltaUv2.Y;
        delatPos2 *= deltaUv1.Y;
        var tangent = delatPos1 - delatPos2;
        tangent *= r;
        v0.AddTangent(tangent);
        v1.AddTangent(tangent);
        v2.AddTangent(tangent);
    }

    private static VertexNormalMap ProcessVertex(String[] vertex, List<VertexNormalMap> vertices, List<uint> indices)
    {
        int index = int.Parse(vertex[0]) - 1;
        VertexNormalMap currentVertex = vertices[index];

        int textureIndex = int.Parse(vertex[1]) - 1;
        int normalIndex = int.Parse(vertex[2]) - 1;

        if (!currentVertex.IsSet)
        {
            currentVertex.TextureIndex = textureIndex;
            currentVertex.NormalIndex = normalIndex;
            indices.Add((uint)index);
            return currentVertex;
        }
        else
        {
            return DealWithAlreadyProcessedVertex(currentVertex, textureIndex, normalIndex, indices, vertices);
        }
    }

    private static float ConvertDataToArrays(List<VertexNormalMap> vertices, List<Vector2D<float>> textures, List<Vector3D<float>> normals, float[] verticesArray)
    {
        float furthestPoint = 0;
        for (int i = 0; i < vertices.Count; i++)
        {
            VertexNormalMap currentVertex = vertices[i];

            if (currentVertex.Length > furthestPoint) {
                furthestPoint = currentVertex.Length;
            }

            var position = currentVertex.Position;
            var textureCoord = textures[currentVertex.TextureIndex];
            var normalVector = normals[currentVertex.NormalIndex];
            var tangent = currentVertex.AveragedTangent;

            // position
            verticesArray[i * 8] = position.X;
            verticesArray[i * 8 + 1] = position.Y;
            verticesArray[i * 8 + 2] = position.Z;

            // normal
            verticesArray[i * 8 + 3] = normalVector.X;
            verticesArray[i * 8 + 4] = normalVector.Y;
            verticesArray[i * 8 + 5] = normalVector.Z;

            // tex coord
            verticesArray[i * 8 + 6] = textureCoord.X;
            verticesArray[i * 8 + 7] = 1 - textureCoord.Y;

            // tangent
            verticesArray[i * 8 + 8] = tangent.X;
            verticesArray[i * 8 + 9] = tangent.Y;
            verticesArray[i * 8 + 10] = tangent.Z;
        }

        return furthestPoint;
    }

    private static VertexNormalMap DealWithAlreadyProcessedVertex(
        VertexNormalMap previousVertex,
        int newTextureIndex,
        int newNormalIndex,
        List<uint> indices,
        List<VertexNormalMap> vertices)
    {
        if (previousVertex.HasSameTextureAndNormal(newTextureIndex, newNormalIndex))
        {
            indices.Add(previousVertex.Index);
            return previousVertex;
        }
        else
        {
            var anotherVertex = previousVertex.DuplicateVertex;
            if (anotherVertex != null)
            {
                return DealWithAlreadyProcessedVertex(anotherVertex, newTextureIndex, newNormalIndex, indices, vertices);
            }
            else
            {
                var duplicateVertex = new VertexNormalMap((uint)vertices.Count, previousVertex.Position)
                {
                    TextureIndex = newTextureIndex,
                    NormalIndex = newNormalIndex
                };

                previousVertex.DuplicateVertex = duplicateVertex;
                vertices.Add(duplicateVertex);
                indices.Add(duplicateVertex.Index);
                return duplicateVertex;
            }
        }
    }
    
    private static void RemoveUnusedVertices(List<VertexNormalMap> vertices)
    {
        foreach(var vertex in vertices)
        {
            if(!vertex.IsSet)
            {
                vertex.TextureIndex = 0;
                vertex.NormalIndex = 0;
            }
        }
    }
}