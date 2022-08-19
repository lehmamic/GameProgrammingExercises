using System.Numerics;
using Silk.NET.Maths;

namespace GameProgrammingExercises.Maths;

public static class GameMath
{
    public static float TwoPi = (float)Math.PI * 2.0f;

    public static float PiOver2 = (float)Math.PI / 2.0f;
    
    public static float Cot(float angle)
    {
        return 1.0f / Scalar.Tan(angle);
    }
    
    public static Matrix4X4<float> CreateLookAt(Vector3D<float> eye, Vector3D<float> target, Vector3D<float> up)
    {
        Vector3D<float> zaxis = Vector3D.Normalize(target - eye);
        Vector3D<float> xaxis = Vector3D.Normalize(Vector3D.Cross(up, zaxis));
        Vector3D<float> yaxis = Vector3D.Normalize(Vector3D.Cross(zaxis, xaxis));
        Vector3D<float> trans;
        trans.X = -Vector3D.Dot(xaxis, eye);
        trans.Y = -Vector3D.Dot(yaxis, eye);
        trans.Z = -Vector3D.Dot(zaxis, eye);

        return new Matrix4X4<float>(
            xaxis.X, yaxis.X, zaxis.X, 0.0f,
            xaxis.Y, yaxis.Y, zaxis.Y, 0.0f,
            xaxis.Z, yaxis.Z, zaxis.Z, 0.0f,
            trans.X, trans.Y, trans.Z, 1.0f);
    }
    
    public static Matrix4X4<float> CreateSimpleViewProj(float width, float height)
    {
        return Matrix4X4<float>.Identity with { M11 = 2.0f/width, M22 = 2.0f/height, M33 = 1.0f, M43 = 1.0f, M44 = 1.0f };
    }
    
    public static Matrix4X4<float> CreatePerspectiveFieldOfView(float fieldOfView, float width, float height, float nearPlaneDistance, float farPlaneDistance)
    {
        if (!Scalar.GreaterThan<float>(fieldOfView, Scalar<float>.Zero) || Scalar.GreaterThanOrEqual<float>(fieldOfView, Scalar<float>.Pi))
        {
            throw new ArgumentOutOfRangeException(nameof (fieldOfView));
        }

        if (!Scalar.GreaterThan<float>(width, Scalar<float>.Zero))
        {
            throw new ArgumentOutOfRangeException(nameof (width));
        }
        
        if (!Scalar.GreaterThan<float>(height, Scalar<float>.Zero))
        {
            throw new ArgumentOutOfRangeException(nameof (height));
        }

        if (!Scalar.GreaterThan<float>(nearPlaneDistance, Scalar<float>.Zero))
        {
            throw new ArgumentOutOfRangeException(nameof (nearPlaneDistance));
        }

        if (!Scalar.GreaterThan<float>(farPlaneDistance, Scalar<float>.Zero))
        {
            throw new ArgumentOutOfRangeException(nameof (farPlaneDistance));
        }

        float yScale = Cot(fieldOfView / 2.0f);
        float xScale = yScale * height / width;

        // Matrix4X4.CreatePerspectiveFieldOfView()
        return new Matrix4X4<float>
        {
            M11 = xScale,
            M22 = yScale,
            M33 = farPlaneDistance / (farPlaneDistance - nearPlaneDistance),
            M34 = 1.0f,
            M43 = -nearPlaneDistance * farPlaneDistance / (farPlaneDistance - nearPlaneDistance),
        };
    }
}