using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace GameProgrammingExercises;

public class PointLightComponent : Component
{
    public PointLightComponent(Actor owner)
        : base(owner)
    {
        Owner.Game.Renderer.AddPointLight(this);
    }

    public Vector3D<float> DiffuseColor { get; set; }

    public float InnerRadius { get; set; }

    public float OuterRadius { get; set; }

    public unsafe void Draw(Shader shader, Mesh mesh)
    {
        // We assume, coming into this function, that the shader is active
        // and the sphere mesh is active

        // World transform is scaled to the outer radius (divided by the mesh radius)
        // and positioned to the world position
        Matrix4X4<float> scale = Matrix4X4.CreateScale(Owner.Scale * OuterRadius / mesh.Radius);
        Matrix4X4<float> trans = Matrix4X4.CreateTranslation(Owner.Position);
        Matrix4X4<float> worldTransform = scale * trans;
        shader.SetUniform("uWorldTransform", worldTransform);
        // Set point light shader constants
        shader.SetUniform("uPointLight.mWorldPos", Owner.Position);
        shader.SetUniform("uPointLight.mDiffuseColor", DiffuseColor);
        shader.SetUniform("uPointLight.mInnerRadius", InnerRadius);
        shader.SetUniform("uPointLight.mOuterRadius", OuterRadius);

        // Draw the sphere
        Owner.Game.Renderer.GL.DrawElements(PrimitiveType.Triangles, (uint)mesh.VertexArray.NumberOfIndices, DrawElementsType.UnsignedInt, null);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            Owner.Game.Renderer.RemovePointLight(this);
        }
    }
}