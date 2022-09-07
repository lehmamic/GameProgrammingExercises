using GameEngine.Models;
using GameEngine.RenderEngine;
using GameEngine.Shaders;
using GameEngine.Toolbox;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace GameEngine.Guis;

public class GuiRenderer : IDisposable
{
    private readonly DisplayManager _displayManager;
    private readonly MasterRenderer _masterRenderer;
    private readonly GL _gl;
    private readonly VertexArrayObject _quad;
    private readonly GuiShader _shader;

    public GuiRenderer(DisplayManager displayManager, MasterRenderer masterRenderer, Loader loader, Matrix4X4<float> projectionMatrix)
    {
        _displayManager = displayManager;
        _masterRenderer = masterRenderer;
        _shader = new GuiShader(displayManager.GL);
        _gl = _displayManager.GL;

        var positions = new[]{ -1.0f, 1.0f, -1.0f, -1.0f, 1.0f, 1.0f, 1.0f, -1.0f };
        _quad = loader.LoadToVAO(positions);

        // shader.Activate()};
        // shader.LoadProjectionMatrix(projectionMatrix);
        // shader.Deactivate();
    }

    public void Render(List<GuiTexture> guis)
    {
        _shader.Activate();
        _quad.Activate();
        _gl.Enable(GLEnum.Blend);
        _gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        _gl.Disable(GLEnum.DepthTest);
        foreach (var gui in guis)
        {
            _gl.ActiveTexture(TextureUnit.Texture0);
            gui.Activate();
            var transformationMatrix = Maths.CreateTransformationMatrix(gui.Position, gui.Scale);
            _shader.LoadTransformation(transformationMatrix);
            _gl.DrawArrays(PrimitiveType.TriangleStrip, 0, (uint)_quad.NumberOfVertices);
        }
        _gl.Disable(GLEnum.Blend);
        _gl.Enable(GLEnum.DepthTest);
        _quad.Deactivate();
        _shader.Deactivate();
    }

    public void Dispose()
    {
        _quad.Dispose();
        _shader.Dispose();
    }
}