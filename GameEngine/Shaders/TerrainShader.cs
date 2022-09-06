using GameEngine.Entities;
using GameEngine.Toolbox;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace GameEngine.Shaders;

public class TerrainShader : Shader
{
    private const string VertexFile = "Shaders/Terrain.vert";
    private const string FragmentFile = "Shaders/Terrain.frag";

    public TerrainShader(GL gl)
        : base(gl, VertexFile, FragmentFile)
    {
    }

    public void LoadTransformationMatrix(Matrix4X4<float> matrix)
    {
        SetUniform("transformationMatrix", matrix);
    }

    public void LoadProjectionMatrix(Matrix4X4<float> matrix)
    {
        SetUniform("projectionMatrix", matrix);
    }
    
    public void LoadViewMatrix(Camera camera)
    {
        var viewMatrix = Maths.CreateViewMatrix(camera);
        SetUniform("viewMatrix", viewMatrix);
    }

    public void LoadLight(Light light)
    {
        SetUniform("lightPosition", light.Position);
        SetUniform("lightColor", light.Color);
    }

    public void LoadShineVariables(float damper, float reflectivity)
    {
        SetUniform("shineDamper", damper);
        SetUniform("reflectivity", reflectivity);
    }
}