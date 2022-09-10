using GameEngine.Entities;
using GameEngine.Toolbox;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Shader = GameEngine.Shaders.Shader;

namespace GameEngine.Water;

public class WaterShader : Shader
{
    private const string VertexFile = "Shaders/Water.vert";
    private const string FragmentFile = "Shaders/Water.frag";

    public WaterShader(GL gl)
        : base(gl, VertexFile, FragmentFile)
    {
    }
    
    public void ConnectTextureUnits()
    {
        SetUniform("reflectionTexture", 0);
        SetUniform("refractionTexture", 1);
        SetUniform("dudvMap", 2);
        SetUniform("normalMap", 3);
        SetUniform("depthMap", 4);
    }

    public void LoadModelMatrix(Matrix4X4<float> matrix)
    {
        SetUniform("modelMatrix", matrix);
    }

    public void LoadProjectionMatrix(Matrix4X4<float> matrix)
    {
        SetUniform("projectionMatrix", matrix);
    }
    
    public void LoadViewMatrix(Camera camera)
    {
        var viewMatrix = Maths.CreateViewMatrix(camera);
        SetUniform("viewMatrix", viewMatrix);
        SetUniform("cameraPosition", camera.Position);
    }

    public void LoadMoveFactor(float moveFactor)
    {
        SetUniform("moveFactor", moveFactor);
    }

    public void LoadLight(Light light)
    {
        SetUniform("lightPosition", light.Position);
        SetUniform("lightColor", light.Color);
    }
}