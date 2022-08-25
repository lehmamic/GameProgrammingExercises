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

    public static Vector3D<T> GetTranslation<T>(this Matrix4X4<T> value) where T : unmanaged, IFormattable, IEquatable<T>, IComparable<T>
    {
        // Get the translation component of the matrix
        return new Vector3D<T>(value[3][0], value[3][1], value[3][2]);
    }
}