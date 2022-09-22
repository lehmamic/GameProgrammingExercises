using System.Runtime.Serialization;

namespace GameProgrammingExercises;

public class AnimationException : Exception
{
    public AnimationException()
    {
    }

    protected AnimationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public AnimationException(string? message) : base(message)
    {
    }

    public AnimationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}