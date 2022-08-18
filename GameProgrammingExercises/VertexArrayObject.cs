using Silk.NET.OpenGL;

namespace GameProgrammingExercises
{
    public class VertexArrayObject : IDisposable
    {
        private readonly uint _handle;
        private readonly GL _gl;
        private readonly BufferObject<float> _vbo;
        private readonly BufferObject<uint> _ebo;
        private readonly bool _leaveOpen;

        public VertexArrayObject(GL gl, BufferObject<float> vbo, BufferObject<uint> ebo, bool leaveOpen = false)
        {
            _gl = gl;
            _vbo = vbo;
            _ebo = ebo;
            _leaveOpen = leaveOpen;

            _handle = _gl.GenVertexArray();
            _gl.BindVertexArray(_handle);

            vbo.Bind();
            ebo.Bind();

            VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 5, 0);
            VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 5, 3);
        }

        private unsafe void VertexAttributePointer(uint index, int count, VertexAttribPointerType type, uint vertexSize, int offSet)
        {
            _gl.VertexAttribPointer(index, count, type, false, vertexSize * (uint) sizeof(float), (void*) (offSet * sizeof(float)));
            _gl.EnableVertexAttribArray(index);
        }

        public void SetActive()
        {
            _gl.BindVertexArray(_handle);
        }

        public void Dispose()
        {
            if (!_leaveOpen)
            {
                _vbo.Dispose();
                _ebo.Dispose();
            }
            _gl.DeleteVertexArray(_handle);
        }
    }
}