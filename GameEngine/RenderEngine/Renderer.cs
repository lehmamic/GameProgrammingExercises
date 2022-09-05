using System.Security.Cryptography;
using GameEngine.Entities;
using GameEngine.Shaders;
using GameEngine.Toolbox;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace GameEngine.RenderEngine;

public class Renderer
{
    private readonly DisplayManager _displayManager;
    private const float FOV = 70.0f;
    private const float NearPlane = 0.1f;
    private const float FarPlane = 1000.0f;

    private readonly GL _gl;

    private readonly Matrix4X4<float> _projectionMatrix;

    public Renderer(DisplayManager displayManager, StaticShader shader)
    {
        _displayManager = displayManager;
        _gl = _displayManager.GL;

        // _projectionMatrix = Matrix4X4.CreatePerspectiveFieldOfView(Scalar.DegreesToRadians(FOV), _displayManager.Width / _displayManager.Height, NearPlane, FarPlane);
        _projectionMatrix = Maths.CreateProjectionMatrix(FOV, _displayManager.Width, _displayManager.Height, NearPlane, FarPlane);
        shader.Activate();
        shader.LoadProjectionMatrix(_projectionMatrix);
        shader.Deactivate();
    }

    public void Prepare()
    {
        _gl.ClearColor(0, 0, 0, 1);
        _gl.Clear((uint) (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
        
        // Enable depth buffer/disable alpha blend
        _gl.Enable(EnableCap.DepthTest);
        _gl.Disable(EnableCap.Blend);
    }

    public unsafe void Render(Entity entity, StaticShader shader)
    {
        entity.Model.VAO.Activate();
        Matrix4X4<float> transformationMatrix = Maths.CreateTranslationMatrix(entity.Position, entity.RotX, entity.RotY, entity.RotZ, entity.Scale);
        shader.LoadTransformationMatrix(transformationMatrix);
        entity.Model.Texture.Activate();
        _gl.DrawElements(PrimitiveType.Triangles, (uint)entity.Model.VAO.NumberOfIndices, DrawElementsType.UnsignedInt, null);
        entity.Model.VAO.Deactivate();
    }
}