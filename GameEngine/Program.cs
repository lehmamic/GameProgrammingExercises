// See https://aka.ms/new-console-template for more information

using GameEngine.Entities;
using GameEngine.Models;
using GameEngine.RenderEngine;
using GameEngine.Shaders;
using GameEngine.Textures;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;

using var displayManager = new DisplayManager(1024, 768, "OpenGL 3D Game Programming Tutorials");
IInputContext input = null!;
IKeyboard primaryKeyboard = null!;
Loader loader = null!;
MasterRenderer renderer = null!;
VertexArrayObject model = null!;
Texture texture = null!;
TexturedModel staticModel = null!;
Entity entity = null!;
Camera camera = null!;
Light light = new Light(new Vector3D<float>(200.0f, 200.0f, 100.0f), new Vector3D<float>(1.0f, 1.0f, 1.0f));

displayManager.Window.Load += () =>
{
    input = displayManager.Window.CreateInput();
    primaryKeyboard = input.Keyboards.First();

    loader = new Loader(displayManager.GL);
    renderer = new MasterRenderer(displayManager);

    model = ObjLoader.LoadObjModel("Assets/dragon.obj", loader);

    texture = loader.LoadTexture("Assets/white.png");
    staticModel = new TexturedModel(model, texture);
    staticModel.Texture.ShineDamper = 10.0f;
    staticModel.Texture.Reflectivity = 1.0f;

    entity = new Entity(staticModel, new Vector3D<float>(0.0f, 0.0f, -25.0f), 0.0f, 0.0f, 0.0f, 1.0f);
    camera = new Camera();
};

displayManager.Window.Closing += () =>
{
    renderer.Dispose();
    loader.Dispose();
};

displayManager.Window.Update += (deltaTime) =>
{
    entity.IncreaseRotation(0.0f, 1.0f, 0.0f);
    camera.Move(primaryKeyboard);
};

displayManager.Window.Render += (deltaTime) =>
{
    // Game logic
    renderer.ProcessEntity(entity);
    renderer.Render(light, camera);
};

displayManager.Window
    .Run();