using Silk.NET.OpenGL;

namespace GameEngine.Shaders;

public abstract class ShaderProgram : IDisposable
{
    private readonly GL _gl;
    private uint _programId;
    private uint _vertexShaderId;
    private uint _fragmentShaderId;

    public ShaderProgram(GL gl, string vertexShaderFile, string fragmentShaderFile)
    {
        _gl = gl;

        // Load and compile vertex and pixel shaders
        _vertexShaderId = LoadShader(vertexShaderFile, ShaderType.VertexShader);
        _fragmentShaderId = LoadShader(fragmentShaderFile, ShaderType.FragmentShader);
        
        // Now create a shader program that links together the vertex/frag shaders
        _programId = _gl.CreateProgram();

        _gl.AttachShader(_programId, _vertexShaderId);
        _gl.AttachShader(_programId, _fragmentShaderId);
        _gl.LinkProgram(_programId);

        // Verify that the program linked successfully
        _gl.GetProgram(_programId, GLEnum.LinkStatus, out var status);
        if (status == 0)
        {
            throw new ShaderException($"Program failed to link with error: {_gl.GetProgramInfoLog(_programId)}");
        }
    }

    public void Start()
    {
        // Set this program as the active one
        _gl.UseProgram(_programId);
    }

    public void Stop()
    {
        _gl.UseProgram(0);
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected abstract void BindAttributes();

    protected void BindAttribute(uint attribute, string variableName)
    {
        _gl.BindAttribLocation(_programId, attribute, variableName);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Stop();

            // Detach and delete the shaders
            _gl.DetachShader(_programId, _vertexShaderId);
            _gl.DetachShader(_programId, _fragmentShaderId);
            _gl.DeleteShader(_vertexShaderId);
            _gl.DeleteShader(_fragmentShaderId);

            // Remember to delete the program when we are done.
            _gl.DeleteProgram(_programId);
        }
    }

    private uint LoadShader(string file, ShaderType type)
    {
        // Read all the text into a string
        string src = File.ReadAllText(file);
        
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
}