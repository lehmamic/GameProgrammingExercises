using GameEngine.Entities;
using GameEngine.Models;
using GameEngine.Shaders;
using GameEngine.Toolbox;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace GameEngine.RenderEngine;

public class EntityRenderer
{
    private readonly DisplayManager _displayManager;
    private readonly MasterRenderer _masterRenderer;
    private readonly StaticShader _shader;
    private readonly GL _gl;

    public EntityRenderer(DisplayManager displayManager, MasterRenderer masterRenderer, StaticShader shader, Matrix4X4<float> projectionMatrix)
    {
        _displayManager = displayManager;
        _masterRenderer = masterRenderer;
        _shader = shader;
        _gl = _displayManager.GL;

        shader.Activate();
        shader.LoadProjectionMatrix(projectionMatrix);
        shader.Deactivate();
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

        if (model.Texture.HasTransparency)
        {
            _masterRenderer.DisableCulling();
        }

        _shader.LoadNumberOfRows(model.Texture.NumberOfRows);
        _shader.LoadFakeLighting(model.Texture.UseFakeLighting);
        _shader.LoadShineVariables(model.Texture.ShineDamper, model.Texture.Reflectivity);

        _gl.ActiveTexture(TextureUnit.Texture0);
        model.Texture.Activate();
    }

    private void UnbindTexturedModel(TexturedModel model)
    {
        _masterRenderer.EnableCulling();

        model.VAO.Deactivate();
    }

    private void PrepareInstance(Entity entity)
    {
        Matrix4X4<float> transformationMatrix = Maths.CreateTransformationMatrix(entity.Position, entity.RotX, entity.RotY, entity.RotZ, entity.Scale);
        _shader.LoadTransformationMatrix(transformationMatrix);
        _shader.LoadOffset(entity.TextureXOffset, entity.TextureYOffset);
    }
}