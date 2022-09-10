using Silk.NET.OpenGL;

namespace GameEngine.Models
{
    public class VertexArrayObject : IDisposable
    {
        private readonly GL _gl;
        private readonly uint _vertexArray;
        private uint _vertexBuffer;
        private readonly uint _indexBuffer;

        public VertexArrayObject(GL gl, float[] vertices, uint[] indices)
        {
            _gl = gl;

            NumberOfVertices = vertices.Length / 8;
            NumberOfIndices = indices.Length;

            // Create vertex array
            _vertexArray = _gl.GenVertexArray();
            _gl.BindVertexArray(_vertexArray);

            // Create vertex buffer
            _vertexBuffer = CreateBuffer<float>(vertices, BufferTargetARB.ArrayBuffer);

            // Create index buffer
            _indexBuffer = CreateBuffer<uint>(indices, BufferTargetARB.ElementArrayBuffer);_gl.GenBuffer();

            // Position is 3 floats with offset 0
            VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 8, 0);

            // Normal is 3 floats with offset 3
            VertexAttributePointer(1, 3, VertexAttribPointerType.Float, 8, 3);

            // Texture coordinates is 2 floats with offset 6
            VertexAttributePointer(2, 2, VertexAttribPointerType.Float, 8, 6);
        }

        public VertexArrayObject(GL gl, float[] vertices)
        {
            _gl = gl;

            NumberOfVertices = vertices.Length / 2;
            NumberOfIndices = 0;

            // Create vertex array
            _vertexArray = _gl.GenVertexArray();
            _gl.BindVertexArray(_vertexArray);

            // Create vertex buffer
            _vertexBuffer = CreateBuffer<float>(vertices, BufferTargetARB.ArrayBuffer);

            // Position is 2 floats with offset 0
            VertexAttributePointer(0, 2, VertexAttribPointerType.Float, 2, 0);
        }

        private unsafe uint CreateBuffer<T>(Span<T> data, BufferTargetARB type)
            where T : unmanaged
        {
            var vboId = _gl.GenBuffer();
            _gl.BindBuffer(type, vboId);

            fixed (void* d = data)
            {
                _gl.BufferData(type, (nuint) (data.Length * sizeof(T)), d, BufferUsageARB.StaticDraw);
            }

            return vboId;
        }

        public int NumberOfVertices { get; }

        public int NumberOfIndices { get; }

        public void Activate()
        {
            _gl.BindVertexArray(_vertexArray);
        }
        
        public void Deactivate()
        {
            _gl.BindVertexArray(0);
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