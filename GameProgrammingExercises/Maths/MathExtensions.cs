using Silk.NET.Input;
using Silk.NET.Maths;

namespace GameProgrammingExercises.Maths;

public static class MathExtensions
{
    public static bool NearZero(this double value)
    {
        return Math.Abs(value) < Double.Epsilon;
    }

    public static bool NearZero(this float value)
    {
        return Math.Abs(value) < Double.Epsilon;
    }

    public static Vector2D<float> ToVector2D(this ScrollWheel value)
    {
        return new Vector2D<float>(value.X, value.Y);
    }
}