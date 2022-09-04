// See https://aka.ms/new-console-template for more information

using GameEngine.RenderEngine;
using Silk.NET.Windowing;

using var displayManager = new DisplayManager(1024, 768, "OpenGL 3D Game Programming Tutorials");

Loader loader = null!;
Renderer renderer = null!;
RawModel model = null!;

// OpenGL expects vertices to be defined counter clockwise by default
float[] vertices = {
    // Left bottom triangle
    -0.5f, 0.5f, 0f,
    -0.5f, -0.5f, 0f,
    0.5f, -0.5f, 0f,
    // Right top triangle
    0.5f, -0.5f, 0f,
    0.5f, 0.5f, 0f,
    -0.5f, 0.5f, 0f
};

displayManager.Window.Load += () =>
{
    loader = new Loader(displayManager.GL);
    renderer = new Renderer(displayManager.GL);

    model = loader.LoadToVAO(vertices);
};

displayManager.Window.Closing += () =>
{
    loader.Dispose();
};

displayManager.Window.Render += (deltaTime) =>
{
    renderer.Prepare();

    // Game logic
    renderer.Render(model);
};

displayManager.Window
    .Run();