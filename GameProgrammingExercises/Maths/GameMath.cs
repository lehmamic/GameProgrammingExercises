using Silk.NET.Maths;

namespace GameProgrammingExercises.Maths;

public static class GameMath
{
    public static float TwoPi = (float)Math.PI * 2.0f;

    public static float PiOver2 = (float)Math.PI / 2.0f;
    
    public static Matrix4X4<float> CreateSimpleViewProj(float width, float height)
    {
        return Matrix4X4<float>.Identity with { M11 = 2.0f/width, M22 = 2.0f/height, M33 = 1.0f, M43 = 1.0f, M44 = 1.0f };
    }
}