using GameEngine.Models;

namespace GameEngine.RenderEngine;

public static class ObjLoader
{
    public static VertexArrayObject LoadObjModel(string fileName, Loader loader)
    {
        var data = ObjFileLoader.LoadObj(fileName);
        return loader.LoadToVAO(data.Vertices, data.Indices);
    }
}