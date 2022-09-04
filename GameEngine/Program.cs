// See https://aka.ms/new-console-template for more information

using GameEngine.Models;
using GameEngine.RenderEngine;
using GameEngine.Shaders;
using GameEngine.Textures;
using Silk.NET.Windowing;

using var displayManager = new DisplayManager(1024, 768, "OpenGL 3D Game Programming Tutorials");

Loader loader = null!;
Renderer renderer = null!;
StaticShader shader = null!;
VertexArrayObject model = null!;
Texture texture = null!;
TexturedModel texturedModel = null!;

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
    loader = new Loader(displayManager.GL);
    renderer = new Renderer(displayManager.GL);
    shader = new StaticShader(displayManager.GL);

    model = loader.LoadToVAO(vertices, indices);
    texture = loader.LoadTexture("Assets/Cube.png");
    texturedModel = new TexturedModel(model, texture);
};

displayManager.Window.Closing += () =>
{
    shader.Dispose();
    loader.Dispose();
};

displayManager.Window.Render += (deltaTime) =>
{
    // Game logic
    renderer.Prepare();
    shader.Activate();
    renderer.Render(texturedModel);
    shader.Deactivate();
};

displayManager.Window
    .Run();