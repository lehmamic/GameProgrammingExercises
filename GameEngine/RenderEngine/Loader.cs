using GameEngine.Guis;
using GameEngine.Models;
using GameEngine.Textures;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Texture = GameEngine.Textures.Texture;

namespace GameEngine.RenderEngine;

public class Loader : IDisposable
{
    private readonly GL _gl;

    private readonly List<VertexArrayObject> _vaos = new();
    private readonly List<IDisposable> _textures = new();

    public Loader(GL gl)
    {
        _gl = gl;
    }

    public VertexArrayObject LoadToVAO(float[] vertices, uint[] indices, bool includeTangents = false)
    {
        var vao = new VertexArrayObject(_gl, vertices, indices, includeTangents);
        _vaos.Add(vao);

        return vao;
    }
    
    public VertexArrayObject LoadToVAO(float[] vertices, int dimensions)
    {
        var vao = new VertexArrayObject(_gl, vertices, dimensions);
        _vaos.Add(vao);

        return vao;
    }

    public ModelTexture LoadModelTexture(string fileName)
    {
        var texture = new ModelTexture(_gl, fileName);
        _textures.Add(texture);

        return texture;
    }

    public TerrainTexture LoadTerrainTexture(string fileName)
    {
        var texture = new TerrainTexture(_gl, fileName);
        _textures.Add(texture);

        return texture;
    }

    public CubeMapTexture LoadCubeMap(string[] textureFiles)
    {
        var texture = new CubeMapTexture(_gl, textureFiles);
        _textures.Add(texture);

        return texture;
    }

    public GuiTexture LoadGuiTexture(string fileName, Vector2D<float> position, Vector2D<float> scale)
    {
        var texture = new GuiTexture(_gl, fileName, position, scale);
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