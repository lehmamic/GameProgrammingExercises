using Silk.NET.OpenGL;

namespace GameProgrammingExercises;

public class GBuffer : FrameBufferObject
{
    private const int NumGbufferTextures = 3;

    private readonly List<Texture> _textures;

    public GBuffer(GL gl, uint handle, List<Texture> textures)
        : base(gl, handle)
    {
        _textures = textures;
    }

    public static unsafe GBuffer Create(GL gl, int width, int height)
    {
        // Generate a frame buffer for the mirror texture
        gl.GenFramebuffers(1, out uint mirrorBuffer);
        gl.BindFramebuffer(FramebufferTarget.Framebuffer, mirrorBuffer);

        // Add a depth buffer to this target
        gl.GenRenderbuffers(1, out uint depthBuffer);
        gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthBuffer);
        gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.DepthComponent, (uint)width, (uint)height);
        gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, depthBuffer);

        // Create textures for each output in the G-buffer
        var textures = new List<Texture>();
        for (int i = 0; i < NumGbufferTextures; i++)
        {
            // We want three 32-bit float components for each texture
            var tex = Texture.CreateForRendering(gl, width, height, InternalFormat.Rgb32f);
            textures.Add(tex);

            // Attach this texture to a color output
            gl.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0 + i, tex.TextureId, 0);
        }

        // Create a vector of the color attachments
        var attachments = new GLEnum[NumGbufferTextures];
        for (int i = 0; i < NumGbufferTextures; i++)
        {
            attachments[i] = GLEnum.ColorAttachment0 + i;
        }

        // Set the list of buffers to draw to
        fixed (void* d = attachments)
        {
            gl.DrawBuffers((uint) attachments.Length, (GLEnum *) d);
        }

        // Make sure everything worked
        if (gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != GLEnum.FramebufferComplete)
        {
            // If it didn't work, delete the framebuffer,
            // unload/delete the texture and return false
            throw new InvalidOperationException("Initializing the framebuffer failed");
        }

        return new GBuffer(gl, depthBuffer, textures);
    }

    public Texture? GetTexture(GBufferType type)
    {
        if (_textures.Count > 0)
        {
            return _textures[(int)type];
        }
        else
        {
            return null;
        }
    }

    public void SetTexturesActive()
    {
        for (int i = 0; i < _textures.Count; i++)
        {
            _textures[i].SetActive(i);
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            foreach (var texture in _textures)
            {
                texture.Dispose();
            }
            _textures.Clear();
        }
    }
}