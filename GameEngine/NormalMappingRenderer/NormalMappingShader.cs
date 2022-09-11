using System.Numerics;
using GameEngine.Entities;
using GameEngine.Toolbox;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Shader = GameEngine.Shaders.Shader;

namespace GameEngine.NormalMappingRenderer;

public class NormalMappingShader : Shader
{
    public const int MaxLights = 4;

    private const string VertexFile = "Shaders/NormalMap.vert";
    private const string FragmentFile = "Shaders/NormalMap.frag";

    public NormalMappingShader(GL gl)
        : base(gl, VertexFile, FragmentFile)
    {
    }
    
    public void ConnectTextureUnits()
    {
        SetUniform("modelTexture", 0);
        SetUniform("normalMap", 1);
    }
    
    public void LoadTransformationMatrix(Matrix4X4<float> matrix)
    {
        SetUniform("transformationMatrix", matrix);
    }

    public void LoadProjectionMatrix(Matrix4X4<float> matrix)
    {
        SetUniform("projectionMatrix", matrix);
    }
    
    public void LoadViewMatrix(Matrix4X4<float> viewMatrix)
    {
        SetUniform("viewMatrix", viewMatrix);
    }

    public void LoadLights(List<Light> light, Matrix4X4<float> viewMatrix)
    {
        for (int i = 0; i < MaxLights; i++)
        {
            if (i < light.Count)
            {
                SetUniform($"lightPositionEyeSpace[{i}]", GetEyeSpacePosition(light[i], viewMatrix));
                SetUniform($"lightColor[{i}]", light[i].Color);
                SetUniform($"attenuation[{i}]", light[i].Attenuation);
            }
            else
            {
                SetUniform($"lightPositionEyeSpace[{i}]", Vector3D<float>.Zero);
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

    public void LoadNumberOfRows(int numberOfRows)
    {
        SetUniform("numberOfRows", (float)numberOfRows);
    }

    public void LoadOffset(float x, float y)
    {
        SetUniform("offset", new Vector2D<float>(x, y));
    }

    public void LoadClipPlane(Vector4D<float> plane)
    {
        SetUniform("plane", plane);
    }
    
    private Vector3D<float> GetEyeSpacePosition(Light light, Matrix4X4<float> viewMatrix){
        var position = light.Position;
        var eyeSpacePos = new Vector4D<float>(position.X, position.Y, position.Z, 1f) * viewMatrix;
        return new Vector3D<float>(eyeSpacePos.X, eyeSpacePos.Y, eyeSpacePos.Z);
    }
}