using Silk.NET.OpenGL;

namespace GameEngine.Shaders;

public class StaticShader : Shader
{
    private const string VertexFile = "Shaders/Shader.vert";
    private const string FragmentFile = "Shaders/Shader.frag";

    public StaticShader(GL gl)
        : base(gl, VertexFile, FragmentFile)
    {
    }
}