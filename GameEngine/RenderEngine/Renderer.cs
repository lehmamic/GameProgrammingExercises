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
        texturedModel.VAO.Activate();
        texturedModel.Texture.Activate();
        _gl.DrawElements(PrimitiveType.Triangles, (uint)texturedModel.VAO.NumberOfIndices, DrawElementsType.UnsignedInt, null);
        texturedModel.VAO.Deactivate();
    }
}