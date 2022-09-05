using System.Security.Cryptography;
using GameEngine.Entities;
using GameEngine.Models;
using GameEngine.Shaders;
using GameEngine.Toolbox;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace GameEngine.RenderEngine;

public class Renderer
{
    private readonly DisplayManager _displayManager;
    private readonly StaticShader _shader;
    private const float FOV = 70.0f;
    private const float NearPlane = 0.1f;
    private const float FarPlane = 1000.0f;

    private readonly GL _gl;

    private readonly Matrix4X4<float> _projectionMatrix;

    public Renderer(DisplayManager displayManager, StaticShader shader)
    {
        _displayManager = displayManager;
        _shader = shader;
        _gl = _displayManager.GL;
        
        _gl.Enable(GLEnum.CullFace);
        _gl.CullFace(CullFaceMode.Back);

        // _projectionMatrix = Matrix4X4.CreatePerspectiveFieldOfView(Scalar.DegreesToRadians(FOV), _displayManager.Width / _displayManager.Height, NearPlane, FarPlane);
        _projectionMatrix = Maths.CreateProjectionMatrix(FOV, _displayManager.Width, _displayManager.Height, NearPlane, FarPlane);
        shader.Activate();
        shader.LoadProjectionMatrix(_projectionMatrix);
        shader.Deactivate();
    }

    public void Prepare()
    {
        _gl.ClearColor(0.486f, 0.086f, 0.086f, 1.0f);
        _gl.Clear((uint) (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
        
        // Enable depth buffer/disable alpha blend
        _gl.Enable(EnableCap.DepthTest);
        _gl.Disable(EnableCap.Blend);
    }

    public unsafe void Render(Dictionary<TexturedModel, List<Entity>> entities)
    {
        foreach (var model in entities.Keys)
        {
            PrepareTexturedModel(model);
            foreach (var entity in entities[model])
            {
                PrepareInstance(entity);
                _gl.DrawElements(PrimitiveType.Triangles, (uint)entity.Model.VAO.NumberOfIndices, DrawElementsType.UnsignedInt, null);
            }
            UnbindTexturedModel(model);
        }
    }

    private void PrepareTexturedModel(TexturedModel model)
    {
        model.VAO.Activate();

        _shader.LoadShineVariables(model.Texture.ShineDamper, model.Texture.Reflectivity);
        model.Texture.Activate();
    }

    private void UnbindTexturedModel(TexturedModel model)
    {
        model.VAO.Deactivate();
    }

    private void PrepareInstance(Entity entity)
    {
        Matrix4X4<float> transformationMatrix = Maths.CreateTranslationMatrix(entity.Position, entity.RotX, entity.RotY, entity.RotZ, entity.Scale);
        _shader.LoadTransformationMatrix(transformationMatrix);
    }
}