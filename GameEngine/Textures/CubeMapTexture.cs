using System.Buffers;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace GameEngine.Textures;

public class CubeMapTexture : Texture
{
    public unsafe CubeMapTexture(GL gl, string[] textureFiles)
        : base(gl)
    {
        var imageConfig = Configuration.Default.Clone();
        imageConfig.PreferContiguousImageBuffers = true;

        Handle = Gl.GenTexture();
        Gl.ActiveTexture(TextureUnit.Texture0);
        Gl.BindTexture(TextureTarget.TextureCubeMap, Handle);

        for (int i = 0; i < textureFiles.Length; i++)
        {
            using var image = Image.Load<Rgba32>(imageConfig, textureFiles[i]);
            
            if (!image.DangerousTryGetSinglePixelMemory(out Memory<Rgba32> memory))
            {
                throw new Exception("This can only happen with multi-GB images or when PreferContiguousImageBuffers is not set to true.");
            }
            
            using MemoryHandle pinHandle = memory.Pin();
            Gl.TexImage2D(
                TextureTarget.TextureCubeMapPositiveX + i,
                0,
                InternalFormat.Rgba,
                (uint)image.Width,
                (uint)image.Height,
                0,
                PixelFormat.Rgba,
                PixelType.UnsignedByte,
                pinHandle.Pointer);
        }

        // Enable bilinear filtering
        Gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int) GLEnum.ClampToEdge);
        Gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int) GLEnum.ClampToEdge);
        Gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int) GLEnum.Linear);
        Gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int) GLEnum.Linear);
    }

    public override void Activate()
    {
        Gl.BindTexture(TextureTarget.TextureCubeMap, Handle);
    }
}