using Silk.NET.OpenGL;

namespace GameProgrammingExercises;

public sealed class BufferObject<TDataType> : IDisposable
    where TDataType : unmanaged
{
    private readonly uint _handle;
    private readonly GL _gl;

    public unsafe BufferObject(GL gl, Span<TDataType> data, BufferTargetARB bufferType)
    {
        _gl = gl;
        BufferType = bufferType;

        _handle = _gl.GenBuffer();
        _gl.BindBuffer(BufferType, _handle);

        fixed (void* d = data)
        {
            _gl.BufferData(bufferType, (nuint) (data.Length * sizeof(TDataType)), d, BufferUsageARB.StaticDraw);
        }
    }

    public BufferTargetARB BufferType { get; }

    public void Bind()
    {
        _gl.BindBuffer(BufferType, _handle);
    }

    public void Dispose()
    {
        _gl.DeleteBuffer(_handle);
    }
}