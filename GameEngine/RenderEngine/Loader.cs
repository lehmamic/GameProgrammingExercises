using GameEngine.Models;
using Silk.NET.OpenGL;
using Texture = GameEngine.Textures.Texture;

namespace GameEngine.RenderEngine;

public class Loader : IDisposable
{
    private readonly GL _gl;

    private readonly List<VertexArrayObject> _vaos = new();
    private readonly List<Texture> _textures = new();

    public Loader(GL gl)
    {
        _gl = gl;
    }

    public VertexArrayObject LoadToVAO(float[] vertices, uint[] indices)
    {
        var vao = new VertexArrayObject(_gl, vertices, indices);
        _vaos.Add(vao);

        return vao;
    }

    public Texture LoadTexture(string fileName)
    {
        var texture = new Texture(_gl, fileName);
        _textures.Add(texture);

        return texture;
    }
    
    public void Dispose()
    {
        foreach (var vao in _vaos)
        {
            vao.Dispose();
        }

        foreach (var texture in _textures)
        {
            texture.Dispose();
        }
    }
}