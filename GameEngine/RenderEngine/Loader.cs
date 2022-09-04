using Silk.NET.OpenGL;

namespace GameEngine.RenderEngine;

public class Loader : IDisposable
{
    private readonly GL _gl;

    private readonly List<uint> _vaos = new();
    private readonly List<uint> _vbos = new();

    public Loader(GL gl)
    {
        _gl = gl;
    }

    public RawModel LoadToVAO(float[] positions)
    {
        uint vaoId = CreateVAO();
        _vaos.Add(vaoId);

        StoreDataInAttributeList(0, positions);
        UnbindVAO();

        return new RawModel(vaoId, (uint)positions.Length / 3);
    }
    
    public void Dispose()
    {
        foreach (var vao in _vaos)
        {
            _gl.DeleteVertexArray(vao);
        }

        foreach (var vbo in _vbos)
        {
            _gl.DeleteBuffer(vbo);
        }
    }

    private uint CreateVAO()
    {
        var vaoId = _gl.GenVertexArray();
        _gl.BindVertexArray(vaoId);
        return vaoId;
    }

    private unsafe void StoreDataInAttributeList(uint attributeNumber, float[] data)
    {
        var vboId = _gl.GenBuffer();
        _vbos.Add(vboId);

        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, vboId);

        fixed (void* d = data)
        {
            _gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint) (data.Length * sizeof(float)), d, BufferUsageARB.StaticDraw);
        }

        VertexAttributePointer(attributeNumber, 3, VertexAttribPointerType.Float, 3, 0);

        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
    }

    private void UnbindVAO()
    {
        _gl.BindVertexArray(0);
    }
    
    private unsafe void VertexAttributePointer(uint index, int count, VertexAttribPointerType type, uint vertexSize, int offSet)
    {
        _gl.VertexAttribPointer(index, count, type, false, vertexSize * (uint) sizeof(float), (void*) (offSet * sizeof(float)));
    }
}