// See https://aka.ms/new-console-template for more information

using GameEngine.Entities;
using GameEngine.Models;
using GameEngine.RenderEngine;
using GameEngine.Terrains;
using GameEngine.Textures;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;

using var displayManager = new DisplayManager(1024, 768, "OpenGL 3D Game Programming Tutorials");
IInputContext input = null!;
IKeyboard primaryKeyboard = null!;
Loader loader = null!;

VertexArrayObject model = null!;
TexturedModel staticModel = null!;

List<Entity> entities = new();
Light light = new Light(new Vector3D<float>(2000.0f, 2000.0f, 2000.0f), new Vector3D<float>(1.0f, 1.0f, 1.0f));

Terrain terrain = null!;
Terrain terrain2 = null!;

Camera camera = null!;
MasterRenderer renderer = null!;

displayManager.Window.Load += () =>
{
    input = displayManager.Window.CreateInput();
    primaryKeyboard = input.Keyboards.First();

    loader = new Loader(displayManager.GL);

    model = ObjLoader.LoadObjModel("Assets/tree.obj", loader);
    staticModel = new TexturedModel(model, loader.LoadTexture("Assets/tree.png"));

    Random random = new Random();
    for(int i = 0; i < 500; i++){
        entities.Add(new Entity(staticModel, new Vector3D<float>(random.NextSingle() * 800 - 400,0,random.NextSingle() * -600),0,0,0,3));
    }

    // originally in teh script: 0,0 / 1,0 but then the terrain was behind the camera 
    terrain = new Terrain(0, -1, loader, loader.LoadTexture("Assets/grass.png"));
    terrain2 = new Terrain(-1, -1, loader, loader.LoadTexture("Assets/grass.png"));

    camera = new Camera();
    renderer = new MasterRenderer(displayManager);
};

displayManager.Window.Closing += () =>
{
    renderer.Dispose();
    loader.Dispose();
};

displayManager.Window.Update += (deltaTime) =>
{
    if (primaryKeyboard.IsKeyPressed(Key.Escape))
    {
        displayManager.Close();
    }

    camera.Move(primaryKeyboard);
};

displayManager.Window.Render += (deltaTime) =>
{
    // Game logic
    renderer.ProcessTerrain(terrain);
    renderer.ProcessTerrain(terrain2);
    foreach(var entity in entities)
    {
        renderer.ProcessEntity(entity);
    }
    renderer.Render(light, camera);
};

displayManager.Window
    .Run();