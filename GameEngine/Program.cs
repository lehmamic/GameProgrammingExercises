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
Renderer renderer = null!;
StaticShader shader = null!;
VertexArrayObject model = null!;
Texture texture = null!;
TexturedModel staticModel = null!;
Entity entity = null!;
Camera camera = null!;
Light light = new Light(new Vector3D<float>(0.0f, 0.0f, -20.0f), new Vector3D<float>(1.0f, 1.0f, 1.0f));

displayManager.Window.Load += () =>
{
    input = displayManager.Window.CreateInput();
    primaryKeyboard = input.Keyboards.First();

    loader = new Loader(displayManager.GL);
    shader = new StaticShader(displayManager.GL);
    renderer = new Renderer(displayManager, shader);

    model = ObjLoader.LoadObjModel("Assets/dragon.obj", loader);
    texture = loader.LoadTexture("Assets/white.png");
    staticModel = new TexturedModel(model, texture);
    entity = new Entity(staticModel, new Vector3D<float>(0.0f, 0.0f, -25.0f), 0.0f, 0.0f, 0.0f, 1.0f);
    camera = new Camera();
};

displayManager.Window.Closing += () =>
{
    shader.Dispose();
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
    renderer.Prepare();
    shader.Activate();
    shader.LoadLight(light);
    shader.LoadViewMatrix(camera);
    renderer.Render(entity, shader);
    shader.Deactivate();
};

displayManager.Window
    .Run();