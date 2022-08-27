using Silk.NET.Maths;

namespace GameProgrammingExercises.Maths.Geometry;

/// <summary>
/// Oriented bounding box 
/// </summary>
public struct OBB
{
    public Vector3D<float> Center;
    public Quaternion<float> Rotation;
    public Vector3D<float> Extents;
}