using Silk.NET.Maths;

namespace GameProgrammingExercises;

public struct BoneTransform
{
    public Quaternion<float> Rotation;

    public Vector3D<float> Translation;

    public Matrix4X4<float> ToMatrix()
    {
        var rot = Matrix4X4.CreateFromQuaternion(Rotation);
        var trans = Matrix4X4.CreateTranslation(Translation);

        return rot * trans;
    }

    public static BoneTransform Interpolation(BoneTransform a, BoneTransform b, float f)
    {
        BoneTransform retVal;
        retVal.Rotation = Quaternion<float>.Slerp(a.Rotation, b.Rotation, f);
        retVal.Translation = Vector3D.Lerp(a.Translation, b.Translation, f);
        return retVal;
    }
}