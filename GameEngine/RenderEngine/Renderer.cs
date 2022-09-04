using Silk.NET.OpenGL;

namespace GameEngine.RenderEngine;

public class Renderer
{
    private readonly GL _gl;

    public Renderer(GL gl)
    {
        _gl = gl;
    }

    public void Prepare()
    {
        _gl.ClearColor(1, 0, 0, 1);
        _gl.Clear(ClearBufferMask.ColorBufferBit);
    }

    public void Render(RawModel model)
    {
        _gl.BindVertexArray(model.VaoId);
        _gl.EnableVertexAttribArray(0);
        _gl.DrawArrays(PrimitiveType.Triangles, 0, model.VertexCount);
        _gl.DisableVertexAttribArray(0);
        _gl.BindVertexArray(0);
    }
}