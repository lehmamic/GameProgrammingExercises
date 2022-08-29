using System.Buffers;
using FreeTypeSharp;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace GameProgrammingExercises;

public class Texture : IDisposable
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

        // Enable bilinear filtering
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) GLEnum.Linear);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) GLEnum.Linear);
    }

    public Texture(GL gl, FreeTypeFaceFacade fontFace)
    {
        _gl = gl;

        Width = (int)fontFace.GlyphBitmap.width;
        Height = (int)fontFace.GlyphBitmap.rows;

        // Generate texture
        _handle = _gl.GenTexture();
        _gl.BindTexture(TextureTarget.Texture2D, _handle);

        _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, fontFace.GlyphBitmap.width, fontFace.GlyphBitmap.rows, 0, PixelFormat.Red, PixelType.UnsignedByte, fontFace.GlyphBitmap.buffer);

        // Set texture options
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) GLEnum.ClampToEdge);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) GLEnum.ClampToEdge);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) GLEnum.Linear);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) GLEnum.Linear);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) GLEnum.Linear);
    }

    public int Width { get; }

    public int Height { get; }

    public void SetActive()
    {
        _gl.BindTexture(TextureTarget.Texture2D, _handle);
    }
    
    public void Dispose()
    {
        _gl.DeleteTextures(1, _handle);
    }
}