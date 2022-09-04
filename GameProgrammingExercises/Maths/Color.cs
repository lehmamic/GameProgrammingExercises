using Silk.NET.Maths;

namespace GameProgrammingExercises.Maths;

public static class Color
{
    public static readonly  Vector3D<float> Black = new(0.0f, 0.0f, 0.0f);
    public static readonly Vector3D<float> White = new(1.0f, 1.0f, 1.0f);
    public static readonly Vector3D<float> Red = new(1.0f, 0.0f, 0.0f);
    public static readonly Vector3D<float> Green = new(0.0f, 1.0f, 0.0f);
    public static readonly Vector3D<float> Blue = new(0.0f, 0.0f, 1.0f);
    public static readonly Vector3D<float> Yellow = new(1.0f, 1.0f, 0.0f);
    public static readonly Vector3D<float> LightYellow = new(1.0f, 1.0f, 0.88f);
    public static readonly Vector3D<float> LightBlue = new(0.68f, 0.85f, 0.9f);
    public static readonly Vector3D<float> LightPink = new(1.0f, 0.71f, 0.76f);
    public static readonly Vector3D<float> LightGreen = new(0.56f, 0.93f, 0.56f);
}