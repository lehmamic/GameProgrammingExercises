using Silk.NET.OpenGL;

namespace GameProgrammingExercises
{
    public class VertexArrayObject : IDisposable
    {
        private readonly GL _gl;
        private readonly uint _vertexArray;
        private readonly uint _vertexBuffer;
        private readonly uint _indexBuffer;

        public unsafe VertexArrayObject(GL gl, float[] vertices, uint[] indices)
        {
            _gl = gl;

            NumberOfVertices = vertices.Length;
            NumberOfIndices = indices.Length;

            // Create vertex array
            _vertexArray = _gl.GenVertexArray();
            _gl.BindVertexArray(_vertexArray);

            // Create vertex buffer
            _vertexBuffer = _gl.GenBuffer();
            _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vertexBuffer);

            fixed (void* d = vertices)
            {
                _gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint) (vertices.Length * sizeof(float)), d, BufferUsageARB.StaticDraw);
            }

            // Create index buffer
            _indexBuffer = _gl.GenBuffer();
            _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _indexBuffer);

            fixed (void* d = indices)
            {
                _gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint) (indices.Length * sizeof(uint)), d, BufferUsageARB.StaticDraw);
            }

            // Position is 3 floats with offset 0
            VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 8, 0);

            // Normal is 3 floats with offset 3
            VertexAttributePointer(1, 3, VertexAttribPointerType.Float, 8, 3);

            // Texture coordinates is 2 floats with offset 6
            VertexAttributePointer(2, 2, VertexAttribPointerType.Float, 8, 6);
        }
        
        public int NumberOfVertices { get; }

        public int NumberOfIndices { get; }

        public void SetActive()
        {
            _gl.BindVertexArray(_vertexArray);
        }

        public void Dispose()
        {
            _gl.DeleteBuffer(_vertexBuffer);
            _gl.DeleteBuffer(_indexBuffer);
            _gl.DeleteVertexArray(_vertexArray);
        }

        private unsafe void VertexAttributePointer(uint index, int count, VertexAttribPointerType type, uint vertexSize, int offSet)
        {
            _gl.EnableVertexAttribArray(index);
            _gl.VertexAttribPointer(index, count, type, false, vertexSize * (uint) sizeof(float), (void*) (offSet * sizeof(float)));
        }
    }
}