using Silk.NET.OpenGL;

namespace GameProgrammingExercises;

public sealed class VertexArrayObject : IDisposable
{
    // Our handle and the GL instance this class will use, these are private because they have no reason to be public.
    // Most of the time you would want to abstract items to make things like this invisible.
    private uint _handle;
    private GL _gl;
    private readonly BufferObject<float> _vbo;
    private readonly BufferObject<uint> _ebo;
    private readonly bool _leaveOpen;

    /// <summary>
    /// Constructor. Creates an instance of VertexArrayObject.
    /// </summary>
    /// <param name="gl">The OpenGL reference.</param>
    /// <param name="vbo">The vertices buffer object.</param>
    /// <param name="ebo">The indices buffer object.</param>
    /// <param name="leaveOpen">Indicates whether to dispose the buffers or not.</param>
    public unsafe VertexArrayObject(GL gl, BufferObject<float> vbo, BufferObject<uint> ebo, bool leaveOpen = false)
    {
        // Saving the GL instance.
        _gl = gl;
        _vbo = vbo;
        _ebo = ebo;
        _leaveOpen = leaveOpen;
        
        _handle = _gl.GenVertexArray();
        _gl.BindVertexArray(_handle);

        vbo.Bind();
        ebo.Bind();

        // Specify the vertex attributes
        // (For now, assume one vertex format)
        // Position is 3 floats starting at offset 0
        _gl.EnableVertexAttribArray(0);
        _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * (uint) sizeof(float), (void*) (0 * sizeof(float)));
        
        _gl.EnableVertexAttribArray(1);
        _gl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 5 * (uint) sizeof(float), (void*) (3 * sizeof(float)));
    }

    /// <summary>
    /// Activate this vertex array (so we can draw it)
    /// </summary>
    public void SetActive()
    {
        _gl.BindVertexArray(_handle);
    }

    public void Dispose()
    {
        // Remember to dispose this object so the data GPU side is cleared.
        // We dont delete the VBO and EBO here, as you can have one VBO stored under multiple VAO's.

        if (!_leaveOpen)
        {
            _vbo.Dispose();
            _ebo.Dispose();
        }

        _gl.DeleteVertexArray(_handle);
    }
}