using GameEngine.Entities;
using GameEngine.Models;
using GameEngine.RenderEngine;
using GameEngine.Toolbox;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace GameEngine.Water;

public class WaterRenderer : IDisposable
{
    private readonly GL _gl;
    private readonly VertexArrayObject _quad;
    private readonly WaterShader _shader;

    public WaterRenderer(DisplayManager displayManager, Loader loader, Matrix4X4<float> projectionMatrix)
    {
        _gl = displayManager.GL;
        _shader = new WaterShader(displayManager.GL);

        _shader.Activate();
        _shader.LoadProjectionMatrix(projectionMatrix);
        _shader.Deactivate();
        _quad = SetUpVAO(loader);
    }

    public void Render(List<WaterTile> water, Camera camera)
    {
        PrepareRender(camera);
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
    
    private void PrepareRender(Camera camera){
        _shader.Activate();
        _shader.LoadViewMatrix(camera);
        _quad.Activate();
    }

    private void Unbind(){
        _quad.Deactivate();
        _shader.Deactivate();
    }

    public void Dispose()
    {
        _quad.Dispose();
        _shader.Dispose();
    }
}