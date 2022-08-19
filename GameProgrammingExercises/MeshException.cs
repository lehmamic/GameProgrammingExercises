using System.Runtime.Serialization;

namespace GameProgrammingExercises;

public class MeshException : Exception
{
    public MeshException()
    {
    }

    protected MeshException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public MeshException(string? message) : base(message)
    {
    }
}