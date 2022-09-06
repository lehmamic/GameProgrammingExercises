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

    public void ConnectTextures()
    {
        SetUniform("backgroundTexture", 0);
        SetUniform("rTexture", 1);
        SetUniform("gTexture", 2);
        SetUniform("bTexture", 3);
        SetUniform("blendMap", 4);
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

    public void LoadSkyColor(float r, float g, float b)
    {
        SetUniform("skyColor", new Vector3D<float>(r, g, b));
    }
}