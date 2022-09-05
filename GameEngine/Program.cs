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

// OpenGL expects vertices to be defined counter clockwise by default
float[] vertices =
{
    -0.5f, 0.5f, 0.0f, 0.0f, 0.0f,    // V0
    -0.5f, -0.5f, 0.0f, 0.0f, 1.0f,   // V1
    0.5f, -0.5f, 0.0f, 1.0f, 1.0f,    // V2
    0.5f, 0.5f, 0.0f, 1.0f, 0.0f,     // V3

};

uint[] indices =
{
    0, 1, 3, // Top left triangle (V0,V1,V3)
    3, 1, 2,
};

displayManager.Window.Load += () =>
{
    input = displayManager.Window.CreateInput();
    primaryKeyboard = input.Keyboards.First();

    loader = new Loader(displayManager.GL);
    shader = new StaticShader(displayManager.GL);
    renderer = new Renderer(displayManager, shader);

    model = loader.LoadToVAO(vertices, indices);
    texture = loader.LoadTexture("Assets/Cube.png");
    staticModel = new TexturedModel(model, texture);
    entity = new Entity(staticModel, new Vector3D<float>(0.0f, 0.0f, -1.0f), 0.0f, 0.0f, 0.0f, 1.0f);
    camera = new Camera();
};

displayManager.Window.Closing += () =>
{
    shader.Dispose();
    loader.Dispose();
};

displayManager.Window.Update += (deltaTime) =>
{
    entity.IncreasePosition(0.00f, 0.0f, -0.01f);
    camera.Move(primaryKeyboard);
};

displayManager.Window.Render += (deltaTime) =>
{
    // Game logic
    renderer.Prepare();
    shader.Activate();
    shader.LoadViewMatrix(camera);
    renderer.Render(entity, shader);
    shader.Deactivate();
};

displayManager.Window
    .Run();