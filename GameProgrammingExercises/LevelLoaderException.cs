using System.Runtime.Serialization;

namespace GameProgrammingExercises;

public class LevelLoaderException : Exception
{
    public LevelLoaderException()
    {
    }

    protected LevelLoaderException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public LevelLoaderException(string? message) : base(message)
    {
    }

    public LevelLoaderException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}