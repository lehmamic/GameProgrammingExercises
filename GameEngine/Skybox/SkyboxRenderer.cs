using GameEngine.Entities;
using GameEngine.Models;
using GameEngine.RenderEngine;
using GameEngine.Textures;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Texture = GameEngine.Textures.Texture;

namespace GameEngine.Skybox;

public class SkyboxRenderer
{
    private readonly DisplayManager _displayManager;
    private readonly Loader _loader;
    private const float Size = 500f;

    private static readonly float[] Vertices = {        
        -Size,  Size, -Size,
        -Size, -Size, -Size,
        Size, -Size, -Size,
        Size, -Size, -Size,
        Size,  Size, -Size,
        -Size,  Size, -Size,

        -Size, -Size,  Size,
        -Size, -Size, -Size,
        -Size,  Size, -Size,
        -Size,  Size, -Size,
        -Size,  Size,  Size,
        -Size, -Size,  Size,

        Size, -Size, -Size,
        Size, -Size,  Size,
        Size,  Size,  Size,
        Size,  Size,  Size,
        Size,  Size, -Size,
        Size, -Size, -Size,

        -Size, -Size,  Size,
        -Size,  Size,  Size,
        Size,  Size,  Size,
        Size,  Size,  Size,
        Size, -Size,  Size,
        -Size, -Size,  Size,

        -Size,  Size, -Size,
        Size,  Size, -Size,
        Size,  Size,  Size,
        Size,  Size,  Size,
        -Size,  Size,  Size,
        -Size,  Size, -Size,

        -Size, -Size, -Size,
        -Size, -Size,  Size,
        Size, -Size, -Size,
        Size, -Size, -Size,
        -Size, -Size,  Size,
        Size, -Size,  Size
    };

    private static readonly string[] TextureFiles =
    {
        "Assets/right.png",
        "Assets/left.png",
        "Assets/top.png",
        "Assets/bottom.png",
        "Assets/back.png",
        "Assets/front.png"
    };

    private readonly VertexArrayObject _cube;
    private readonly CubeMapTexture _texture;
    private readonly SkyboxShader _shader;
    private readonly GL _gl;

    public SkyboxRenderer(DisplayManager displayManager, Loader loader, Matrix4X4<float> projectionMatrix)
    {
        _displayManager = displayManager;
        _gl = displayManager.GL;
        _loader = loader;
        _cube = loader.LoadToVAO(Vertices, 3);
        _texture = loader.LoadCubeMap(TextureFiles);
        _shader = new SkyboxShader(_gl);
        
        _shader.Activate();
        _shader.LoadProjectionMatrix(projectionMatrix);
        _shader.Deactivate();
    }

    public void Render(float deltaTime, Camera camera, float r, float g, float b)
    {
        _shader.Activate();
        _shader.LoadViewMatrix(deltaTime, camera);
        _shader.LoadFogColor(r, g, b);
        _cube.Activate();
        _gl.ActiveTexture(TextureUnit.Texture0);
        _texture.Activate();
        // _gl.DepthMask(false);
        // _gl.DepthRange(1.0f, 1.0f);

        _gl.DrawArrays(PrimitiveType.Triangles, 0, (uint)_cube.NumberOfVertices);

        // _gl.DepthRange(0.0f, 1.0f);
        // _gl.DepthMask(true);
        _cube.Deactivate();
        _shader.Deactivate();
    }
}