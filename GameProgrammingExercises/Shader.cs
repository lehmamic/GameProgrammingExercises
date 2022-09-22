using System.Numerics;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace GameProgrammingExercises;

public sealed class Shader : IDisposable
{
    // Our handle and the GL instance this class will use, these are private because they have no reason to be public.
    // Most of the time you would want to abstract items to make things like this invisible.
    private readonly uint _handle;
    private readonly GL _gl;
    private uint _vertex;
    private uint _fragment;

    public Shader(GL gl, string vertexShaderFilePath, string fragmentShaderFilePath)
    {
        _gl = gl;

        // Load and compile vertex and pixel shaders
        _vertex = CompileShader(ShaderType.VertexShader, vertexShaderFilePath);
        _fragment = CompileShader(ShaderType.FragmentShader, fragmentShaderFilePath);
        
        // Now create a shader program that links together the vertex/frag shaders
        _handle = _gl.CreateProgram();

        _gl.AttachShader(_handle, _vertex);
        _gl.AttachShader(_handle, _fragment);
        _gl.LinkProgram(_handle);

        // Verify that the program linked successfully
        _gl.GetProgram(_handle, GLEnum.LinkStatus, out var status);
        if (status == 0)
        {
            throw new ShaderException($"Program failed to link with error: {_gl.GetProgramInfoLog(_handle)}");
        }
    }

    public void SetActive()
    {
        // Set this program as the active one
        _gl.UseProgram(_handle);
    }

    public void SetUniform(string name, int value)
    {
        int location = _gl.GetUniformLocation(_handle, name);
        if (location == -1)
        {
            throw new ShaderException($"{name} uniform not found on shader.");
        }
        _gl.Uniform1(location, value);
    }
    
    public void SetUniform(string name, float value)
    {
        int location = _gl.GetUniformLocation(_handle, name);
        if (location == -1)
        {
            throw new ShaderException($"{name} uniform not found on shader.");
        }
        _gl.Uniform1(location, value);
    }

    public void SetUniform(string name, Vector2D<float> value)
    {
        int location = _gl.GetUniformLocation(_handle, name);
        if (location == -1)
        {
            throw new ShaderException($"{name} uniform not found on shader.");
        }
        _gl.Uniform2(location, value.X, value.Y);
    }

    public void SetUniform(string name, Vector3D<float> value)
    {
        int location = _gl.GetUniformLocation(_handle, name);
        if (location == -1)
        {
            throw new ShaderException($"{name} uniform not found on shader.");
        }
        _gl.Uniform3(location, value.X, value.Y, value.Z);
    }

    public unsafe void SetUniform(string name, Matrix4X4<float> value)
    {
        // Find the uniform by this name
        int location = _gl.GetUniformLocation(_handle, name);
        if (location == -1) //If GetUniformLocation returns -1 the uniform is not found.
        {
            throw new ShaderException($"{name} uniform not found on shader.");
        }

        // Send the matrix data to the uniform
        _gl.UniformMatrix4(location, 1, true, (float*) &value);
    }
    
    public unsafe void SetUniform(string name, Matrix4X4<float>[] values, uint count)
    {
        // Find the uniform by this name
        int location = _gl.GetUniformLocation(_handle, name);
        if (location == -1) //If GetUniformLocation returns -1 the uniform is not found.
        {
            throw new ShaderException($"{name} uniform not found on shader.");
        }

        fixed(void* d = values)
        {
            // Send the matrix data to the uniform
            _gl.UniformMatrix4(location, count, true, (float *) d);
        }
    }

    private uint CompileShader(ShaderType type, string path)
    {
        // Read all the text into a string
        string src = File.ReadAllText(path);
        
        // Create a shader of the specified type
        uint handle = _gl.CreateShader(type);

        // Set the source characters and try to compile
        _gl.ShaderSource(handle, src);
        _gl.CompileShader(handle);

        string infoLog = _gl.GetShaderInfoLog(handle);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            throw new ShaderException($"Error compiling shader of type {type}, failed with error {infoLog}");
        }

        return handle;
    }

    public void Dispose()
    {
        // Detach and delete the shaders
        _gl.DetachShader(_handle, _vertex);
        _gl.DetachShader(_handle, _fragment);
        _gl.DeleteShader(_vertex);
        _gl.DeleteShader(_fragment);

        // Remember to delete the program when we are done.
        _gl.DeleteProgram(_handle);
    }
}