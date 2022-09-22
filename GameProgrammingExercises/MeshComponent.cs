using Silk.NET.OpenGL;

namespace GameProgrammingExercises;

public class MeshComponent : Component
{
    public MeshComponent(Actor owner, bool isSkeletal = false)
        : base(owner)
    {
        IsSkeletal = isSkeletal;
        Owner.Game.Renderer.AddMeshComp(this);
    }

    public bool Visible { get; set; } = true;

    public Mesh Mesh { get; set; }

    public int TextureIndex { get; set; }

    public bool IsSkeletal { get; }

    /// <summary>
    /// Draw the mesh component with the provided shader.
    /// </summary>
    /// <param name="shader">The shader to use for drawing the mesh.</param>
    public virtual unsafe void Draw(Shader shader)
    {
        // Set the world transform
        shader.SetUniform("uWorldTransform", Owner.WorldTransform);

        // Set specular power
        try
        {
            shader.SetUniform("uSpecPower", Mesh.SpecularPower);
        }
        catch (ShaderException)
        {
            // we catch it because the gbuffer shader has no lighting information
        }

        // Set the active texture
        var texture = Mesh.GetTexture(TextureIndex);
        if (texture is not null)
        {
            texture.SetActive();
        }

        // Set mesh's vertex array as active
        var vao = Mesh.VertexArray;
        vao.SetActive();

        // Draw
        Owner.Game.Renderer.GL.DrawElements(PrimitiveType.Triangles, (uint)vao.NumberOfIndices, DrawElementsType.UnsignedInt, null);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Owner.Game.Renderer.RemoveMeshComp(this);
        }

        base.Dispose(disposing);
    }
}