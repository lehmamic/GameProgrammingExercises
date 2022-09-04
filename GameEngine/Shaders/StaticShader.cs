using Silk.NET.OpenGL;

namespace GameEngine.Shaders;

public class StaticShader : ShaderProgram
{
    private const string VertexFile = "Shaders/Shader.vert";
    private const string FragmentFile = "Shaders/Shader.frag";

    public StaticShader(GL gl)
        : base(gl, VertexFile, FragmentFile)
    {
    }

    protected override void BindAttributes()
    {
        BindAttribute(0, "position");
        BindAttribute(1, "textureCoords");
    }
}