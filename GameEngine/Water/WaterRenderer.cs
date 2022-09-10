using GameEngine.Entities;
using GameEngine.Models;
using GameEngine.RenderEngine;
using GameEngine.Textures;
using GameEngine.Toolbox;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace GameEngine.Water;

public class WaterRenderer : IDisposable
{
    private const float WaveSpeed = 0.03f;

    private readonly GL _gl;
    private readonly VertexArrayObject _quad;
    private readonly WaterShader _shader;
    private readonly WaterFrameBuffers _fbos;
    private readonly ModelTexture _dudvTexture;
    private readonly ModelTexture _normalMap;

    private float _moveFactor = 0.0f;

    public WaterRenderer(DisplayManager displayManager, Loader loader, Matrix4X4<float> projectionMatrix, WaterFrameBuffers fbos)
    {
        _fbos = fbos;
        _gl = displayManager.GL;
        _shader = new WaterShader(displayManager.GL);
        _dudvTexture = loader.LoadModelTexture("Assets/waterDUDV.png");
        _normalMap = loader.LoadModelTexture("Assets/normalMap.png");

        _shader.Activate();
        _shader.ConnectTextureUnits();
        _shader.LoadProjectionMatrix(projectionMatrix);
        _shader.Deactivate();
        _quad = SetUpVAO(loader);
    }

    public void Render(float deltaTime, List<WaterTile> water, Camera camera, Light sun)
    {
        PrepareRender(deltaTime, camera, sun);
        foreach (var tile in water)
        {
            var modelMatrix = Maths.CreateTransformationMatrix(
                new Vector3D<float>(tile.X, tile.Height, tile.Z),
                0,
                0,
                0,
                WaterTile.TileSize);
            _shader.LoadModelMatrix(modelMatrix);
            _gl.DrawArrays(PrimitiveType.Triangles, 0, (uint)_quad.NumberOfVertices);

        }
        Unbind();
    }
    
    private VertexArrayObject SetUpVAO(Loader loader) {
        // Just x and z vectex positions here, y is set to 0 in v.shader
        float[] vertices = { -1, -1, -1, 1, 1, -1, 1, -1, -1, 1, 1, 1 };
        return loader.LoadToVAO(vertices, 2);
    }
    
    private void PrepareRender(float deltaTime, Camera camera, Light sun){
        _shader.Activate();
        _shader.LoadViewMatrix(camera);
        
        _moveFactor += WaveSpeed * deltaTime;
        _moveFactor %= 1;
        _shader.LoadMoveFactor(_moveFactor);

        _shader.LoadLight(sun);
        _quad.Activate();
        _gl.ActiveTexture(TextureUnit.Texture0);
        _gl.BindTexture(TextureTarget.Texture2D, _fbos.ReflectionTexture);
        _gl.ActiveTexture(TextureUnit.Texture1);
        _gl.BindTexture(TextureTarget.Texture2D, _fbos.RefractionTexture);
        _gl.ActiveTexture(TextureUnit.Texture2);
        _dudvTexture.Activate();
        _gl.ActiveTexture(TextureUnit.Texture3);
        _normalMap.Activate();
        _gl.ActiveTexture(TextureUnit.Texture4);
        _gl.BindTexture(TextureTarget.Texture2D, _fbos.RefractionDepthTexture);
        
        _gl.Enable(GLEnum.Blend);
        _gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    }

    private void Unbind(){
        _gl.Disable(GLEnum.Blend);
        _quad.Deactivate();
        _shader.Deactivate();
    }

    public void Dispose()
    {
        _quad.Dispose();
        _shader.Dispose();
    }
}