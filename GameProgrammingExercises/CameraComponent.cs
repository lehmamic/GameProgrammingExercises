using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class CameraComponent : Component
{
    public CameraComponent(Actor owner, int updateOrder = 200)
        : base(owner, updateOrder)
    {
    }

    protected void SetViewMatrix(Matrix4X4<float> view)
    {
        // Pass view matrix to render and audio system
        Owner.Game.Renderer.ViewMatrix = view;
        Owner.Game.AudioSystem.SetListener(view);
    }
}