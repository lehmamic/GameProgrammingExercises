using Silk.NET.OpenGL;

namespace GameProgrammingExercises;

public class FrameBufferObject : IDisposable
{
    private readonly GL _gl;
    private readonly uint _handle;

    public FrameBufferObject(GL gl, uint handle)
    {
        _gl = gl;
        _handle = handle;
    }

    ~FrameBufferObject()
    {
        Dispose(false);
    }

    public uint BufferId => _handle;

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
            _gl.Dispose();
        }
    }

    private void ReleaseUnmanagedResources()
    {
        _gl.DeleteFramebuffers(1, _handle);
    }
}