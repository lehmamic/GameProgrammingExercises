using Silk.NET.Maths;

namespace GameProgrammingExercises.Maths;

public static class VectorRandom
{
    public static float GetFloat()
    {
        return GetFloatRange(0.0f, 1.0f);
    }

    public static float GetFloatRange(float min, float max)
    {
        float val = Random.Shared.NextSingle() * (max - min) + min;
        return val;
    }

    public static int GetIntRange(int min, int max)
    {
        return Random.Shared.Next(min, max);
    }

    public static Vector2D<float> GetVector(Vector2D<float> min, Vector2D<float> max)
    {
        Vector2D<float> r = new Vector2D<float>(GetFloat(), GetFloat());
        return min + (max - min) * r;
    }

    public static Vector3D<float> GetVector(Vector3D<float> min, Vector3D<float> max)
    {
        Vector3D<float> r = new Vector3D<float>(GetFloat(), GetFloat(), GetFloat());
        return min + (max - min) * r;
    }
}