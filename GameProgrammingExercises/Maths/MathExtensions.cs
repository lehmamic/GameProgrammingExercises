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
}