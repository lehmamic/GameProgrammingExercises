using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace GameProgrammingExercises;

public class SpriteComponent : Component
{
    public SpriteComponent(Actor owner, int drawOrder = 100)
        : base(owner)
    {
        DrawOrder = drawOrder;

        Owner.Game.Renderer.AddSprite(this);
    }

    public int DrawOrder { get; }

    public bool Visible { get; set; } = true;

    public Texture Texture { get; set; }

    public virtual unsafe void Draw(Shader shader)
    {
        // Scale the quad by the width/height of texture
        Matrix4X4<float> scaleMat = Matrix4X4.CreateScale(Texture.Width, Texture.Height, 1.0f);

        Matrix4X4<float> world = scaleMat * Owner.WorldTransform;

        // Since all sprites use the same shader/vertices,
        // the game first sets them active before any sprite draws

        // Set world transform
        shader.SetUniform("uWorldTransform", world);

        // Set current texture
        Texture.SetActive();

        // Draw quad
        Owner.Game.Renderer.GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, null);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Owner.Game.Renderer.RemoveSprite(this);
        }

        base.Dispose(disposing);
    }
}