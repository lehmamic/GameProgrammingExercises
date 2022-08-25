using FMOD;
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
    
    // Get the X axis of the matrix (forward)
    public static Vector3D<T> GetXAxis<T>(this Matrix4X4<T> value) where T : unmanaged, IFormattable, IEquatable<T>, IComparable<T>
    {
        return Vector3D.Normalize(new Vector3D<T>(value[0][0], value[0][1], value[0][2]));
    }

    // Get the Y axis of the matrix (left)
    public static Vector3D<T> GetYAxis<T>(this Matrix4X4<T> value) where T : unmanaged, IFormattable, IEquatable<T>, IComparable<T>
    {
        return Vector3D.Normalize(new Vector3D<T>(value[1][0], value[1][1], value[1][2]));
    }

    // Get the Z axis of the matrix (up)
    public static Vector3D<T> GetZAxis<T>(this Matrix4X4<T> value) where T : unmanaged, IFormattable, IEquatable<T>, IComparable<T>
    {
        return Vector3D.Normalize(new Vector3D<T>(value[2][0], value[2][1], value[2][2]));
    }

    // Extract the scale component from the matrix
    public static Vector3D<T> GetScale<T>(this Matrix4X4<T> value) where T : unmanaged, IFormattable, IEquatable<T>, IComparable<T>
    {
        return new Vector3D<T>(
            new Vector3D<T>(value[0][0], value[0][1], value[0][2]).Length,
            new Vector3D<T>(value[1][0], value[1][1], value[1][2]).Length,
            new Vector3D<T>(value[2][0], value[2][1], value[2][2]).Length);
    }

    public static VECTOR VecToFMOD(this Vector3D<float> value)
    {
        // Convert from our coordinates (+x forward, +y right, +z up)
        // to FMOD (+z forward, +x right, +y up)
        VECTOR v;
        v.x = value.Y;
        v.y = value.Z;
        v.z = value.X;

        return v;
    }
}