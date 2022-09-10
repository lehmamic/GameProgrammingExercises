using GameEngine.Entities;
using GameEngine.Models;
using GameEngine.Shaders;
using GameEngine.Skybox;
using GameEngine.Terrains;
using GameEngine.Toolbox;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace GameEngine.RenderEngine;

public class MasterRenderer : IDisposable
{
    private const float FOV = 70.0f;
    private const float NearPlane = 0.1f;
    private const float FarPlane = 1000.0f;

    private const float Red = 0.5444f;
    private const float Green = 0.62f;
    private const float Blue = 0.69f;

    private readonly DisplayManager _displayManager;
    private readonly GL _gl;

    private readonly StaticShader _entityShader;
    private readonly EntityRenderer _entityRenderer;
    
    private readonly TerrainShader _terrainShader;
    private readonly TerrainRenderer _terrainRenderer;

    private readonly SkyboxRenderer _skyboxRenderer;

    private readonly Matrix4X4<float> _projectionMatrix;

    private readonly Dictionary<TexturedModel, List<Entity>> _entities = new();
    private readonly List<Terrain> _terrains = new();

    public MasterRenderer(DisplayManager displayManager, Loader loader)
    {
        _displayManager = displayManager;
        _gl = displayManager.GL;

        EnableCulling();

        // _projectionMatrix = Matrix4X4.CreatePerspectiveFieldOfView(Scalar.DegreesToRadians(FOV), _displayManager.Width / _displayManager.Height, NearPlane, FarPlane);
        _projectionMatrix = Maths.CreateProjectionMatrix(FOV, _displayManager.Width, _displayManager.Height, NearPlane, FarPlane);

        _entityShader = new StaticShader(displayManager.GL);
        _entityRenderer = new EntityRenderer(displayManager, this, _entityShader, _projectionMatrix);
        
        _terrainShader = new TerrainShader(displayManager.GL);
        _terrainRenderer = new TerrainRenderer(displayManager, _terrainShader, _projectionMatrix);

        _skyboxRenderer = new SkyboxRenderer(displayManager, loader, _projectionMatrix);
    }

    public Matrix4X4<float> ProjectionMatrix => _projectionMatrix;

    public void RenderScene(float deltaTime, List<Entity> entities, List<Terrain> terrains, List<Light> lights, Camera camera)
    {
        foreach (Terrain terrain in terrains)
        {
            ProcessTerrain(terrain);
        }
        foreach(var entity in entities)
        {
            ProcessEntity(entity);
        }
        Render(deltaTime, lights, camera);
    }

    public void Render(float deltaTime, List<Light> lights, Camera camera)
    {
        Prepare();

        _entityShader.Activate();
        _entityShader.LoadSkyColor(Red, Green, Blue);
        _entityShader.LoadLights(lights);
        _entityShader.LoadViewMatrix(camera);
        _entityRenderer.Render(_entities);
        _entityShader.Deactivate();
        _entities.Clear();

        _terrainShader.Activate();
        _terrainShader.LoadSkyColor(Red, Green, Blue);
        _terrainShader.LoadLights(lights);
        _terrainShader.LoadViewMatrix(camera);
        _terrainRenderer.Render(_terrains);
        _terrainShader.Deactivate();
        _terrains.Clear();
        
        _skyboxRenderer.Render(deltaTime, camera, Red, Green, Blue);
    }

    public void ProcessTerrain(Terrain terrain)
    {
        _terrains.Add(terrain);
    }

    public void ProcessEntity(Entity entity)
    {
        if (!_entities.ContainsKey(entity.Model))
        {
            _entities[entity.Model] = new List<Entity>();
        }

        _entities[entity.Model].Add(entity);
    }

    public void EnableCulling()
    {
        _gl.Enable(GLEnum.CullFace);
        _gl.CullFace(CullFaceMode.Back);
    }

    public void DisableCulling()
    {
        _gl.Disable(GLEnum.CullFace);
    }

    public void Dispose()
    {
        _terrainShader.Dispose();
        _entityShader.Dispose();
    }
    
    private void Prepare()
    {
        _gl.ClearColor(Red, Green, Blue, 1.0f);
        _gl.Clear((uint) (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
        
        // Enable depth buffer/disable alpha blend
        _gl.Enable(EnableCap.DepthTest);
        _gl.Disable(EnableCap.Blend);
    }
}