using GameEngine.Entities;
using GameEngine.Toolbox;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace GameEngine.Shaders;

public class StaticShader : Shader
{
    public const int MaxLights = 4;

    private const string VertexFile = "Shaders/Shader.vert";
    private const string FragmentFile = "Shaders/Shader.frag";

    public StaticShader(GL gl)
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

    public void LoadFakeLighting(bool useFake)
    {
        SetUniform("useFakeLighting", useFake);
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
}