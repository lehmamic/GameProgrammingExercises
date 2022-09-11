using GameEngine.Entities;
using GameEngine.Models;
using GameEngine.RenderEngine;
using GameEngine.Textures;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace GameEngine.Skybox;

public class SkyboxRenderer
{
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
        "Assets/sRight.png",
        "Assets/sLeft.png",
        "Assets/sUp.png",
        "Assets/sDown.png",
        "Assets/sBack.png",
        "Assets/sFront.png"
    };

    private static readonly string[] NightTextureFiles =
    {
        "Assets/nightRight.png",
        "Assets/nightLeft.png",
        "Assets/nightTop.png",
        "Assets/nightBottom.png",
        "Assets/nightBack.png",
        "Assets/nightFront.png"
    };

    private readonly VertexArrayObject _cube;
    private readonly CubeMapTexture _texture;
    private readonly CubeMapTexture _nightTexture;
    private readonly SkyboxShader _shader;
    private readonly GL _gl;

    private float _time = 0.0f;

    public SkyboxRenderer(DisplayManager displayManager, Loader loader, Matrix4X4<float> projectionMatrix)
    {
        _gl = displayManager.GL;
        _cube = loader.LoadToVAO(Vertices, 3);
        _texture = loader.LoadCubeMap(TextureFiles);
        _nightTexture = loader.LoadCubeMap(NightTextureFiles);
        _shader = new SkyboxShader(_gl);
        
        _shader.Activate();
        _shader.ConnectTextureUnits();
        _shader.LoadProjectionMatrix(projectionMatrix);
        _shader.Deactivate();
    }

    public void Render(float deltaTime, Camera camera, float r, float g, float b)
    {
        _shader.Activate();
        _shader.LoadViewMatrix(deltaTime, camera);
        _shader.LoadFogColor(r, g, b);
        _cube.Activate();
        BindTextures(deltaTime);

        _gl.DrawArrays(PrimitiveType.Triangles, 0, (uint)_cube.NumberOfVertices);

        _cube.Deactivate();
        _shader.Deactivate();
    }

    private void BindTextures(float deltaTime)
    {
        _time += deltaTime * 1000;
        _time %= 24000;

        CubeMapTexture texture1;
        CubeMapTexture texture2;
        float blendFactor;

        if(_time >= 0 && _time < 5000)
        {
            texture1 = _nightTexture;
            texture2 = _nightTexture;
            blendFactor = (_time - 0) / (5000 - 0);
        }
        else if (_time >= 5000 && _time < 8000)
        {
            texture1 = _nightTexture;
            texture2 = _texture;
            blendFactor = (_time - 5000) / (8000 - 5000);
        }
        else if (_time >= 8000 && _time < 21000)
        {
            texture1 = _texture;
            texture2 = _texture;
            blendFactor = (_time - 8000) / (21000 - 8000);
        }
        else
        {
            texture1 = _texture;
            texture2 = _nightTexture;
            blendFactor = (_time - 21000) / (24000 - 21000);
        }
        
        _gl.ActiveTexture(TextureUnit.Texture0);
        texture1.Activate();
        _gl.ActiveTexture(TextureUnit.Texture1);
        texture2.Activate();
        _shader.LoadBlendFactor(blendFactor);
    }
}