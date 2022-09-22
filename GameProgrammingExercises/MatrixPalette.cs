using Silk.NET.Maths;

namespace GameProgrammingExercises;

public struct MatrixPalette
{
    // We could go as high as 256 because we send teh bone indices as a byte to the shader
    public const byte MaxSkeletonBones = 96;

    public Matrix4X4<float>[] Entry = new Matrix4X4<float>[MaxSkeletonBones];

    public MatrixPalette()
    {
    }
}