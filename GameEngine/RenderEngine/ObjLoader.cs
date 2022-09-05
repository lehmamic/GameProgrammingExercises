using GameEngine.Models;
using Silk.NET.Maths;

namespace GameEngine.RenderEngine;

public class ObjLoader
{
    public static VertexArrayObject LoadObjModel(string fileName, Loader loader)
    {
        List<Vector3D<float>> vertices = new();
        List<Vector2D<float>> textures = new();
        List<Vector3D<float>> normals = new();

        List<uint> indices = new();
        float[]? verticesArray = null;

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
                vertices.Add(vertex);
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
                verticesArray ??= new float[vertices.Count * 8];

                var vertex1 = currentLine[1].Split("/");
                var vertex2 = currentLine[2].Split("/");
                var vertex3 = currentLine[3].Split("/");

                ProcessVertex(vertex1, indices, vertices, textures, normals, verticesArray);
                ProcessVertex(vertex2, indices, vertices, textures, normals, verticesArray);
                ProcessVertex(vertex3, indices, vertices, textures, normals, verticesArray);
            }
        }

        var indicesArray = indices.ToArray();

        return loader.LoadToVAO(verticesArray ?? Array.Empty<float>(), indicesArray);
    }

    private static void ProcessVertex(
        string[] vertexData,
        List<uint> indices,
        List<Vector3D<float>> vertices,
        List<Vector2D<float>> textures,
        List<Vector3D<float>> normals,
        float[] verticesArray)
    {
        var currentVertexPointer = int.Parse(vertexData[0]) - 1;
        indices.Add((uint)currentVertexPointer);

        var currentVertex = vertices[currentVertexPointer];
        verticesArray[currentVertexPointer * 8] = currentVertex.X;
        verticesArray[currentVertexPointer * 8 + 1] = currentVertex.Y;
        verticesArray[currentVertexPointer * 8 + 2] = currentVertex.Z;
        
        var currentNorm = normals[int.Parse(vertexData[2]) - 1];
        verticesArray[currentVertexPointer * 8 + 3] = currentNorm.X;
        verticesArray[currentVertexPointer * 8 + 4] = currentNorm.Y;
        verticesArray[currentVertexPointer * 8 + 5] = currentNorm.Z;

        var currentTex = textures[int.Parse(vertexData[1]) - 1];
        verticesArray[currentVertexPointer * 8 + 6] = currentTex.X;
        verticesArray[currentVertexPointer * 8 + 7] = 1 - currentTex.Y;
    }
    // Orginal from the tutorial
    // private static void ProcessVertex(
    //     string[] vertexData,
    //     List<uint> indices,
    //     List<Vector2D<float>> textures,
    //     List<Vector3D<float>> normals,
    //     float[] textureArray,
    //     float[] normalsArray)
    // {
    //     var currentVertexPointer = int.Parse(vertexData[0]) - 1;
    //     indices.Add((uint)currentVertexPointer);
    //
    //     var currentTex = textures[int.Parse(vertexData[1]) - 1];
    //     textureArray[currentVertexPointer * 2] = currentTex.X;
    //     textureArray[currentVertexPointer * 2 + 1] = 1 - currentTex.Y;
    //
    //     var currentNorm = normals[int.Parse(vertexData[2]) - 1];
    //     normalsArray[currentVertexPointer * 3] = currentNorm.X;
    //     normalsArray[currentVertexPointer * 3 + 1] = currentNorm.Y;
    //     normalsArray[currentVertexPointer * 3 + 2] = currentNorm.Z;
    // }
}