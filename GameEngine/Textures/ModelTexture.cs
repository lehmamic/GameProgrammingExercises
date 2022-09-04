using System.Buffers;
using FreeTypeSharp;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace GameEngine.Textures;

public class ModelTexture : IDisposable
{
    private readonly GL _gl;

    public unsafe ModelTexture(GL gl, string path)
    {
        _gl = gl;

        var imageConfig = Configuration.Default.Clone();
        imageConfig.PreferContiguousImageBuffers = true;

        using var image = Image.Load<Rgba32>(imageConfig, path);

        Width = image.Width;
        Height = image.Height;

        TextureId = _gl.GenTexture();
        _gl.BindTexture(TextureTarget.Texture2D, TextureId);

        if (!image.DangerousTryGetSinglePixelMemory(out Memory<Rgba32> memory))
        {
            throw new InvalidOperationException("This can only happen with multi-GB images or when PreferContiguousImageBuffers is not set to true.");
        }

        using MemoryHandle pinHandle = memory.Pin();
        _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)image.Width, (uint)image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pinHandle.Pointer);

        // Enable bilinear filtering
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) GLEnum.Linear);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) GLEnum.Linear);
    }

    public uint TextureId { get; }

    public int Width { get; }

    public int Height { get; }

    public void Dispose()
    {
        _gl.DeleteTextures(1, TextureId);
    }
}