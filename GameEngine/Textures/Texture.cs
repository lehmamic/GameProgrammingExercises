using System.Buffers;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace GameEngine.Textures;

public abstract class Texture : IDisposable
{
    public unsafe Texture(GL gl, string path)
    {
        Gl = gl;

        var imageConfig = Configuration.Default.Clone();
        imageConfig.PreferContiguousImageBuffers = true;

        using var image = Image.Load<Rgba32>(imageConfig, path);

        Handle = Gl.GenTexture();
        Gl.BindTexture(TextureTarget.Texture2D, Handle);

        if (!image.DangerousTryGetSinglePixelMemory(out Memory<Rgba32> memory))
        {
            throw new Exception("This can only happen with multi-GB images or when PreferContiguousImageBuffers is not set to true.");
        }

        using MemoryHandle pinHandle = memory.Pin();
        Gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)image.Width, (uint)image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pinHandle.Pointer);

        Gl.GenerateMipmap(TextureTarget.Texture2D);

        // Enable bilinear filtering
        Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) GLEnum.Repeat);
        Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) GLEnum.Repeat);
        Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) GLEnum.LinearMipmapLinear);
        Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) GLEnum.LinearMipmapLinear);

        // Renders the texture in a slightly higher resolution with mipmap, should not be to high otherwise we lose the advantage from mipmaping
        Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureLodBias, -0.4f);
    }

    protected Texture(GL gl)
    {
        Gl = gl;
    }

    protected uint Handle { get; set; }

    protected GL Gl { get; }

    public virtual void Activate()
    {
        Gl.BindTexture(TextureTarget.Texture2D, Handle);
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
            Gl.DeleteTextures(1, Handle);
        }
    }
}