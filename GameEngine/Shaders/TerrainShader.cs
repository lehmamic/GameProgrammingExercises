using GameEngine.Entities;
using GameEngine.Toolbox;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace GameEngine.Shaders;

public class TerrainShader : Shader
{
    public const int MaxLights = 4;

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

    public void LoadLights(List<Light> light)
    {
        for (int i = 0; i < MaxLights; i++)
        {
            if (i < light.Count)
            {
                SetUniform($"lightPosition[{i}]", light[i].Position);
                SetUniform($"lightColor[{i}]", light[i].Color);
                SetUniform($"attenuation[{i}]", light[i].Attenuation);
            }
            else
            {
                SetUniform($"lightPosition[{i}]", Vector3D<float>.Zero);
                SetUniform($"lightColor[{i}]", Vector3D<float>.Zero);
                SetUniform($"attenuation[{i}]", new Vector3D<float>(1.0f, 0.0f, 0.0f));
            }
        }
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