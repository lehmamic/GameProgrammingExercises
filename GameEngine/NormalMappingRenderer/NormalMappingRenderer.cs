using GameEngine.Entities;
using GameEngine.Models;
using GameEngine.RenderEngine;
using GameEngine.Toolbox;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace GameEngine.NormalMappingRenderer;

public class NormalMappingRenderer : IDisposable
{
    private readonly DisplayManager _displayManager;
    private readonly MasterRenderer _masterRenderer;
    private readonly GL _gl;
    private readonly NormalMappingShader _shader;

    public NormalMappingRenderer(DisplayManager displayManager, MasterRenderer masterRenderer, Matrix4X4<float> projectionMatrix) {
        _displayManager = displayManager;
        _masterRenderer = masterRenderer;
        _gl = displayManager.GL;
        _shader = new NormalMappingShader(displayManager.GL);

        _shader.Activate();
        _shader.LoadProjectionMatrix(projectionMatrix);
        _shader.ConnectTextureUnits();
        _shader.Deactivate();
    }
    
    public unsafe void Render(Dictionary<TexturedModel, List<Entity>> entities, Vector4D<float> clipPlane, List<Light> lights, Camera camera) {
        _shader.Activate();
        Prepare(clipPlane, lights, camera);
        foreach (TexturedModel model in entities.Keys)
        {
            PrepareTexturedModel(model);
            List<Entity> batch = entities[model];
            foreach (Entity entity in batch)
            {
                PrepareInstance(entity);
                // vertex count? really?
                _gl.DrawElements(PrimitiveType.Triangles, (uint)entity.Model.VAO.NumberOfIndices, DrawElementsType.UnsignedInt, null);
                // GL11.glDrawElements(GL11.GL_TRIANGLES, model.getRawModel().getVertexCount(), GL11.GL_UNSIGNED_INT, 0);
            }
            UnbindTexturedModel(model);
        }
        _shader.Deactivate();
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void PrepareTexturedModel(TexturedModel model) {
        model.VAO.Activate();

        if (model.Texture.HasTransparency)
        {
            _masterRenderer.DisableCulling();
        }

        _shader.LoadNumberOfRows(model.Texture.NumberOfRows);
        _shader.LoadShineVariables(model.Texture.ShineDamper, model.Texture.Reflectivity);

        _gl.ActiveTexture(TextureUnit.Texture0);
        model.Texture.Activate();

        _gl.ActiveTexture(TextureUnit.Texture1);
        model.NormalMap?.Activate();
    }
    
    private void UnbindTexturedModel(TexturedModel model) {
        _masterRenderer.EnableCulling();

        model.VAO.Deactivate();
    }

    private void PrepareInstance(Entity entity) {
        var transformationMatrix = Maths.CreateTransformationMatrix(entity.Position, entity.RotX, entity.RotY, entity.RotZ, entity.Scale);
        _shader.LoadTransformationMatrix(transformationMatrix);
        _shader.LoadOffset(entity.TextureXOffset, entity.TextureYOffset);
    }

    private void Prepare(Vector4D<float> clipPlane, List<Light> lights, Camera camera) {
        _shader.LoadClipPlane(clipPlane);
        //need to be public variables in MasterRenderer
        _shader.LoadSkyColor(MasterRenderer.Red, MasterRenderer.Green, MasterRenderer.Blue);
        var viewMatrix = Maths.CreateViewMatrix(camera);

        _shader.LoadLights(lights, viewMatrix);
        _shader.LoadViewMatrix(viewMatrix);
    }


    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            _shader.Dispose();
        }
    }
}