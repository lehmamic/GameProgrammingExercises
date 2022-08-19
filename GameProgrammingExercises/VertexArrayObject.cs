using Silk.NET.OpenGL;

namespace GameProgrammingExercises
{
    public class VertexArrayObject : IDisposable
    {
        private readonly uint _handle;
        private readonly GL _gl;
        private readonly bool _leaveOpen;

        public VertexArrayObject(GL gl, BufferObject<float> vbo, BufferObject<uint> ebo, bool leaveOpen = false)
        {
            _gl = gl;
            Vbo = vbo;
            Ebo = ebo;
            _leaveOpen = leaveOpen;

            _handle = _gl.GenVertexArray();
            _gl.BindVertexArray(_handle);

            vbo.Bind();
            ebo.Bind();

            // Position is 3 floats with offset 0
            VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 8, 0);

            // Normal is 3 floats with offset 3
            VertexAttributePointer(1, 3, VertexAttribPointerType.Float, 8, 3);

            // Texture coordinates is 2 floats with offset 6
            VertexAttributePointer(2, 2, VertexAttribPointerType.Float, 8, 6);
        }
        
        public BufferObject<float> Vbo { get; }

        public BufferObject<uint> Ebo { get; }

        public void SetActive()
        {
            _gl.BindVertexArray(_handle);
        }

        public void Dispose()
        {
            if (!_leaveOpen)
            {
                Vbo.Dispose();
                Ebo.Dispose();
            }
            _gl.DeleteVertexArray(_handle);
        }

        private unsafe void VertexAttributePointer(uint index, int count, VertexAttribPointerType type, uint vertexSize, int offSet)
        {
            _gl.VertexAttribPointer(index, count, type, false, vertexSize * (uint) sizeof(float), (void*) (offSet * sizeof(float)));
            _gl.EnableVertexAttribArray(index);
        }
    }
}