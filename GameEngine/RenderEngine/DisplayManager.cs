using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace GameEngine.RenderEngine;

public class DisplayManager : IDisposable
{
    // public static readonly int Width = 1024;
    // public static readonly int Height = 768;

    public DisplayManager(int width, int height, string windowTitle)
    {
        Width = width;
        Height = height;

        var options = WindowOptions.Default;
        options.Size = new Vector2D<int>(Width, Height);
        options.Title = windowTitle;
        options.PreferredDepthBufferBits = 32;

        Window = Silk.NET.Windowing.Window.Create(options);

        Window.Load += () =>
        {
            // Getting the opengl api for drawing to the screen.
            GL = GL.GetApi(Window);
        };
    }


    public int Width { get; }

    public int Height { get; }

    public IWindow Window { get; }

    public GL GL { get; private set; }

    public void Close()
    {
        Window?.Close();
    }

    public void Dispose()
    {
        Window.Dispose();
    }
}