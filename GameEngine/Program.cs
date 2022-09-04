// See https://aka.ms/new-console-template for more information

using GameEngine.RenderEngine;
using Silk.NET.Windowing;

using var displayManager = new DisplayManager(1024, 768, "OpenGL 3D Game Programming Tutorials");

displayManager.Window
    .Run();