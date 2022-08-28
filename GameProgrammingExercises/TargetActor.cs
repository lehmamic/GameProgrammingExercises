using GameProgrammingExercises.Maths;
using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class TargetActor : Actor
{
    public TargetActor(Game game)
        : base(game)
    {
        //SetScale(10.0f);
        Rotation = GameMath.CreateQuaternion(Vector3D<float>.UnitZ, Scalar<float>.Pi);
        var mc = new MeshComponent(this);
        Mesh mesh = Game.Renderer.GetMesh("Assets/Target.gpmesh");
        mc.Mesh = mesh;

        // Add collision box
        var bc = new BoxComponent(this);
        bc.ObjectBox = mesh.Box;
    }
}