using System.Runtime.Serialization;

namespace GameEngine.Shaders;

public class ShaderException : Exception
{
    public ShaderException()
    {
    }

    protected ShaderException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public ShaderException(string? message) : base(message)
    {
    }

    public ShaderException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}