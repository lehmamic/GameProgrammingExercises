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
    private readonly StaticShader _shader;
    private readonly GL _gl;

    public EntityRenderer(DisplayManager displayManager, StaticShader shader, Matrix4X4<float> projectionMatrix)
    {
        _displayManager = displayManager;
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