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

        using var image = Image.Load<Rgba32>(path);

        Width = image.Width;
        Height = image.Height;

        _handle = _gl.GenTexture();
        _gl.BindTexture(TextureTarget.Texture2D, _handle);

        _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba8, (uint)image.Width, (uint)image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, null);
        image.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < accessor.Height; y++)
            {
                fixed (void* data = accessor.GetRowSpan(y))
                {
                    gl.TexSubImage2D(TextureTarget.Texture2D, 0, 0, y, (uint) accessor.Width, 1, PixelFormat.Rgba, PixelType.UnsignedByte, data);
                }
            }
        });

        // Enable bilinear filtering
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