using Silk.NET.Maths;

namespace GameProgrammingExercises;

public static class JsonHelper
{
    public static bool TryGetVector3D<T>(T[]? values, out Vector3D<T> vector)
        where T : unmanaged, IFormattable, IEquatable<T>, IComparable<T>
    {
        if (values?.Length != 3)
        {
            vector = Vector3D<T>.Zero;
            return false;
        }

        vector = new Vector3D<T>(values[0], values[1], values[2]);
        return true;
    }

    public static bool TryGetQuaternion<T>(T[]? values, out Quaternion<T> quaternion)
        where T : unmanaged, IFormattable, IEquatable<T>, IComparable<T>
    {
        if (values?.Length != 4)
        {
            quaternion = Quaternion<T>.Identity;
            return false;
        }

        quaternion = new Quaternion<T>(values[0], values[1], values[2], values[3]);
        return true;
    }
}