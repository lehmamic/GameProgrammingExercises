using System.Runtime.Serialization;

namespace GameProgrammingExercises;

public class SkeletonException : Exception
{
    public SkeletonException()
    {
    }

    protected SkeletonException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public SkeletonException(string? message) : base(message)
    {
    }

    public SkeletonException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}