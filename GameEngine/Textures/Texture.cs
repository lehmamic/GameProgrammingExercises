using System.Buffers;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace GameEngine.Textures;

public abstract class Texture : IDisposable
{
    private readonly GL _gl;
    private readonly uint _handle;
    
    public unsafe Texture(GL gl, string path)
    {
        _gl = gl;

        var imageConfig = Configuration.Default.Clone();
        imageConfig.PreferContiguousImageBuffers = true;

        using var image = Image.Load<Rgba32>(imageConfig, path);

        Width = image.Width;
        Height = image.Height;

        _handle = _gl.GenTexture();
        _gl.BindTexture(TextureTarget.Texture2D, _handle);

        if (!image.DangerousTryGetSinglePixelMemory(out Memory<Rgba32> memory))
        {
            throw new Exception("This can only happen with multi-GB images or when PreferContiguousImageBuffers is not set to true.");
        }

        using MemoryHandle pinHandle = memory.Pin();
        _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)image.Width, (uint)image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pinHandle.Pointer);

        _gl.GenerateMipmap(TextureTarget.Texture2D);

        // Enable bilinear filtering
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) GLEnum.Repeat);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) GLEnum.Repeat);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) GLEnum.LinearMipmapLinear);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) GLEnum.LinearMipmapLinear);

        // Renders the texture in a slightly higher resolution with mipmap, should not be to high otherwise we lose the advantage from mipmaping
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureLodBias, -0.4f);
    }

    public int Width { get; }

    public int Height { get; }

    public void Activate()
    {
        _gl.BindTexture(TextureTarget.Texture2D, _handle);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _gl.DeleteTextures(1, _handle);
        }
    }
}