using System.Numerics;
using GameEngine.Entities;
using Silk.NET.Maths;

namespace GameEngine.Toolbox;

public static class Maths
{
    public static Matrix4X4<float> CreateTranslationMatrix(Vector3D<float> translation, float rx, float ry, float rz, float scale)
    {
        var matrix = Matrix4X4<float>.Identity;
        matrix *= Matrix4X4.CreateScale(scale);
        matrix *= Matrix4X4.CreateRotationX(Scalar.DegreesToRadians(rx));
        matrix *= Matrix4X4.CreateRotationY(Scalar.DegreesToRadians(ry));
        matrix *= Matrix4X4.CreateRotationZ(Scalar.DegreesToRadians(rz));
        matrix *= Matrix4X4.CreateTranslation(translation);

        return matrix;
    }

    public static Matrix4X4<float> CreateViewMatrix(Camera camera)
    {
        var matrix = Matrix4X4<float>.Identity;


        var cameraPos = camera.Position;
        var negativeCameraPos = new Vector3D<float>(-cameraPos.X, -cameraPos.Y, -cameraPos.Z);
        matrix *= Matrix4X4.CreateTranslation(negativeCameraPos);

        matrix *= Matrix4X4.CreateRotationZ(Scalar.DegreesToRadians(camera.Roll));
        matrix *= Matrix4X4.CreateRotationY(Scalar.DegreesToRadians(camera.Yaw));
        matrix *= Matrix4X4.CreateRotationX(Scalar.DegreesToRadians(camera.Pitch));

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

    // find the exact point on a triangle
    public static float BarryCentric(Vector3D<float> p1, Vector3D<float> p2, Vector3D<float> p3, Vector2D<float> pos) {
        float det = (p2.Z - p3.Z) * (p1.X - p3.X) + (p3.X - p2.X) * (p1.Z - p3.Z);
        float l1 = ((p2.Z - p3.Z) * (pos.X - p3.X) + (p3.X - p2.X) * (pos.Y - p3.Z)) / det;
        float l2 = ((p3.Z - p1.Z) * (pos.X - p3.X) + (p1.X - p3.X) * (pos.Y - p3.Z)) / det;
        float l3 = 1.0f - l1 - l2;
        return l1 * p1.Y + l2 * p2.Y + l3 * p3.Y;
    }
}