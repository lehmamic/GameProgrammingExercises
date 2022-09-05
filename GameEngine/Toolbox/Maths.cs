using System.Numerics;
using GameEngine.Entities;
using Silk.NET.Maths;

namespace GameEngine.Toolbox;

public static class Maths
{
    public static Matrix4X4<float> CreateTranslationMatrix(Vector3D<float> translation, float rx, float ry, float rz, float scale)
    {
        var matrix = Matrix4X4<float>.Identity;
        matrix *= Matrix4X4.CreateTranslation(translation);
        matrix *= Matrix4X4.CreateRotationX(Scalar.DegreesToRadians(rx));
        matrix *= Matrix4X4.CreateRotationY(Scalar.DegreesToRadians(ry));
        matrix *= Matrix4X4.CreateRotationZ(Scalar.DegreesToRadians(rz));
        matrix *= Matrix4X4.CreateScale(scale);

        return matrix;
    }

    public static Matrix4X4<float> CreateViewMatrix(Camera camera)
    {
        var matrix = Matrix4X4<float>.Identity;
        matrix *= Matrix4X4.CreateFromYawPitchRoll<float>(Scalar.DegreesToRadians(camera.Yaw), Scalar.DegreesToRadians(camera.Pitch), Scalar.DegreesToRadians(camera.Roll));

        var cameraPos = camera.Position;
        var negativeCameraPos = new Vector3D<float>(-cameraPos.X, -cameraPos.Y, -cameraPos.Z);
        matrix *= Matrix4X4.CreateTranslation(negativeCameraPos);

        return matrix;
    }

    public static Matrix4X4<float> CreateProjectionMatrix(float fov, int width, int height, float nearPlane, float farPlane)
    {
        float aspectRation = (float) width / (float) height;
        float yScale = (1.0f / Scalar.Tan(Scalar.DegreesToRadians(fov / 2.0f))) * aspectRation;
        float xScale = yScale / aspectRation;
        float frustumLength = farPlane - nearPlane;

        var matrix = Matrix4X4<float>.Identity with
        {
            M11 = xScale,
            M22 = yScale,
            M33 = -((farPlane + nearPlane) / frustumLength),
            M34 = -1.0f,
            M43 = -((2.0f * nearPlane * farPlane) / frustumLength),
            M44 = 0.0f,
        };

        return matrix;
    }
}