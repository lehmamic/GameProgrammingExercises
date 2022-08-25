using System.Runtime.Serialization;

namespace GameProgrammingExercises;

public class AudioSystemException : Exception
{
    public AudioSystemException()
    {
    }

    protected AudioSystemException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public AudioSystemException(string? message) : base(message)
    {
    }

    public AudioSystemException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}