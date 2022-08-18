using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace GameProgrammingExercises;

public class SpriteComponent : Component
{
    private readonly GL _gl;

    public SpriteComponent(GL gl, Actor owner, int drawOrder = 100)
        : base(owner)
    {
        _gl = gl;
        DrawOrder = drawOrder;

        Owner.Game.AddSprite(this);
    }
    
    public int DrawOrder { get; }

    public Texture Texture { get; set; }

    public unsafe void Draw(Shader shader)
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
        _gl.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, null);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Owner.Game.RemoveSprite(this);
        }

        base.Dispose(disposing);
    }
}