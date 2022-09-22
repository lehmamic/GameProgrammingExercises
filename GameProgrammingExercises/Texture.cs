using System.Buffers;
using FreeTypeSharp.Native;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace GameProgrammingExercises;

public class Texture : IDisposable
{
    private readonly GL _gl;
    private readonly uint _handle;

    private Texture(GL gl, uint handle, int width, int height)
    {
        _gl = gl;
        _handle = handle;
        Width = width;
        Height = height;
    }

    ~Texture()
    {
        Dispose(false);
    }

    public static unsafe Texture Load(GL gl, string path)
    {
        var imageConfig = Configuration.Default.Clone();
        imageConfig.PreferContiguousImageBuffers = true;

        using var image = Image.Load<Rgba32>(imageConfig, path);

        var handle = gl.GenTexture();
        gl.BindTexture(TextureTarget.Texture2D, handle);

        if (!image.DangerousTryGetSinglePixelMemory(out Memory<Rgba32> memory))
        {
            throw new Exception("This can only happen with multi-GB images or when PreferContiguousImageBuffers is not set to true.");
        }

        using MemoryHandle pinHandle = memory.Pin();
        gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)image.Width, (uint)image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pinHandle.Pointer);

        // Generate mipmaps for texture
        gl.GenerateMipmap(TextureTarget.Texture2D);

        // Set texture options
        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) GLEnum.LinearMipmapLinear);
        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) GLEnum.Linear);

        // Enable anisotropic filtering, if supported
        if (gl.IsExtensionPresent("EXT_texture_filter_anisotropic"))
        {
            // Get the maximum anisotropy value
            gl.GetFloat(GLEnum.MaxTextureMaxAnisotropy, out float largest);
            // Enable it
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxAnisotropy, largest);
        }

        return new Texture(gl, handle, image.Width, image.Height);
    }

    public static unsafe Texture CreateFromGlyph(GL gl, FT_Bitmap glyph)
    {
        // Generate texture
        var handle = gl.GenTexture();
        gl.BindTexture(TextureTarget.Texture2D, handle);

        gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Red, glyph.width, glyph.rows, 0, PixelFormat.Red, PixelType.UnsignedByte, glyph.buffer.ToPointer());

        // Set texture options
        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) GLEnum.ClampToEdge);
        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) GLEnum.ClampToEdge);
        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) GLEnum.Linear);
        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) GLEnum.Linear);

        return new Texture(gl, handle, (int) glyph.width, (int) glyph.rows);
    }
    
    public static unsafe Texture CreateForRendering(GL gl, int width, int height, InternalFormat format)
    {
        // Create the texture id
        var handle = gl.GenTexture();
        gl.BindTexture(TextureTarget.Texture2D, handle);

        // Set the image width/height with null initial data
        gl.TexImage2D(TextureTarget.Texture2D, 0, format, (uint)width, (uint)height, 0, PixelFormat.Rgba, PixelType.Float, null);

        // For a texture we'll render to, just use nearest neighbor
        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) GLEnum.Nearest);
        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) GLEnum.Nearest);

        return new Texture(gl, handle, width, height);
    }

    public int Width { get; }

    public int Height { get; }

    public uint TextureId => _handle;

    public void SetActive(int index = 0)
    {
        _gl.ActiveTexture(TextureUnit.Texture0 + index);
        _gl.BindTexture(TextureTarget.Texture2D, _handle);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        ReleaseUnmanagedResources();
        if (disposing)
        {
        }
    }

    private void ReleaseUnmanagedResources()
    {
        _gl.DeleteTextures(1, _handle);
    }
}