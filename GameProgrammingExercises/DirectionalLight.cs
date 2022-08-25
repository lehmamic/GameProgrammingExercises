using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class DirectionalLight
{
    public DirectionalLight(Vector3D<float> direction, Vector3D<float> diffuseColor, Vector3D<float> specularColor)
    {
        Direction = direction;
        DiffuseColor = diffuseColor;
        SpecularColor = specularColor;
    }

    /// <summary>
    /// Direction of light
    /// </summary>
    public Vector3D<float> Direction { get; }

    /// <summary>
    /// Diffuse color
    /// </summary>
    public Vector3D<float> DiffuseColor { get; }

    /// <summary>
    /// Specular color
    /// </summary>
    public Vector3D<float> SpecularColor { get; }
}