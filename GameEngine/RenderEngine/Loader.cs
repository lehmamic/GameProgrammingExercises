using GameEngine.Models;
using GameEngine.Textures;
using Silk.NET.OpenGL;

namespace GameEngine.RenderEngine;

public class Loader : IDisposable
{
    private readonly GL _gl;

    private readonly List<uint> _vaos = new();
    private readonly List<uint> _vbos = new();
    private readonly List<ModelTexture> _textures = new();

    public Loader(GL gl)
    {
        _gl = gl;
    }

    public RawModel LoadToVAO(float[] positions, float[] textureCoords, uint[] indices)
    {
        uint vaoId = CreateVAO();
        _vaos.Add(vaoId);

        BindIndicesBuffer(indices);
        StoreDataInAttributeList(0, 3, positions);
        StoreDataInAttributeList(1, 2, textureCoords);
        UnbindVAO();

        return new RawModel(vaoId, (uint)indices.Length);
    }

    public ModelTexture LoadTexture(string fileName)
    {
        var texture = new ModelTexture(_gl, fileName);
        _textures.Add(texture);

        return texture;
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

        foreach (var texture in _textures)
        {
            texture.Dispose();
        }
    }

    private uint CreateVAO()
    {
        var vaoId = _gl.GenVertexArray();
        _gl.BindVertexArray(vaoId);
        return vaoId;
    }

    private unsafe void StoreDataInAttributeList(uint attributeNumber, int size, Span<float> vertices)
    {
        var vboId = _gl.GenBuffer();
        _vbos.Add(vboId);

        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, vboId);

        fixed (void* d = vertices)
        {
            _gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint) (vertices.Length * sizeof(float)), d, BufferUsageARB.StaticDraw);
        }

        VertexAttributePointer(attributeNumber, size, VertexAttribPointerType.Float, (uint)size, 0);

        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
    }

    private void UnbindVAO()
    {
        _gl.BindVertexArray(0);
    }

    private unsafe void BindIndicesBuffer(Span<uint> indices)
    {
        var vboId = _gl.GenBuffer();
        _vbos.Add(vboId);

        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, vboId);
        
        fixed (void* d = indices)
        {
            _gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint) (indices.Length * sizeof(uint)), d, BufferUsageARB.StaticDraw);
        }
    }
    private unsafe void VertexAttributePointer(uint index, int count, VertexAttribPointerType type, uint vertexSize, int offSet)
    {
        _gl.VertexAttribPointer(index, count, type, false, vertexSize * (uint) sizeof(float), (void*) (offSet * sizeof(float)));
    }
}