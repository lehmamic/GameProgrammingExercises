using GameEngine.Entities;
using GameEngine.Models;
using GameEngine.Shaders;

namespace GameEngine.RenderEngine;

public class MasterRenderer : IDisposable
{
    private readonly StaticShader _shader;
    private readonly Renderer _renderer;

    private readonly Dictionary<TexturedModel, List<Entity>> _entities = new();

    public MasterRenderer(DisplayManager displayManager)
    {
        _shader = new StaticShader(displayManager.GL);
        _renderer = new Renderer(displayManager, _shader);
    }

    public void Render(Light sun, Camera camera)
    {
        _renderer.Prepare();
        _shader.Activate();
        _shader.LoadLight(sun);
        _shader.LoadViewMatrix(camera);
        _renderer.Render(_entities);
        _shader.Deactivate();
        _entities.Clear();
    }

    public void ProcessEntity(Entity entity)
    {
        if (!_entities.ContainsKey(entity.Model))
        {
            _entities[entity.Model] = new List<Entity>();
        }

        _entities[entity.Model].Add(entity);
    }

    public void Dispose()
    {
        _shader.Dispose();
    }
}