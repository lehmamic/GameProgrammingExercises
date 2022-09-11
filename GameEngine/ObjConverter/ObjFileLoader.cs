using Silk.NET.Maths;

namespace GameEngine.ObjConverter;

public static class ObjFileLoader
{
    public static ModelData LoadObj(string fileName)
    {
        List<Vertex> vertices = new();
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
                Vertex newVertex = new Vertex((uint)vertices.Count, vertex);
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

                ProcessVertex(vertex1, vertices, indices);
                ProcessVertex(vertex2, vertices, indices);
                ProcessVertex(vertex3, vertices, indices);
            }
        }

        RemoveUnusedVertices(vertices);

        float[] verticesArray = new float[vertices.Count * 8];
        float furthest = ConvertDataToArrays(vertices, textures, normals, verticesArray);
        uint[] indicesArray = indices.ToArray();

        return new ModelData(verticesArray, indicesArray, furthest);
    }

    private static void ProcessVertex(String[] vertex, List<Vertex> vertices, List<uint> indices)
    {
        int index = int.Parse(vertex[0]) - 1;
        Vertex currentVertex = vertices[index];

        int textureIndex = int.Parse(vertex[1]) - 1;
        int normalIndex = int.Parse(vertex[2]) - 1;

        if (!currentVertex.IsSet)
        {
            currentVertex.TextureIndex = textureIndex;
            currentVertex.NormalIndex = normalIndex;
            indices.Add((uint)index);
        }
        else
        {
            DealWithAlreadyProcessedVertex(currentVertex, textureIndex, normalIndex, indices, vertices);
        }
    }

    private static float ConvertDataToArrays(List<Vertex> vertices, List<Vector2D<float>> textures, List<Vector3D<float>> normals, float[] verticesArray)
    {
        float furthestPoint = 0;
        for (int i = 0; i < vertices.Count; i++)
        {
            Vertex currentVertex = vertices[i];

            if (currentVertex.Length > furthestPoint) {
                furthestPoint = currentVertex.Length;
            }

            var position = currentVertex.Position;
            var textureCoord = textures[currentVertex.TextureIndex];
            var normalVector = normals[currentVertex.NormalIndex];

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
        }

        return furthestPoint;
    }

    private static void DealWithAlreadyProcessedVertex(
        Vertex previousVertex,
        int newTextureIndex,
        int newNormalIndex,
        List<uint> indices,
        List<Vertex> vertices)
    {
        if (previousVertex.HasSameTextureAndNormal(newTextureIndex, newNormalIndex))
        {
            indices.Add(previousVertex.Index);
        }
        else
        {
            var anotherVertex = previousVertex.DuplicateVertex;
            if (anotherVertex != null)
            {
                DealWithAlreadyProcessedVertex(anotherVertex, newTextureIndex, newNormalIndex, indices, vertices);
            }
            else
            {
                var duplicateVertex = new Vertex((uint)vertices.Count, previousVertex.Position)
                {
                    TextureIndex = newTextureIndex,
                    NormalIndex = newNormalIndex
                };

                previousVertex.DuplicateVertex = duplicateVertex;
                vertices.Add(duplicateVertex);
                indices.Add(duplicateVertex.Index);
            }
        }
    }
    
    private static void RemoveUnusedVertices(List<Vertex> vertices){
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