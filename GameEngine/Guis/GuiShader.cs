using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Shader = GameEngine.Shaders.Shader;

namespace GameEngine.Guis;

public class GuiShader : Shader
{
    private const string VertexFile = "Shaders/Gui.vert";
    private const string FragmentFile = "Shaders/Gui.frag";

    public GuiShader(GL gl)
        : base(gl, VertexFile, FragmentFile)
    {
    }

    public void LoadTransformation(Matrix4X4<float> matrix)
    {
        SetUniform("transformationMatrix", matrix);
    }
}
