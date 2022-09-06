using GameEngine.Entities;
using GameEngine.Toolbox;
using Silk.NET.Maths;
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

    public void LoadFakeLighting(bool useFake)
    {
        SetUniform("useFakeLighting", useFake);
    }

    public void LoadSkyColor(float r, float g, float b)
    {
        SetUniform("skyColor", new Vector3D<float>(r, g, b));
    }
}