using GameEngine.Entities;
using GameEngine.Toolbox;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Shader = GameEngine.Shaders.Shader;

namespace GameEngine.Skybox;

public class SkyboxShader : Shader
{
    private const string VertexFile = "Shaders/Skybox.vert";
    private const string FragmentFile = "Shaders/Skybox.frag";

    private const float RotateSpeed = 1.0f;

    private float rotation = 0;

    public SkyboxShader(GL gl)
        : base(gl, VertexFile, FragmentFile)
    {
    }
    
    public void LoadProjectionMatrix(Matrix4X4<float> matrix){
        SetUniform("projectionMatrix", matrix);
    }

    public void LoadViewMatrix(float deltaTime, Camera camera){
        Matrix4X4<float> matrix = Maths.CreateViewMatrix(camera);
        // edit the view matrix in order that the skybox doesnt move in relation to the camera
        matrix.M41 = 0;
        matrix.M42 = 0;
        matrix.M43 = 0;
        rotation += RotateSpeed * deltaTime;
        matrix *= Matrix4X4.CreateRotationY(Scalar.DegreesToRadians(rotation));
        SetUniform("viewMatrix", matrix);
    }

    public void LoadFogColor(float r, float g, float b)
    {
        SetUniform("fogColor", new Vector3D<float>(r, g, b));
    }
}