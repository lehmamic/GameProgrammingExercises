using GameEngine.Models;
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

    public unsafe void Render(TexturedModel texturedModel)
    {
        var model = texturedModel.RawModel;

        _gl.BindVertexArray(model.VaoId);
        _gl.EnableVertexAttribArray(0);
        _gl.EnableVertexAttribArray(1);
        _gl.ActiveTexture(TextureUnit.Texture0);
        _gl.BindTexture(TextureTarget.Texture2D, texturedModel.Texture.TextureId);
        _gl.DrawElements(PrimitiveType.Triangles, model.VertexCount, DrawElementsType.UnsignedInt, null);
        _gl.DisableVertexAttribArray(0);
        _gl.DisableVertexAttribArray(1);
        _gl.BindVertexArray(0);
    }
}