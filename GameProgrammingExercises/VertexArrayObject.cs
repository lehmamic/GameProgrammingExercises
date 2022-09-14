using System.Runtime.InteropServices;
using Silk.NET.OpenGL;

namespace GameProgrammingExercises
{
    public class VertexArrayObject : IDisposable
    {
        private readonly GL _gl;
        private readonly uint _vertexArray;
        private readonly uint _vertexBuffer;
        private readonly uint _indexBuffer;
        
        public unsafe VertexArrayObject(GL gl, ReadOnlySpan<VertexPosNormTex> vertices, uint[] indices)
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

            fixed (void* d = &MemoryMarshal.GetReference(vertices))
            {
                _gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint) (vertices.Length * VertexPosNormTex.SizeInBytes), d, BufferUsageARB.StaticDraw);
            }

            // Create index buffer
            _indexBuffer = _gl.GenBuffer();
            _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _indexBuffer);

            fixed (void* d = indices)
            {
                _gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint) (indices.Length * sizeof(uint)), d, BufferUsageARB.StaticDraw);
            }

            // Specify the vertex attributes

                // Position is 3 floats with offset 0
                _gl.EnableVertexAttribArray(0);
                _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, VertexPosNormTex.SizeInBytes, (void*) 0);

                // Normal is 3 floats with offset 3
                _gl.EnableVertexAttribArray(1);
                _gl.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, VertexPosNormTex.SizeInBytes, (void*) (3 * sizeof(float)));

                // Texture coordinates is 2 floats with offset 6
                _gl.EnableVertexAttribArray(2);
                _gl.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, VertexPosNormTex.SizeInBytes, (void*) (6 * sizeof(float)));
        }

        public unsafe VertexArrayObject(GL gl, ReadOnlySpan<VertexPosNormSkinTex> vertices, uint[] indices)
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
                _gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint) (vertices.Length * VertexPosNormSkinTex.SizeInBytes), d, BufferUsageARB.StaticDraw);
            }

            // Create index buffer
            _indexBuffer = _gl.GenBuffer();
            _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _indexBuffer);

            fixed (void* d = indices)
            {
                _gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint) (indices.Length * sizeof(uint)), d, BufferUsageARB.StaticDraw);
            }

            // Specify the vertex attributes
            
            // Position is 3 floats with offset 0
            _gl.EnableVertexAttribArray(0);
            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, VertexPosNormSkinTex.SizeInBytes, (void*) 0);

            // Normal is 3 floats with offset 3
            _gl.EnableVertexAttribArray(1);
            _gl.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, VertexPosNormSkinTex.SizeInBytes, (void*) (3 * sizeof(float)));

            // Skinning indices (keep as ints)
            _gl.EnableVertexAttribArray(2);
            _gl.VertexAttribIPointer(2, 4, VertexAttribIType.UnsignedByte, VertexPosNormSkinTex.SizeInBytes, (void*) (6 * sizeof(float)));

            // Skinning weights (convert to floats)
            _gl.EnableVertexAttribArray(3);
            _gl.VertexAttribPointer(3, 4, VertexAttribPointerType.UnsignedByte, true, VertexPosNormSkinTex.SizeInBytes, (void*) (6 * sizeof(float) + 4 * sizeof(byte)));

            // Texture coordinates is 2 floats
            _gl.EnableVertexAttribArray(4);
            _gl.VertexAttribPointer(4, 2, VertexAttribPointerType.Float, false, VertexPosNormSkinTex.SizeInBytes, (void*) (6 * sizeof(float) + 8 * sizeof(byte)));
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
    }
}