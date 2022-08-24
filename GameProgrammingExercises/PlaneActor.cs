namespace GameProgrammingExercises;

public class PlaneActor : Actor
{
    public PlaneActor(Game game)
        : base(game)
    {
        Scale = 10.0f;
        var mesh = new MeshComponent(this);
        mesh.Mesh = Game.Renderer.GetMesh("Assets/Plane.gpmesh");
    }
}