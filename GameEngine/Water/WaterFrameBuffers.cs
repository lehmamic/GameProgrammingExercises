using GameEngine.RenderEngine;
using Silk.NET.OpenGL;

namespace GameEngine.Water;

public class WaterFrameBuffers : IDisposable
{
    protected const int ReflectionWidth = 320;
    private const int ReflectionHeight = 180;

    protected const int RefractionWidth = 1280;
    private const int RefractionHeight = 720;

    private readonly DisplayManager _displayManager;
    private readonly GL _gl;

    private uint _reflectionFrameBuffer;
    private uint _reflectionTexture;
    private uint _reflectionDepthBuffer;
    
    private uint _refractionFrameBuffer;
    private uint _refractionTexture;
    private uint _refractionDepthTexture;

    //call when loading the game
    public WaterFrameBuffers(DisplayManager displayManager)
    {
        _displayManager = displayManager;
        _gl = displayManager.GL;
        InitialiseReflectionFrameBuffer();
        InitialiseRefractionFrameBuffer();
    }

    public uint ReflectionTexture => _reflectionTexture;

    public uint RefractionTexture => _refractionTexture;

    public uint RefractionDepthTexture => _refractionDepthTexture;

    public void BindReflectionFrameBuffer()
    {//call before rendering to this FBO
        BindFrameBuffer(_reflectionFrameBuffer,ReflectionWidth,ReflectionHeight);
    }
    
    public void BindRefractionFrameBuffer()
    {//call before rendering to this FBO
        BindFrameBuffer(_refractionFrameBuffer,RefractionWidth,RefractionHeight);
    }
    
    public void UnbindCurrentFrameBuffer()
    {//call to switch to default frame buffer
        _gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        _gl.Viewport(0, 0, (uint)_displayManager.Width, (uint) _displayManager.Height);
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void InitialiseReflectionFrameBuffer()
    {
        _reflectionFrameBuffer = CreateFrameBuffer();
        _reflectionTexture = CreateTextureAttachment(ReflectionWidth,ReflectionHeight);
        _reflectionDepthBuffer = CreateDepthBufferAttachment(ReflectionWidth,ReflectionHeight);
        UnbindCurrentFrameBuffer();
    }
    
    private void InitialiseRefractionFrameBuffer()
    {
        _refractionFrameBuffer = CreateFrameBuffer();
        _refractionTexture = CreateTextureAttachment(RefractionWidth,RefractionHeight);
        _refractionDepthTexture = CreateDepthTextureAttachment(RefractionWidth,RefractionHeight);
        UnbindCurrentFrameBuffer();
    }
    
    private void BindFrameBuffer(uint frameBuffer, uint width, uint height)
    {
        _gl.BindTexture(TextureTarget.Texture2D, 0);//To make sure the texture isn't bound
        _gl.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
        _gl.Viewport(0, 0, width, height);
    }

    private uint CreateFrameBuffer()
    {
        uint frameBuffer = _gl.GenFramebuffer();
        //generate name for frame buffer
        _gl.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
        //create the framebuffer
       _gl.DrawBuffer(DrawBufferMode.ColorAttachment0);
        //indicate that we will always render to color attachment 0
        return frameBuffer;
    }

    private unsafe uint CreateTextureAttachment(uint width, uint height)
    {
        uint texture = _gl.GenTexture();
        _gl.BindTexture(TextureTarget.Texture2D, texture);
        _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, null);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) GLEnum.Linear);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) GLEnum.Linear);
        _gl.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, texture, 0);
        return texture;
    }
    
    private unsafe uint CreateDepthTextureAttachment(uint width, uint height)
    {
        uint texture = _gl.GenTexture();
        _gl.BindTexture(TextureTarget.Texture2D, texture);
        _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.DepthComponent32, width, height, 0, PixelFormat.DepthComponent, PixelType.Float, null);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) GLEnum.Linear);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) GLEnum.Linear);
        _gl.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, texture, 0);
        return texture;
    }

    private uint CreateDepthBufferAttachment(uint width, uint height)
    {
        uint depthBuffer = _gl.GenRenderbuffer();
        _gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthBuffer);
        _gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.DepthComponent, width, height);
        _gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, depthBuffer);
        return depthBuffer;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _gl.DeleteFramebuffer(_reflectionFrameBuffer);
            _gl.DeleteTexture(_reflectionTexture);
            _gl.DeleteRenderbuffer(_reflectionDepthBuffer);
            _gl.DeleteFramebuffer(_refractionFrameBuffer);
            _gl.DeleteTexture(_refractionTexture);
            _gl.DeleteTexture(_refractionDepthTexture);
        }
    }
}